# HELM

Helm chart для развёртывания сервиса CustomerService в Kubernetes.

## Состав chart

- Deployment — приложение (3 реплики)
- Service — доступ внутри кластера
- Ingress — доступ снаружи (`arch.homework`)
- ConfigMap — конфигурация приложения
- Secret — доступы к базе данных
- Job — миграции базы данных

## Особенности

### Миграции

Миграции выполняются автоматически через Kubernetes Job.

Используются Helm hooks:

- `pre-install`
- `pre-upgrade`

Порядок запуска обеспечен через веса:

- Secret → `-20`
- ConfigMap → `-10`
- Job → `0`

## Установка

```bash
helm install customer-service . -n customer-service
```

## Конфигурация

Основные параметры задаются в values.yaml:

- config — настройки приложения
- secret — доступы к БД
- deployment — параметры приложения
- service — настройки сервиса
- ingress — настройки ingress
- job — параметры миграций

## Примечание

PostgreSQL не входит в chart и должен быть развернут отдельно.