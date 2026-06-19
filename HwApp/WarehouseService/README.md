# WarehouseService

Сервис **склада и каталога товаров**: продукты, остатки, **резервирование** товара под заказ и отмена резерва. Внешнее API с JWT для администрирования каталога; внутренние ручки без пользовательского JWT вызывает **OrderService** при саге (resolve SKU → create/cancel reservation).

## Архитектура

- `WarehouseService.Api` — HTTP API (external + internal), JWT на внешних ручках, Swagger
- `WarehouseService.Application` — MediatR: продукты, остатки, resolve, резервы
- `WarehouseService.Domain` — продукт, резерв, статусы
- `WarehouseService.Infrastructure` — EF Core, репозитории, персистентность
- `WarehouseService.DbMigrator` — миграции БД

## API

Пример хоста за Ingress: `http://arch.homework` (см. [ДЗ 8 / K8s](../../ДЗ%208/K8s/README.md)).

### Внешний API (JWT)

Префикс: **`/api/warehouse/products`**.

| Метод | Путь | Описание |
|-------|------|----------|
| GET | `/api/warehouse/products` | Список товаров |
| POST | `/api/warehouse/products` | Создание товара |
| POST | `/api/warehouse/products/{id}/stock` | Увеличение остатка (поступление на склад) |

### Внутренний API

| Метод | Путь | Описание |
|-------|------|----------|
| POST | `/api/internal/warehouse/products/resolve` | Проверка/разрешение позиций (SKU, количество) для заказа |
| POST | `/api/internal/warehouse/reservations` | Резерв под `orderId` + позиции |
| POST | `/api/internal/warehouse/reservations/cancel` | Отмена резерва по `orderId` (компенсация саги) |

### Документация и health

- Swagger UI (Development): **`/api/warehouse/swagger`**
- Health: `/health/live`, `/health/ready`, `/health/startup`

## Связанные сервисы

- [OrderService](../OrderService/README.md) — сага заказа (резерв / отмена)

## Сборка Docker-образов

Из каталога `WarehouseService/`:

```bash
docker build --platform linux/amd64 -f Dockerfile.Api -t maslovdeveloper/hwapp-warehouse-service:8.0 .
docker build --platform linux/amd64 -f Dockerfile.Migration -t maslovdeveloper/hwapp-warehouse-migration:8.0 .
```

## Развёртывание

Helm, Ingress (`/api/warehouse`, `/api/internal/warehouse`): [ДЗ 8 / K8s](../../ДЗ%208/K8s/README.md). Общий указатель: [HwApp/README.md](../README.md).
