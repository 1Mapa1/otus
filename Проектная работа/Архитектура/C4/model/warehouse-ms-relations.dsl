onlineStore.apiGateway -> onlineStore.warehouseMs.warehouseAdminApi "Маршрутизирует /api/warehouse/*" "HTTPS/JSON"
onlineStore.orderMs -> onlineStore.warehouseMs.internalReservationApi "Создаёт и отменяет резерв товаров" "HTTP/JSON"

onlineStore.warehouseMs.warehouseAdminApi -> onlineStore.warehouseMs.stockCommandHandlers "Передаёт stock commands" "MediatR"
onlineStore.warehouseMs.warehouseAdminApi -> onlineStore.warehouseMs.stockQueryHandlers "Передаёт stock queries" "MediatR"
onlineStore.warehouseMs.warehouseAdminApi -> onlineStore.authMs "Получает signing keys из JWKS" "HTTPS/JWKS"

onlineStore.warehouseMs.internalReservationApi -> onlineStore.warehouseMs.reservationHandlers "Передаёт reserve/cancel commands" "MediatR"

onlineStore.warehouseMs.stockCommandHandlers -> onlineStore.warehouseDb "Обновляет StockItem, StockMovement и outbox" "EF Core/PostgreSQL"
onlineStore.warehouseMs.stockQueryHandlers -> onlineStore.warehouseDb "Читает остатки и историю движений" "EF Core/PostgreSQL"

onlineStore.warehouseMs.reservationHandlers -> onlineStore.warehouseDb "Атомарно обновляет StockItem, StockReservation, StockMovement и outbox" "EF Core/PostgreSQL"

onlineStore.kafka -> onlineStore.warehouseMs.productLifecycleConsumer "Доставляет product.created.v1 / product.archived.v1 / product.restored.v1 (topic products)" "Kafka"
onlineStore.warehouseMs.productLifecycleConsumer -> onlineStore.warehouseDb "Создаёт, архивирует или восстанавливает StockItem" "EF Core/PostgreSQL"

onlineStore.warehouseMs.outboxPublisher -> onlineStore.warehouseDb "Читает outbox и отмечает сообщения опубликованными" "EF Core/PostgreSQL"
onlineStore.warehouseMs.outboxPublisher -> onlineStore.kafka "Публикует stock.changed.v1 (topic stocks)" "Kafka"
