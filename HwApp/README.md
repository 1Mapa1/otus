# HwApp

Здесь собраны микросервисы домашних заданий: общая точка входа для кода вне папок `ДЗ *`.

## Сервисы

| Проект | Описание |
|--------|----------|
| [CustomerService](./CustomerService/README.md) | CRUD клиентов, внешний API с JWT для `/api/customers/me`, внутренние ручки для AuthService |
| [AuthService](./AuthService/README.md) | Регистрация, вход, JWT, JWKS, вызов CustomerService при регистрации |

## Развёртывание

Сборка образов и выкладка в Kubernetes описаны в каталоге задания, например [ДЗ 6 / K8s](../ДЗ%206/K8s/README.md).
