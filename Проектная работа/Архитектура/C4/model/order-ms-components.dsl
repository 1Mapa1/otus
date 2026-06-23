ordersApi = component "Orders API" "Public API для создания заказа и получения собственных заказов. Проверяет JWT через ASP.NET Core middleware." "ASP.NET Core MVC" {
    tags "OrderComponent"
}

checkoutHandler = component "Checkout Command Handler" "Создаёт заказ в статусе Processing, сохраняет product snapshots и snapshot адреса, рассчитывает итоговую сумму." "MediatR" {
    tags "OrderComponent"
}

orderQueryHandlers = component "Order Query Handlers" "Возвращает список собственных заказов и детали конкретного заказа." "MediatR" {
    tags "OrderComponent"
}

idempotencyService = component "Idempotency Service" "Защищает создание заказа от повторного выполнения по Idempotency-Key." "Application Service" {
    tags "OrderComponent"
}

sagaWorker = component "Saga Background Worker" "Находит Processing-заказы, выполняет retry технически неуспешных шагов и запускает обработку Saga." "BackgroundService" {
    tags "OrderComponent"
}

sagaOrchestrator = component "Saga Orchestrator" "Управляет State Machine и обработчиками шагов AuthorizePayment, ReserveStock, ReserveDelivery, CapturePayment и Compensating." "Application Service" {
    tags "OrderComponent"
}

integrationClients = component "Integration Clients" "HTTP-клиенты CatalogMs, BillingMs, WarehouseMs и DeliveryMs." "HttpClient" {
    tags "OrderComponent"
}

outboxPublisher = component "Outbox Publisher" "Публикует непереданные OrderConfirmed и OrderRejected из outbox в Kafka." "BackgroundService" {
    tags "OrderComponent"
}