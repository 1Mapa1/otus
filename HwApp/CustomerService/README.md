# CustomerService

REST API для управления клиентами (CRUD). Операции с «текущим» профилем защищены JWT: валидация подписи по **JWKS AuthService** (кэш ключей с ленивой загрузкой, настройка через `CustomerService.Api/Authentication`).

Внутренний контроллер **`POST /api/internal/customers`** вызывается **AuthService** при регистрации (создание клиента по `Id` пользователя). События домена уходят в **Kafka** через паттерн **outbox** (`OutboxPublisher`, см. `CustomerService.Infrastructure`).

## Архитектура

- `CustomerService.Api` — HTTP API (внешние и internal-контроллеры), AutoMapper, JWT, Swagger
- `CustomerService.Domain` — сущность клиента, доменные события, контракт репозитория
- `CustomerService.Infrastructure` — EF Core, репозитории, outbox, продюсер Kafka
- `CustomerService.DbMigrator` — миграции БД

## API

Пример хоста за Ingress: `http://arch.homework` (см. [ДЗ 7 / K8s](../../ДЗ%207/K8s/README.md)).

### CRUD

| Метод | Путь |
|-------|------|
| POST | `/api/customers` |
| GET | `/api/customers/{id}` |
| PUT | `/api/customers/{id}` |
| DELETE | `/api/customers/{id}` |

### Профиль по JWT

Заголовок `Authorization: Bearer <token>` (issuer — AuthService).

| Метод | Путь |
|-------|------|
| GET | `/api/customers/me` |
| PUT | `/api/customers/me` |

### Внутренний API (для Auth)

| Метод | Путь | Описание |
|-------|------|----------|
| POST | `/api/internal/customers` | Идемпотентное создание клиента при регистрации |

### Документация и служебные пути

- Swagger UI (Development): `/api/customers/swagger`
- Метрики Prometheus (HTTP): **`/metrics`**
- Health: `/health/live`, `/health/ready`, `/health/startup`

## Связанные сервисы

- [AuthService](../AuthService/README.md) — JWT и регистрация

## Сборка Docker-образов

```bash
docker build --platform linux/amd64 -f Dockerfile.Api .
docker build --platform linux/amd64 -f Dockerfile.Migration .
```

## Развёртывание

Helm и маршрут Ingress `/api/customers`: [ДЗ 7 / K8s](../../ДЗ%207/K8s/README.md). Общий указатель по репозиторию: [HwApp/README.md](../README.md).
