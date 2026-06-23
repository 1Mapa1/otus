### Назначение

NotificationMs отвечает за создание, хранение и отображение пользовательских уведомлений.

Сервис получает доменные события из Kafka, создает уведомления в собственной базе данных и предоставляет пользователю API для просмотра своих уведомлений.

### Зона ответственности
- создание уведомлений о результате оформления заказа;
- хранение уведомлений пользователя;
- поддержание локальной проекции клиента;
- получение списка собственных уведомлений;
- получение уведомления по идентификатору;
- идемпотентная обработка входящих событий.
### Основные сущности
- `Notification`
- `NotificationCustomer`
- `InboxMessage`
#### Notification

Уведомление пользователя.

Поля:
- `Id`
- `CustomerId`
- `Type`
- `Title`
- `Message`
- `CreatedAt`
- `ReadAt`
#### NotificationCustomer
Локальная проекция клиента, получаемая из Kafka-событий CustomerMs.

Поля:
- `CustomerId`
- `Name`
- `Email`
- `UpdatedAt`
#### InboxMessage

Техническая сущность для дедупликации Kafka-событий.

Сервис сохраняет идентификатор обработанного события. Повторная доставка того же события не должна создавать второе уведомление.

### HTTP API

|Method|Endpoint|Назначение|Доступ|
|---|---|---|---|
|GET|`/api/notifications/me`|Получить список собственных уведомлений|USER|
|GET|`/api/notifications/{notificationId}`|Получить конкретное уведомление|USER|
### Взаимодействует с

|Сервис|Протокол|Зачем|
|---|---|---|
|CustomerMs|Kafka|Получает CustomerCreated и CustomerUpdated для поддержания локальной проекции клиента|
|OrderMs|Kafka|Получает финальные события заказа и создает уведомления|
|AuthMs|JWKS|Получает публичный ключ для проверки JWT на пользовательских endpoint'ах|
### Kafka events

Публикует:
- —

Потребляет:
- `CustomerCreated`
- `CustomerUpdated`
- `OrderConfirmed`
- `OrderRejected`

### Ключевые паттерны
- Database per Service
- JWT Validation via JWKS
- Event-Driven Architecture
- Inbox Pattern
- Idempotency
- Eventual Consistency
- Local Read Model