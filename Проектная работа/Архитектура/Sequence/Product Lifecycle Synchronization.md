### Цель

Показать, как `CatalogMs` передаёт `WarehouseMs` информацию о создании и архивировании товара, чтобы `WarehouseMs` мог создать или деактивировать локальную складскую запись.
### Триггер

Администратор создаёт или архивирует товар в CatalogMs.
### Участники

|Участник|Роль|Основные действия|
|---|---|---|
|Admin|Администратор|Создаёт или архивирует товар|
|Frontend|Клиентское приложение|Отправляет команды управления каталогом|
|ApiGateway|Единая точка входа|Маршрутизирует запросы в CatalogMs|
|CatalogMs|Владелец товарной карточки|Создаёт и архивирует Product, публикует события|
|Kafka|Брокер сообщений|Передаёт события жизненного цикла товара|
|WarehouseMs|Владелец складских остатков|Создаёт и деактивирует локальный StockItem|
### Предусловия

- Администратор аутентифицирован и имеет роль `ADMIN`.
- Для архивирования товар существует в CatalogMs.
- Начальный остаток нового товара равен нулю.
### Диаграмма
#### Диаграмма создание товара

```mermaid
sequenceDiagram
    autonumber

    actor A as Admin
    participant F as Frontend
    participant G as ApiGateway
    participant C as CatalogMs
    participant K as Kafka
    participant W as WarehouseMs

	A->>F: Создать товар
	F->>G: POST /api/catalog/products
	G->>C: create product

	note over C: Create Product<br/>IsActive = true

	C-->>G: 201 Created
	G-->>F: 201 Created
	F-->>A: Товар создан

	C->>K: publish ProductCreated (через outbox)

	K->>W: ProductCreated
	note over W: Create StockItem<br/>AvailableQuantity = 0<br/>ReservedQuantity = 0<br/>IsActive = true
```
#### Диаграмма архивирование товара
```mermaid
sequenceDiagram
    autonumber

    actor A as Admin
    participant F as Frontend
    participant G as ApiGateway
    participant C as CatalogMs
    participant K as Kafka
    participant W as WarehouseMs

	A->>F: Архивировать товар
	F->>G: DELETE /api/catalog/products/{productId}
	G->>C: archive product

	note over C: Product IsActive = false

	C-->>G: 204 No Content
	G-->>F: 204 No Content
	F-->>A: Товар архивирован

	Note over C,K: Outbox worker публикует событие
	C->>K: publish ProductArchived (через outbox)

	K->>W: ProductArchived
	note over W: StockItem IsActive = false<br/>Запретить новые резервы
```
#### Диаграмма разархивирование товара
```mermaid
sequenceDiagram
    autonumber

    actor A as Admin
    participant F as Frontend
    participant G as ApiGateway
    participant C as CatalogMs
    participant K as Kafka
    participant W as WarehouseMs

	A->>F: Разархивировать товар
	F->>G: POST /api/catalog/products/{productId}/restore
	G->>C: restored product

	note over C: Product IsActive = true

	C-->>G: 204 No Content
	G-->>F: 204 No Content
	F-->>A: Товар разархивирован

	Note over C,K: Outbox worker публикует событие
	C->>K: publish ProductRestored (через outbox)

	K->>W: ProductRestored
	note over W: StockItem IsActive = true<br/>Разрешить новые резервы
```
### Результат

- После создания товара в `WarehouseMs` появляется локальный `StockItem`.
- Новый товар имеет нулевой доступный и зарезервированный остаток.
- После архивирования `WarehouseMs` запрещает новые резервы этого товара.
- После разархивирования `WarehouseMs` разрешает новые резервы этого товара. 
- `CatalogMs` остаётся источником истины для названия, цены, описания и других данных карточки товара.
- `WarehouseMs` хранит только `productId`, остатки и признак активности.