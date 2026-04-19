# Customer Service

REST API для управления клиентами (CRUD). Внешние операции с «текущим» профилем защищены JWT (валидация по JWKS AuthService).

## Архитектура

- CustomerService.Api — HTTP API
- CustomerService.Domain — доменная модель
- CustomerService.Infrastructure — работа с БД (EF Core)
- CustomerService.DbMigrator — миграции базы данных

## API

Базовый URL (пример с Ingress): `http://arch.homework`

### CRUD (как в предыдущих ДЗ)

| Метод | Путь |
|-------|------|
| POST | `/api/customers` |
| GET | `/api/customers/{id}` |
| PUT | `/api/customers/{id}` |
| DELETE | `/api/customers/{id}` |

### Профиль по JWT (ДЗ 6)

Требуется заголовок `Authorization: Bearer <token>`.

| Метод | Путь |
|-------|------|
| GET | `/api/customers/me` |
| PUT | `/api/customers/me` |

### Документация

**GET** `/swagger`

## Связь с репозиторием

Код в каталоге [HwApp](../README.md). Развёртывание — в README соответствующего задания (например [ДЗ 6 / K8s](../../ДЗ%206/K8s/README.md)).

## Сборка Docker-образа

### Сервис 

```bash
docker build --platform linux/amd64 -f Dockerfile.Api .
```

### Миграции

```bash
docker build --platform linux/amd64 -f Dockerfile.Migration .
```