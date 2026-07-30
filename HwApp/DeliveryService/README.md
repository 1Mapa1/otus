# DeliveryService

Сервис доставки: управляет зонами и слотами с ограниченной capacity, резервирует слот для заказа и выполняет компенсационную отмену.

## Ответственность

- административное управление зонами и слотами;
- поиск доступных слотов по адресу;
- атомарное увеличение `ReservedCount`;
- идемпотентный резерв и отмена по `orderId`;
- хранение snapshot адреса доставки.

## Состав

- `DeliveryService.Api` — user, admin и internal API;
- `DeliveryService.Application` — команды и запросы;
- `DeliveryService.Domain` — зоны, слоты и резервы;
- `DeliveryService.Infrastructure` — EF Core и атомарные операции capacity;
- `DeliveryService.DbMigrator` — миграции БД.

## Интеграции

| Направление | Система | Назначение |
|---|---|---|
| HTTP ← | OrderService | Резерв слота и компенсационная отмена |
| HTTP → | AuthService | Загрузка JWKS |
| PostgreSQL | `delivery_db` | зоны, слоты и резервы |

Kafka, outbox и inbox в этом сервисе не используются.

## API

| Метод | Путь | Доступ | Назначение |
|---|---|---|---|
| POST | `/api/delivery/slots/available` | JWT | Найти доступные слоты по адресу |
| GET | `/api/delivery/zones` | `ADMIN` | Получить зоны |
| POST | `/api/delivery/zones` | `ADMIN` | Создать зону |
| PUT | `/api/delivery/zones/{zoneId}` | `ADMIN` | Изменить зону |
| GET | `/api/delivery/slots` | `ADMIN` | Получить слоты |
| POST | `/api/delivery/slots` | `ADMIN` | Создать слот |
| PUT | `/api/delivery/slots/{slotId}` | `ADMIN` | Изменить слот |
| POST | `/api/internal/delivery/reservations` | internal | Идемпотентный резерв |
| POST | `/api/internal/delivery/reservations/cancel` | internal | Идемпотентная отмена |

Swagger: `/api/delivery/swagger`.

## Основная конфигурация

- `ConnectionStrings`;
- `Auth:Url`.

## Сборка

```powershell
dotnet build DeliveryService.Api/DeliveryService.Api.csproj

docker build --platform linux/amd64 -f Dockerfile.Api -t maslovdeveloper/hwapp-delivery-service:<tag> .
docker build --platform linux/amd64 -f Dockerfile.Migration -t maslovdeveloper/hwapp-delivery-migration:<tag> .
```

## Эксплуатационные endpoints

- `/health/live`
- `/health/ready`
- `/health/startup`
- `/metrics`

Система целиком: [HwApp](../README.md). Развёртывание: [K8s](../../Проектная%20работа/K8s/README.md).
