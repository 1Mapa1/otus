onlineStore.apiGateway -> onlineStore.catalogMs.catalogApi "Маршрутизирует /api/catalog/*" "HTTPS/JSON"
onlineStore.orderMs -> onlineStore.catalogMs.catalogQueryHandlers "Запрашивает product snapshots для checkout" "HTTP/JSON"

onlineStore.catalogMs.catalogApi -> onlineStore.catalogMs.productCommandHandlers "Передаёт product commands" "MediatR"
onlineStore.catalogMs.catalogApi -> onlineStore.catalogMs.catalogQueryHandlers "Передаёт catalog queries" "MediatR"
onlineStore.catalogMs.catalogApi -> onlineStore.authMs "Получает signing keys из JWKS" "HTTPS/JWKS"

onlineStore.catalogMs.productCommandHandlers -> onlineStore.catalogMs.productReadModelProjector "Обновляет ProductReadModel после изменения товара" "In-process"
onlineStore.catalogMs.productCommandHandlers -> onlineStore.catalogMs.catalogCacheService "Инвалидирует cache entries товара и поиска" "In-process"
onlineStore.catalogMs.productCommandHandlers -> onlineStore.catalogDb "Сохраняет Product и OutboxMessage" "EF Core/PostgreSQL"

onlineStore.catalogMs.catalogQueryHandlers -> onlineStore.catalogMs.catalogCacheService "Запрашивает данные каталога" "In-process"
onlineStore.catalogMs.catalogQueryHandlers -> onlineStore.catalogDb "Получает актуальные product snapshots для OrderMs" "EF Core/PostgreSQL"

onlineStore.catalogMs.catalogCacheService -> onlineStore.catalogRedis "Читает и записывает cache entries" "Redis protocol"
onlineStore.catalogMs.catalogCacheService -> onlineStore.catalogReadReplica "При cache miss читает ProductReadModel" "EF Core/PostgreSQL"

onlineStore.kafka -> onlineStore.catalogMs.stockChangedConsumer "Доставляет StockChanged" "Kafka"
onlineStore.catalogMs.stockChangedConsumer -> onlineStore.catalogMs.productReadModelProjector "Передаёт изменение availability status" "In-process"

onlineStore.catalogMs.productReadModelProjector -> onlineStore.catalogMs.catalogCacheService "Инвалидирует affected cache entries" "In-process"
onlineStore.catalogMs.productReadModelProjector -> onlineStore.catalogDb "Обновляет ProductReadModel" "EF Core/PostgreSQL"

onlineStore.catalogMs.outboxPublisher -> onlineStore.catalogDb "Читает outbox и отмечает сообщения опубликованными" "EF Core/PostgreSQL"
onlineStore.catalogMs.outboxPublisher -> onlineStore.kafka "Публикует ProductCreated / ProductUpdated / ProductArchived" "Kafka"