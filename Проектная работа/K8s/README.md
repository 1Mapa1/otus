# Kubernetes

Helm chart и значения для развёртывания приложений (**Auth**, **Customer**, **Catalog**, **Billing**, **Warehouse**, **Delivery**, **Notification**, **Order**), **PostgreSQL** (общий + отдельный для Catalog с read replica), **Redis**, **Kafka**, **Kafka UI** и **Ingress** (nginx).

## Требования

- Kubernetes (minikube)
- Helm 3
- Ingress Controller (ingress-nginx через Helm)
- В `hosts`: `<IP minikube> electronics.store`

## Makefile (быстрая установка)

Из каталога **`K8s`** (рядом с `Helm/`): `make help`.

**`make install`** — полный стенд:

1. Helm repos
2. ingress-nginx (namespace `m`)
3. namespace приложений **`electronics-store`**
4. Postgres (7 MS)
5. Postgres Catalog (primary + read replica)
6. Redis
7. Kafka (топики: `auth`, `customers`, `orders`, `products`, `stocks`, `billing.dlq`)
8. Kafka UI
9. umbrella **`homework-apps`** (все 8 MS)

Свой namespace: `make install NS=my-namespace` (тогда поправьте `bootstrapServers` в `Helm/kafka-ui-values.yaml`).

Снятие: **`make uninstall`** → при необходимости **`make purge-ns`**.

## Ingress

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

`/api/internal/*` в Ingress **не** публикуется — вызовы между MS идут in-cluster.

Swagger: `/api/<service>/swagger` (если включён в образе).

## Инфраструктура

| Релиз Helm | Назначение |
|------------|------------|
| `postgres` | БД для Auth, Customer, Order, Billing, Warehouse, Delivery, Notification |
| `postgres-catalog` | `catalog_db` на primary + streaming read replica |
| `redis` | кэш Catalog (brands/categories/products list) |
| `kafka` | event bus |
| `kafka-ui` | UI для просмотра топиков |

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

## Проверка

```bash
make status
# или
kubectl get pods -n electronics-store
kubectl get ingress -n electronics-store
```

```bash
curl -sS http://electronics.store/.well-known/jwks.json
```
