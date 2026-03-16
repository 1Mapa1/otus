# ДЗ 2 — Приложение в Docker-образе

## Цель задания

Обернуть приложение в Docker-образ и загрузить его в Docker Hub.

---

## Реализация

Создан минимальный сервис на **ASP.NET Core**, который:

- работает на порту **8000**
- поддерживает endpoint **GET /health/**
- возвращает JSON:

```json
{"status": "OK"}
```

## Ссылки

🐳 Dockerfile:
[DockerFile](https://github.com/1Mapa1/otus/blob/main/ДЗ%202/Health/Dockerfile)

🐳 Docker Hub образ:
[DockerHub](https://hub.docker.com/repository/docker/maslovdeveloper/otus-hw2)

## Сборка Docker-образа

```bash
docker build --platform linux/amd64 .
```

## Запуск контейнера
```bash
docker run -p 8000:8000 maslovdeveloper/otus-hw2:1.0
```

## Проверка сервиса

```bash
curl http://localhost:8000/health/
```