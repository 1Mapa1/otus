### Назначение

CatalogMs отвечает за товарный каталог интернет-магазина.

Сервис владеет карточками товаров, категориями, брендами, характеристиками, ценами и изображениями. Он предоставляет пользовательский поиск и просмотр товаров, а также административное управление ассортиментом.
### Зона ответственности
- создание, изменение и архивирование товаров;
- хранение названия, описания, цены, бренда, категории, характеристик и изображений;
- поиск, фильтрация и пагинация товаров;
- отображение карточки товара;
- получение snapshot товаров для оформления заказа;
- поддержание read model каталога;
- хранение актуального статуса наличия товара;
- публикация событий жизненного цикла товара;
- кеширование read-heavy запросов.
### Основные сущности
- `Product`
- `Category`
- `Brand`
- `ProductAttribute`
- `ProductReadModel`
- `OutboxMessage`
#### Product

Основная товарная карточка.

Поля:
- `Id`
- `Name`
- `Description`
- `CategoryId`
- `BrandId`
- `Price`
- `Attributes`
- `ImageUrl`
- `IsActive`
- `CreatedAt`
- `UpdatedAt`

`Product` является источником истины для названия, цены и описания товара.
#### Category

Категория товара.

Примеры:
- ноутбуки;
- мониторы;
- комплектующие;
- периферия.
#### Brand

Бренд товара.

Примеры:
- ASUS;
- Lenovo;
- Samsung;
- Logitech.
#### ProductReadModel

Денормализованная read model для пользовательских запросов.

Содержит данные товарной карточки, необходимые для поиска и отображения:
- название;
- цена;
- бренд;
- категория;
- изображение;
- статус доступности;
- признак активности.

Read model обновляется при изменении товара и после получения события `StockChanged`.

#### OutboxMessage

Сообщение для надежной публикации доменных событий в Kafka.

Изменение товара и создание записи в outbox выполняются в одной транзакции базы данных.
### HTTP API

| Method | Endpoint                                    | Назначение                                     | Доступ   |
| ------ | ------------------------------------------- | ---------------------------------------------- | -------- |
| GET    | `/api/catalog/products`                     | Поиск, фильтрация и постраничный вывод товаров | PUBLIC   |
| GET    | `/api/catalog/products/{productId}`         | Получить карточку товара                       | PUBLIC   |
| GET    | `/api/catalog/categories`                   | Получить список категорий                      | PUBLIC   |
| GET    | `/api/catalog/brands`                       | Получить список брендов                        | PUBLIC   |
| POST   | `/api/internal/catalog/products/snapshot`   | Получить snapshot товаров для создания заказа  | INTERNAL |
| POST   | `/api/catalog/products`                     | Создать товар                                  | ADMIN    |
| PUT    | `/api/catalog/products/{productId}`         | Изменить товар                                 | ADMIN    |
| DELETE | `/api/catalog/products/{productId}`         | Архивировать товар                             | ADMIN    |
| POST   | `/api/catalog/products/{productId}/restore` | Разархивировать товар                          | ADMIN    |
### Взаимодействует с

|Сервис|Протокол|Зачем|
|---|---|---|
|WarehouseMs|Kafka|Передает события жизненного цикла товара и получает изменения складской доступности|
|OrderMs|HTTP|Возвращает актуальные товарные snapshots для нового заказа|
|AuthMs|JWKS|Получает публичный ключ для проверки JWT на административных endpoint'ах|
|ApiGateway|HTTP|Получает внешние запросы каталога через маршрутизацию gateway|
### Kafka events

Публикует:
- `ProductCreatedV1` (`product.created.v1`) 
- `ProductRestoredV1` (`product.restored.v1`) 
- `ProductArchivedV1` (`product.archived.v1`) 

Потребляет:
- `StockChangedV1` (`stock.changed.v1`)
### Ключевые паттерны
- Database per Service
- Cache-Aside
- Redis
- Read Replication
- CQRS-lite
- Read Model
- Outbox Pattern
- Event-Driven Architecture
- Eventual Consistency
- JWT Validation via JWKS
- Role-Based Access Control