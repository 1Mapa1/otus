# CustomerService

Сервис профиля клиента и адресной книги. Идентификатор клиента совпадает с `sub` пользователя из JWT AuthService.

## Ответственность

- идемпотентное создание профиля по internal HTTP-вызову AuthService;
- чтение и изменение собственного профиля;
- создание, изменение и soft delete адресов;
- публикация событий клиента через outbox.

## Состав

- `CustomerService.Api` — внешний и internal API, JWT, Swagger, health и metrics;
- `CustomerService.Domain` — профиль, адреса и доменные события;
- `CustomerService.Infrastructure` — EF Core, Kafka и outbox;
- `CustomerService.DbMigrator` — миграции БД.

## Интеграции

| Направление | Система | Назначение |
|---|---|---|
| HTTP ← | AuthService | Создание профиля при регистрации |
| HTTP → | AuthService | Загрузка JWKS |
| Kafka → | topic `customers` | `customer.created.v1`, `customer.updated.v1` |
| PostgreSQL | `customer_db` | профили, адреса и outbox |

В demo-режиме `OutboxPublisherEnabled=false`: записи outbox создаются без отправки в Kafka.

## API

| Метод | Путь | Доступ | Назначение |
|---|---|---|---|
| GET | `/api/customers/me` | JWT | Получить свой профиль |
| PUT | `/api/customers/me` | JWT | Изменить свой профиль |
| GET | `/api/customers/me/addresses` | JWT | Получить активные адреса |
| POST | `/api/customers/me/addresses` | JWT | Создать адрес |
| PUT | `/api/customers/me/addresses/{addressId}` | JWT | Изменить адрес |
| DELETE | `/api/customers/me/addresses/{addressId}` | JWT | Деактивировать адрес |
| POST | `/api/internal/customers` | internal | Идемпотентно создать профиль |

Swagger: `/api/customers/swagger`.

## Основная конфигурация

- `ConnectionStrings`;
- `Auth:Url`;
- `Kafka`;
- `OutboxPublisherEnabled`.

## Сборка

```powershell
dotnet build CustomerService.Api/CustomerService.Api.csproj

docker build --platform linux/amd64 -f Dockerfile.Api -t maslovdeveloper/hwapp-customer-service:<tag> .
docker build --platform linux/amd64 -f Dockerfile.Migration -t maslovdeveloper/hwapp-customer-migration:<tag> .
```

## Эксплуатационные endpoints

- `/health/live`
- `/health/ready`
- `/health/startup`
- `/metrics`

Система целиком: [HwApp](../README.md). Развёртывание: [K8s](../../Проектная%20работа/K8s/README.md).
