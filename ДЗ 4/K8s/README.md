# Kubernetes

В данной директории находятся Kubernetes манифесты и Helm chart для развёртывания приложения CustomerService.

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
kubectl create namespace customer-service

helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update

helm install postgres bitnami/postgresql \
  -n customer-service \
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

### Развертывание приложения (Helm)

```bash
cd Helm/customer-service

helm install customer-service . \
  -n customer-service \
  --create-namespace
```  

## Что происходит при установке
1. создаётся ConfigMap с настройками приложения
2. создаётся Secret с доступами к БД
3. запускается Job для миграций (pre-install hook)
4. после успешных миграций разворачивается Deployment
5. создаётся Service и Ingress

## Проверка

### Pods

```bash
kubectl get pods -n customer-service
```

Ожидается:

- pod приложения (3 реплики)
- pod PostgreSQL

### Ingress

```bash
kubectl get ingress -n customer-service
```

## Проверка API

```bash
curl http://arch.homework/swagger
```