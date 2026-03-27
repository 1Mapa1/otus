# ДЗ 4 — CRUD сервис с Kubernetes и Helm

## Цель

Реализовать REST CRUD сервис с базой данных и развернуть его в Kubernetes.

:contentReference[oaicite:1]{index=1}

## Структура проекта

CustomerService/ # .NET сервис + миграции
K8s/ # Kubernetes манифесты и Helm chart
Postman/ # Postman коллекция

## Сервис

👉 Customer-service:

CustomerService/README.md

## Быстрый старт

Инструкция по запуску:

👉 Kubernetes и Helm:

K8s/README.md

👉 Helm chart:

K8s/Helm/customer-service/README.md

👉 Postman тесты:

Postman/README.md

## Что реализовано

- CRUD API для пользователей
- PostgreSQL база данных
- Миграции через Kubernetes Job
- ConfigMap для конфигурации
- Secret для доступов к БД
- Ingress с host `arch.homework`
- Helm chart (шаблонизация)

## Проверка

Postman коллекция:

Postman/otus-hw4.postman_collection.json

:contentReference[oaicite:2]{index=2}