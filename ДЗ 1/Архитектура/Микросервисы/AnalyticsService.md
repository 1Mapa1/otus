### Название микросервиса
AnalyticsService

### Назначение и зона ответственности
AnalyticsService отвечает за сбор бизнес-событий и формирование статистики национальной сети и отдельных франчайзинговых точек. Сервис строит собственные аналитические проекции и не обращается напрямую к операционным базам других микросервисов.

### Бизнес-контекст
Business Analytics

### Собственные данные
Сущности:
- `OrderFact`
- `PaymentFact`
- `DeliveryFact`
- `FeedbackFact`
- `DailyStoreMetric`
- `DailyNationalMetric`
- `ProcessedEvent`

Объекты-значения:
- `MetricPeriod`
- `Money`
- `Rating`

Корень агрегата:
- `DailyStoreMetric`

### Высокоуровневая модель данных
#### OrderFact
- `orderId`
- `storeId`
- `customerId`
- `status`
- `totalAmount`
- `deliveryType`
- `createdAt`
- `completedAt`

#### DeliveryFact
- `deliveryId`
- `orderId`
- `storeId`
- `status`
- `estimatedMinutes`
- `actualMinutes`
- `completedAt`

#### FeedbackFact
- `feedbackId`
- `orderId`
- `storeId`
- `deliveryId`
- `rating`
- `createdAt`

#### DailyStoreMetric
- `storeId`
- `date`
- `ordersCreated`
- `ordersCompleted`
- `ordersCancelled`
- `revenue`
- `averageOrderAmount`
- `averageDeliveryMinutes`
- `averageRating`

### Состояния
- собственная state machine отсутствует;
- аналитические проекции обновляются при получении событий.

### Бизнес-правила
- аналитика не изменяет операционные данные исходных сервисов;
- каждое событие обрабатывается не более одного раза по `eventId`;
- повторная доставка события не должна повторно увеличивать показатели;
- владелец точки видит статистику только принадлежащих ему заведений;
- менеджер сети видит агрегированные показатели всей сети;
- показатели пересчитываются за выбранный период и часовой пояс точки продаж;
- персональные данные не включаются в долгосрочные аналитические агрегаты.

### API
#### Пользовательские операции
- **GET** `/analytics/stores/{storeId}?from=...&to=...`
- **GET** `/analytics/stores/{storeId}/orders`
- **GET** `/analytics/stores/{storeId}/delivery`
- **GET** `/analytics/stores/{storeId}/ratings`

#### Административные операции
- **GET** `/analytics/national?from=...&to=...`
- **POST** `/analytics/rebuild?from=...&to=...`

### Публикуемые события
- `StoreMetricsCalculated`
- `NationalMetricsCalculated`

### Потребляемые события
- `CartCheckedOut`
- `OrderCreated`
- `OrderAccepted`
- `OrderCompleted`
- `OrderCancelled`
- `PaymentCompleted`
- `PaymentRefunded`
- `DeliveryAssigned`
- `DeliveryCompleted`
- `DeliveryFailed`
- `OrderFeedbackCreated`
- `StoreFeedbackCreated`
- `DeliveryFeedbackCreated`

### Зависимости
| Сервис | Назначение |
|---|---|
| StoreService | проверка прав владельца на просмотр статистики точки |
| Event Broker | получение бизнес-событий остальных микросервисов |

### Внешние интеграции
- отсутствуют.