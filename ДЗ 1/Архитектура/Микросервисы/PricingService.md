### Название микросервиса
PricingService

### Назначение и зона ответственности
PricingService отвечает за расчёт итоговой стоимости заказа, управление национальными и локальными акциями, проверку промокодов и применение скидок. Сервис фиксирует результат расчёта так, чтобы OrderService мог сохранить неизменяемый снимок стоимости заказа.

### Бизнес-контекст
Pricing & Promotions

### Собственные данные
Сущности:
- `Promotion`
- `PromoCode`
- `PricingRule`
- `PromotionUsage`

Объекты-значения:
- `Money`
- `Discount`
- `PromotionPeriod`
- `PricingItem`
- `PricingResult`

Корень агрегата:
- `Promotion`

### Высокоуровневая модель данных
#### Promotion
- `id`
- `name`
- `scope` — `National` или `Local`
- `storeId` — для локальной акции
- `discountType` — процент или фиксированная сумма
- `discountValue`
- `minimumOrderAmount`
- `startsAt`
- `endsAt`
- `status`
- `createdAt`
- `updatedAt`

#### PromoCode
- `id`
- `code`
- `promotionId`
- `usageLimit`
- `usageCount`
- `isActive`
- `startsAt`
- `endsAt`

#### PricingResult
- `subtotal`
- `discountTotal`
- `deliveryFee`
- `total`
- `appliedPromotionIds`
- `calculatedAt`

### Состояния
Для акции:
- `Draft`
- `Scheduled`
- `Active`
- `Expired`
- `Cancelled`

### Бизнес-правила
- национальная акция применяется ко всем подходящим точкам продаж;
- локальная акция принадлежит только одной точке продаж;
- промокод проверяется по периоду действия, лимиту использований и условиям заказа;
- одна и та же акция не может быть применена к заказу повторно;
- итоговая сумма не может быть отрицательной;
- результат расчёта содержит исходную стоимость, все применённые скидки, стоимость доставки и итоговую сумму;
- после создания заказа его стоимость хранится в OrderService как неизменяемый снимок.

### API
#### Пользовательские операции
- **POST** `/pricing/calculate`
- **POST** `/promo-codes/validate`

#### Административные операции
- **POST** `/promotions`
- **GET** `/promotions/{promotionId}`
- **GET** `/promotions?scope=...&storeId=...`
- **PATCH** `/promotions/{promotionId}`
- **POST** `/promotions/{promotionId}/activate`
- **POST** `/promotions/{promotionId}/cancel`
- **POST** `/promo-codes`
- **PATCH** `/promo-codes/{promoCodeId}`

### Публикуемые события
- `PromotionPublished`
- `PromotionChanged`
- `PromotionCancelled`
- `PromoCodeActivated`

### Потребляемые события
- отсутствуют.
### Зависимости
| Сервис | Назначение |
|---|---|
| ProductsService | получение актуальной цены и состава выбранных позиций |
| StoreService | проверка точки продаж и принадлежности локальной акции |

### Внешние интеграции
- отсутствуют.