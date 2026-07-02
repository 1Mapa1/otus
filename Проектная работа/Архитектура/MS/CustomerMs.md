### Назначение

CustomerMs отвечает за профиль клиента и его адреса доставки.

Сервис владеет персональными данными клиента: именем, email, телефоном и адресами.

Сохраненные адреса используются как адресная книга пользователя. При оформлении заказа пользователь может выбрать сохраненный адрес либо указать разовый адрес. Разовый адрес не обязан сохраняться в CustomerMs.
### Зона ответственности
- хранение профиля клиента;
- создание профиля при регистрации пользователя;
- просмотр и изменение собственного профиля;
- управление адресами доставки;
- публикация событий об изменении адресов;
### Основные сущности
- `Customer`
- `CustomerAddress`
- `OutboxMessage`
#### Customer

Профиль клиента, создаваемый при регистрации в AuthMs.

Поля:
- `Id`
- `Name`
- `Email`
- `Phone`
- `DateOfBirth`
- `CreatedAt`
- `UpdatedAt`

`Customer.Id` совпадает с `User.Id` в AuthMs. Это позволяет связывать профиль с JWT claim `sub`, не создавая отдельную таблицу соответствий.
#### CustomerAddress

Сохраненный адрес из адресной книги клиента.

Поля:
- `Id`
- `CustomerId`
- `City`
- `Street`
- `House`
- `Apartment`
- `IsActive`
- `CreatedAt`
- `UpdatedAt`

Адрес принадлежит только одному клиенту. Клиент может иметь несколько адресов.
#### OutboxMessage

Сообщение для публикации доменных событий в Kafka. Создается в одной транзакции с изменением `Customer`.
### HTTP API

| Method | Endpoint                                  | Назначение                                   | Доступ   |
| ------ | ----------------------------------------- | -------------------------------------------- | -------- |
| GET    | `/api/customers/me`                       | Получить свой профиль                        | USER     |
| PUT    | `/api/customers/me`                       | Обновить свой профиль                        | USER     |
| GET    | `/api/customers/me/addresses`             | Получить список своих адресов                | USER     |
| POST   | `/api/customers/me/addresses`             | Создать адрес доставки                       | USER     |
| PUT    | `/api/customers/me/addresses/{addressId}` | Изменить свой адрес                          | USER     |
| DELETE | `/api/customers/me/addresses/{addressId}` | Деактивировать свой адрес                    | USER     |
| POST   | `/api/internal/customers`                 | Идемпотентно создать профиль при регистрации | INTERNAL |
### Взаимодействует с

| Сервис         | Протокол   | Зачем                                                                                      |
| -------------- | ---------- | ------------------------------------------------------------------------------------------ |
| AuthMs         | HTTP       | Создание профиля клиента при регистрации                                                   |
| AuthMs         | JWKS / JWT | Проверка JWT на пользовательских endpoint'ах                                               |
| ApiGateway     | JWT / JWKS | Проверка JWT до передачи пользовательских запросов в CustomerMs                            |
| NotificationMs | Kafka      | NotificationMs подписывается на события жизненного цикла клиента и создает уведомления     |
### Kafka events

Публикует:
- `CustomerCreatedV1` (`customer.created.v1`)
- `CustomerUpdatedV1` (`customer.updated.v1`)

Потребляет:
- —

### Ключевые паттерны
- Database per Service
- JWT Authentication
- Service-to-Service HTTP
- Outbox Pattern
- Event-Driven Architecture
- Soft Delete