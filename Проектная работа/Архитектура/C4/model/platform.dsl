customer = person "Покупатель" "Пользователь интернет-магазина: просматривает каталог, управляет профилем, оформляет и отслеживает заказы."

administrator = person "Администратор" "Сотрудник магазина: управляет товарами, остатками на складе и параметрами доставки."

onlineStore = softwareSystem "Интернет-магазин электроники" "Онлайн-магазин электроники и компьютерной техники." {
    tags "OnlineStore"

    apiGateway = container "API Gateway" "Единая внешняя точка входа: маршрутизация public API, rate limiting, X-Request-ID и access logs. JWT не валидирует." "Traefik" {
        tags "Gateway"
    }

    authMs = container "AuthMs" "Регистрация, аутентификация, выпуск JWT RS256 и публикация JWKS." "ASP.NET Core (.NET 8.0)" {
        tags "Microservice"
    }

    catalogMs = container "CatalogMs" "Каталог товаров, поиск, фильтрация, цены и read model." "ASP.NET Core (.NET 8.0)" {
		tags "Microservice"

		!include catalog-ms-components.dsl
	}

	customerMs = container "CustomerMs" "Профиль клиента и адресная книга доставки." "ASP.NET Core (.NET 8.0)" {
        tags "Microservice"
    }

    warehouseMs = container "WarehouseMs" "Складские остатки, поступления, резервы товаров и движения склада." "ASP.NET Core (.NET 8.0)" {
		tags "Microservice"

		!include warehouse-ms-components.dsl
	}

    orderMs = container "OrderMs" "Заказы, snapshots и Saga Orchestration оформления заказа." "ASP.NET Core (.NET 8.0)" {
        tags "Microservice"

        !include order-ms-components.dsl
    }

    billingMs = container "BillingMs" "Внутренний prepaid wallet и обработка оплаты заказа." "ASP.NET Core (.NET 8.0)" {
        tags "Microservice"
    }

    deliveryMs = container "DeliveryMs" "Зоны, слоты доставки, capacity и резервирование доставки." "ASP.NET Core (.NET 8.0)" {
        tags "Microservice"
    }

    notificationMs = container "NotificationMs" "Получает события и сохраняет пользовательские уведомления." "ASP.NET Core (.NET 8.0)" {
        tags "Microservice"
    }

    kafka = container "Event Broker" "Передача доменных событий между микросервисами." "Apache Kafka" {
        tags "MessageBroker"
    }

    authDb = container "AuthMs Database" "Учётные записи, роли, refresh tokens и outbox." "PostgreSQL" {
        tags "Database"
    }

    customerDb = container "CustomerMs Database" "Профили клиентов, адреса и outbox." "PostgreSQL" {
        tags "Database"
    }

    catalogDb = container "CatalogMs Primary Database" "Товары, категории, бренды, read model и outbox." "PostgreSQL" {
        tags "Database"
    }

    catalogReadReplica = container "CatalogMs Read Replica" "Реплика для read-heavy запросов каталога." "PostgreSQL" {
        tags "Database"
    }

    catalogRedis = container "CatalogMs Cache" "Cache-aside: список товаров, бренды и категории." "Redis" {
        tags "Cache"
    }

    warehouseDb = container "WarehouseMs Database" "Остатки, резервы, движения склада и outbox." "PostgreSQL" {
        tags "Database"
    }

    orderDb = container "OrderMs Database" "Заказы, snapshots, состояние Saga, idempotency records и outbox." "PostgreSQL" {
        tags "Database"
    }

    billingDb = container "BillingMs Database" "Счета, платежи, ledger и inbox." "PostgreSQL" {
        tags "Database"
    }

    deliveryDb = container "DeliveryMs Database" "Зоны, слоты и резервы доставки." "PostgreSQL" {
        tags "Database"
    }

    notificationDb = container "NotificationMs Database" "Уведомления, проекция клиента и inbox." "PostgreSQL" {
        tags "Database"
    }
}
