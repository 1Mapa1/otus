### Назначение

OrderMs отвечает за создание заказа, хранение его неизменяемых snapshots и оркестрацию Saga оформления заказа.

Сервис не владеет товарным каталогом, складскими остатками, платежами или слотами доставки. Он координирует работу других сервисов и хранит итоговое состояние заказа.
### Зона ответственности
- создание заказа из checkout-запроса;
- получение актуального snapshot товаров и цен из CatalogMs;
- сохранение snapshot товаров, цен и адреса доставки;
- расчет итоговой стоимости заказа;
- хранение статуса заказа и состояния Saga;
- выполнение Saga через background worker;
- повтор технически неуспешных шагов;
- запуск компенсаций при бизнес-ошибках;
- публикация финальных событий заказа через outbox;
- предоставление пользователю статуса и деталей своих заказов.
### Основные сущности
- `Order`
- `OrderItem`
- `DeliveryAddressSnapshot`
- `OutboxMessage`
#### Order
Заказ пользователя.

Поля:
- `Id`
- `UserId`
- `Status`
- `SagaStep`
- `TotalAmount`
- `DeliverySlotId`
- `FailureReason`
- `DeliveryAddressSnapshot`
- `CreatedAt`
- `UpdatedAt`

Статусы заказа:
- `Processing`
- `Confirmed`
- `Rejected`
#### OrderItem
Неизменяемая snapshot-позиция заказа.

Поля:
- `ProductId`
- `Name`
- `UnitPrice`
- `Quantity`
- `TotalPrice`
#### DeliveryAddressSnapshot
Адрес конкретной доставки, переданный в checkout.

Поля:
- `City`
- `Street`
- `House`
- `Apartment`
#### OutboxMessage
Техническая сущность для надежной публикации финальных событий заказа в Kafka.

Изменение финального статуса заказа и создание outbox-сообщения выполняются в одной транзакции.
### HTTP API

|Method|Endpoint|Назначение|Доступ|
|---|---|---|---|
|POST|`/api/orders`|Создать заказ и запустить Saga|USER|
|GET|`/api/orders/me`|Получить список собственных заказов|USER|
|GET|`/api/orders/{orderId}`|Получить детали и статус конкретного заказа|USER|
### Взаимодействует с

|Сервис|Протокол|Зачем|
|---|---|---|
|CatalogMs|HTTP|Получить актуальный snapshot активных товаров и цен до создания заказа|
|BillingMs|HTTP|Авторизовать, списать или отменить оплату в Saga|
|WarehouseMs|HTTP|Создать или отменить резерв товаров в Saga|
|DeliveryMs|HTTP|Создать или отменить резерв слота доставки в Saga|
|NotificationMs|Kafka|NotificationMs подписывается на финальные события заказа|
|AuthMs|JWKS|Получение публичного ключа для проверки JWT|
|ApiGateway|HTTP|Маршрутизация внешних запросов к OrderMs|
### Kafka events
Публикует:
- `OrderConfirmed`
- `OrderRejected`

Потребляет:
- —

### Выполнение Saga
После успешного создания заказа он сохраняется в статусе `Processing` и с шагом `Created`.

Background worker выполняет шаги:

```text
Created
→ PaymentAuthorized
→ StockReserved
→ DeliveryReserved
→ Completed
```

При бизнес-ошибке выполняются компенсации:

```text
Compensating
→ Compensated
→ Rejected
```

Технические ошибки не приводят к отклонению заказа сразу. Заказ остается в `Processing`, а worker повторяет текущий шаг позднее.

### Ключевые паттерны
- Database per Service
- Saga Orchestration
- Saga State Machine
- Background Worker
- Retry
- Compensation
- Idempotency
- Outbox Pattern
- Event-Driven Architecture
- JWT Validation via JWKS