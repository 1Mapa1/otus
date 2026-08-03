systemContext onlineStore "SystemContext" {
    title "C1. Контекст системы интернет-заказов"
    include *
    autoLayout lr
}

container onlineStore "ContainerDiagram" {
    title "C2. Взаимодействие микросервисов"
    description "Синхронные HTTP-контракты, доменные события и внешние интеграции."
    include *
    autoLayout lr
}

