# OrderService

Сервис заказов: создаёт заказ идемпотентно и оркестрирует распределённую сагу оплаты, резервирования товара и доставки.

## Ответственность

- создание заказа с обязательным `Idempotency-Key`;
- snapshot товара и цены через CatalogService;
- фоновая обработка саги;
- шаги Billing authorize → Warehouse reserve → Delivery reserve → Billing capture;
- компенсации в обратном порядке при ошибке;
- публикация результата заказа через outbox.

## Состав

- `OrderService.Api` — пользовательский API;
- `OrderService.Application` — команды, запросы и шаги саги;
- `OrderService.Domain` — заказ, позиции, статусы и состояние саги;
- `OrderService.Infrastructure` — EF Core, HTTP clients, outbox, Kafka и `OrderSagaWorker`;
- `OrderService.DbMigrator` — миграции БД.

## Интеграции

| Направление | Система | Назначение |
|---|---|---|
| HTTP → | CatalogService | Snapshot товаров |
| HTTP → | BillingService | authorize, capture, cancel |
| HTTP → | WarehouseService | reserve, cancel |
| HTTP → | DeliveryService | reserve, cancel |
| Kafka → | topic `orders` | `order.confirmed.v1`, `order.rejected.v2` |
| PostgreSQL | `order_db` | заказы, saga state, idempotency и outbox |

## API

Все endpoints требуют JWT.

| Метод | Путь | Назначение |
|---|---|---|
| POST | `/api/orders` | Создать заказ; требуется UUID в `Idempotency-Key`, ответ `202 Accepted` |
| GET | `/api/orders/me` | Заказы текущего пользователя |
| GET | `/api/orders/{id}` | Детали своего заказа |

Swagger: `/api/orders/swagger`.

## Основная конфигурация

- `ConnectionStrings`;
- `Auth:Url`;
- `Ms:Catalog`, `Ms:Billing`, `Ms:Warehouse`, `Ms:Delivery`;
- `Idempotency`;
- `OrderSaga`;
- `Kafka`.

## Сборка

```powershell
dotnet build OrderService.Api/OrderService.Api.csproj

docker build --platform linux/amd64 -f Dockerfile.Api -t maslovdeveloper/hwapp-order-service:<tag> .
docker build --platform linux/amd64 -f Dockerfile.Migration -t maslovdeveloper/hwapp-order-migration:<tag> .
```

## Эксплуатационные endpoints

- `/health/live`
- `/health/ready`
- `/health/startup`
- `/metrics`

Система целиком: [HwApp](../README.md). Развёртывание: [K8s](../../Проектная%20работа/K8s/README.md).
