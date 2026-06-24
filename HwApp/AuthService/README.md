# AuthService

Сервис аутентификации: учётные записи, JWT (RS256), публикация JWKS. При **регистрации** создаётся запись клиента в **CustomerService** (HTTP), после успешной активации пользователя публикуется событие `user.activated.v1` в **Kafka** через outbox.

## Структура решения

- `AuthService.Api` — HTTP API, health checks
- `AuthService.Application` — сценарии регистрации и входа
- `AuthService.Domain` — сущности, доменные события и контракты репозитория
- `AuthService.Infrastructure` — EF Core, выпуск JWT, HTTP-клиент к CustomerService, Kafka producer, outbox
- `AuthService.DbMigrator` — миграции БД

## Конфигурация интеграций

В `appsettings` задаются:

- `Ms:Customer` — HTTP-клиент CustomerService
- `Kafka` — bootstrap servers и параметры producer (outbox publisher)
- `Jwt` — issuer, lifetime, ключи RS256

См. также Helm values umbrella chart в [Проектная работа / K8s](../../Проектная%20работа/K8s/Helm/homework-apps).

## API

Базовый путь: `/api/auth`.

| Метод | Путь | Описание |
|-------|------|----------|
| POST | `/api/auth/register` | Регистрация: пользователь в БД Auth + клиент в CustomerService + событие `user.activated.v1` |
| POST | `/api/auth/login` | Вход, ответ с `accessToken` (JWT с ролью) |

| Метод | Путь | Описание |
|-------|------|----------|
| GET | `/.well-known/jwks.json` | JWKS для проверки подписи JWT |

Swagger UI (в Development): префикс `/api/auth/swagger`.

## Kafka events

Публикует (topic `auth`):

- `user.activated.v1` — пользователь активирован после успешной регистрации (`UserId` в payload)

## Связанные сервисы

- [CustomerService](../CustomerService/README.md) — профиль клиента при регистрации
- [BillingService](../BillingService/README.md) — потребляет `user.activated.v1` (асинхронное создание счёта)

## Docker

```bash
docker build --platform linux/amd64 -f Dockerfile.Api .
docker build --platform linux/amd64 -f Dockerfile.Migration .
```

## Развёртывание

Helm chart: [Проектная работа / K8s / Helm](../../Проектная%20работа/K8s/Helm/homework-apps).
