# NotificationService

Потребление событий из Kafka, сохранение уведомлений в БД, HTTP API для чтения списка уведомлений.

## Структура решения

- `NotificationService.Api` — HTTP API
- `NotificationService.Application` — обработчики и запросы
- `NotificationService.Domain` — сущность уведомления
- `NotificationService.Infrastructure` — EF Core, Kafka consumer
- `NotificationService.DbMigrator` — миграции БД

## API

Базовый путь: `/api/notifications` (см. Swagger в образе).

## Развёртывание

Kafka и приложения: [ДЗ 7 / K8s](../../ДЗ%207/K8s/README.md).
