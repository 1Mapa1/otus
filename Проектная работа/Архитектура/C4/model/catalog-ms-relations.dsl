onlineStore.apiGateway -> onlineStore.catalogMs.catalogApi "Маршрутизирует /api/catalog/*" "HTTPS/JSON"
onlineStore.orderMs -> onlineStore.catalogMs.internalCatalogApi "Запрашивает product snapshot для checkout" "HTTP/JSON"

onlineStore.catalogMs.catalogApi -> onlineStore.catalogMs.productCommandHandlers "Передаёт product commands" "MediatR"
onlineStore.catalogMs.catalogApi -> onlineStore.catalogMs.referenceDataCommandHandlers "Передаёт brand/category commands" "MediatR"
onlineStore.catalogMs.catalogApi -> onlineStore.catalogMs.catalogQueryHandlers "Передаёт catalog queries" "MediatR"
onlineStore.catalogMs.catalogApi -> onlineStore.authMs "Получает signing keys из JWKS" "HTTPS/JWKS"

onlineStore.catalogMs.internalCatalogApi -> onlineStore.catalogDb "Читает актуальные цены и проверяет expectedUnitPrice" "EF Core/PostgreSQL"

onlineStore.catalogMs.productCommandHandlers -> onlineStore.catalogDb "Сохраняет Product, ProductReadModel и OutboxMessage" "EF Core/PostgreSQL"

onlineStore.catalogMs.referenceDataCommandHandlers -> onlineStore.catalogDb "Сохраняет Brand и Category" "EF Core/PostgreSQL"
onlineStore.catalogMs.referenceDataCommandHandlers -> onlineStore.catalogMs.catalogCacheService "Обновляет cache брендов и категорий из primary" "In-process"

onlineStore.catalogMs.catalogQueryHandlers -> onlineStore.catalogMs.catalogCacheService "Запрашивает кешированные read-данные" "In-process"
onlineStore.catalogMs.catalogQueryHandlers -> onlineStore.catalogReadReplica "При cache miss читает ProductReadModel и справочники" "EF Core/PostgreSQL"

onlineStore.catalogMs.catalogCacheService -> onlineStore.catalogRedis "Читает и записывает cache entries" "Redis protocol"
onlineStore.catalogMs.catalogCacheService -> onlineStore.catalogReadReplica "При cache miss читает с read replica" "EF Core/PostgreSQL"

onlineStore.kafka -> onlineStore.catalogMs.stockChangedConsumer "Доставляет stock.changed.v1 (topic stocks)" "Kafka"
onlineStore.catalogMs.stockChangedConsumer -> onlineStore.catalogDb "Обновляет AvailabilityStatus в ProductReadModel" "EF Core/PostgreSQL"

onlineStore.catalogMs.outboxPublisher -> onlineStore.catalogDb "Читает outbox и отмечает сообщения опубликованными" "EF Core/PostgreSQL"
onlineStore.catalogMs.outboxPublisher -> onlineStore.kafka "Публикует product.created.v1 / product.archived.v1 / product.restored.v1 (topic products)" "Kafka"
