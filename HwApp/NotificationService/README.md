# NotificationService

Сервис **уведомлений**: потребление событий из **Kafka**, сохранение уведомлений в БД и **HTTP API** для чтения списка. Топики и группа consumer настраиваются (см. Helm `values.yaml` у notification-service). События публикуют другие сервисы (например, **OrderService** через outbox).

## Архитектура

- `NotificationService.Api` — HTTP API, health
- `NotificationService.Application` — MediatR: обработка входящих событий, запросы списка уведомлений
- `NotificationService.Domain` — сущность уведомления
- `NotificationService.Infrastructure` — EF Core, **Kafka consumer**, персистентность
- `NotificationService.DbMigrator` — миграции БД

## Конфигурация Kafka

В `appsettings` / Helm: bootstrap-серверы, **group id**, список **topics** для подписки — должны совпадать с настройками продюсеров в кластере (см. umbrella `notificationService.config`).

## API

Пример хоста за Ingress: `http://arch.homework` (см. [ДЗ 8 / K8s](../../ДЗ%208/K8s/README.md)).

Префикс приложения: **`/api/notifications`** (детальные маршруты и тело ответов — в **Swagger** в образе).

### Документация и служебные пути

- Swagger UI (Development): **`/api/notifications/swagger`**
- Health: `/health/live`, `/health/ready`, `/health/startup`

## Связанные сервисы

- [OrderService](../OrderService/README.md) — доменные события в Kafka (outbox)

## Сборка Docker-образов

Из каталога `NotificationService/`:

```bash
docker build --platform linux/amd64 -f Dockerfile.Api -t maslovdeveloper/hwapp-notification-service:8.0 .
docker build --platform linux/amd64 -f Dockerfile.Migration -t maslovdeveloper/hwapp-notification-migration:8.0 .
```

## Развёртывание

Kafka в том же namespace, что и приложения; Helm и Ingress `/api/notifications`: [ДЗ 8 / K8s](../../ДЗ%208/K8s/README.md). Общий указатель: [HwApp/README.md](../README.md).
