catalogApi = component "Catalog API" "Public API каталога и административные endpoint'ы управления товарами. Проверяет JWT на защищённых endpoint'ах." "ASP.NET Core MVC" {
    tags "CatalogComponent"
}

productCommandHandlers = component "Product Command Handlers" "Создают, изменяют и архивируют товары. Сохраняют Product и OutboxMessage в одной транзакции." "MediatR" {
    tags "CatalogComponent"
}

catalogQueryHandlers = component "Catalog Query Handlers" "Выполняют поиск, фильтрацию, просмотр карточек товара и internal-запрос product snapshots для OrderMs." "MediatR" {
    tags "CatalogComponent"
}

catalogCacheService = component "Catalog Cache Service" "Реализует Cache-Aside: читает и записывает cache entries, при miss получает данные из Read Replica." "Application Service" {
    tags "CatalogComponent"
}

productReadModelProjector = component "Product Read Model Projector" "Поддерживает денормализованный ProductReadModel после изменения товара и получения StockChanged." "Application Service" {
    tags "CatalogComponent"
}

stockChangedConsumer = component "StockChanged Consumer" "Получает StockChanged из Kafka и передаёт обновление доступности в Product Read Model Projector." "BackgroundService / Kafka Consumer" {
    tags "CatalogComponent"
}

outboxPublisher = component "Outbox Publisher" "Публикует ProductCreated, ProductUpdated и ProductArchived из outbox в Kafka." "BackgroundService" {
    tags "CatalogComponent"
}