# AuthService

Сервис идентификации: регистрирует пользователей, проверяет логин и пароль, выпускает JWT с подписью RS256 и публикует JWKS.

## Ответственность

- регистрация пользователя с ролью `USER`;
- идемпотентный seed администратора через DbMigrator;
- аутентификация и выпуск access token;
- синхронное создание профиля в CustomerService;
- сохранение `user.activated.v1` в outbox и публикация в Kafka.

## Состав

- `AuthService.Api` — HTTP API, Swagger, health и metrics;
- `AuthService.Application` — сценарии регистрации и входа;
- `AuthService.Domain` — пользователи, роли и доменные события;
- `AuthService.Infrastructure` — EF Core, JWT/JWKS, Customer HTTP client, Kafka и outbox;
- `AuthService.DbMigrator` — миграции и seed администратора.

## Интеграции

| Направление | Система | Назначение |
|---|---|---|
| HTTP → | CustomerService | `POST /api/internal/customers` при регистрации |
| Kafka → | topic `auth` | `user.activated.v1` через outbox |
| PostgreSQL | `auth_db` | пользователи, refresh tokens и outbox |

В demo-режиме `OutboxPublisherEnabled=false`: событие сохраняется в таблицу outbox, но не отправляется, поскольку Kafka не устанавливается.

## API

| Метод | Путь | Доступ | Назначение |
|---|---|---|---|
| POST | `/api/auth/register` | публичный | Регистрация пользователя |
| POST | `/api/auth/login` | публичный | Получение JWT |
| GET | `/.well-known/jwks.json` | публичный | Публичный ключ для проверки JWT |

Swagger: `/api/auth/swagger`.

## Основная конфигурация

- `ConnectionStrings`;
- `Jwt`;
- `Ms:Customer`;
- `Kafka`;
- `OutboxPublisherEnabled`;
- `ADMIN_LOGIN`, `ADMIN_PASSWORD` для DbMigrator.

Полные Kubernetes values: [`Проектная работа/K8s/Helm/homework-apps`](../../Проектная%20работа/K8s/Helm/homework-apps).

## Сборка

```powershell
dotnet build AuthService.Api/AuthService.Api.csproj

docker build --platform linux/amd64 -f Dockerfile.Api -t maslovdeveloper/hwapp-auth-service:<tag> .
docker build --platform linux/amd64 -f Dockerfile.Migration -t maslovdeveloper/hwapp-auth-migration:<tag> .
```

## Эксплуатационные endpoints

- `/health/live`
- `/health/ready`
- `/health/startup`
- `/metrics`

Система целиком: [HwApp](../README.md). Развёртывание: [K8s](../../Проектная%20работа/K8s/README.md).
