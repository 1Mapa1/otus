# BillingService

Сервис **счетов и платежей**: у пользователя есть счёт (баланс, холдированные средства), пополнение, а для сценария заказа — **внутреннее API платежей** (authorize → capture или отмена авторизации). Счёт создаётся при регистрации через **AuthService** (`POST /api/internal/billing/accounts`). **OrderService** вызывает платежи по внутренним ручкам без JWT пользователя.

## Архитектура

- `BillingService.Api` — HTTP API (внешние и internal-контроллеры), JWT на внешних ручках, Swagger
- `BillingService.Application` — MediatR: счета, депозит, authorize/capture/cancel authorization
- `BillingService.Domain` — счёт, транзакции, состояния платежа
- `BillingService.Infrastructure` — EF Core, репозитории, персистентность
- `BillingService.DbMigrator` — миграции БД

## API

Пример хоста за Ingress: `http://arch.homework` (см. [ДЗ 8 / K8s](../../ДЗ%208/K8s/README.md)).

### Внешний API (JWT пользователя)

Префикс: **`/api/billing/accounts`**. Заголовок `Authorization: Bearer <token>`.

| Метод | Путь | Описание |
|-------|------|----------|
| GET | `/api/billing/accounts/me` | Текущий счёт: баланс, холд, доступно |
| POST | `/api/billing/accounts/deposit` | Пополнение счёта |

### Внутренний API — счета

Префикс: **`/api/internal/billing/accounts`**.

| Метод | Путь | Описание |
|-------|------|----------|
| POST | `/api/internal/billing/accounts` | Создание счёта для `userId` (вызывает Auth при регистрации) |

### Внутренний API — платежи по заказу

Префикс: **`/api/internal/billing/payments`**. Вызывается **OrderService** (сага заказа).

| Метод | Путь | Описание |
|-------|------|----------|
| POST | `/api/internal/billing/payments/authorize` | Авторизация суммы по `orderId` (холд) |
| POST | `/api/internal/billing/payments/capture` | Списание после успешных шагов саги |
| POST | `/api/internal/billing/payments/cancel-authorization` | Отмена холда (компенсация) |

### Документация и health

- Swagger UI (Development): **`/api/billing/swagger`**
- Health: `/health/live`, `/health/ready`, `/health/startup`

## Связанные сервисы

- [AuthService](../AuthService/README.md) — создание счёта при регистрации
- [OrderService](../OrderService/README.md) — сага заказа (authorize / capture / cancel)

## Сборка Docker-образов

Из каталога `BillingService/`:

```bash
docker build --platform linux/amd64 -f Dockerfile.Api -t maslovdeveloper/hwapp-billing-service:8.0 .
docker build --platform linux/amd64 -f Dockerfile.Migration -t maslovdeveloper/hwapp-billing-migration:8.0 .
```

(Тег подставьте свой; общий скрипт см. [HwApp/build-images-8.0.sh](../build-images-8.0.sh).)

## Развёртывание

Helm, Ingress (`/api/billing`, `/api/internal/billing`): [ДЗ 8 / K8s](../../ДЗ%208/K8s/README.md). Общий указатель: [HwApp/README.md](../README.md).
