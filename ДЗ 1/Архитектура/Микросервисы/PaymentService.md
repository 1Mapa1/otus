### Название микросервиса
PaymentService

### Назначение и зона ответственности
PaymentService отвечает за создание и проведение онлайн- и офлайн-платежей, хранение состояния оплаты, подтверждение получения наличных и выполнение возвратов при отмене заказа. Сервис изолирует остальные микросервисы от особенностей внешнего платёжного провайдера.

### Бизнес-контекст
Payment Management

### Собственные данные
Сущности:
- `Payment`
- `Refund`
- `PaymentAttempt`

Объекты-значения:
- `Money`
- `PaymentMethod`
- `ProviderReference`

Корень агрегата:
- `Payment`

### Высокоуровневая модель данных
#### Payment
- `id`
- `orderId`
- `customerId`
- `amount`
- `currency`
- `method` — `Online` или `Offline`
- `status`
- `providerPaymentId`
- `idempotencyKey`
- `createdAt`
- `updatedAt`
- `paidAt`

#### PaymentAttempt
- `id`
- `paymentId`
- `providerRequestId`
- `status`
- `failureCode`
- `createdAt`

#### Refund
- `id`
- `paymentId`
- `amount`
- `status`
- `providerRefundId`
- `reason`
- `createdAt`
- `completedAt`

### Состояния
Для платежа:
- `Created`
- `Pending`
- `Paid`
- `Failed`
- `Cancelled`
- `RefundRequested`
- `Refunded`

Для возврата:
- `Requested`
- `Processing`
- `Completed`
- `Failed`

### Бизнес-правила
- для одного заказа существует не более одного активного платежа;
- сумма платежа должна совпадать с зафиксированной стоимостью заказа;
- повторная команда с тем же ключом идемпотентности не создаёт новый платёж;
- онлайн-платёж считается завершённым только после подтверждения провайдера;
- офлайн-платёж подтверждается сотрудником точки или курьером;
- возврат создаётся только для успешно оплаченного заказа;
- сумма возврата не может превышать сумму платежа;
- результат обработки платежа публикуется как доменное событие.

### API
#### Пользовательские операции
- **POST** `/payments`
- **GET** `/payments/{paymentId}`
- **GET** `/payments?orderId=...`

#### Внутренние операции
- **POST** `/payments/{paymentId}/confirm`
- **POST** `/payments/{paymentId}/fail`
- **POST** `/payments/{paymentId}/cancel`
- **POST** `/payments/{paymentId}/refund`
- **POST** `/payments/provider/callback`

### Публикуемые события
- `PaymentCreated`
- `PaymentCompleted`
- `PaymentFailed`
- `PaymentRefundRequested`
- `PaymentRefunded`

### Потребляемые события
- `OrderCancelled`

### Зависимости
| Сервис | Назначение |
|---|---|
| OrderService | получение идентификатора заказа и зафиксированной суммы оплаты |

### Внешние интеграции
- внешний платёжный провайдер — проведение онлайн-платежей и возвратов.

