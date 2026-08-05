### Название микросервиса
CustomerService
### Назначение и зона ответственности
CustomerService отвечает за хранение и управление профилем пользователя, его контактными данными, адресами доставки и пользовательскими предпочтениями.
### Бизнес-контекст
Customer Management
### Собственные данные
Сущности
- `CustomerProfile`
- `CustomerAddress`
- `CustomerPreference`
- `Avatar`
Объекты-значения
- `FullName`
- `Email`
- `Phone`
- `AddressInfo`
Корень агрегата:
- Customer
### Высокоуровневая модель данных
#### CustomerProfile
- `id`
- `accountId`
- `firstName`
- `lastName`
- `email`
- `phone`
- `status`
- `avatarId`
- `createdAt`
- `updatedAt`
#### CustomerAddress
- `id`
- `customerId`
- `addressLine`
- `entrance`
- `floor`
- `apartment`
- `comment`
- `lat`
- `lon`
- `isDefault`
- `createdAt`
- `updatedAt`
#### CustomerPreference
- `id`
- `customerId`
- `language`
- `notificationChannels`
- `createdAt`
- `updatedAt`
#### Avatar
- `id`
- `code`
- `name`
- `imageUrl`
- `isActive`

### Состояния
- `Created`
- `Active`
- `Blocked`
- `Deleted`
### Бизнес-правила
- Профиль пользователя создается один раз для одного аккаунта
- Email и phone являются мастер-данными пользователя
- Пользователь может иметь несколько адресов доставки
- Только один адрес может быть адресом по умолчанию
- Контактные данные и предпочтения могут изменяться пользователем
- Удаленный или заблокированный профиль недоступен для бизнес-операций
- Пользователь может выбрать один аватар из доступного каталога
- Пользователь не может использовать неактивный аватар
- Каталог аватаров управляется администратором
### API
#### Пользовательские операции
- **GET** `/customers/me`
- **PATCH** `/customers/me`
- **GET** `/customers/me/addresses`
- **POST** `/customers/me/addresses`
- **PATCH** `/customers/me/addresses/{addressId}`
- **DELETE** `/customers/me/addresses/{addressId}`
- **PATCH** `/customers/me/preferences`
- **GET** `/customers/me/avatars`
- **PATCH** `/customers/me/avatar`
#### Административные операции
- **GET** `/customers/{customerId}`
- **PATCH** `/customers/{customerId}/block`
- **PATCH** `/customers/{customerId}/unblock`
- **POST** `/avatars`
- **PATCH** `/avatars/{avatarId}`
- **GET** `/avatars`
### Публикуемые события
- `CustomerCreated`
- `CustomerProfileUpdated`
- `CustomerContactsChanged`
- `CustomerAddressAdded`
- `CustomerAddressUpdated`
- `CustomerAddressDeleted`
- `CustomerPreferencesChanged`
- `CustomerBlocked`
### Потребляемые события
- `AccountRegistered`
- `AccountBlocked`
### Зависимости
| Сервис     | Назначение                                      |
| :---------- | -------------------------------------------- |
| AuthService | связь профиля пользователя с учетной записью |

### Внешние интеграции
MinIO — хранение аватаров пользователей