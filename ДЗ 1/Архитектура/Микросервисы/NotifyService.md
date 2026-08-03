### Название микросервиса
NotifyService
### Назначение и зона ответственности
NotifyService отвечает за отправку уведомлений в системе.
### Бизнес-контекст
Notify Management
### Собственные данные
Сущности
- `NotificationProfile`
- `NotificationMessage`
Объекты-значения
- `EmailAddress`
- `PhoneNumber`
- `NotificationChannel`
- `NotificationPreference`
Корень агрегата:
- `Notification`
### Высокоуровневая модель данных
#### NotificationProfile
- `id`
- `customerId`
- `email`
- `phone`
- `isEmailVerified`
- `isPhoneVerified`
- `createdAt`
- `updatedAt`
#### NotificationMessage
- `id`
- `customerId`
- `type`
- `channels`
- `status`
- `retryCount`
- `createdAt`
- `sentAt`
### Состояния
- `Created`
- `Pending`
- `Sending`
- `Sent`
- `Failed`
### Бизнес-правила
- Контактные данные пользователя мастерятся в `CustomerService`
- `NotifyService` хранит локальную копию контактных данных и настроек уведомлений
- При недоступности внешнего канала уведомление повторно отправляется по retry policy
- Уведомление может быть отправлено по нескольким каналам одновременно
- Отправка уведомлений должна быть идемпотентной
### API
#### Пользовательские операции
- **GET** `/notifications/{customerId}`
- **WS** `/notifications/{customerId}/stream`
#### Административные операции
- **PATCH** `/notifications/{customerId}/preferences`
- **GET** `/notifications/messages/{messageId}`
### Публикуемые события
- NotificationSent
- NotificationFailed
### Потребляемые события
- OrderAccepted  
- OrderReady  
- DeliveryAssigned      
- CourierSearchingStarted  
- CourierOfferPending
- CourierOfferReject
- CourierAssigned
- CourierArrivedAtShop
- DeliveryPickedUp
- DeliveryOutForDelivery
- CustomerCreate
- CustomerContactsChanged
- CustomerNotificationPreferencesChanged
### Зависимости
| Сервис         | Назначение                                                    |
| :-------------- | ---------------------------------------------------------- |
| CustomerService | проверка существования и получение ссылок на пользователей |

### Внешние интеграции
SMTP-Server - сервер отправки почтой
SMS API - отправка sms