# HwApp

Здесь собраны микросервисы домашних заданий: общая точка входа для кода вне папок `ДЗ *`.

## Сервисы

| Проект | Описание |
|--------|----------|
| [AuthService](./AuthService/README.md) | Регистрация, вход, JWT, JWKS; при регистрации — CustomerService и BillingService (счёт) |
| [CustomerService](./CustomerService/README.md) | Клиенты, внешний API с JWT (`/api/customers/me`), внутренние ручки для Auth |
| [BillingService](./BillingService/README.md) | Счета, пополнение, списание; внутренний API для Auth и Order (authorize/capture/cancel) |
| [WarehouseService](./WarehouseService/README.md) | Складские остатки, резервы под заказ; Kafka lifecycle от Catalog |
| [CatalogService](./CatalogService/README.md) | Каталог товаров, витрина, snapshot для Order; Kafka lifecycle + StockChanged |
| [DeliveryService](./DeliveryService/README.md) | Слоты доставки, резерв под заказ; внутренний API для Order |
| [NotificationService](./NotificationService/README.md) | Kafka consumer, сохранение уведомлений в БД, HTTP API |
| [OrderService](./OrderService/README.md) | Заказы, **сага** (Billing + Warehouse + Delivery), события в Kafka (outbox) |

У каждого сервиса в таблице — ссылка на **`README.md` в каталоге проекта**; детали API дополнительно смотрите в Swagger в образе.

## Развёртывание

Сборка и push всех образов (16 шт., тег по умолчанию `10.0`):

```powershell
cd HwApp
docker login
.\build-push.ps1              # build + push
.\build-push.ps1 -Action build  # только сборка
.\build-push.ps1 -Service catalog  # один сервис
```

Сборка образов и выкладка в Kubernetes: [Проектная работа / K8s](../Проектная%20работа/K8s/README.md). Ранее: [ДЗ 8 / K8s](../ДЗ%208/K8s/README.md), [ДЗ 7 / K8s](../ДЗ%207/K8s/README.md).
