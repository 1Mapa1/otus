# Customer Service

REST API для управления пользователями (CRUD).

## Архитектура

- CustomerService.Api — HTTP API
- CustomerService.Domain — доменная модель
- CustomerService.Infrastructure — работа с БД (EF Core)
- CustomerService.DbMigrator — миграции базы данных

## API

Base URL:
http://arch.homework
Примеры ручек

(сильно не раздувай, просто ключевые)

### Создать пользователя

POST /api/customers

### Получить пользователя

GET /api/customers/{id}

### Обновить пользователя

PUT /api/customers/{id}

### Удалить пользователя

DELETE /api/customers/{id}

### Swagger:
http://arch.homework/swagger

## Сборка Docker-образа

### Сервис 

```bash
docker build --platform linux/amd64 -f Dockerfile.Api .
```

### Миграции

```bash
docker build --platform linux/amd64 -f Dockerfile.Migration .
```