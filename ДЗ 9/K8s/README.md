# Kubernetes

Helm chart и значения для развёртывания приложений (**Auth**, **Customer**, **Billing**, **Warehouse**, **Delivery**, **Notification**, **Order**), общий **PostgreSQL**, **Kafka** (для OrderService и NotificationService) и **Ingress**.

## Требования

- Kubernetes (например, minikube)
- Helm 3
- Ingress Controller (ниже — установка ingress-nginx через Helm)
- В `/etc/hosts` (или аналог): `<IP кластера> arch.homework`

## Makefile (быстрая установка)

Из каталога **`ДЗ 8/K8s`** (рядом с `Helm/`): `make help`. Типовой сценарий: **`make install`** — репозитории Helm, namespace для ingress (`m` по умолчанию), **ingress-nginx**, namespace приложений (**`homework`** по умолчанию), Postgres, Kafka, один релиз umbrella **`homework-apps`** (все сервисы приложения ставятся вместе). Опционально: **`make kafka-ui`**. Свой namespace: `make install NS=my-namespace`. Снятие релизов Helm: **`make uninstall`**; при необходимости затем **`make purge-ns`** (удалит namespace `NS` целиком). Namespace ingress (`INGRESS_NS`) `purge-ns` не трогает.

Нужны **GNU Make** и shell как в WSL / Git Bash / Linux (на чистом `cmd.exe` без `make` этот файл не используется).

## Ingress

```bash
kubectl create namespace m

helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx
helm repo update

helm install nginx ingress-nginx/ingress-nginx \
  --namespace m \
  -f Helm/nginx-ingress.yaml
```

## Namespace

В командах ниже везде стоит **`homework`** — это **короткий placeholder**: подставьте **свой** namespace (`-n <ваш>`) во всех шагах одинаково (Postgres, Kafka, `homework-apps`). Какой namespace выбрать — **на ваше усмотрение**.

При пустом `global.kafkaBootstrapServers` в `homework-apps` bootstrap по умолчанию:  
`<релиз-kafka>-controller-headless.<ваш-namespace>.svc.cluster.local:9092` (см. `global.kafkaClusterReleaseName`, обычно релиз **`kafka`**).

## PostgreSQL

Один релиз Bitnami PostgreSQL с init-скриптом: пользователи и БД для сервисов, включая **warehouse** и **delivery** (см. `Helm/postgres-values.yaml`).

```bash
kubectl create namespace homework

helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update

helm install postgres bitnami/postgresql \
  -n homework \
  -f Helm/postgres-values.yaml
```

## Kafka

Нужен для **OrderService** (outbox → продюсер) и **NotificationService** (consumer). Ставьте в **тот же namespace**, что и `homework-apps` и Postgres.

Из каталога **`ДЗ 8/K8s`** (рядом лежат `Helm/kafka-values.yaml`, `Helm/kafka-ui-values.yaml`). Если команды запускаете из **`ДЗ 8/K8s/Helm`**, укажите `-f kafka-values.yaml` и `-f kafka-ui-values.yaml`.

Репозиторий Bitnami нужен для `helm search` / привычки; сам Kafka ставится **OCI-чартом** `bitnamicharts/kafka`:

```bash
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update

helm upgrade --install kafka oci://registry-1.docker.io/bitnamicharts/kafka \
  -n homework \
  -f Helm/kafka-values.yaml \
  --wait=false
```

Опционально **Kafka UI** (Provectus):

```bash
helm repo add kafka-ui https://provectus.github.io/kafka-ui-charts
helm repo update

helm upgrade --install kafka-ui kafka-ui/kafka-ui \
  -n homework \
  -f Helm/kafka-ui-values.yaml
```

В `Helm/kafka-ui-values.yaml` в `bootstrapServers` может быть зашит **другой** namespace — приведите к тому же, куда ставите Kafka (`<релиз>-controller-headless.<namespace>.svc.cluster.local:9092`).

Имя релиза Kafka в `homework-apps/values.yaml`: **`kafkaClusterReleaseName: kafka`** (полный bootstrap при пустом `kafkaBootstrapServers`: **`kafka-controller-headless.<ваш-namespace>.svc.cluster.local:9092`**).

## Приложения (Helm chart `homework-apps`)

Chart: [Helm/homework-apps](./Helm/homework-apps/README.md) — umbrella chart с подчартами в `subcharts/` (**auth**, **customer**, **billing**, **warehouse**, **delivery**, **notification**, **order**). Все подчарты ставятся **одним** `helm upgrade --install` родителя (включение/выключение — через `values.yaml` и `enabled`).

```bash
cd Helm/homework-apps

helm dependency update

helm upgrade --install homework-apps . \
  -n homework \
  --create-namespace
```

Либо одной командой из **`ДЗ 8/K8s`**: **`make apps`** или добавьте **`--dependency-update`** к `helm upgrade --install`, если не вызывали `helm dependency update` вручную.

При установке создаются ресурсы для подключённых сервисов (ConfigMap, Secret, Job миграций, Deployment, Service), Ingress с маршрутами (см. `templates/ingress.yaml` и `values.yaml`):

- `/api/auth`, `/.well-known` → AuthService
- `/api/customers` → CustomerService
- `/api/notifications` → NotificationService
- `/api/billing` → BillingService
- `/api/warehouse` → WarehouseService
- `/api/delivery` → DeliveryService
- `/api/orders` → OrderService (Swagger: `.../api/orders/swagger/...`)

**OrderService** получает in-cluster URL сервисов Billing, Warehouse и Delivery через переменные окружения `Ms__Billing__*`, `Ms__Warehouse__*`, `Ms__Delivery__*` и настройки саги `OrderSaga__*` (см. `homework-apps/values.yaml` → `orderService.config`).

## Проверка

```bash
kubectl get pods -n homework
kubectl get ingress -n homework
```

(Замените `homework` на тот namespace, который использовали в командах выше.)

Примеры запросов (после записи `arch.homework`):

```bash
curl -sS http://arch.homework/.well-known/jwks.json
curl -sS -X POST http://arch.homework/api/auth/login -H "Content-Type: application/json" -d "{}"
```

Swagger (если включён в образе) — по путям сервисов за Ingress согласно настройке приложения.
