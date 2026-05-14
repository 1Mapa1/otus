# ДЗ 7 — Заказы, биллинг и нотификации

Заказ с оплатой через биллинг и фиксацией результата в нотификациях (без реальной почты). Код: [HwApp](../HwApp/). Развёртывание: [K8s](./K8s/). Теория и IDL: [Архитектура](./Архитектура/). Сценарий проверки: [Postman](./Postman/).

| Раздел задания | Куда смотреть |
|----------------|---------------|
| Теория (в т.ч. sequence, IDL, **вариант 4**) | [Архитектура](./Архитектура/), файл [4. Notification only broker (extended).md](./Архитектура/4.%20Notification%20only%20broker%20(extended).md) |
| I) схема и описание решения | тот же каталог **Архитектура** + при необходимости скриншоты в **Postman** |
| II) установка (namespace, Helm) | [K8s/README.md](./K8s/README.md), chart [homework-apps](./K8s/Helm/homework-apps/README.md) |
| III) Postman / Newman (`{{baseUrl}}`, `arch.homework`, `--verbose`) | [Postman/README.md](./Postman/README.md) |

**Namespace** и порядок установки Postgres / Kafka / приложений — только в [K8s/README.md](./K8s/README.md) (один общий namespace для связанных релизов).

**Состав HwApp** — [HwApp/README.md](../HwApp/README.md). Детали по каждому сервису — в его **`README.md`** в каталоге проекта в [HwApp](../HwApp/).
