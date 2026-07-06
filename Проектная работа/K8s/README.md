# Kubernetes

Helm chart и значения для развёртывания приложений (**Auth**, **Customer**, **Catalog**, **Billing**, **Warehouse**, **Delivery**, **Notification**, **Order**), **PostgreSQL** (общий + отдельный для Catalog с read replica), **Redis**, **Kafka**, **Kafka UI** и **API Gateway** (**Traefik**, HTTP без TLS).

## Требования

- Kubernetes (minikube)
- Helm 3
- Traefik (через Helm, namespace `m`)
- В `hosts`: `<IP minikube> electronics.store`

## Makefile (быстрая установка)

Из каталога **`K8s`** (рядом с `Helm/`): `make help`.

**`make install`** — полный стенд:

1. Helm repos
2. **Traefik** (namespace `m`, HTTP :80)
3. namespace приложений **`electronics-store`**
4. **kube-prometheus-stack** (namespace **`monitoring`**: Prometheus + Grafana)
5. Postgres (7 MS)
6. Postgres Catalog (primary + read replica)
7. Redis
8. Kafka (топики: `auth`, `customers`, `orders`, `products`, `stocks`, `billing.dlq`)
9. Kafka UI
10. umbrella **`homework-apps`** (все 8 MS)

Свой namespace: `make install NS=my-namespace` (тогда поправьте `bootstrapServers` в `Helm/kafka-ui-values.yaml`).

Снятие: **`make uninstall`** → при необходимости **`make purge-ns`** и **`make purge-monitoring-ns`**.

## API Gateway (Traefik)

Конфиг: `Helm/traefik-values.yaml`. Только **HTTP** (без TLS и redirect на HTTPS). Плагин `traefik-plugin-request-id` для `X-Request-ID`.

Host по умолчанию: **`electronics.store`** (`homework-apps/values.yaml` → `ingress.host`).

Маршруты (только public API):

| Path | Service |
|------|---------|
| `/api/auth`, `/.well-known` | Auth |
| `/api/customers` | Customer |
| `/api/catalog` | Catalog |
| `/api/notifications` | Notification |
| `/api/billing` | Billing |
| `/api/warehouse` | Warehouse |
| `/api/delivery` | Delivery |
| `/api/orders` | Order |

`/api/internal/*` через Traefik **не** публикуется — вызовы между MS идут in-cluster.

JWT валидируют **микросервисы**, не Traefik (см. `Архитектура/ApiGateway.md`).

**Middleware Traefik** (CRD в `homework-apps`):

| Middleware | Назначение |
|------------|------------|
| `*-request-id` | `X-Request-ID` (UUID, если заголовка нет) на все public routes |
| `*-rate-limit-auth` | Rate limit только `POST /api/auth/login` и `POST /api/auth/register` |

Параметры rate limit: `homework-apps/values.yaml` → `traefik.middlewares.rateLimitAuth` (по умолчанию 10 req/min, burst 20).

Swagger: `/api/<service>/swagger` (если включён в образе).

## Инфраструктура

| Релиз Helm | Назначение |
|------------|------------|
| `traefik` | API Gateway (ingress controller) |
| `postgres` | БД для Auth, Customer, Order, Billing, Warehouse, Delivery, Notification |
| `postgres-catalog` | `catalog_db` на primary + streaming read replica |
| `redis` | кэш Catalog (brands/categories/products list) |
| `kafka` | event bus |
| `kafka-ui` | UI для просмотра топиков |
| `monitoring` | Prometheus + Grafana (`kube-prometheus-stack`) |

Bootstrap Kafka (при пустом `global.kafkaBootstrapServers` в umbrella):

`kafka-controller-headless.<namespace>.svc.cluster.local:9092`

## Приложения (`homework-apps`)

Образы: `maslovdeveloper/hwapp-*`, тег **`10.0`**.

```bash
cd K8s
make install
```

Или только apps (если инфра уже поднята): `make apps`.

**OrderService** — in-cluster URL Billing, Catalog, Warehouse, Delivery (`Ms__*__*`).

**CatalogService** — dual DB (`DB_PRIMARY_*` / `DB_REPLICA_*`), Redis, Kafka consumer `stocks`.

**BillingService** — Kafka consumer `auth`, DLQ `billing.dlq`.

**WarehouseService** — Kafka consumer `products`, producer topic `stocks`.

## Мониторинг (Prometheus + Grafana)

Конфиг: `Helm/prometheus-values.yaml`. Prometheus подхватывает **ServiceMonitor** из namespace `electronics-store` (все 8 MS + Postgres).

Grafana (логин/пароль по умолчанию `admin` / `admin`):

```bash
kubectl port-forward -n monitoring svc/monitoring-grafana 3000:80
# http://localhost:3000
```

Метрики MS: `/metrics` (порт `monitor` в Service → targetPort приложения 8000).

## Проверка

```bash
make status
# или
kubectl get pods -n electronics-store
kubectl get ingress -n electronics-store
kubectl get pods -n m
```

```bash
curl -sS http://electronics.store/.well-known/jwks.json
```
