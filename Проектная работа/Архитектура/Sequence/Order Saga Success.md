### Цель  
  
Показать, как OrderMs оркестрирует успешное оформление заказа: авторизацию оплаты, резерв товара, резерв слота доставки и публикацию итогового события.  
### Триггер
Пользователь вызывает:  

```http
POST /api/orders  
```
### Участники 
| Участник    | Роль                            | Основные действия                                          |
| ----------- | ------------------------------- | ---------------------------------------------------------- |
| User        | Пользователь                    | Создает заказ                                              |
| Frontend    | Клиентское приложение           | Отображает данные о заказе                                 |
| ApiGateway  | Маршрутизирует запрос           | Маршрутизирует запросы от Frontend к микросервисам         |
| OrderMs     | Оркестратор Saga                | Создает заказ, хранит состояния Saga и вызывает участников |
| CatalogMs   | Владелец информации о продуктах | Возвращает snapshot товаров и цен                          |
| BillingMs   | Участник Saga                   | Авторизует и списывает средства                            |
| WarehouseMs | Участник Saga                   | Резервирует товары                                         |
| DeliveryMs  | Участник Saga                   | Резервирует слот доставки                                  |
| Kafka       | Брокер сообщений                | Передает финальное событие                                 |

### Предусловия  
  
- Пользователь аутентифицирован.  
- Товары существуют и активны.  
- Пользователь выбрал адрес и слот доставки.  
- На балансе пользователя достаточно средств.  
- На складе есть товары.  
- В слоте доставки есть свободная capacity.  
  
### Диаграммы

#### Диаграмма создания заказа

```mermaid  
sequenceDiagram  
	actor U as User  
	
	participant F as Frontend
	participant G as ApiGateway 
	participant OM as OrderMs  
	participant CM as CatalogMs  
	  
	autonumber
	
	U->>F: оформить заказ
	F->>G: POST /api/orders
	G->>OM: create order
	OM->>CM: GET /api/internal/catalog/products/snapshot
	CM-->>OM: snapshot
	note over OM: Status = Processing<br>SagaStep = Created<br> save order snapshot
	OM-->>G: 202 { orderId, status: Processing }
	G-->>F: 202 { orderId, status: Processing }
	
	loop check status every 10 sec
		F->>G: GET /api/orders/{orderId}
		G->>OM: get status
		OM-->>G: 200 { orderId, status: Processing }
		G-->>F: 200 { orderId, status: Processing|Competed }
		F-->>U: отрисовка статуса
	end
```

#### Диаграмма Background Worker / Saga

```mermaid  
sequenceDiagram  
	participant OS as OrderMs
    participant BS as BillingMs
    participant WS as WarehouseMs
    participant DS as DeliveryMs
    participant K as Kafka
	
	autonumber
	
	note over OS: Background worker<br>Status = Processing
	
	OS->>BS: POST /api/internal/billing/payments/authorize
	note over BS: hold amount
	BS-->>OS: Authorized
	note over OS: SagaStep = PaymentAuthorized
	
	OS->>WS: POST /api/internal/warehouse/reservations
	WS-->>OS: Reserved
	note over OS: SagaStep = StockReserved
	
	OS->>DS: POST /api/internal/delivery/reservations
	DS-->>OS: Reserved  
	note over OS: SagaStep = DeliveryReserved

	OS->>BS: POST /api/internal/billing/payments/capture 
	note over BS: decrease balance<br>release hold  
	BS-->>OS: Captured  
	note over OS: Status = Confirmed<br>SagaStep = Completed
	
	OS->>K: publish OrderConfirmed (через Outbox)
```
### Результат

- Заказ получает статус `Confirmed`.
- Средства списаны.
- Товары и слот доставки зарезервированы.
- В Kafka опубликовано событие `OrderConfirmed`.