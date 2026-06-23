### Назначение

DeliveryMs отвечает за планирование доставки: зоны доставки, временные слоты, capacity слотов и резервирование доставки для заказа.
### Зона ответственности
- управление зонами доставки;
- создание и изменение временных слотов доставки;
- настройка capacity слотов;
- определение зоны доставки по переданному адресу;
- получение доступных слотов для адреса;
- резервирование слота под заказ;
- отмена резерва как компенсация Saga;
- хранение snapshot адреса в резерве доставки;
- защита от превышения capacity;
- идемпотентная обработка reserve и cancel по `orderId`.
### Основные сущности
- `DeliveryZone`
- `DeliverySlot`
- `DeliveryReservation`
#### DeliveryZone
Зона, в которой магазин выполняет доставку.

Поля:
- `Id`
- `Name`
- `City`
- `IsActive`
- `CreatedAt`
- `UpdatedAt`

#### DeliverySlot
Временной интервал доставки для конкретной зоны.

Поля:
- `Id`
- `ZoneId`
- `StartAt`
- `EndAt`
- `Capacity`
- `ReservedCount`
- `Status`
- `CreatedAt`
- `UpdatedAt`

Доступное количество мест рассчитывается так:

```text
AvailableCapacity = Capacity - ReservedCount
```

#### DeliveryReservation

Резерв доставки, созданный для конкретного заказа.

Поля:

- `Id`
- `OrderId`
- `CustomerId`
- `DeliverySlotId`
- `ZoneId`
- `Status`
- `DeliveryAddressSnapshot`
- `CreatedAt`
- `CanceledAt`

`OrderId` уникален: один заказ может иметь только один резерв доставки.

### HTTP API

|Method|Endpoint|Назначение|Доступ|
|---|---|---|---|
|POST|`/api/delivery/slots/available`|Получить доступные слоты для переданного адреса|USER|
|GET|`/api/delivery/zones`|Получить список зон доставки|ADMIN|
|POST|`/api/delivery/zones`|Создать зону доставки|ADMIN|
|PUT|`/api/delivery/zones/{zoneId}`|Изменить или деактивировать зону|ADMIN|
|GET|`/api/delivery/slots`|Получить список слотов|ADMIN|
|POST|`/api/delivery/slots`|Создать слот доставки для зоны|ADMIN|
|PUT|`/api/delivery/slots/{slotId}`|Изменить время, capacity или статус слота|ADMIN|
|POST|`/api/internal/delivery/reservations`|Зарезервировать слот под заказ|INTERNAL|
|POST|`/api/internal/delivery/reservations/cancel`|Отменить резерв доставки|INTERNAL|

### Взаимодействует с

| Сервис     | Протокол   | Зачем                                                                                          |
| ---------- | ---------- | ---------------------------------------------------------------------------------------------- |
| OrderMs    | HTTP       | OrderMs получает результат резервирования и запускает компенсацию при ошибке                   |
| AuthMs     | JWKS       | Получение публичного ключа для проверки JWT на пользовательских и административных endpoint'ах |
| ApiGateway | JWT / JWKS | Проверка JWT до передачи пользовательских запросов в DeliveryMs                                |

### Kafka events

Публикует:
- —

Потребляет:
- —

### Ключевые паттерны
- Database per Service
- JWT Validation via JWKS
- Role-Based Access Control
- Saga Participant
- Idempotency
- Optimistic Locking
- Transactional Consistency