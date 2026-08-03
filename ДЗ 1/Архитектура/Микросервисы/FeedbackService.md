### Название микросервиса
FeedbackService
### Назначение и зона ответственности
FeedbackService отвечает за хранение отзывов и оценок по заказам, точкам продаж и доставке.
### Бизнес-контекст
Feedback Management
### Собственные данные
Сущности
- `OrderFeedback`
- `StoreFeedback`
- `DeliveryFeedback`
Объекты-значения
- `Rating`
- `Comment`
- `ImageInfo`
- `OrderInfo`
- `StoreInfo`
- `DeliveryInfo`
Корень агрегата:
- `Feedback`
### Высокоуровневая модель данных
#### FeedbackOrder
- `id`
- `customerId`
- `orderId`
- `comment`
- `rating`
- `imageUrl`
- `createdAt`
#### FeedbackStore
- `id`
- `customerId`
- `storeId`
- `comment`
- `rating`
- `imageUrl`
- `createdAt`
#### DeliveryFeedback
- `id`
- `customerId`
- `orderId`
- `deliveryId`
- `rating`
- `comment`
- `createdAt`
#### FeedbackChat
- отсутствуют.
### Состояния
- отсутствуют.
### Бизнес-правила
- Пользователь может оставить не более одного `OrderFeedback` на один `orderId`
- Пользователь может оставить не более одного `StoreFeedback` на один `storeId`
- Если заказ был доставлен курьером, пользователь может оставить не более одного `DeliveryFeedback` на один `orderId`
- Отзыв можно оставить только для заказа в статусе `Completed`
- Изображения отзыва хранятся во внешнем object storage
### API
#### Пользовательские операции
- **POST** `/feedback/orders`
- **POST** `/feedback/stores`
- **POST** `/feedback/deliveries`
- **GET** `/feedback/stores/{storeId}`
- **GET** `/feedback/orders/{orderId}`
- **GET** `/feedback/customers/{customerId}`
### Публикуемые события
- `OrderFeedbackCreated`
- `StoreFeedbackCreated`
- `DeliveryFeedbackCreated`
### Потребляемые события
- OrderCompleted
### Зависимости
| Сервис         | Назначение                                                              |
| :-------------- | -------------------------------------------------------------------- |
| CustomerService | источник событий о завершении заказа и данных о заказе для валидации |
| StoreService    | источник справочных данных о точке продаж                            |
| OrderService    | источник данных о пользователе                                       |

### Внешние интеграции
MinIO — хранение изображений отзывов
