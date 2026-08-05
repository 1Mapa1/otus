# Развертывание в Kubernetes

В каталоге находятся Helm-чарты и Makefile для развертывания проекта в minikube.

Основной стенд включает восемь микросервисов, Traefik, PostgreSQL, отдельный кластер PostgreSQL для Catalog, Redis и Kafka. Дополнительно могут быть установлены Kafka UI, Prometheus, Grafana, Elasticsearch, Kibana и Filebeat.

## Требования

- WSL, Linux или другая среда с GNU Make и POSIX shell;
- запущенный Kubernetes-кластер minikube;
- `kubectl`;
- Helm 3;
- Docker Desktop;
- не менее 12 ГБ памяти и 4 CPU для полного стенда с мониторингом и логированием.

Рекомендуемая конфигурация minikube:

```bash
minikube start --memory=12288 --cpus=4
```

Все команды ниже выполняются из каталога `Проектная работа/K8s`.

## Варианты установки

| Команда | Состав стенда | Назначение |
|---|---|---|
| `make install` | Все 8 сервисов, PostgreSQL, PostgreSQL Catalog, Redis, Kafka и Traefik | Полное функциональное и E2E-тестирование |
| `make install-demo` | Auth, Customer, PostgreSQL, Traefik, Prometheus, Grafana и ELK | Демонстрация метрик и централизованных логов |
| `make install-all` | `make install` + Kafka UI, Prometheus, Grafana и ELK | Полный стенд вместе с наблюдаемостью |

Установка выполняется идемпотентно через `helm upgrade --install`, поэтому ту же команду можно использовать для первичного запуска и обновления стенда.

### Основной стенд

```bash
make install
```

### Demo-стенд

```bash
make install-demo
```

Demo-режим оставляет включенными только Auth и Customer. Kafka и Redis в нем отсутствуют, а фоновые издатели outbox отключены. Сами интеграционные события продолжают сохраняться в таблицах outbox.

При переходе на demo-режим Makefile удаляет релизы Kafka, Redis и PostgreSQL Catalog, если они были установлены ранее.

### Полный стенд с наблюдаемостью

```bash
make install-all
```

## Доступ к API

Приложение публикуется через Traefik по адресу:

```text
http://electronics.store
```

Добавьте в Windows-файл `C:\Windows\System32\drivers\etc\hosts`:

```text
127.0.0.1 electronics.store
```

Затем оставьте в отдельном окне WSL проброс порта Traefik:

```bash
sudo KUBECONFIG="$HOME/.kube/config" \
kubectl port-forward --address 0.0.0.0 -n m svc/traefik 80:80
```

Права `sudo` требуются для локального порта 80.

Проверка доступности:

```bash
curl http://electronics.store/.well-known/jwks.json
```

## Маршруты API Gateway

| Путь | Сервис |
|---|---|
| `/api/auth`, `/.well-known` | Auth |
| `/api/customers` | Customer |
| `/api/catalog` | Catalog |
| `/api/notifications` | Notification |
| `/api/billing` | Billing |
| `/api/warehouse` | Warehouse |
| `/api/delivery` | Delivery |
| `/api/orders` | Order |

Внутренние маршруты `/api/internal/*` через Traefik не публикуются. Межсервисные запросы выполняются по внутренним Kubernetes Service.

JWT проверяется микросервисами. Traefik отвечает за маршрутизацию и ограничивает частоту запросов регистрации и входа: по умолчанию 10 запросов в минуту с burst 20.

`X-Request-ID` создается и передается ASP.NET middleware каждого сервиса. Внешний Traefik-плагин генерации Request ID отключен, чтобы доступность маршрутов не зависела от загрузки плагина.

## Администратор

Migration Job сервиса Auth идемпотентно создает администратора:

| Параметр | Значение по умолчанию |
|---|---|
| Логин | `admin` |
| Пароль | `Admin123!` |

Значения задаются в `Helm/homework-apps/values.yaml` через `authService.secret.adminLogin` и `authService.secret.adminPassword`.

## Namespace и Helm-релизы

| Namespace | Содержимое |
|---|---|
| `electronics-store` | Микросервисы и прикладная инфраструктура |
| `m` | Traefik |
| `monitoring` | Prometheus, Grafana и ELK |

Основные релизы:

| Релиз | Назначение |
|---|---|
| `homework-apps` | Umbrella chart восьми микросервисов |
| `postgres` | Общий PostgreSQL для семи сервисов |
| `postgres-catalog` | Primary и read replica Catalog |
| `redis` | Кэш Catalog |
| `kafka` | Брокер сообщений |
| `kafka-ui` | Просмотр Kafka |
| `traefik` | API Gateway |
| `monitoring` | Prometheus и Grafana |
| `electronics-dashboard` | Дашборд проекта в Grafana |
| `elasticsearch`, `kibana`, `filebeat` | Централизованные логи |

Namespace можно переопределить:

```bash
make install NS=my-namespace
```

Доступные параметры Makefile:

- `NS` — namespace приложений;
- `MONITORING_NS` — namespace наблюдаемости;
- `INGRESS_NS` — namespace Traefik;
- `HELM_TIMEOUT` — тайм-аут операций Helm.

## Grafana

Grafana устанавливается командами `make install-demo`, `make install-all` или `make prometheus`.

```bash
kubectl port-forward --address 0.0.0.0 \
  -n monitoring svc/monitoring-grafana 3000:80
```

Откройте `http://localhost:3000` и войдите с логином `admin` и паролем `admin`.

Дашборд **Electronics Store - Services and PostgreSQL** создается автоматически. Он содержит метрики доступности сервисов и баз данных, HTTP-нагрузки, кодов ответа, P95, CPU, памяти и PostgreSQL.

## Kibana

ELK устанавливается командами `make install-demo`, `make install-all` или `make logging`.

```bash
kubectl port-forward --address 0.0.0.0 \
  -n monitoring svc/kibana 5601:5601
```

Откройте `http://localhost:5601`.

Data View **Electronics Store** и сохраненный поиск **Electronics Store - Request trace** импортируются автоматически. Filebeat собирает JSON-логи контейнеров из namespace приложений и записывает их в индексы `filebeat-electronics-*`.

## Проверка состояния

```bash
make status
```

Для стенда с наблюдаемостью:

```bash
make status-all
```

Список всех доступных целей:

```bash
make help
```

## Удаление

Удалить основной стенд:

```bash
make uninstall
```

Удалить основной стенд, Kafka UI, Prometheus, Grafana и ELK:

```bash
make uninstall-all
```

Эти команды не удаляют namespace. При необходимости:

```bash
make purge-ns
make purge-monitoring-ns
```

## Состав каталога

- `Makefile` — установка, обновление, проверка и удаление стендов.
- `Helm/homework-apps` — umbrella chart микросервисов.
- `Helm/grafana-dashboard` — автоматически устанавливаемый дашборд Grafana.
- `Helm/kibana` — Kibana и импорт сохраненных объектов.
- `Helm/filebeat` — сбор логов контейнеров.
- `Helm/*-values.yaml` — настройки внешних Helm-чартов.

Подробное устройство umbrella chart описано в [Helm/homework-apps/README.md](Helm/homework-apps/README.md).
