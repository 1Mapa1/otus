# WarehouseService

Сервис **складских остатков** (`WarehouseMs`): `StockItem`, резервы под заказ, движения склада, публикация `StockChanged` в Kafka. **Товарной карточки** (name, price) в Warehouse **нет** — она в CatalogMs.

## Архитектура

- `WarehouseService.Api` — HTTP API (admin stocks + internal reservations), JWT, Swagger
- `WarehouseService.Application` — MediatR: stocks, income, movements, резервы
- `WarehouseService.Domain` — `StockItem`, `StockReservation`, `StockMovement`
- `WarehouseService.Infrastructure` — EF Core, `FOR UPDATE`, outbox, Kafka consumer/publisher
- `WarehouseService.DbMigrator` — миграции БД

## Модель остатков

```text
AvailableQuantity = свободно для нового резерва
ReservedQuantity  = уже зарезервировано
```

Операции: `Income`, `Reserve`, `CancelReservation` — атомарно в транзакции с row lock.

## API

Пример хоста: `http://arch.homework` (см. [ДЗ 8 / K8s](../../ДЗ%208/K8s/README.md)).

### Административный API (JWT, роль `ADMIN`)

Префикс: **`/api/warehouse/stocks`**. Claim роли: `role` = `ADMIN`.

| Метод | Путь | Описание |
|-------|------|----------|
| GET | `/api/warehouse/stocks` | Все складские записи (активные и архивные) |
| GET | `/api/warehouse/stocks/{productId}` | Остаток по товару |
| POST | `/api/warehouse/stocks/{productId}/income` | Поступление на склад |
| GET | `/api/warehouse/stocks/{productId}/movements` | История движений (newest first) |

### Внутренний API (Saga, без JWT)

| Метод | Путь | Описание |
|-------|------|----------|
| POST | `/api/internal/warehouse/reservations` | Идемпотентный резерв |
| POST | `/api/internal/warehouse/reservations/cancel` | Идемпотентная отмена |

> **Примечание:** `POST /api/internal/warehouse/products/resolve` удалён. OrderService должен перейти на CatalogMs snapshot — отдельная задача.

### Документация и health

- Swagger: **`/api/warehouse/swagger`**
- Health: `/health/live`, `/health/ready`, `/health/startup`

## Kafka

**Потребляет** (topics из `Kafka:Topics`, envelope как Billing):

- `ProductCreated` — создать `StockItem` с нулевым остатком (идемпотентно)
- `ProductArchived` / `ProductRestored` — `IsActive` (без изменения quantities)

**Публикует** (topic `Kafka:WarehouseStockTopic`, outbox):

- `StockChanged` после Income / Reserve / Cancel

Конфигурация (`appsettings` / env):

```text
Kafka:BootstrapServers
Kafka:GroupId
Kafka:Topics
Kafka:WarehouseStockTopic
Kafka:Acks
```

Inbox в Warehouse **не используется**.

## Связанные сервисы

- [OrderService](../OrderService/README.md) — сага (резерв / отмена)
- CatalogMs (вне репозитория) — lifecycle события и потребитель `StockChanged`

## Сборка Docker

```bash
cd HwApp/WarehouseService
docker build --platform linux/amd64 -f Dockerfile.Api -t maslovdeveloper/hwapp-warehouse-service:8.0 .
docker build --platform linux/amd64 -f Dockerfile.Migration -t maslovdeveloper/hwapp-warehouse-migration:8.0 .
```

## Миграции

```bash
cd HwApp/WarehouseService
dotnet ef database update --project WarehouseService.Infrastructure/WarehouseService.Infrastructure.csproj --startup-project WarehouseService.Api/WarehouseService.Api.csproj
```

Миграция `ProductToStockItem`: `products` → `stock_items`, пересчёт `available_quantity`, новые таблицы `stock_movements` и `outbox_messages`.

## Развёртывание

[ДЗ 8 / K8s](../../ДЗ%208/K8s/README.md), [HwApp/README.md](../README.md).
