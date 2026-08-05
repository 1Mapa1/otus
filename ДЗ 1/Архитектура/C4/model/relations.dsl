customer -> onlineStore.clientApps "Оформляет и отслеживает заказ" "HTTPS"
courier -> onlineStore.clientApps "Работает с доставками" "HTTPS"
storeEmployee -> onlineStore.clientApps "Обрабатывает очередь заказов" "HTTPS"
storeOwner -> onlineStore.clientApps "Управляет точкой продаж" "HTTPS"
networkManager -> onlineStore.clientApps "Управляет сетью" "HTTPS"

onlineStore.clientApps -> onlineStore.auth "Регистрация и вход" "HTTP/JSON"
onlineStore.clientApps -> onlineStore.customer "Профиль и адреса" "HTTP/JSON"
onlineStore.clientApps -> onlineStore.store "Точки продаж и персонал" "HTTP/JSON"
onlineStore.clientApps -> onlineStore.products "Каталог и меню" "HTTP/JSON"
onlineStore.clientApps -> onlineStore.cart "Корзина" "HTTP/JSON"
onlineStore.clientApps -> onlineStore.order "Заказы" "HTTP/JSON"
onlineStore.clientApps -> onlineStore.delivery "Доставка курьера" "HTTP/JSON"
onlineStore.clientApps -> onlineStore.feedback "Отзывы" "HTTP/JSON"
onlineStore.clientApps -> onlineStore.analytics "Статистика" "HTTP/JSON"

onlineStore.store -> onlineStore.customer "Проверяет владельца и сотрудников" "HTTP/JSON"
onlineStore.products -> onlineStore.store "Проверяет точку продаж" "HTTP/JSON"
onlineStore.cart -> onlineStore.customer "Проверяет покупателя" "HTTP/JSON"
onlineStore.cart -> onlineStore.store "Проверяет активность точки" "HTTP/JSON"
onlineStore.cart -> onlineStore.products "Получает цену и доступность" "HTTP/JSON"
onlineStore.pricing -> onlineStore.products "Получает снимки позиций" "HTTP/JSON"
onlineStore.pricing -> onlineStore.store "Проверяет локальные акции" "HTTP/JSON"
onlineStore.order -> onlineStore.cart "Получает и закрывает корзину" "HTTP/JSON"
onlineStore.order -> onlineStore.pricing "Рассчитывает стоимость" "HTTP/JSON"
onlineStore.order -> onlineStore.payment "Создаёт платёж" "HTTP/JSON"
onlineStore.order -> onlineStore.delivery "Создаёт доставку" "HTTP/JSON"
onlineStore.order -> onlineStore.eta "Получает маршрут самовывоза" "HTTP/JSON"
onlineStore.delivery -> onlineStore.eta "Рассчитывает маршрут курьера" "HTTP/JSON"
onlineStore.delivery -> onlineStore.payment "Подтверждает оплату при доставке" "HTTP/JSON"
onlineStore.feedback -> onlineStore.order "Проверяет завершение заказа" "HTTP/JSON"
onlineStore.feedback -> onlineStore.store "Проверяет точку продаж" "HTTP/JSON"

onlineStore.eta -> maps "Запрашивает маршрут и дорожную обстановку" "HTTPS/JSON"
onlineStore.payment -> paymentProvider "Проводит оплату и возврат" "HTTPS/JSON"
onlineStore.notify -> messageChannels "Отправляет уведомления" "SMTP/SMS/Push"
onlineStore.customer -> objectStorage "Хранит аватары" "S3 API"
onlineStore.store -> objectStorage "Хранит изображения точек" "S3 API"
onlineStore.products -> objectStorage "Хранит изображения продуктов" "S3 API"
onlineStore.feedback -> objectStorage "Хранит изображения отзывов" "S3 API"

onlineStore.auth -> onlineStore.broker "AccountRegistered, AccountBlocked" "Events" {
    tags "Async"
}
onlineStore.customer -> onlineStore.broker "Customer contacts and preferences changed" "Events" {
    tags "Async"
}
onlineStore.cart -> onlineStore.broker "CartCheckedOut" "Events" {
    tags "Async"
}
onlineStore.order -> onlineStore.broker "Order lifecycle events" "Events" {
    tags "Async"
}
onlineStore.payment -> onlineStore.broker "Payment lifecycle events" "Events" {
    tags "Async"
}
onlineStore.delivery -> onlineStore.broker "Delivery lifecycle events" "Events" {
    tags "Async"
}
onlineStore.feedback -> onlineStore.broker "Feedback created events" "Events" {
    tags "Async"
}

onlineStore.broker -> onlineStore.customer "Account events" "Events" {
    tags "Async"
}
onlineStore.broker -> onlineStore.order "Payment and delivery events" "Events" {
    tags "Async"
}
onlineStore.broker -> onlineStore.delivery "Order events" "Events" {
    tags "Async"
}
onlineStore.broker -> onlineStore.feedback "OrderCompleted" "Events" {
    tags "Async"
}
onlineStore.broker -> onlineStore.notify "Customer, order, payment and delivery events" "Events" {
    tags "Async"
}
onlineStore.broker -> onlineStore.analytics "Operational business events" "Events" {
    tags "Async"
}
