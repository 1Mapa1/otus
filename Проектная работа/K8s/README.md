# Kubernetes

Helm chart и значения для развёртывания приложений (**Auth**, **Customer**, **Catalog**, **Billing**, **Warehouse**, **Delivery**, **Notification**, **Order**), **PostgreSQL** (общий + отдельный для Catalog с read replica), **Redis**, **Kafka**, **Kafka UI** и **API Gateway** (**Traefik**, HTTP без TLS).

## Требования

- Kubernetes (minikube)
- Helm 3
- Traefik (через Helm, namespace `m`)
- В `hosts`: `<IP minikube> electronics.store`

## Makefile (быстрая установка)

Auth DbMigrator при установке идемпотентно создаёт администратора приложения. Значения задаются в `Helm/homework-apps/values.yaml` как `authService.secret.adminLogin` и `authService.secret.adminPassword`; по умолчанию — `admin` / `Admin123!`. Публичный `/api/auth/register` по-прежнему создаёт только роль `USER`.

Перед установкой версии с admin seed нужно собрать и опубликовать migration image `maslovdeveloper/hwapp-auth-migration:10.1` из `HwApp/AuthService/Dockerfile.Migration`.

Из каталога **`K8s`** (рядом с `Helm/`): `make help`.

**`make install`** — основной стенд для проверки API в Postman:

1. Helm repos
2. **Traefik** (namespace `m`, HTTP :80)
3. namespace приложений **`electronics-store`**
4. Postgres (7 MS)
5. Postgres Catalog (primary + read replica)
6. Redis
7. Kafka (топики: `auth`, `customers`, `orders`, `products`, `stocks`, `billing.dlq`)
8. umbrella **`homework-apps`** (все 8 MS)

**`make install-all`** дополнительно устанавливает Kafka UI, kube-prometheus-stack
(Prometheus + Grafana) и EFK (Elasticsearch + Kibana + Filebeat) в namespace
`monitoring`.

> **RAM:** для стенда с логами (Elasticsearch + Kibana) поднимите minikube минимум до **12 GB**:
> `minikube start --memory=12288 --cpus=4`

Свой namespace: `make install NS=my-namespace` (тогда поправьте `bootstrapServers` в `Helm/kafka-ui-values.yaml`).

Снятие: **`make uninstall`** → при необходимости **`make purge-ns`** и **`make purge-monitoring-ns`**.

## API Gateway (Traefik)

Конфиг: `Helm/traefik-values.yaml`. Только **HTTP** (без TLS и redirect на HTTPS). Catalog-плагин `github.com/mdklapwijk/traefik-plugin-request-id:v0.1.1` генерирует `X-Request-ID`.

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
| `elasticsearch` | хранилище логов (single-node, HTTP без TLS) |
| `kibana` | UI для просмотра логов |
| `filebeat` | DaemonSet: сбор stdout-логов `electronics-store` → Elasticsearch |

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

## Логи (EFK)

Конфиги: `Helm/elasticsearch-values.yaml`, `Helm/kibana-values.yaml`, `Helm/filebeat-values.yaml`. Всё в namespace `monitoring` (рядом с Prometheus/Grafana).

Микросервисы пишут JSON в stdout (Serilog `RenderedCompactJsonFormatter`). **Filebeat** (DaemonSet) читает `/var/log/containers/*`, собирает только namespace `electronics-store`, парсит JSON и шлёт в **Elasticsearch** (индекс `filebeat-electronics-*`).

Только `make logging` (если инфра уже поднята): Elasticsearch + Kibana + Filebeat.

Kibana:

```bash
kubectl port-forward -n monitoring svc/kibana-kibana 5601:5601
# http://localhost:5601
```

В Kibana создайте data view по паттерну `filebeat-electronics-*` (time field `@timestamp`). Поля из логов: `@mt` (шаблон), `@l` (уровень), `RequestId`, `kubernetes.pod.name`, `kubernetes.container.name`.

Проверка сквозного `RequestId`: сделайте запрос `Order → Catalog` и отфильтруйте в Kibana по `RequestId` — события обоих сервисов будут с одинаковым значением.

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
