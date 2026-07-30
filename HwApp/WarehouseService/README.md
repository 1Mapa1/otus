# WarehouseService

Сервис складских остатков: хранит доступное и зарезервированное количество, движения склада и резервы товаров.

## Ответственность

- создание складской записи по lifecycle-событию товара;
- поступление товара и история движений;
- идемпотентный резерв и отмена резерва для саги заказа;
- атомарное изменение остатков с блокировкой строки;
- публикация изменения доступного количества.

## Состав

- `WarehouseService.Api` — admin и internal API;
- `WarehouseService.Application` — команды остатков, движений и резервов;
- `WarehouseService.Domain` — StockItem, StockReservation и StockMovement;
- `WarehouseService.Infrastructure` — EF Core, row lock, Kafka и outbox;
- `WarehouseService.DbMigrator` — миграции БД.

## Интеграции

| Направление | Система | Назначение |
|---|---|---|
| HTTP ← | OrderService | Резерв и компенсационная отмена |
| Kafka ← | topic `products` | lifecycle товара |
| Kafka → | topic `stocks` | `stock.changed.v1` |
| HTTP → | AuthService | Загрузка JWKS |
| PostgreSQL | `warehouse_db` | остатки, движения, резервы и outbox |

## API

| Метод | Путь | Доступ | Назначение |
|---|---|---|---|
| GET | `/api/warehouse/stocks` | `ADMIN` | Все складские записи |
| GET | `/api/warehouse/stocks/{productId}` | `ADMIN` | Остаток товара |
| POST | `/api/warehouse/stocks/{productId}/income` | `ADMIN` | Поступление |
| GET | `/api/warehouse/stocks/{productId}/movements` | `ADMIN` | Движения товара |
| POST | `/api/internal/warehouse/reservations` | internal | Идемпотентный резерв |
| POST | `/api/internal/warehouse/reservations/cancel` | internal | Идемпотентная отмена |

Swagger: `/api/warehouse/swagger`.

## Основная конфигурация

- `ConnectionStrings`;
- `Auth:Url`;
- `Kafka`.

## Сборка

```powershell
dotnet build WarehouseService.Api/WarehouseService.Api.csproj

docker build --platform linux/amd64 -f Dockerfile.Api -t maslovdeveloper/hwapp-warehouse-service:<tag> .
docker build --platform linux/amd64 -f Dockerfile.Migration -t maslovdeveloper/hwapp-warehouse-migration:<tag> .
```

## Эксплуатационные endpoints

- `/health/live`
- `/health/ready`
- `/health/startup`
- `/metrics`

Система целиком: [HwApp](../README.md). Развёртывание: [K8s](../../Проектная%20работа/K8s/README.md).
