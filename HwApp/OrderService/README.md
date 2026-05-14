# OrderService

Создание заказа, списание средств через BillingService, публикация доменных событий в Kafka через outbox.

## Структура решения

- `OrderService.Api` — HTTP API
- `OrderService.Application` — команды и запросы (создание заказа, список, детали)
- `OrderService.Domain` — заказ, статусы, доменные события
- `OrderService.Infrastructure` — EF Core, клиент Billing, Kafka producer, outbox
- `OrderService.DbMigrator` — миграции БД

## API

Базовый путь: `/api/orders` (см. Swagger в образе).

## Развёртывание

Postgres, Kafka и Ingress: [ДЗ 7 / K8s](../../ДЗ%207/K8s/README.md).
