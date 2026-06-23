### Назначение
AuthMs отвечает за регистрацию пользователей, аутентификацию и выпуск JWT-токенов. Сервис владеет учетными данными пользователя и публикует JWKS, чтобы ApiGateway и другие микросервисы могли проверять подпись JWT.
### Зона ответственности
- регистрация пользователя;
- хранение учетной записи и хеша пароля;
- проверка логина и пароля;
- выпуск JWT RS256;
- публикация JWKS с публичным ключом;
- хранение роли пользователя;
- синхронное создание CustomerProfile в CustomerMs при регистрации;
- публикация события `UserActivated` после успешной регистрации.
### Основные сущности
- `User`
- `UserStatus`
- `UserRole`
- `OutboxEvent`
#### User

Основная сущность учетной записи пользователя.

Поля:
- `Id`
- `Login`
- `PasswordHash`
- `Status`
- `Role`
- `CreatedAt`
#### UserStatus

Статусы пользователя:
- `Pending` — пользователь создан, но регистрация еще не завершена;
- `Active` — пользователь успешно зарегистрирован и может пользоваться системой;
- `Blocked` — пользователь заблокирован.

#### UserRole

Роли пользователя:
- `USER`
- `ADMIN`
### HTTP API

| Method | Endpoint                 | Назначение                                  | Доступ |
| ------ | ------------------------ | ------------------------------------------- | ------ |
| POST   | `/api/auth/register`     | Регистрация пользователя                    | PUBLIC |
| POST   | `/api/auth/login`        | Логин и получение JWT access token          | PUBLIC |
| GET    | `/.well-known/jwks.json` | Получение публичных ключей для проверки JWT | PUBLIC |
### Взаимодействует с

| Сервис     | Протокол    | Зачем                                                                         |
| ---------- | ----------- | ----------------------------------------------------------------------------- |
| CustomerMs | HTTP        | Создать профиль клиента при регистрации                                       |
| BillingMs  | Kafka       | Передать событие `UserActivated`, чтобы BillingMs мог создать счет асинхронно |
| ApiGateway | HTTP / JWKS | ApiGateway проверяет JWT пользователя по публичному ключу                     |
| CustomerMs | JWKS / JWT  | CustomerMs валидирует JWT на пользовательских endpoint'ах                     |
| OrderMs    | JWKS / JWT  | OrderMs получает `userId` из JWT при создании заказа                          |
| BillingMs  | JWKS / JWT  | BillingMs использует JWT на внешних пользовательских endpoint'ах              |
| DeliveryMs | JWKS / JWT  | DeliveryMs использует JWT на внешних пользовательских endpoint'ах             |
### Kafka events

Публикует:
- `UserActivated`

Потребляет:
- —
### Ключевые паттерны

- Database per Service
- JWT Authentication
- JWKS
- Role-Based Access Control
- Service-to-Service HTTP
- Event-Driven Architecture
- Outbox Pattern