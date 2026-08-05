### Название микросервиса
StoreService
### Назначение и зона ответственности
StoreService отвечает за управление точками продаж, их настройками, временем работы и статусом доступности, а также за процесс согласования точки продаж менеджером.
### Бизнес-контекст
Store Management
### Собственные данные
Сущности
- `Store`
- `StoreHours`
- `StoreStaff`
Объекты-значения
- `Address`
- `ImageInfo`
Корень агрегата:
- `Store`
### Высокоуровневая модель данных
#### Store
- `id`
- `name`
- `description`
- `baseImageUrl`
- `hasDelivery`
- `address`
- `approvalStatus`
- `approvedByUserId`
- `createdAt`
- `updatedAt`
#### StoreHours
- `id`
- `storeId`
- `dayOfWeek` { 1 - 7 }
- `openTime`
- `closeTime`
- `isClosed`
#### StoreStaff
- `id`
- `storeId`
- `customerId`
- `role` `{ owner, manager, operator }`
- `isActive`
- `createdAt`
### Состояния
- `Created`
- `PendingApproval`
- `Approved`
- `Rejected`
- `Inactive`
### Бизнес-правила
- Общие данные точки продаж определяются на уровне `Store`
- Время работы точки продаж определяется на уровне `StoreHours`
- Недоступная или не одобренная точка продаж не должна отображаться пользователям
- При создании точка продаж получает статус `PendingApproval`
- Одобрить точку продаж может только пользователь с ролью `Manager`
- Изображения точки продаж хранятся во внешнем object storage
### API
#### Пользовательские операции
- **GET** `/stores/{storeId}`
- **GET** `/stores?is_closed=...`
#### Административные операции
- **POST** `/stores`
- **GET** `/stores/{storeId}`
- **GET** `/stores?is_closed=...` 
- **PATCH** `/stores/{storeId}`
- **POST** `/stores{storeId}/approve`
- **POST** `/stores{storeId}/reject`
- **POST** `/stores/{storeId}/staff`
- **GET** `/stores/{storeId}/staff`
- **PATCH** `/stores/{storeId}/staff/{staffId}`
### Публикуемые события
- отсутствуют.
### Потребляемые события
- отсутствуют.
### Зависимости
| Сервис         | Назначение                                                                             |
| :-------------- | ----------------------------------------------------------------------------------- |
| CustomerService | проверка существования owner, manager и operator, получение ссылок на пользователей |

### Внешние интеграции
MinIO — хранение изображений точки продаж