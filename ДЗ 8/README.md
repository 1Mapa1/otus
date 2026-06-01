# ДЗ 8 — Распределённая транзакция при создании заказа

Создание заказа с участием **Billing**, **Warehouse** и **Delivery**; при ошибке на шагах после оплаты — **откат** уже выполненных действий. Код: [HwApp](../HwApp/). Теория, диаграммы и IDL: [Архитектура](./Архитектура/). Развёртывание: [K8s](./K8s/). Проверка сценария: [Postman](./Postman/).

| Раздел задания | Куда смотреть |
|----------------|---------------|
| **0)** какой паттерн использован, схема, IDL | [Архитектура](./Архитектура/), документ [Saga Sync Orchestration.md](./Архитектура/Saga%20Sync%20Orchestration.md) |
| **1)** установка (namespace, Helm) | [K8s/README.md](./K8s/README.md), chart [homework-apps](./K8s/Helm/homework-apps/README.md) |
| **2)** Postman / Newman (`{{baseUrl}}`, `arch.homework`, `--verbose`) | [Postman/README.md](./Postman/README.md) |

**Namespace** и порядок установки Postgres / Kafka / приложений — в [K8s/README.md](./K8s/README.md) (один общий namespace для связанных релизов).

**Состав HwApp** — [HwApp/README.md](../HwApp/README.md). Детали по каждому сервису — в его **`README.md`** в каталоге проекта в [HwApp](../HwApp/).
