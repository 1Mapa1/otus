# ДЗ 4 — CRUD сервис с Kubernetes и Helm

## Цель

Реализовать REST CRUD сервис с базой данных и развернуть его в Kubernetes.

## Структура проекта

CustomerService/ # .NET сервис + миграции

K8s/ # Kubernetes манифесты и Helm chart

Postman/ # Postman коллекция

## Сервис

👉 Customer-service:

- [CustomerService](./CustomerService/)

## Быстрый старт

Инструкция по запуску:

👉 Kubernetes и Helm:

- [K8s](./K8s/)

👉 Helm chart:

- [Helm](./K8s/Helm/customer-service/)

👉 Postman тесты:

- [Postman](./Postman/)

## Что реализовано

- CRUD API для пользователей
- PostgreSQL база данных
- Миграции через Kubernetes Job
- ConfigMap для конфигурации
- Secret для доступов к БД
- Ingress с host `arch.homework`
- Helm chart (шаблонизация)