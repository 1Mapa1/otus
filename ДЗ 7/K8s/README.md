# Kubernetes

Helm chart и значения для развёртывания AuthService и CustomerService, общий PostgreSQL и Ingress.

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

## PostgreSQL

Один релиз Bitnami PostgreSQL с init-скриптом: пользователи и БД для `auth_db` и `customer_db`.

```bash
kubectl create namespace homework

helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update

helm install postgres bitnami/postgresql \
  -n homework \
  -f Helm/postgres-values.yaml
```

## Приложения (Helm chart `homework-apps`)

Chart: [Helm/homework-apps](./Helm/homework-apps/README.md) — umbrella chart с подчартами `auth-service` и `customer-service` (исходники в `subcharts/`).

```bash
cd Helm/homework-apps

helm dependency update

helm upgrade --install homework-apps . \
  -n homework \
  --create-namespace
```

Либо одной командой: добавьте **`--dependency-update`** к `helm upgrade --install`, если не вызывали `helm dependency update` вручную.

При установке создаются ресурсы для Auth, Customer и Notification (ConfigMap, Secret, Job миграций, Deployment, Service), Ingress с маршрутами:

- `/api/auth`, `/.well-known` → AuthService
- `/api/customers` → CustomerService
- `/api/notifications` → NotificationService

## Мониторинг (опционально)

В каталоге есть `Helm/prometheus-values.yaml` для kube-prometheus-stack (как в предыдущих ДЗ). Установка в отдельный namespace `monitoring` по аналогии с [ДЗ 4](../../ДЗ%204/K8s/README.md). ServiceMonitor для CustomerService включается в `values.yaml` chart при необходимости.

## Проверка

```bash
kubectl get pods -n homework
kubectl get ingress -n homework
```

Примеры запросов (после записи `arch.homework`):

```bash
curl -sS http://arch.homework/.well-known/jwks.json
curl -sS -X POST http://arch.homework/api/auth/login -H "Content-Type: application/json" -d "{}"
```

Swagger (если включён в образе) — по путям сервисов за Ingress согласно настройке приложения.
