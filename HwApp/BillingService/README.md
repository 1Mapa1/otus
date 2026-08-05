# BillingService

Сервис счетов и платежей: хранит баланс пользователя, выполняет пополнение и управляет авторизацией платежа заказа.

## Ответственность

- идемпотентное создание счёта по `user.activated.v1`;
- чтение баланса и пополнение счёта;
- authorize суммы заказа с переводом средств в hold;
- capture после успешной саги;
- cancel authorization как компенсация.

## Состав

- `BillingService.Api` — пользовательский и internal API;
- `BillingService.Application` — счета и платежные операции;
- `BillingService.Domain` — Account, Payment и транзакции;
- `BillingService.Infrastructure` — EF Core, Kafka consumer, inbox/DLQ;
- `BillingService.DbMigrator` — миграции БД.

## Интеграции

| Направление | Система | Назначение |
|---|---|---|
| Kafka ← | topic `auth` | `user.activated.v1` создаёт счёт |
| HTTP ← | OrderService | authorize, capture и cancel |
| HTTP → | AuthService | Загрузка JWKS |
| Kafka → | DLQ | сообщения, которые невозможно обработать |
| PostgreSQL | `billing_db` | счета, платежи, транзакции и inbox |

## API

| Метод | Путь | Доступ | Назначение |
|---|---|---|---|
| GET | `/api/billing/accounts/me` | JWT | Получить свой счёт |
| POST | `/api/billing/accounts/deposit` | JWT | Пополнить счёт |
| POST | `/api/internal/billing/payments/authorize` | internal | Авторизовать сумму |
| POST | `/api/internal/billing/payments/capture` | internal | Списать авторизованную сумму |
| POST | `/api/internal/billing/payments/cancel-authorization` | internal | Отменить авторизацию |

Swagger: `/api/billing/swagger`.

## Основная конфигурация

- `ConnectionStrings`;
- `Auth:Url`;
- `Kafka`.

## Сборка

```powershell
dotnet build BillingService.Api/BillingService.Api.csproj

docker build --platform linux/amd64 -f Dockerfile.Api -t maslovdeveloper/hwapp-billing-service:<tag> .
docker build --platform linux/amd64 -f Dockerfile.Migration -t maslovdeveloper/hwapp-billing-migration:<tag> .
```

## Эксплуатационные endpoints

- `/health/live`
- `/health/ready`
- `/health/startup`
- `/metrics`

Система целиком: [HwApp](../README.md). Развёртывание: [K8s](../../Проектная%20работа/K8s/README.md).
