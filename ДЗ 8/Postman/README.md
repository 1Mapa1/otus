# Postman — ДЗ 8

Коллекция проверяет сценарий **создания заказа** с распределённой транзакцией: успешный путь и отказы по Billing / Warehouse / Delivery. В окружении **`baseUrl`** по умолчанию задан как **`http://arch.homework`** (требование задания для `{{baseUrl}}`).

## Файлы

| Файл | Назначение |
|------|------------|
| `otus-hw8.postman_collection.json` | Запросы и тесты |
| `otus-hw8.postman_environment.json` | **`baseUrl`** и переменные сценария |

Hosts / Ingress / namespace: [K8s/README.md](../K8s/README.md).

## Newman

Каталог: **`ДЗ 8/Postman`**. Для отчёта удобно использовать **`--verbose`**; **`--delay-request`** — по желанию, если кластер отвечает с задержкой (сага обрабатывается воркером не мгновенно).

**PowerShell**

```powershell
newman run .\otus-hw8.postman_collection.json `
  -e .\otus-hw8.postman_environment.json `
  --delay-request 300 `
  --reporters cli `
  --verbose
```

**Bash**

```bash
newman run otus-hw8.postman_collection.json \
  -e otus-hw8.postman_environment.json \
  --delay-request 300 \
  --reporters cli \
  --verbose
```

## Скриншоты / записи

Фрагменты прогона:

![Успешное подтверждение заказа](./order_confirmed.jpg)

![Отказ: недостаточно средств](./order_rejected_insufficient-funds.jpg)

![Отказ: нет товара на складе](./order_rejected_stock-not-available.jpg)

![Отказ: слот доставки недоступен](./order_rejected_delivery-slot-unavailable.jpg)

![Запись прогона / консоль](./test-record.jpg)
