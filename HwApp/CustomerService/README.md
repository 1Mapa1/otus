# CustomerService

Сервис профиля клиента и адресной книги. `Customer.Id` совпадает с `User.Id` из AuthService — `customerId` для пользовательских endpoint'ов берётся из JWT claim `sub`.

При регистрации **AuthService** вызывает internal API и создаёт профиль. Изменения профиля публикуются в **Kafka** через outbox.

## Структура решения

- `CustomerService.Api` — HTTP API (внешние и internal-контроллеры), AutoMapper, JWT, Swagger
- `CustomerService.Domain` — сущности `Customer`, `CustomerAddress`, доменные события, контракты репозиториев
- `CustomerService.Infrastructure` — EF Core, репозитории, outbox, Kafka producer
- `CustomerService.DbMigrator` — миграции БД

## Конфигурация

В `appsettings` задаются:

- `Auth:Url` — базовый URL AuthService для загрузки JWKS
- `Kafka` — bootstrap servers и параметры producer (outbox publisher)
- строка подключения к PostgreSQL

См. также Helm values umbrella chart в [Проектная работа / K8s](../../Проектная%20работа/K8s/Helm/homework-apps).

## API

Базовый путь: `/api/customers`.

Пользовательские endpoint'ы требуют `Authorization: Bearer <token>`.

### Профиль

| Метод | Путь | Описание |
|-------|------|----------|
| GET | `/api/customers/me` | Получить свой профиль |
| PUT | `/api/customers/me` | Обновить свой профиль |

### Адресная книга

Сохранённые адреса для checkout. Разовый адрес заказа может быть указан без сохранения в CustomerService.

`customerId` не передаётся в route/body — только из JWT.

| Метод | Путь | Описание |
|-------|------|----------|
| GET | `/api/customers/me/addresses` | Список активных адресов (`IsActive = true`) |
| POST | `/api/customers/me/addresses` | Создать адрес |
| PUT | `/api/customers/me/addresses/{addressId}` | Изменить свой активный адрес |
| DELETE | `/api/customers/me/addresses/{addressId}` | Деактивировать адрес (soft delete, `IsActive = false`) |

### Внутренний API

| Метод | Путь | Описание |
|-------|------|----------|
| POST | `/api/internal/customers` | Идемпотентное создание профиля при регистрации (AuthService) |

Swagger UI (в Development): `/api/customers/swagger`.

Health: `/health/live`, `/health/ready`, `/health/startup`.

## Kafka events

Публикует (topic `customers`):

- `customer.created.v1` — профиль создан при регистрации
- `customer.updated.v1` — профиль обновлён

## Связанные сервисы

- [AuthService](../AuthService/README.md) — JWT, регистрация, создание профиля
- [NotificationService](../NotificationService/README.md) — потребляет события клиента

## Docker

```bash
docker build --platform linux/amd64 -f Dockerfile.Api .
docker build --platform linux/amd64 -f Dockerfile.Migration .
```

## Развёртывание

Helm chart: [Проектная работа / K8s / Helm](../../Проектная%20работа/K8s/Helm/homework-apps).
