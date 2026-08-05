### Название микросервиса
OrderService
### Назначение и зона ответственности
OrderService управляет жизненным циклом заказов пользователей.
Сервис отвечает за создание, хранение и изменение состояния заказа, а так же за координацию связанных процессов: оплаты, доставки, оплаты и уведомлений
### Бизнес-контекст
Order Management
### Собственные данные
Сущности и объекты-значения
- `Order`
- `OrderItem`
- `Money` (стоимость, валюта)
- `DeliveryInfo` (адрес/комментарий/тип доставки)
- `CustomerInfo` (customerId и snapshot контактных данных)
- `StoreInfo` (storeId и snapshot названия/адреса точки)
- `OrderStatus`
Корень агрегата:
- `Order`
### Высокоуровневая модель данных
Order:
- `id` 
- `customerId`  
- `storeId`  
- `status`  
- `pricing`: { subtotal, discountTotal, deliveryFee, total }  
- `delivery`: { type, address, comment }
- `payment`: { method, paymentId?, paymentStatus? } 
- `createdAt`, `updatedAt`
OrderItem: 
- `id`  
- `nameSnapshot`
- `orderId ` 
- `productId`  
- `quantity`
- `unitPrice`
- `totalPrice`
### Состояния
- `Created`
- `Accepted`
- `Rejected`
- `Preparing`
- `Ready`
- `Completed`
- `Cancelled`
### Бизнес-правила
- Заказ должен содержать минимум одну позицию
- Заказ нельзя изменить после статуса `Ready`
- Заказа возможно отменить только до статуса `Preparing`
- Сумма заказа фиксируется при создании
- Повторная обработка заказа должна быть идемпотентной
### API
#### Пользовательские операции
- **POST** `/orders`
- **GET** `/orders/{orderId} 
- **GET** `/orders?customerId=...` 
- **POST** `/orders/{orderId}/cancel`
- **POST** `/orders/{orderId}/accept`
- **POST** `/orders/{orderId}/reject`
- **POST** `/orders/{orderId}/ready`  
- **POST** `/orders/{orderId}/complete`
### Публикуемые события
- OrderCreated  
- OrderAccepted  
- OrderReady  
- OrderCancelled  
- OrderCompleted
### Потребляемые события
- PaymentCompleted  
- PaymentFailed  
- DeliveryAssigned  
- DeliveryCompleted
### Зависимости
| Сервис          | Назначение                       |
| :--------------- | ----------------------------- |
| CartService      | получение содержимого корзины |
| PricingService   | расчет итоговой стоимости     |
| PaymentService   | создание платежа              |
| DeliveryService  | создание доставки             |
| NotifyService    | отправка уведомлений          |
| AnalyticsService | сбор данных                   |
### Внешние интеграции
- отсутствуют.