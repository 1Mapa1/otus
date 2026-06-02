# ДЗ 9 — Идемпотентное создание заказа

Метод **создания заказа** в **OrderService** сделан **идемпотентным**: повтор запроса с тем же ключом не приводит к дублированию заказа; конфликт при другом теле запроса обнаруживается и отклоняется. Код: [HwApp](../HwApp/). Описание паттерна: [Архитектура.md](./Архитектура/Архитектура.md). Развёртывание: [K8s](./K8s/). Проверка: [Postman](./Postman/).

| Раздел задания | Куда смотреть |
|----------------|---------------|
| **0)** какой паттерн идемпотентности | [Архитектура/Архитектура.md](./Архитектура/Архитектура.md) |
| **1)** установка (namespace, Helm) | [K8s/README.md](./K8s/README.md), chart [homework-apps](./K8s/Helm/homework-apps/README.md) |
| **2)** Postman / Newman (`{{baseUrl}}`, `arch.homework`, `--verbose`) | [Postman/README.md](./Postman/README.md) |

**Namespace** и порядок установки Postgres / Kafka / приложений — в [K8s/README.md](./K8s/README.md) (один общий namespace для связанных релизов).

**Состав HwApp** — [HwApp/README.md](../HwApp/README.md). Детали по **OrderService** — [OrderService/README.md](../HwApp/OrderService/README.md).
