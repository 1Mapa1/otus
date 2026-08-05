### Назначение

BillingMs отвечает за внутренний платежный счет пользователя и обработку оплаты заказа.

Сервис моделирует prepaid wallet: пользователь пополняет внутренний баланс, а при оформлении заказа средства сначала резервируются, затем списываются или возвращаются в доступный баланс при отмене заказа.
### Зона ответственности
- хранение внутреннего счета пользователя;
- хранение баланса, зарезервированных и доступных средств;
- пополнение баланса;
- авторизация оплаты заказа;
- списание авторизованной оплаты;
- отмена авторизации как компенсация Saga;
- хранение платежей по заказам;
- ведение ledger операций;
- асинхронное создание счета после регистрации пользователя;
- lazy creation счета при первом платежном обращении.
### Основные сущности
- `Account`
- `Payment`
- `AccountTransaction`
- `InboxMessage`
#### Account
Внутренний счет пользователя.

Поля:
- `UserId`
- `Balance`
- `HeldAmount`
- `AvailableBalance`
- `CreatedAt`
- `UpdatedAt`

`AvailableBalance` вычисляется как:

```text
AvailableBalance = Balance - HeldAmount
```
#### Payment
Платеж, связанный с заказом.

Поля:
- `Id`
- `OrderId`
- `UserId`
- `Amount`
- `Status`
- `CreatedAt`
- `AuthorizedAt`
- `CapturedAt`
- `CanceledAt`

Один заказ может иметь только один платеж. Ограничение уникальности по `OrderId`.
#### AccountTransaction

Неизменяемая запись ledger, фиксирующая изменение состояния счета.

Примеры типов операций:
- `Deposit`
- `Authorization`
- `Capture`
- `AuthorizationCanceled`

Каждая операция хранит баланс и hold до и после изменения.
#### InboxMessage

Запись обработанного Kafka-события. Используется для защиты от повторной обработки `UserActivated`.

### HTTP API

| Method | Endpoint                                              | Назначение                                         | Доступ   |
| ------ | ----------------------------------------------------- | -------------------------------------------------- | -------- |
| GET    | `/api/billing/accounts/me`                            | Получить текущий баланс, hold и доступные средства | USER     |
| POST   | `/api/billing/accounts/deposit`                       | Пополнить внутренний баланс                        | USER     |
| POST   | `/api/internal/billing/payments/authorize`            | Авторизовать сумму заказа и установить hold        | INTERNAL |
| POST   | `/api/internal/billing/payments/capture`              | Списать ранее авторизованную сумму                 | INTERNAL |
| POST   | `/api/internal/billing/payments/cancel-authorization` | Снять hold как компенсацию Saga                    | INTERNAL |

### Взаимодействует с

| Сервис     | Протокол   | Зачем                                                               |
| ---------- | ---------- | ------------------------------------------------------------------- |
| AuthMs     | Kafka      | Получает событие `UserActivated` и создает пустой счет пользователя |
| OrderMs    | HTTP       | Выполняет authorize, capture и cancel authorization в рамках Saga   |
| ApiGateway | JWT / JWKS | Проверяет JWT на пользовательских endpoint'ах                       |
| AuthMs     | JWKS       | Получает публичный ключ для проверки подписи JWT                    |
### Kafka events

Публикует:
- —

Потребляет:
- `UserActivatedV1` (`user.activated.v1`)
### Поведение lazy creation

Счет не является обязательным условием успешной регистрации.

Если обработчик `UserActivated` еще не успел создать счет, BillingMs создает пустой `Account` при первом обращении:

```text
deposit
→ account отсутствует
→ создать account с Balance = 0 и HeldAmount = 0
→ выполнить пополнение

authorize
→ account отсутствует
→ создать account с Balance = 0 и HeldAmount = 0
→ проверить AvailableBalance
→ вернуть InsufficientFunds
```
### Ключевые паттерны
- Database per Service
- Saga Participant
- Idempotency
- Inbox Pattern
- DLQ
- Event-Driven Architecture
- Eventual Consistency
- Ledger Pattern
- Lazy Creation
- Atomic Conditional Update
- Transactional Consistency