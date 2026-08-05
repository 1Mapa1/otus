systemContext onlineStore "SystemContext" {
    title "C1. Контекст системы — Интернет-магазин электроники"
    description "Пользователи и граница системы без отображения внутренней реализации."

    include *
    autoLayout lr
}

container onlineStore "PlatformOverview" {
    title "C2. Контейнеры — обзор платформы"
    description "API Gateway, микросервисы, event broker и независимые хранилища данных."

    include *
    autoLayout lr
}
