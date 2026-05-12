# Chart homework-apps

Umbrella chart: общий Ingress родителя и подчарты **auth-service**, **customer-service**, **notification-service**, **billing-service** (каталог `subcharts/`). Postgres, Kafka и прочая инфраструктура в chart не входят.

## Состав

- **Родитель**: Ingress, `NOTES.txt`.
- **auth-service**: Deployment, Service, ConfigMap, Secret (JWT), Job миграций, при необходимости ServiceMonitor.
- **customer-service**: Deployment, Service, ConfigMap, Secret, Job миграций, ServiceMonitor (`/metrics`).
- **notification-service**: Deployment, Service, ConfigMap, Secret, Job миграций; Kafka consumer + HTTP API (`/api/notifications`).
- **billing-service**: Deployment, Service, ConfigMap, Secret, Job миграций; HTTP API (`/api/billing/...`, `/api/internal/billing/...`).

## Требования

- **Helm 3** на машине, с которой выполняется установка.

## Установка

Из каталога этого chart:

```bash
helm dependency update

helm upgrade --install homework-apps . \
  -n homework \
  --create-namespace
```

Флаг **`--dependency-update`** у `helm upgrade --install` может заменить отдельный вызов `helm dependency update` (подтянет зависимости из `subcharts/` в `charts/*.tgz` перед рендером).

## Конфигурация

Файл `values.yaml`:

- **`global`** — общие настройки для подчартов: БД, окружение, домен кластера, Kafka bootstrap, имена компонентов для in-cluster URL (`peerAuthName`, `peerCustomerName`, `umbrellaChartName`, `kubernetesAppName`).
- **`authService`** / **`customerService`** / **`notificationService`** / **`billingService`** — параметры подчартов (образы, реплики, пробы, секреты, список Kafka-топиков для Notification и т.д.).
- **`ingress`** — хост, класс, пути и привязка к сервисам.

PostgreSQL и остальное — отдельно, см. [README уровня K8s](../../README.md).
