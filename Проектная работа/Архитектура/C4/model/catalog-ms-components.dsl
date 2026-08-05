catalogApi = component "Catalog API" "Public API каталога: список товаров, карточки, бренды и категории. Проверяет JWT на защищённых endpoint'ах." "ASP.NET Core MVC" {
    tags "CatalogComponent"
}

internalCatalogApi = component "Internal Catalog API" "Internal HTTP snapshot товаров для OrderMs: читает актуальные цены с primary, проверяет expectedUnitPrice." "ASP.NET Core MVC" {
    tags "CatalogComponent"
}

productCommandHandlers = component "Product Command Handlers" "Создают, изменяют и архивируют товары. В одной транзакции сохраняют Product, синхронно обновляют ProductReadModel и OutboxMessage." "MediatR" {
    tags "CatalogComponent"
}

referenceDataCommandHandlers = component "Reference Data Command Handlers" "Создают и изменяют бренды и категории. После commit обновляют cache справочников из primary." "MediatR" {
    tags "CatalogComponent"
}

catalogQueryHandlers = component "Catalog Query Handlers" "Поиск, фильтрация и просмотр карточек товара. Читают через cache-aside с read replica." "MediatR" {
    tags "CatalogComponent"
}

catalogCacheService = component "Catalog Cache Service" "Cache-aside: список товаров (короткий TTL), бренды и категории (длинный TTL). Miss — replica; справочники после admin write — refresh из primary." "Application Service" {
    tags "CatalogComponent"
}

stockChangedConsumer = component "Stock Changed Consumer" "Получает stock.changed.v1 (topic stocks) и обновляет AvailabilityStatus в ProductReadModel на primary." "BackgroundService / Kafka Consumer" {
    tags "CatalogComponent"
}

outboxPublisher = component "Outbox Publisher" "Публикует product.created.v1, product.archived.v1 и product.restored.v1 (topic products) из outbox в Kafka." "BackgroundService" {
    tags "CatalogComponent"
}
