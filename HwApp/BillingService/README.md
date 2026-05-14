# BillingService

Счета, пополнение и списание; внутреннее API для Auth (создание счёта при регистрации) и Order (списание при оплате заказа).

## Структура решения

- `BillingService.Api` — HTTP API
- `BillingService.Application` — сценарии операций со счётом
- `BillingService.Domain` — сущности счёта и транзакций
- `BillingService.Infrastructure` — EF Core, персистентность
- `BillingService.DbMigrator` — миграции БД

## API

Базовые пути: внешний `/api/billing`, внутренний `/api/internal/billing` (см. Swagger в образе).

## Развёртывание

Helm и Ingress: [ДЗ 7 / K8s](../../ДЗ%207/K8s/README.md).
