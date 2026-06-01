# DeliveryService

Сервис **доставки**: **слоты** (интервалы времени) и **резерв слота** под заказ с возможностью отмены. Внешнее API с JWT для управления слотами; внутренние ручки резерва вызывает **OrderService** в саге заказа.

## Архитектура

- `DeliveryService.Api` — HTTP API (external + internal), JWT на внешних ручках, Swagger
- `DeliveryService.Application` — MediatR: слоты, резерв, отмена
- `DeliveryService.Domain` — слот, резерв доставки, статусы
- `DeliveryService.Infrastructure` — EF Core, репозитории
- `DeliveryService.DbMigrator` — миграции БД

## API

Пример хоста за Ingress: `http://arch.homework` (см. [ДЗ 8 / K8s](../../ДЗ%208/K8s/README.md)).

### Внешний API (JWT)

Префикс: **`/api/delivery/slots`**.

| Метод | Путь | Описание |
|-------|------|----------|
| GET | `/api/delivery/slots` | Список доступных слотов |
| POST | `/api/delivery/slots` | Создание слота (интервал доставки) |

### Внутренний API

Префикс: **`/api/internal/delivery/reservations`**.

| Метод | Путь | Описание |
|-------|------|----------|
| POST | `/api/internal/delivery/reservations` | Резерв слота под `orderId`, `userId`, `deliverySlotId` |
| POST | `/api/internal/delivery/reservations/cancel` | Отмена резерва по `orderId` (компенсация саги) |

### Документация и health

- Swagger UI (Development): **`/api/delivery/swagger`**
- Health: `/health/live`, `/health/ready`, `/health/startup`

## Связанные сервисы

- [OrderService](../OrderService/README.md) — сага заказа (резерв доставки / отмена)

## Сборка Docker-образов

Из каталога `DeliveryService/`:

```bash
docker build --platform linux/amd64 -f Dockerfile.Api -t maslovdeveloper/hwapp-delivery-service:8.0 .
docker build --platform linux/amd64 -f Dockerfile.Migration -t maslovdeveloper/hwapp-delivery-migration:8.0 .
```

## Развёртывание

Helm, Ingress (`/api/delivery`, `/api/internal/delivery`): [ДЗ 8 / K8s](../../ДЗ%208/K8s/README.md). Общий указатель: [HwApp/README.md](../README.md).
