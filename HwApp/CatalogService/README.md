# CatalogService

Сервис **каталога товаров** (`CatalogMs`): карточки товаров, бренды, категории, витринный список, snapshot для OrderService, lifecycle-события в Kafka. **Остатки** владеет [WarehouseService](../WarehouseService/README.md).

## Архитектура

- `CatalogService.Api` — HTTP API, JWT (admin), Swagger, health
- `CatalogService.Application` — MediatR handlers, Result/Error
- `CatalogService.Domain` — Product, Brand, Category, ProductAttribute, ProductReadModel
- `CatalogService.Infrastructure` — EF Core (**primary + replica**), Redis cache-aside, Kafka consumer/producer, outbox
- `CatalogService.DbMigrator` — миграции (только primary) + минимальный seed

### CQRS-lite

| Слой | Назначение |
|------|------------|
| **Product** (primary) | Source of truth: name, description, price, image, attributes, IsActive |
| **ProductReadModel** (primary, читается с replica) | Денормализованная витрина для списка: brandName, attribute values JSON, availabilityStatus |
| **CatalogWriteDbContext** | Admin writes, snapshot, StockChanged consumer, outbox |
| **CatalogReadDbContext** | Public GET (products list/details, brands, categories) |

### Primary / Replica

- **Primary** (`ConnectionStrings:CatalogPrimary`) — все записи, snapshot, Kafka, миграции
- **Replica** (`ConnectionStrings:CatalogReplica`) — только публичные read-запросы
- Нет fallback на primary при недоступной replica
- `/health/ready` — healthy, если primary доступен (replica может быть недоступна)
- `/health/replica` — отдельная проверка replica

### Redis Cache-Aside

Только для `GET /api/catalog/products`:

- `IDistributedCache` + `IConnectionMultiplexer`, `InstanceName = catalog:`
- TTL: **60 с + jitter 0–15 с**
- `CatalogCacheKeyBuilder` — нормализованный ключ запроса
- `RedisCacheLock` — distributed lock против cache stampede (Lua release)
- Redis недоступен → bypass cache, читаем replica
- Replica недоступна при cache miss → **503**

### Kafka

| Направление | Topic | События |
|-------------|-------|---------|
| **Publish** (outbox) | `products` | `product.created.v1`, `product.archived.v1`, `product.restored.v1` |
| **Consume** | `stocks` | `stock.changed.v1` → обновление `ProductReadModel.AvailabilityStatus` |

**Нет** `ProductUpdated` — изменение карточки не влияет на склад.

Payload lifecycle: `{ "productId": "guid" }`. Key: `productId`.

`stock.changed.v1` использует только `availableQuantity`; порог low stock: `Catalog:LowStockThreshold` (default **5**).

## Конфигурация

```json
{
  "ConnectionStrings": {
    "CatalogPrimary": "",
    "CatalogReplica": ""
  },
  "Auth": { "Url": "" },
  "Catalog": { "LowStockThreshold": 5 },
  "Redis": {
    "Configuration": "localhost:6379",
    "InstanceName": "catalog:"
  },
  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "GroupId": "catalog-service",
    "Topics": [ "stocks" ]
  }
}
```

Env overrides: `DB_PRIMARY_*`, `DB_REPLICA_*`.

## API

Префикс: `/api/catalog`

### Public (replica, без JWT)

| Метод | Путь | Описание |
|-------|------|----------|
| GET | `/api/catalog/products` | Список активных товаров (ProductReadModel, Redis cache) |
| GET | `/api/catalog/products/{id}` | Детали (Product + brand + category + attributes + availability) |
| GET | `/api/catalog/brands` | Справочник брендов |
| GET | `/api/catalog/categories` | Справочник категорий |

Query для списка: `search`, `brandId`, `categoryId`, `minPrice`, `maxPrice`, `sort` (`price-asc`, `price-desc`, `name-asc`, `name-desc`), `page` (default 1), `pageSize` (default 24, max 100).

`search` — по **Name** и **BrandName** (ILIKE).

### Admin (`[Authorize(Roles = "ADMIN")]`, `RoleClaimType = "role"`)

| Метод | Путь |
|-------|------|
| POST | `/api/catalog/products` |
| PUT | `/api/catalog/products/{id}` |
| DELETE | `/api/catalog/products/{id}` (archive) |
| POST | `/api/catalog/products/{id}/restore` |
| POST | `/api/catalog/brands` |
| PUT | `/api/catalog/brands/{id}` |
| POST | `/api/catalog/categories` |
| PUT | `/api/catalog/categories/{id}` |

### Internal (primary, без JWT)

| Метод | Путь | Описание |
|-------|------|----------|
| POST | `/api/internal/catalog/products/snapshot` | Снимок для [OrderService](../OrderService/README.md) |

Request item: `{ productId, quantity, expectedUnitPrice? }`. Ответ 1:1 с request. При mismatch цены (если `expectedUnitPrice` передан) → **409** `PriceChanged`.

### Health & Swagger

- Swagger (Development): `/api/catalog/swagger`
- `/health/live`, `/health/ready`, `/health/startup`, `/health/replica`

## Связанные сервисы

- [OrderService](../OrderService/README.md) — snapshot при checkout
- [WarehouseService](../WarehouseService/README.md) — `stock.changed.v1`, потребитель lifecycle events
- [AuthService](../AuthService/README.md) — JWT / JWKS

## Docker

Из каталога `CatalogService/`:

```bash
docker build --platform linux/amd64 -f Dockerfile.Api -t maslovdeveloper/hwapp-catalog-service:8.0 .
docker build --platform linux/amd64 -f Dockerfile.Migration -t maslovdeveloper/hwapp-catalog-migration:8.0 .
```

## Seed (DbMigrator)

При пустой БД: бренды ASUS/Lenovo, категории Laptops/Accessories, два товара с атрибутами.
