### Название микросервиса
ETAService
### Назначение и зона ответственности
ETAService отвечает за расчёт маршрута, расстояния и примерного времени в пути между двумя точками.  
Сервис выбирает подходящего картографического провайдера, вызывает внешний API и возвращает результат другим сервисам системы.
### Бизнес-контекст
Geo & Routing
### Собственные данные
Сущности и объекты-значения
- `MapProvider`
- `Address`
- `GeoPoint`
- `RouteResult`
Корень агрегата:
- `RouteRequest`
### Высокоуровневая модель данных
RouteRequest:
- `id` 
- `isTrackPosition`  
- `status`  
- `externalRequestId`
- `providerId`
- `fromAddress`
- `toAddress`
- `etaMinutes`
- `distanceMeters`
- `routeSummary`
- `createdAt`, `updatedAt`
MapProvider:
- `id`
- `name`
- `apiKey`
- `baseUrl`
- `priority`
- `region`
- `isAvailable`
- `hasLiveTracking`
### Состояния
- `Created`
- `ProviderSelected`
- `Calculated`
- `Failed`
### Бизнес-правила
- Поставщик карт выбирается по приоритету и региону
- При недоступности основного поставщика используется fallback provider
- Результат расчёта ETA может кэшироваться на ограниченное время
- Для track-position используются только провайдеры, поддерживающие live tracking
- Если ни один провайдер недоступен, запрос переходит в статус `Failed`
### API
#### Пользовательские операции
- **POST** `/routes ` 
- **GET** `/routes/{routesId}`
#### Административные операции
- **GET** `/providers`  
- **GET** `/providers/{providerId}`
- **POST** `/providers`  
- **PATCH** `/providers/{providerId}`
### Публикуемые события
- отсутствуют.
### Потребляемые события
- отсутствуют.
### Зависимости
| Сервис          | Назначение                                         |
| :--------------- | ----------------------------------------------- |
| DeliveryService  | маршрут до точки пользователя и до пользователя |
| OrderService     | маршрут до точки продаж при самовывозе          |

### Внешние интеграции
- GoogleMaps
- Яндекс Карты
- 2ГИС
- Maps.me
- ...

