warehouseAdminApi = component "Warehouse Admin API" "Административный API для просмотра остатков, поступления товара и истории складских движений. Проверяет JWT на защищённых endpoint'ах." "ASP.NET Core MVC" {
    tags "WarehouseComponent"
}

internalReservationApi = component "Internal Reservation API" "Internal API для создания и отмены резервов товаров в рамках Saga OrderMs." "ASP.NET Core MVC" {
    tags "WarehouseComponent"
}

stockCommandHandlers = component "Stock Command Handlers" "Обрабатывают поступления товара, обновляют StockItem и создают StockMovement. Используют optimistic locking." "MediatR" {
    tags "WarehouseComponent"
}

stockQueryHandlers = component "Stock Query Handlers" "Возвращают текущие складские остатки и историю движений товара." "MediatR" {
    tags "WarehouseComponent"
}

reservationHandlers = component "Reservation Handlers" "Создают и отменяют резервы. Проверяют доступный остаток и обеспечивают идемпотентность по orderId." "MediatR" {
    tags "WarehouseComponent"
}

productLifecycleConsumer = component "Product Lifecycle Consumer" "Получает ProductCreated, ProductUpdated и ProductArchived из Kafka. Создаёт, обновляет или деактивирует локальный StockItem." "BackgroundService / Kafka Consumer" {
    tags "WarehouseComponent"
}

reservationTtlWorker = component "Reservation TTL Worker" "Находит просроченные активные резервы и запускает их отмену." "BackgroundService" {
    tags "WarehouseComponent"
}

outboxPublisher = component "Outbox Publisher" "Публикует StockChanged из outbox в Kafka." "BackgroundService" {
    tags "WarehouseComponent"
}