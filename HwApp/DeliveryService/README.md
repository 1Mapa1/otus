# DeliveryService

Сервис **доставки** для онлайн-магазина: **зоны** по городу, **слоты** с **capacity** (несколько заказов на один интервал), **резерв** под заказ и **отмена** для саги **OrderService**. Kafka / outbox / inbox в DeliveryService **не используются**.

## Архитектура

- `DeliveryService.Api` — HTTP API (user / admin / internal), JWT на внешних ручках, Swagger
- `DeliveryService.Application` — MediatR, `Result` / `Error`, команды и запросы
- `DeliveryService.Domain` — зоны, слоты, резервы, snapshot адреса
- `DeliveryService.Infrastructure` — EF Core, репозитории, транзакции и atomic `ExecuteUpdateAsync` для capacity
- `DeliveryService.DbMigrator` — миграции БД

## Модель capacity

Слот **не** переводится целиком в «зарезервирован». У слота есть:

- `Capacity` — сколько заказов можно принять;
- `ReservedCount` — сколько уже зарезервировано;
- статусы слота: `Draft`, `Open`, `Closed` (не `Reserved`).

Резерв увеличивает `ReservedCount` атомарно; отмена уменьшает. Параллельные reserve безопасны за счёт conditional update в репозитории.

## API

Пример хоста за Ingress: `http://arch.homework` (см. [ДЗ 8 / K8s](../../ДЗ%208/K8s/README.md)).

### Пользовательский API (JWT)

| Метод | Путь | Описание |
|-------|------|----------|
| POST | `/api/delivery/slots/available` | Доступные слоты по **полному адресу** (зона определяется по `address.city`, trim + ignore case) |

Тело запроса включает `address`: `city`, `street`, `house`, `apartment` (nullable). Резерв **не** выполняется.

### Административный API (JWT, роль `ADMIN`)

Роль в токене Auth: claim `role` = `ADMIN` (см. `JwtTokenGenerator` в AuthService). В Delivery задано `RoleClaimType = "role"`.

| Метод | Путь | Описание |
|-------|------|----------|
| GET | `/api/delivery/zones` | Все зоны |
| POST | `/api/delivery/zones` | Создать зону (`name`, `city`) |
| PUT | `/api/delivery/zones/{zoneId}` | Обновить `name`, `isActive` (city не меняется) |
| GET | `/api/delivery/slots` | Все слоты |
| POST | `/api/delivery/slots` | Создать слот в статусе `Draft` |
| PUT | `/api/delivery/slots/{slotId}` | Обновить слот (правила зависят от статуса) |

### Внутренний API (Saga, без JWT)

Вызывается **OrderService** из внутренней сети.

| Метод | Путь | Описание |
|-------|------|----------|
| POST | `/api/internal/delivery/reservations` | Идемпотентный резерв: `orderId`, `customerId`, `deliverySlotId`, `address` |
| POST | `/api/internal/delivery/reservations/cancel` | Идемпотентная отмена по `orderId` |

Коды конфликтов для саги: `DeliverySlotUnavailable`, `InvalidReservationState`.

> **Примечание:** контракт internal reserve расширен (`customerId`, `address`). Обновление **OrderService** / `DeliveryClient` — отдельная задача; до неё сага шлёт старый payload.

### Документация и health

- Swagger UI (Development): **`/api/delivery/swagger`**
- Health: `/health/live`, `/health/ready`, `/health/startup`

## Связанные сервисы

- [OrderService](../OrderService/README.md) — сага заказа (резерв / отмена доставки)
- [AuthService](../AuthService/README.md) — JWT и роль `ADMIN`

## Сборка Docker-образов

Из каталога `DeliveryService/`:

```bash
docker build --platform linux/amd64 -f Dockerfile.Api -t maslovdeveloper/hwapp-delivery-service:8.0 .
docker build --platform linux/amd64 -f Dockerfile.Migration -t maslovdeveloper/hwapp-delivery-migration:8.0 .
```

## Развёртывание

Helm, Ingress (`/api/delivery`, `/api/internal/delivery`): [ДЗ 8 / K8s](../../ДЗ%208/K8s/README.md). Общий указатель: [HwApp/README.md](../README.md).

## Миграции

После обновления модели примените миграцию `CapacityBasedDeliveryModel`. Локально при необходимости сбросьте БД delivery и выполните migrate заново (production-данных нет).

```bash
cd HwApp/DeliveryService
dotnet ef database update --project DeliveryService.Infrastructure/DeliveryService.Infrastructure.csproj --startup-project DeliveryService.Api/DeliveryService.Api.csproj
```

Или через `DeliveryService.DbMigrator` в Docker/K8s.
