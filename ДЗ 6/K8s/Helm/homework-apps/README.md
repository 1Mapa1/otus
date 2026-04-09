# HELM

Helm chart для развёртывания двух сервисов в Kubernetes:

- `AuthService`
- `CustomerService`

Оба сервиса устанавливаются одним chart и одним release.

## Состав chart

Для каждого сервиса создаются:

- `Deployment`
- `Service`
- `ConfigMap`
- `Secret`
- `Job` для миграций (через Helm hooks)

Дополнительно:

- общий `Ingress` с маршрутами на оба сервиса
- `ServiceMonitor` (включён по умолчанию для `CustomerService`)

## Установка

```bash
helm install homework-apps . -n homework-apps
```

## Конфигурация

Основные секции `values.yaml`:

- `common` — общие параметры (БД host/port, environment)
- `authService` — настройки `AuthService`
- `customerService` — настройки `CustomerService`
- `ingress` — внешний доступ и маршруты

URL связи между сервисами формируются автоматически внутри шаблонов:

- `AuthService -> CustomerService`
- `CustomerService -> AuthService`
- `Jwt:Issuer` для `AuthService` (по умолчанию URL самого `AuthService`)

по схеме `http://<service-name>.<namespace>.svc.<clusterDomain>`.
При необходимости можно задать override:

- `authService.config.msCustomerBaseUrl`
- `customerService.config.authUrl`
- `authService.config.jwtIssuer`

### Важно перед установкой

1. Указать корректные образы:
   - `authService.deployment.imageRepository`
   - `authService.migrationJob.imageRepository`
   - `customerService.deployment.imageRepository`
   - `customerService.migrationJob.imageRepository`
2. Для `AuthService` задать реальный приватный ключ:
   - `authService.secret.jwtPrivateKeyPem`

## Примечание

PostgreSQL не входит в chart и должен быть развернут отдельно.