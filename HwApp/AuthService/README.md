# AuthService

Сервис аутентификации: учётные записи, JWT (RS256), публикация ключей, интеграция с CustomerService при регистрации.

## Структура решения

- `AuthService.Api` — HTTP API
- `AuthService.Application` — сценарии регистрации и входа
- `AuthService.Domain` — сущности и контракты репозитория
- `AuthService.Infrastructure` — EF Core, JWT, HTTP-клиент к CustomerService
- `AuthService.DbMigrator` — миграции БД

## API

Базовый путь: `/api/auth`.

| Метод | Путь | Описание |
|-------|------|----------|
| POST | `/api/auth/register` | Регистрация (создание пользователя и клиента во внешнем сервисе) |
| POST | `/api/auth/login` | Вход, ответ с `accessToken` |

| Метод | Путь | Описание |
|-------|------|----------|
| GET | `/.well-known/jwks.json` | JWKS |
| GET | `/.well-known/openid-configuration` | Метаданные (issuer, jwks_uri и др.) |

## Docker

```bash
docker build --platform linux/amd64 -f Dockerfile.Api .
docker build --platform linux/amd64 -f Dockerfile.Migration .
```

Теги и публикация образов задаются в Helm values задания (см. [ДЗ 6 / K8s](../../ДЗ%206/K8s/README.md)).
