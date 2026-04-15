# Chart homework-apps

Helm chart для AuthService и CustomerService: отдельные Deployment, Service, миграции, общий Ingress.

## Состав

- **AuthService**: Deployment, Service, ConfigMap, Secret (в т.ч. ключ JWT), Job миграций, при необходимости шаблоны ServiceMonitor.
- **CustomerService**: Deployment, Service, ConfigMap, Secret, Job миграций, ServiceMonitor (метрики `/metrics`).

## Миграции

Выполняются Kubernetes Job с Helm hooks (`pre-install` / `pre-upgrade`), порядок задаётся весами аннотаций в шаблонах.

## Установка

Из каталога chart (указать namespace и имя релиза по необходимости):

```bash
helm install homework-apps . -n homework
```

## Конфигурация

Основные параметры — в `values.yaml`:

- `common` — хост БД, порт, домен кластера.
- `authService` / `customerService` — образы, реплики, пробы, секреты БД, настройки JWT и URL взаимодействия сервисов.
- `ingress` — хост, класс, префиксы путей и сопоставление с сервисами.

PostgreSQL в chart не входит; развёртывается отдельно, см. [README уровня K8s](../../README.md).
