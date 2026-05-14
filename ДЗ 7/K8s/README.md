# Kubernetes

Helm chart и значения для развёртывания приложений (Auth, Customer, Billing, Notification, **Order**), общий **PostgreSQL**, **Kafka** (для OrderService и NotificationService) и **Ingress**.

## Требования

- Kubernetes (например, minikube)
- Helm 3
- Ingress Controller (ниже — установка ingress-nginx через Helm)
- В `/etc/hosts` (или аналог): `<IP кластера> arch.homework`

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

В командах ниже везде стоит **`homework`** — это просто **короткий placeholder**: подставьте **свой** namespace (`-n <ваш>`) во всех шагах одинаково (Postgres, Kafka, `homework-apps`). Какой namespace выбрать — **на ваше усмотрение**; вариант `customer-service` в репозитории не «рекомендация», а **пример**, как автор гонял стенд локально.

При пустом `global.kafkaBootstrapServers` в `homework-apps` bootstrap по умолчанию:  
`<релиз-kafka>-controller-headless.<ваш-namespace>.svc.cluster.local:9092` (см. `global.kafkaClusterReleaseName`, обычно релиз **`kafka`**).

## PostgreSQL

Один релиз Bitnami PostgreSQL с init-скриптом: пользователи и БД для сервисов (см. `Helm/postgres-values.yaml`).

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

Из каталога **`ДЗ 7/K8s`** (рядом лежат `Helm/kafka-values.yaml`, `Helm/kafka-ui-values.yaml`). Если команды запускаете из **`ДЗ 7/K8s/Helm`**, укажите `-f kafka-values.yaml` и `-f kafka-ui-values.yaml`.

Репозиторий Bitnami нужен для `helm search` / привычки; сам Kafka ставится **OCI-чартом** `bitnamicharts/kafka` (как в рабочем стенде):

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

Chart: [Helm/homework-apps](./Helm/homework-apps/README.md) — umbrella chart с подчартами в `subcharts/` (**auth**, **customer**, **billing**, **notification**, **order**).

```bash
cd Helm/homework-apps

helm dependency update

helm upgrade --install homework-apps . \
  -n homework \
  --create-namespace
```

Либо одной командой: добавьте **`--dependency-update`** к `helm upgrade --install`, если не вызывали `helm dependency update` вручную.

При установке создаются ресурсы для подключённых сервисов (ConfigMap, Secret, Job миграций, Deployment, Service), Ingress с маршрутами:

- `/api/auth`, `/.well-known` → AuthService
- `/api/customers` → CustomerService
- `/api/notifications` → NotificationService
- `/api/billing` — BillingService
- `/api/internal/billing` — внутреннее API BillingService
- `/api/orders` — OrderService (Swagger: `.../api/orders/swagger/...`)

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
