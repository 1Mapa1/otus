# Kubernetes

В данной директории находятся Kubernetes манифесты и Helm chart для развёртывания приложений `AuthService` и `CustomerService`.

## Предварительные требования

- установлен Kubernetes (например minikube)
- установлен Helm
- настроен ingress-nginx
- добавлен host:

<minikube ip> arch.homework

## Установка ingress-nginx

```bash
kubectl create namespace m

helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx
helm repo update

helm install nginx ingress-nginx/ingress-nginx \
  --namespace m \
  -f Helm/nginx-ingress.yaml
``` 

### Установка PostgreSQL

```bash
kubectl create namespace homework-apps

helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update

helm install postgres bitnami/postgresql \
  -n homework-apps \
  -f Helm/postgres-values.yaml
```


### Установка Prometheus + Grafana + Alertmanager

```bash
kubectl create namespace monitoring

helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
helm repo update

helm upgrade monitoring prometheus-community/kube-prometheus-stack \
  -n monitoring \
  -f Helm/prometheus-values.yaml
```

### Развертывание приложений (Helm)

```bash
cd Helm/homework-apps

helm install homework-apps . \
  -n homework-apps \
  --create-namespace
```  

## Что происходит при установке
1. создаются ConfigMap и Secret для каждого сервиса
2. запускаются Job миграций (pre-install hook) для `AuthService` и `CustomerService`
3. после успешных миграций разворачиваются оба Deployment
4. создаются Service для обоих сервисов
5. создаётся общий Ingress

## Проверка

### Pods

```bash
kubectl get pods -n homework-apps
```

Ожидается:

- pod `AuthService`
- pod `CustomerService`
- pod PostgreSQL

### Ingress

```bash
kubectl get ingress -n homework-apps
```

## Проверка API

```bash
curl http://arch.homework/api/auth/login
curl http://arch.homework/api/customers/me
```