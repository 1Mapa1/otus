onlineStore.apiGateway -> onlineStore.orderMs.ordersApi "Маршрутизирует /api/orders/*" "HTTPS/JSON"

onlineStore.orderMs.ordersApi -> onlineStore.orderMs.checkoutHandler "Передаёт create-order command" "MediatR"
onlineStore.orderMs.ordersApi -> onlineStore.orderMs.orderQueryHandlers "Передаёт order queries" "MediatR"
onlineStore.orderMs.ordersApi -> onlineStore.authMs "Получает signing keys из JWKS" "HTTPS/JWKS"

onlineStore.orderMs.checkoutHandler -> onlineStore.orderMs.idempotencyService "Проверяет и завершает идемпотентный запрос" "In-process"
onlineStore.orderMs.checkoutHandler -> onlineStore.orderMs.integrationClients "Получает product snapshots" "In-process"
onlineStore.orderMs.checkoutHandler -> onlineStore.orderDb "Создаёт Order, snapshots и idempotency record" "EF Core/PostgreSQL"

onlineStore.orderMs.orderQueryHandlers -> onlineStore.orderDb "Читает собственные заказы" "EF Core/PostgreSQL"
onlineStore.orderMs.idempotencyService -> onlineStore.orderDb "Читает и записывает IdempotencyRecord" "EF Core/PostgreSQL"

onlineStore.orderMs.sagaWorker -> onlineStore.orderDb "Находит Processing-заказы и планирует retry" "EF Core/PostgreSQL"
onlineStore.orderMs.sagaWorker -> onlineStore.orderMs.sagaOrchestrator "Передаёт заказ в обработку" "In-process"

onlineStore.orderMs.sagaOrchestrator -> onlineStore.orderMs.integrationClients "Выполняет команды Saga и компенсации" "In-process"
onlineStore.orderMs.sagaOrchestrator -> onlineStore.orderDb "Обновляет status, SagaStep и outbox" "EF Core/PostgreSQL"

onlineStore.orderMs.integrationClients -> onlineStore.catalogMs "Получает product snapshots и цены" "HTTP/JSON"
onlineStore.orderMs.integrationClients -> onlineStore.billingMs "Authorize, capture и cancel authorization" "HTTP/JSON"
onlineStore.orderMs.integrationClients -> onlineStore.warehouseMs "Создаёт и отменяет резерв товаров" "HTTP/JSON"
onlineStore.orderMs.integrationClients -> onlineStore.deliveryMs "Создаёт и отменяет резерв слота" "HTTP/JSON"

onlineStore.orderMs.outboxPublisher -> onlineStore.orderDb "Читает outbox и отмечает сообщения опубликованными" "EF Core/PostgreSQL"
onlineStore.orderMs.outboxPublisher -> onlineStore.kafka "Публикует OrderConfirmed / OrderRejected" "Kafka"