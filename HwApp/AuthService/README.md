# AuthService

Сервис аутентификации: учётные записи, JWT (RS256), публикация ключей (JWKS, OpenID discovery). При **регистрации** создаются запись клиента в **CustomerService** и счёт в **BillingService** (HTTP-клиенты по внутренним URL кластера).

## Структура решения

- `AuthService.Api` — HTTP API, health checks
- `AuthService.Application` — сценарии регистрации и входа
- `AuthService.Domain` — сущности и контракты репозитория
- `AuthService.Infrastructure` — EF Core, выпуск JWT, HTTP-клиенты к CustomerService и BillingService
- `AuthService.DbMigrator` — миграции БД

## Конфигурация интеграций

В `appsettings` задаются базовые URL внешних сервисов (типовые секции `Ms:Customer`, `Ms:Billing`) — см. также Helm values umbrella chart.

## API

Базовый путь: `/api/auth`.

| Метод | Путь | Описание |
|-------|------|----------|
| POST | `/api/auth/register` | Регистрация: пользователь в БД Auth + клиент в CustomerService + счёт в BillingService |
| POST | `/api/auth/login` | Вход, ответ с `accessToken` |

| Метод | Путь | Описание |
|-------|------|----------|
| GET | `/.well-known/jwks.json` | JWKS |
| GET | `/.well-known/openid-configuration` | Метаданные (issuer, jwks_uri и др.) |

Swagger UI (в Development): префикс `/api/auth/swagger`.

## Связанные сервисы

- [CustomerService](../CustomerService/README.md) — профиль клиента
- [BillingService](../BillingService/README.md) — счета

## Docker

```bash
docker build --platform linux/amd64 -f Dockerfile.Api .
docker build --platform linux/amd64 -f Dockerfile.Migration .
```

## Развёртывание

Теги образов и Ingress: [ДЗ 7 / K8s](../../ДЗ%207/K8s/README.md) (актуальный umbrella `homework-apps`). Ранние варианты стенда: [ДЗ 6 / K8s](../../ДЗ%206/K8s/README.md).
