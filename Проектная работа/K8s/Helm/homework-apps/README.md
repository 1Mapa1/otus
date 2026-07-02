# homework-apps

Umbrella Helm chart для микросервисов HwApp.

## Subcharts

| Alias | Service |
|-------|---------|
| `authService` | Auth |
| `customerService` | Customer |
| `catalogService` | Catalog |
| `billingService` | Billing |
| `warehouseService` | Warehouse |
| `deliveryService` | Delivery |
| `notificationService` | Notification |
| `orderService` | Order |

Включение/выключение: `values.yaml` → `<alias>.enabled`.

## Global (`values.yaml`)

- `dbHost` / `dbPort` — общий Postgres для 7 MS
- `catalogDbPrimaryHost` / `catalogDbReplicaHost` — отдельный Postgres Catalog
- `redisHost` / `redisPort` — Redis для Catalog
- `kafkaClusterReleaseName` — имя Helm-релиза Kafka (по умолчанию `kafka`)
- `peerCatalogName` — component name для in-cluster URL Catalog

## Установка

Из каталога `K8s`:

```bash
make apps
```

Или вручную:

```bash
helm dependency update
helm upgrade --install homework-apps . -n electronics-store --create-namespace
```

PostgreSQL, Redis, Kafka — отдельно, см. [README уровня K8s](../../README.md).

## Traefik middleware

- `templates/traefik-middleware.yaml` — `X-Request-ID`, rate limit auth
- `templates/traefik-ingressroute-auth-rate-limit.yaml` — `POST /api/auth/login|register`
- `values.yaml` → `traefik.middlewares.*`
