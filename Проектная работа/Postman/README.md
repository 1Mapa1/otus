# Postman-сценарии проекта

## Структура

- `Main` — полный E2E-тест всех микросервисов для `make install`.
- `Observability` — короткий Auth + Customer сценарий для `make install-demo`.

## Demo: запуск теста

Сначала из WSL запустите стенд:

```bash
cd "/mnt/c/Users/maslo/source/repos/otus/Проектная работа/K8s"
make install-demo
```

Оставьте в отдельном WSL-терминале проброс Traefik:

```bash
sudo KUBECONFIG=/home/mapa/.kube/config \
kubectl port-forward --address 0.0.0.0 -n m svc/traefik 80:80
```

Проверьте, что в Windows hosts есть строка:

```text
127.0.0.1 electronics.store
```

Запустите из PowerShell пять итераций, чтобы на графиках было достаточно данных:

```powershell
cd "C:\Users\maslo\source\repos\otus\Проектная работа\Postman\Observability"

newman run .\electronics-store-observability.postman_collection.json `
  -e .\electronics-store-observability.postman_environment.json `
  --iteration-count 5 `
  --delay-request 100 `
  --reporters cli
```

Коллекция выполняет регистрацию и вход, работу с профилем и адресом, а также
ожидаемые ответы `401`, `404` и один контролируемый `500` от входа с неверным
паролем. Этот `500` специально оставлен в demo-сценарии, чтобы в Kibana
гарантированно появился Error. Каждый HTTP-запрос получает отдельный
`X-Request-ID`. Newman печатает название запроса и его идентификатор строкой
`Kibana RequestId [название запроса]: ...`.

## Grafana

В отдельном WSL-терминале:

```bash
kubectl port-forward --address 0.0.0.0 \
  -n monitoring svc/monitoring-grafana 3000:80
```

Откройте `http://localhost:3000`:

- логин: `admin`
- пароль: `admin`

Откройте **Explore**, выберите источник **Prometheus** и диапазон
**Last 15 minutes**.

Количество запросов по сервисам:

```promql
sum(rate(http_request_duration_seconds_count[1m])) by (job)
```

Ответы по HTTP-кодам:

```promql
sum(increase(http_request_duration_seconds_count[15m])) by (code)
```

P95 времени ответа:

```promql
histogram_quantile(
  0.95,
  sum(rate(http_request_duration_seconds_bucket[5m])) by (le, job)
)
```

CPU контейнеров demo-стенда:

```promql
sum(rate(container_cpu_usage_seconds_total{namespace="electronics-store",container!=""}[5m])) by (pod)
```

Для скриншота удобнее выполнить Newman, выбрать **Last 15 minutes** и включить
режим отображения **Time series**.

## Kibana и ELK

В отдельном WSL-терминале:

```bash
kubectl port-forward --address 0.0.0.0 \
  -n monitoring svc/kibana 5601:5601
```

Откройте `http://localhost:5601`.

При первом запуске:

1. Откройте **Stack Management → Data Views**.
2. Создайте Data View с шаблоном `filebeat-electronics-*`.
3. В качестве поля времени выберите `@timestamp`.
4. Откройте **Discover** и выберите созданный Data View.
5. Установите диапазон **Last 15 minutes**.

Все логи demo-сервисов:

```text
kubernetes.namespace : "electronics-store"
```

Только Auth и Customer:

```text
kubernetes.container.name : ("auth" or "customer")
```

Логи одного HTTP-запроса:

```text
RequestId : "observability-..."
```

Вместо `observability-...` вставьте Request ID нужного запроса из вывода
Newman. Для регистрации будут видны входящий запрос Auth и внутренний вызов
Customer, потому что Auth передаёт тот же `X-Request-ID` дальше.

Только ошибки и предупреждения:

```text
kubernetes.namespace : "electronics-store" and @l : ("Error" or "Warning")
```
