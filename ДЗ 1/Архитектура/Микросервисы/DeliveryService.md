### Название микросервиса
DeliveryService
### Назначение и зона ответственности
DeliveryService управляет жизненным циклом доставки: созданием доставки, поиском и назначением курьера, отслеживанием этапов доставки и завершением доставки. Сервис отвечает за состояние доставки и координирует связанные процессы уведомлений и, при необходимости, подтверждения оплаты при доставке.
### Бизнес-контекст
Delivery Management
### Собственные данные
Сущности и объекты-значения
- `Delivery`
- `Courier`
- `Money` (стоимость доставки)
- `DeliveryInfo` (адрес клиента, адрес точки, комментарий)
- `CourierInfo` (customerId и snapshot данных)
- `OrderInfo` (orderId)
- `EtaSnapshot` (estimatedTime, distance)
Корень агрегата:
- `Delivery`
### Высокоуровневая модель данных
Delivery:
- `id` 
- `orderId`  
- `status`  
- `courierId`
- `storeAddress`
- `customerAddress`
- `comment`
- `deliveryFee`
- `estimatedArrivalAt`
- `createdAt`, `updatedAt`
Courier:
- `id`
- `status` { free, busy }
- `currentLocation`
- `customerId` { профиль курьера }
### Состояния
- `Created`
- `CourierSearching`
- `CourierOfferPending`
- `CourierAssigned`
- `CourierArrivedAtShop` 
- `PickedUp`
- `OutForDelivery`
- `Delivered`
- `Failed` 
### Бизнес-правила
- У доставки может быть только один активный курьер
- После создания доставка переходит в статус `CourierSearching`
- Если назначенный курьер не подтверждает доставку в течение заданного времени, доставка возвращается в статус `CourierSearching`
- Адрес точки продаж и пользователя фиксируется при создании
- Доставка может быть только 1 на заказ
- Повторная обработка доставки должна быть идемпотентной
### API
#### Пользовательские операции
- **GET** `/delivery/{deliveryId}` 
- **GET** `/delivery?courierId=...` 
- **POST** `/orders/{orderId}/courierArrivedAtShop`
- **POST** `/orders/{orderId}/pickedUp`
- **POST** `/orders/{orderId}/outForDelivery`
- **POST** `/orders/{orderId}/delivered`  
- **POST** `/orders/{orderId}/failed`
### Публикуемые события
- DeliveryAssigned   
- DeliveryCompleted   
- CourierSearchingStarted  
- CourierOfferPending
- CourierOfferReject
- CourierAssigned
- CourierArrivedAtShop
- DeliveryPickedUp
- DeliveryOutForDelivery
- DeliveryDelivered
- DeliveryFailed
### Потребляемые события
- OrderAccepted  
- OrderReady
### Зависимости
| Сервис          | Назначение                       |
| :--------------- | ----------------------------- |
| OrderService     | получение информации о заказе |
| PaymentService   | окончание платежа наличными   |
| NotifyService    | отправка уведомлений          |
| ETAService       | расчет пути курьера           |
| AnalyticsService | сбор данных                   |
### Внешние интеграции
- отсутствуют.