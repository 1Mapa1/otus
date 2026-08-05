### Назначение

WarehouseMs отвечает за складские остатки и резервирование товаров под заказы.

Сервис владеет количеством товара на складе, доступным остатком, резервами, движениями склада и статусами резервирования. WarehouseMs не хранит название, описание, цену, бренд или другие данные товарной карточки.

### Зона ответственности
- хранение складских остатков;
- учет доступного и зарезервированного количества;
- прием товара на склад;
- создание и отмена резерва под заказ;
- предотвращение резервирования товара сверх доступного остатка;
- хранение движений склада;
- публикация изменений остатка;
- идемпотентная обработка резерва по `orderId`;
- блокировка резервирования архивных товаров.
### Основные сущности
- `StockItem`
- `StockReservation`
- `StockReservationItem`
- `StockMovement`
- `OutboxMessage`
#### StockItem
Локальная складская запись товара.

Поля:
- `ProductId`
- `AvailableQuantity`
- `ReservedQuantity`
- `IsActive`
- `CreatedAt`
- `UpdatedAt`

`ProductId` создается в CatalogMs. WarehouseMs хранит только идентификатор товара и складские данные.
#### StockReservation
Резерв товара, связанный с заказом.

Поля:
- `Id`
- `OrderId`
- `UserId`
- `Status`
- `CreatedAt`
- `CanceledAt`

Один заказ может иметь только один активный резерв.
#### StockReservationItem
Позиция товара внутри резерва.

Поля:
- `ReservationId`
- `ProductId`
- `Quantity`
#### StockMovement
История изменения складского остатка.

Поля:
- `Id`
- `ProductId`
- `Type`
- `Quantity`
- `OrderId`
- `CreatedAt`

Типы операций:
- `Income`;
- `ReservationCreated`;
- `ReservationCanceled`.
#### OutboxMessage
Сообщение для надежной публикации события `StockChanged` в Kafka.

Изменение остатка, создание движения склада и outbox-сообщения выполняются в одной транзакции.
### HTTP API

| Method | Endpoint                                      | Назначение                           | Доступ   |
| ------ | --------------------------------------------- | ------------------------------------ | -------- |
| GET    | `/api/warehouse/stocks`                       | Получить список складских остатков   | ADMIN    |
| GET    | `/api/warehouse/stocks/{productId}`           | Получить остаток конкретного товара  | ADMIN    |
| POST   | `/api/warehouse/stocks/{productId}/income`    | Оформить поступление товара на склад | ADMIN    |
| GET    | `/api/warehouse/stocks/{productId}/movements` | Получить историю движений товара     | ADMIN    |
| POST   | `/api/internal/warehouse/reservations`        | Создать резерв товаров под заказ     | INTERNAL |
| POST   | `/api/internal/warehouse/reservations/cancel` | Отменить резерв товаров              | INTERNAL |
### Взаимодействует с

|Сервис|Протокол|Зачем|
|---|---|---|
|CatalogMs|Kafka|Получает изменения товарного каталога и публикует изменения складской доступности|
|OrderMs|HTTP|Создает и отменяет резерв в рамках Saga оформления заказа|
|AuthMs|JWKS|Получает публичный ключ для проверки JWT на административных endpoint'ах|
|ApiGateway|HTTP|Получает административные запросы через маршрутизацию gateway|
### Kafka events

Публикует:
- `StockChangedV1` (`stock.changed.v1`)

Потребляет:
- `ProductCreatedV1` (`product.created.v1`) 
- `ProductRestoredV1` (`product.restored.v1`) 
- `ProductArchivedV1` (`product.archived.v1`) 

### Ключевые паттерны
- Database per Service
- Saga Participant
- Idempotency
- Transactional Consistency
- Pessimistic locking
- Outbox Pattern
- Event-Driven Architecture
- Eventual Consistency
- JWT Validation via JWKS
- Role-Based Access Control