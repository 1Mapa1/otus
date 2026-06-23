# ApiGateway

## Назначение

`ApiGateway` реализован на базе **Traefik Proxy OSS**.

Traefik является единственной публичной точкой входа в систему. Он принимает внешние запросы от Frontend, применяет общие инфраструктурные политики и маршрутизирует запрос в нужный микросервис.

## Зона ответственности

* маршрутизация публичных HTTP endpoint'ов;
* TLS termination и redirect HTTP → HTTPS;
* rate limiting для `login` и `registration`;
* прокидывание `X-Request-ID`;
* access logs;
* CORS только для локальной разработки.

## Публичные маршруты

| Path                     | Сервис         |
| ------------------------ | -------------- |
| `/api/auth/**`           | AuthMs         |
| `/.well-known/jwks.json` | AuthMs         |
| `/api/catalog/**`        | CatalogMs      |
| `/api/customers/**`      | CustomerMs     |
| `/api/orders/**`         | OrderMs        |
| `/api/billing/**`        | BillingMs      |
| `/api/delivery/**`       | DeliveryMs     |
| `/api/warehouse/**`      | WarehouseMs    |
| `/api/notifications/**`  | NotificationMs |

Traefik передаёт исходный path в сервис без `StripPrefix`.

## Internal API

Endpoint'ы `/api/internal/**` не публикуются через Traefik и недоступны из внешней сети.

Они используются для синхронных команд между микросервисами, например:

* `AuthMs → CustomerMs`;
* `OrderMs → CatalogMs`;
* `OrderMs → BillingMs`;
* `OrderMs → WarehouseMs`;
* `OrderMs → DeliveryMs`.

Внутренние вызовы выполняются напрямую через Kubernetes Service DNS. Traefik не участвует в HTTP-командах Saga.

## TLS и CORS

Traefik принимает внешний HTTPS-трафик на порту `443` и перенаправляет весь HTTP-трафик с порта `80` на HTTPS.

TLS завершается на Traefik. Микросервисы принимают HTTP-трафик только во внутренней сети Kubernetes.

В production Frontend и API публикуются через один origin:

```text
https://shop.example.com/       → Frontend
https://shop.example.com/api/** → API через Traefik
```

Поэтому CORS в production не требуется.

Для локальной разработки разрешается origin `http://localhost:5173`.

## Rate limiting и логи

Rate limiting применяется только к:

```text
POST /api/auth/login
POST /api/auth/register
```

Traefik прокидывает `X-Request-ID` в микросервисы и ведёт access logs: HTTP method, path, status code, duration, router и backend service.

Для асинхронной Saga основным идентификатором в логах является `orderId`.

## JWT

Traefik Proxy OSS не валидирует JWT и не выполняет RBAC.

JWT validation выполняют микросервисы:

```text
AuthMs → выпускает JWT RS256 и публикует JWKS
Микросервисы → валидируют JWT, role claims и ownership ресурсов
```

## Ключевые паттерны

* API Gateway;
* Ingress Controller;
* TLS Termination;
* Rate Limiting;
* Correlation ID;
* Centralized Access Logs.
