# NotificationService

Сервис уведомлений: получает интеграционные события из Kafka, дедуплицирует их и сохраняет пользовательские уведомления.

## Ответственность

- обработка событий клиента и заказа;
- inbox-дедупликация по идентификатору события;
- retry и отправка необрабатываемого сообщения в DLQ;
- сохранение customer projection для отображения уведомлений;
- выдача списка и деталей уведомлений текущего пользователя.

## Состав

- `NotificationService.Api` — пользовательский API;
- `NotificationService.Application` — обработчики событий и запросы уведомлений;
- `NotificationService.Domain` — уведомление и customer projection;
- `NotificationService.Infrastructure` — EF Core, Kafka consumer, inbox и DLQ;
- `NotificationService.DbMigrator` — миграции БД.

## Интеграции

| Направление | Система | Назначение |
|---|---|---|
| Kafka ← | topic `customers` | `customer.created.v1`, `customer.updated.v1` |
| Kafka ← | topic `orders` | `order.confirmed.v1`, `order.rejected.v1/v2`, совместимость с `order.paid.v1` |
| HTTP → | AuthService | Загрузка JWKS |
| Kafka → | DLQ | сообщения после исчерпания retry |
| PostgreSQL | `notification_db` | уведомления, customer projection и inbox |

## API

Все endpoints требуют JWT.

| Метод | Путь | Назначение |
|---|---|---|
| GET | `/api/notifications/me` | Уведомления текущего пользователя |
| GET | `/api/notifications/{notificationId}` | Детали своего уведомления |

Swagger: `/api/notifications/swagger`.

## Основная конфигурация

- `ConnectionStrings`;
- `Auth:Url`;
- `Kafka` — bootstrap servers, group id, topics, retry и DLQ.

## Сборка

```powershell
dotnet build NotificationService.Api/NotificationService.Api.csproj

docker build --platform linux/amd64 -f Dockerfile.Api -t maslovdeveloper/hwapp-notification-service:<tag> .
docker build --platform linux/amd64 -f Dockerfile.Migration -t maslovdeveloper/hwapp-notification-migration:<tag> .
```

## Эксплуатационные endpoints

- `/health/live`
- `/health/ready`
- `/health/startup`
- `/metrics`

Система целиком: [HwApp](../README.md). Развёртывание: [K8s](../../Проектная%20работа/K8s/README.md).
