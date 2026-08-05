# Helm chart `homework-apps`

Umbrella chart развертывает восемь микросервисов проекта и общие Kubernetes-ресурсы для входящего трафика.

## Subcharts

| Alias в `values.yaml` | Микросервис |
|---|---|
| `authService` | Auth |
| `customerService` | Customer |
| `catalogService` | Catalog |
| `notificationService` | Notification |
| `billingService` | Billing |
| `orderService` | Order |
| `warehouseService` | Warehouse |
| `deliveryService` | Delivery |

Каждый сервис можно включить или отключить параметром `<alias>.enabled`.

## Файлы конфигурации

- `values.yaml` — полный стенд со всеми сервисами.
- `demo-values.yaml` — дополнение для demo-режима: включены только Auth и Customer, их outbox publishers отключены.
- `subcharts/` — Helm chart каждого микросервиса.
- `templates/ingress.yaml` — публичные маршруты.
- `templates/traefik-middleware.yaml` — middleware ограничения частоты запросов.
- `templates/traefik-ingressroute-auth-rate-limit.yaml` — отдельный маршрут для регистрации и входа.

При совместном использовании файлов более поздний файл переопределяет основной:

```bash
helm upgrade --install homework-apps Helm/homework-apps \
  -n electronics-store \
  --create-namespace \
  --dependency-update \
  -f Helm/homework-apps/demo-values.yaml
```

## Глобальные параметры

| Параметр | Назначение |
|---|---|
| `global.dbHost`, `global.dbPort` | Общий PostgreSQL |
| `global.catalogDbPrimaryHost` | Primary-база Catalog |
| `global.catalogDbReplicaHost` | Read replica Catalog |
| `global.catalogDbPort` | Порт баз Catalog |
| `global.redisHost`, `global.redisPort` | Redis Catalog |
| `global.kafkaClusterReleaseName` | Имя Helm-релиза Kafka |
| `global.kafkaBootstrapServers` | Явный адрес Kafka; пустое значение вычисляется автоматически |
| `global.clusterDomain` | Домен Kubernetes-кластера |
| `global.environment` | ASP.NET Core environment |
| `global.peer*Name` | Имена сервисов для внутренних HTTP-вызовов |

## Параметры микросервисов

У каждого subchart используется единая структура:

| Раздел | Назначение |
|---|---|
| `deployment` | Образ приложения, тег, количество реплик, порт и probes |
| `service` | ClusterIP-сервис и отдельный порт метрик |
| `serviceMonitor` | Сбор метрик Prometheus с `/metrics` |
| `migrationJob` | Образ мигратора и настройки Kubernetes Job |
| `config` | Несекретная конфигурация приложения |
| `secret` | Учетные данные БД и другие секретные значения |

Перед запуском Deployment Helm выполняет migration Job соответствующего сервиса. Миграции идемпотентны и используют отдельные Docker-образы.

## Текущие образы

| Сервис | Приложение | Миграции |
|---|---|---|
| Auth | `maslovdeveloper/hwapp-auth-service:10.8` | `maslovdeveloper/hwapp-auth-migration:10.3` |
| Customer | `maslovdeveloper/hwapp-customer-service:10.4` | `maslovdeveloper/hwapp-customer-migration:10.0` |
| Catalog | `maslovdeveloper/hwapp-catalog-service:10.3` | `maslovdeveloper/hwapp-catalog-migration:10.1` |
| Notification | `maslovdeveloper/hwapp-notification-service:10.3` | `maslovdeveloper/hwapp-notification-migration:10.0` |
| Billing | `maslovdeveloper/hwapp-billing-service:10.3` | `maslovdeveloper/hwapp-billing-migration:10.0` |
| Order | `maslovdeveloper/hwapp-order-service:10.3` | `maslovdeveloper/hwapp-order-migration:10.3` |
| Warehouse | `maslovdeveloper/hwapp-warehouse-service:10.3` | `maslovdeveloper/hwapp-warehouse-migration:10.1` |
| Delivery | `maslovdeveloper/hwapp-delivery-service:10.3` | `maslovdeveloper/hwapp-delivery-migration:10.1` |

Актуальным источником версий является `values.yaml`.

## Полный и demo-режимы

В обычном режиме все восемь сервисов включены, а Auth и Customer публикуют сохраненные outbox-сообщения в Kafka.

В `demo-values.yaml`:

- включены Auth и Customer;
- отключены остальные шесть сервисов;
- `authService.config.outboxPublisherEnabled` имеет значение `false`;
- `customerService.config.outboxPublisherEnabled` имеет значение `false`.

При этом запись сообщений в outbox сохраняется. Отключается только их фоновая публикация, поскольку Kafka в demo-стенде не устанавливается.

## Ingress и Traefik

Ingress публикует только внешние API включенных сервисов на хосте `electronics.store`. Внутренние `/api/internal/*` маршруты отсутствуют.

Rate limit применяется к:

- `POST /api/auth/login`;
- `POST /api/auth/register`.

Параметры находятся в `traefik.middlewares.rateLimitAuth`.

Middleware `traefik.middlewares.requestId` по умолчанию отключен. `X-Request-ID` создается и передается middleware внутри приложений.

## Установка

Рекомендуемый способ из каталога `Проектная работа/K8s`:

```bash
make apps
```

Для demo-конфигурации:

```bash
make apps-demo
```

Эти цели предполагают, что необходимая инфраструктура уже установлена. Для развертывания готового стенда используйте `make install`, `make install-demo` или `make install-all`, описанные в [README каталога K8s](../../README.md).

Ручная установка полного umbrella chart:

```bash
helm upgrade --install homework-apps Helm/homework-apps \
  -n electronics-store \
  --create-namespace \
  --dependency-update
```

## Создаваемые ресурсы

Для каждого включенного сервиса создаются:

- Deployment;
- ClusterIP Service;
- ConfigMap;
- Secret;
- migration Job;
- ServiceMonitor.

Umbrella chart дополнительно создает Ingress и Traefik middleware для rate limit.
