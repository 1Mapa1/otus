customer -> onlineStore.reactSpa "Использует витрину магазина" "HTTPS"
administrator -> onlineStore.reactSpa "Использует административные экраны" "HTTPS"

onlineStore.reactSpa -> onlineStore.apiGateway "Вызывает public API" "HTTPS/JSON"

onlineStore.apiGateway -> onlineStore.authMs "Маршрутизирует /api/auth/*; получает JWKS" "HTTPS/JSON"
onlineStore.apiGateway -> onlineStore.customerMs "Маршрутизирует /api/customers/*" "HTTPS/JSON"
onlineStore.apiGateway -> onlineStore.catalogMs "Маршрутизирует /api/catalog/*" "HTTPS/JSON"
onlineStore.apiGateway -> onlineStore.warehouseMs "Маршрутизирует /api/warehouse/*" "HTTPS/JSON"
onlineStore.apiGateway -> onlineStore.orderMs "Маршрутизирует /api/orders/*" "HTTPS/JSON"
onlineStore.apiGateway -> onlineStore.billingMs "Маршрутизирует /api/billing/*" "HTTPS/JSON"
onlineStore.apiGateway -> onlineStore.deliveryMs "Маршрутизирует /api/delivery/*" "HTTPS/JSON"
onlineStore.apiGateway -> onlineStore.notificationMs "Маршрутизирует /api/notifications/*" "HTTPS/JSON"

onlineStore.authMs -> onlineStore.customerMs "Создаёт профиль при регистрации" "HTTP/JSON" {
    tags "InternalHttp"
}

onlineStore.orderMs -> onlineStore.catalogMs "Получает product snapshots и цены" "HTTP/JSON" {
    tags "InternalHttp"
}

onlineStore.orderMs -> onlineStore.billingMs "Authorize, capture и cancel authorization" "HTTP/JSON" {
    tags "InternalHttp"
}

onlineStore.orderMs -> onlineStore.warehouseMs "Создаёт и отменяет резерв товаров" "HTTP/JSON" {
    tags "InternalHttp"
}

onlineStore.orderMs -> onlineStore.deliveryMs "Создаёт и отменяет резерв слота" "HTTP/JSON" {
    tags "InternalHttp"
}

onlineStore.authMs -> onlineStore.kafka "Публикует UserActivated" "Kafka" {
    tags "Async"
}

onlineStore.catalogMs -> onlineStore.kafka "Публикует ProductCreated, ProductUpdated, ProductArchived" "Kafka" {
    tags "Async"
}

onlineStore.warehouseMs -> onlineStore.kafka "Публикует StockChanged" "Kafka" {
    tags "Async"
}

onlineStore.customerMs -> onlineStore.kafka "Публикует CustomerCreated, CustomerUpdated" "Kafka" {
    tags "Async"
}

onlineStore.orderMs -> onlineStore.kafka "Публикует OrderConfirmed, OrderRejected" "Kafka" {
    tags "Async"
}

onlineStore.kafka -> onlineStore.billingMs "Доставляет UserActivated" "Kafka" {
    tags "Async"
}

onlineStore.kafka -> onlineStore.warehouseMs "Доставляет события жизненного цикла товара" "Kafka" {
    tags "Async"
}

onlineStore.kafka -> onlineStore.catalogMs "Доставляет StockChanged" "Kafka" {
    tags "Async"
}

onlineStore.kafka -> onlineStore.notificationMs "Доставляет customer events и финальные события заказа" "Kafka" {
    tags "Async"
}

onlineStore.authMs -> onlineStore.authDb "Читает и записывает данные" "PostgreSQL"
onlineStore.customerMs -> onlineStore.customerDb "Читает и записывает данные" "PostgreSQL"

onlineStore.catalogMs -> onlineStore.catalogDb "Записывает каталог, read model и outbox" "PostgreSQL"
onlineStore.catalogMs -> onlineStore.catalogReadReplica "Читает данные каталога" "PostgreSQL"
onlineStore.catalogDb -> onlineStore.catalogReadReplica "Реплицирует данные" "PostgreSQL replication"
onlineStore.catalogMs -> onlineStore.catalogRedis "Использует Cache-Aside" "Redis protocol"

onlineStore.warehouseMs -> onlineStore.warehouseDb "Читает и записывает данные" "PostgreSQL"
onlineStore.orderMs -> onlineStore.orderDb "Читает и записывает данные" "PostgreSQL"
onlineStore.billingMs -> onlineStore.billingDb "Читает и записывает данные" "PostgreSQL"
onlineStore.deliveryMs -> onlineStore.deliveryDb "Читает и записывает данные" "PostgreSQL"
onlineStore.notificationMs -> onlineStore.notificationDb "Читает и записывает данные" "PostgreSQL"