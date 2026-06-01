# HwApp

Здесь собраны микросервисы домашних заданий: общая точка входа для кода вне папок `ДЗ *`.

## Сервисы

| Проект | Описание |
|--------|----------|
| [AuthService](./AuthService/README.md) | Регистрация, вход, JWT, JWKS; при регистрации — CustomerService и BillingService (счёт) |
| [CustomerService](./CustomerService/README.md) | Клиенты, внешний API с JWT (`/api/customers/me`), внутренние ручки для Auth |
| [BillingService](./BillingService/README.md) | Счета, пополнение, списание; внутренний API для Auth и Order (authorize/capture/cancel) |
| [WarehouseService](./WarehouseService/README.md) | Каталог и склад, резервы под заказ; внутренний API для Order |
| [DeliveryService](./DeliveryService/README.md) | Слоты доставки, резерв под заказ; внутренний API для Order |
| [NotificationService](./NotificationService/README.md) | Kafka consumer, сохранение уведомлений в БД, HTTP API |
| [OrderService](./OrderService/README.md) | Заказы, **сага** (Billing + Warehouse + Delivery), события в Kafka (outbox) |

У каждого сервиса в таблице — ссылка на **`README.md` в каталоге проекта**; детали API дополнительно смотрите в Swagger в образе.

## Развёртывание

Сборка образов и выкладка в Kubernetes: [ДЗ 8 / K8s](../ДЗ%208/K8s/README.md) (полный стенд: заказ с сагой, склад, доставка, биллинг, Kafka и др.). Ранее: [ДЗ 7 / K8s](../ДЗ%207/K8s/README.md). Для более ранних заданий см. также [ДЗ 6 / K8s](../ДЗ%206/K8s/README.md).
