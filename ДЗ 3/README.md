# ДЗ 3 — Основы работы с Kubernetes

## Цель

В этом ДЗ вы научитесь создавать минимальный сервис.

## Реализация

### Приложение:

- работает на порту **8000**
- endpoint: **GET /health**
- ответ:
```json
{"status": "OK"}
```

### Kubernetes

Реализованы манифесты:

Deployment (3 реплики)
Service (ClusterIP)
Ingress (nginx)

📁 Манифесты находятся в:

```
K8s/Apps/
```

#### Ingress

Настроены два маршрута:

Прямой доступ:
```bash
curl http://arch.homework/health
```
Rewrite путь:
```bash
curl http://arch.homework/otusapp/maslov/health
```
#### Установка ingress-nginx через Helm
```bash
minikube addons disable ingress

kubectl create namespace m

helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx
helm repo update

helm install nginx ingress-nginx/ingress-nginx \
  --namespace m \
  -f K8s/Helm/nginx-ingress.yaml
```

#### Развертывание приложения
```bash
kubectl apply -f K8s/Apps/
```

### Postman

Коллекция находится в:

```
Postman/otus-hw3.postman_collection.json
```

Содержит 2 запроса:

- /health
- /otusapp/maslov/health

Оба проверяют статус 200.

#### Запуск через Newman
```bash
newman run Postman/otus-hw3.postman_collection.json
```

Ожидаемый результат:
```
✓ Status code is 200
```

### Hosts

Необходимо добавить:

```
<minikube ip> arch.homework
```

## Итог

Приложение доступно по:

- http://arch.homework/health
- http://arch.homework/otusapp/maslov/health