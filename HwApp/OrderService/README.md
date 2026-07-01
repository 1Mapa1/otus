# OrderService

Создание заказа с **распределённой транзакцией (сага)**: авторизация платежа в **Billing**, резерв товара в **Warehouse**, резерв слота в **Delivery**, затем **capture** платежа; при сбое на шагах после оплаты — **компенсации** (отмена доставки → склада → авторизации). Доменные события уходят в **Kafka** через паттерн **outbox**. **Идемпотентность** `POST /api/orders`: заголовок **`Idempotency-Key`** (UUID), запись в БД и повтор того же ответа — см. [ДЗ 9 / Архитектура](../../ДЗ%209/Архитектура/Архитектура.md).

## Архитектура

- `OrderService.Api` — HTTP API, JWT, Swagger
- `OrderService.Application` — команды/запросы, **оркестрация саги** (`Orders/Saga/`, обработчики шагов в `Orders/Saga/Steps/`)
- `OrderService.Domain` — заказ, позиции, статусы, шаги саги, причины отказа, доменные события
- `OrderService.Infrastructure` — EF Core, HTTP-клиенты Billing / Catalog / Warehouse / Delivery, Kafka producer, outbox, **`OrderSagaWorker`** (фоновая обработка заказов в работе)
- `OrderService.DbMigrator` — миграции БД

## Конфигурация

- **`Ms:*`** — базовые URL и таймауты Billing, Catalog, Warehouse, Delivery (Helm: `Ms__Billing__*`, `Ms__Catalog__*`, `Ms__Warehouse__*`, `Ms__Delivery__*`).
- **`Idempotency`** — TTL блокировки обработки и TTL записи (`Idempotency__ProcessingLockTtl`, `Idempotency__RecordTtl` в Helm).
- **`OrderSaga`** — размер пачки, длительность блокировки, интервал опроса (`OrderSaga__*` в Helm).
- **`Kafka`**, **`Auth`** — как в остальных сервисах стенда.

## API

Пример хоста за Ingress: `http://arch.homework` (см. [ДЗ 8 / K8s](../../ДЗ%208/K8s/README.md), [ДЗ 9 / K8s](../../ДЗ%209/K8s/README.md) — идемпотентность и переменные `Idempotency__*`).

Префикс: **`/api/orders`**, JWT обязателен.

| Метод | Путь | Описание |
|-------|------|----------|
| POST | `/api/orders` | Создание заказа (`deliverySlotId`, `deliveryAddress`, позиции); снимок каталога через **Catalog**; заголовок **`Idempotency-Key`** обязателен; ответ **202 Accepted** — дальнейшая обработка сагой |
| GET | `/api/orders/me` | Список заказов текущего пользователя |
| GET | `/api/orders/{id}` | Детали заказа по идентификатору (включая `deliveryAddress`) |

### Документация и health

- Swagger UI (Development): **`/api/orders/swagger`**
- Health: `/health/live`, `/health/ready`, `/health/startup`

## Связанные сервисы

- [BillingService](../BillingService/README.md) — authorize / capture / cancel authorization
- CatalogMs — снимок товаров при оформлении (`POST api/internal/catalog/products/snapshot`)
- [WarehouseService](../WarehouseService/README.md) — резерв и отмена товара
- [DeliveryService](../DeliveryService/README.md) — резерв и отмена слота (адрес доставки из заказа)
- [NotificationService](../NotificationService/README.md) — события в Kafka

## Сборка Docker-образов

Из каталога `OrderService/`:

```bash
docker build --platform linux/amd64 -f Dockerfile.Api -t maslovdeveloper/hwapp-order-service:8.0 .
docker build --platform linux/amd64 -f Dockerfile.Migration -t maslovdeveloper/hwapp-order-migration:8.0 .
```

## Развёртывание

Postgres, Kafka, Ingress `/api/orders`: [ДЗ 8 / K8s](../../ДЗ%208/K8s/README.md), [ДЗ 9 / K8s](../../ДЗ%209/K8s/README.md). Общий указатель: [HwApp/README.md](../README.md).
