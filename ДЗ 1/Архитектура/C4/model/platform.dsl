customer = person "Покупатель" "Формирует корзину, оформляет, оплачивает и оценивает заказ."
courier = person "Курьер" "Получает доставку, следует по маршруту и изменяет её статус."
storeEmployee = person "Сотрудник точки" "Подтверждает заказ и отмечает его готовность."
storeOwner = person "Владелец точки" "Управляет заведением, меню, локальными акциями и статистикой."
networkManager = person "Менеджер сети" "Согласует точки и управляет национальными акциями."

maps = softwareSystem "Картографические сервисы" "Google Maps, Яндекс Карты, 2ГИС и другие поставщики маршрутов." {
    tags "External"
}
paymentProvider = softwareSystem "Платёжный провайдер" "Проводит онлайн-платежи и возвраты." {
    tags "External"
}
messageChannels = softwareSystem "Каналы уведомлений" "SMTP, SMS и push-провайдеры." {
    tags "External"
}
objectStorage = softwareSystem "Object Storage" "Хранит изображения точек, товаров, аватаров и отзывов." {
    tags "External"
}

onlineStore = softwareSystem "Система интернет-заказов" "Онлайн-заказы сети сэндвич-кафе." {
    tags "OnlineStore"

    clientApps = container "Клиентские приложения" "Web- и mobile-интерфейсы покупателей, курьеров и сотрудников." "Web / Mobile" {
        tags "Frontend"
    }
    auth = container "AuthService" "Аккаунты, аутентификация, сессии и роли." "Microservice" {
        tags "Core"
    }

    customer = container "CustomerService" "Профили, контакты, адреса и предпочтения." "Microservice" {
        tags "Core"
    }

    store = container "StoreService" "Точки продаж, расписание, персонал и согласование." "Microservice" {
        tags "Commerce"
    }

    products = container "ProductsService" "Каталог продуктов и меню конкретных точек." "Microservice" {
        tags "Commerce"
    }

    cart = container "CartService" "Корзины покупателей и их позиции." "Microservice" {
        tags "Commerce"
    }

    pricing = container "PricingService" "Цены, национальные и локальные акции, промокоды." "Microservice" {
        tags "Commerce"
    }

    order = container "OrderService" "Создание заказа и управление его жизненным циклом." "Microservice" {
        tags "OrderFlow"
    }

    payment = container "PaymentService" "Онлайн- и офлайн-платежи и возвраты." "Microservice" {
        tags "OrderFlow"
    }

    delivery = container "DeliveryService" "Поиск курьера и жизненный цикл доставки." "Microservice" {
        tags "OrderFlow"
    }

    eta = container "ETAService" "Маршруты, расстояния и ожидаемое время прибытия." "Microservice" {
        tags "Supporting"
    }

    feedback = container "FeedbackService" "Отзывы о заказах, точках и доставке." "Microservice" {
        tags "Supporting"
    }

    notify = container "NotifyService" "Уведомления через выбранные каналы." "Microservice" {
        tags "Supporting"
    }

    analytics = container "AnalyticsService" "Статистика сети и отдельных точек." "Microservice" {
        tags "Supporting"
    }

    broker = container "Event Broker" "Доставка доменных событий между сервисами." "Message Broker" {
        tags "Broker"
    }
}
