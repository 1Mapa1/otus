component onlineStore.catalogMs "CatalogMsComponents" {
    title "C3. Компоненты — CatalogMs"
    description "Внутренняя архитектура CatalogMs: CQRS-lite, cache-aside (products, brands, categories), read replica, internal snapshot на primary, Kafka consumer и outbox."

    include *?

    autoLayout lr
}