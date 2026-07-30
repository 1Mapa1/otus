# CatalogService

Сервис каталога: управляет карточками товаров, брендами, категориями и read model витрины. Остатками владеет WarehouseService.

## Ответственность

- публичный поиск, фильтрация и просмотр товаров;
- административное управление товарами и справочниками;
- snapshot товаров и цен для OrderService;
- CQRS-lite: primary для записи, replica для публичного чтения;
- Redis cache-aside и защита от cache stampede;
- обработка складских событий и публикация lifecycle-событий товара.

## Состав

- `CatalogService.Api` — public, admin и internal API;
- `CatalogService.Application` — команды и запросы MediatR;
- `CatalogService.Domain` — Product, Brand, Category и ProductReadModel;
- `CatalogService.Infrastructure` — EF Core primary/replica, Redis, Kafka и outbox;
- `CatalogService.DbMigrator` — миграции primary и seed каталога.

## Интеграции

| Направление | Система | Назначение |
|---|---|---|
| HTTP ← | OrderService | Snapshot товаров |
| Kafka → | topic `products` | `product.created.v1`, `product.archived.v1`, `product.restored.v1` |
| Kafka ← | topic `stocks` | `stock.changed.v1` обновляет availability |
| Redis | cache `catalog:*` | списки товаров, брендов и категорий |
| PostgreSQL | primary + replica | write model и read model |

## API

| Метод | Путь | Доступ | Назначение |
|---|---|---|---|
| GET | `/api/catalog/products` | публичный | Поиск и список активных товаров |
| GET | `/api/catalog/products/{productId}` | публичный | Карточка товара |
| GET | `/api/catalog/brands` | публичный | Бренды |
| GET | `/api/catalog/categories` | публичный | Категории |
| POST/PUT | `/api/catalog/brands` | `ADMIN` | Управление брендами |
| POST/PUT | `/api/catalog/categories` | `ADMIN` | Управление категориями |
| POST/PUT/DELETE | `/api/catalog/products` | `ADMIN` | Управление товарами |
| POST | `/api/catalog/products/{productId}/restore` | `ADMIN` | Восстановить товар |
| POST | `/api/internal/catalog/products/snapshot` | internal | Snapshot для заказа |

Параметры списка: `search`, `brandId`, `categoryId`, `minPrice`, `maxPrice`, `sort`, `page`, `pageSize`.

Swagger: `/api/catalog/swagger`.

## Основная конфигурация

- `ConnectionStrings:CatalogPrimary`, `ConnectionStrings:CatalogReplica`;
- `Auth:Url`;
- `Catalog` — TTL и порог low stock;
- `Redis`;
- `Kafka`;
- `DB_PRIMARY_*`, `DB_REPLICA_*`.

## Сборка

```powershell
dotnet build CatalogService.Api/CatalogService.Api.csproj

docker build --platform linux/amd64 -f Dockerfile.Api -t maslovdeveloper/hwapp-catalog-service:<tag> .
docker build --platform linux/amd64 -f Dockerfile.Migration -t maslovdeveloper/hwapp-catalog-migration:<tag> .
```

## Эксплуатационные endpoints

- `/health/live`
- `/health/ready`
- `/health/startup`
- `/health/replica`
- `/metrics`

Система целиком: [HwApp](../README.md). Развёртывание: [K8s](../../Проектная%20работа/K8s/README.md).
