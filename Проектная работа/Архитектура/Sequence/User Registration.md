### Цель

Показать регистрацию пользователя, синхронное создание клиентского профиля в `CustomerMs` и асинхронное создание счёта в `BillingMs`.
### Триггер

Пользователь отправляет запрос:

```http
POST /api/auth/register
```
### Участники

| Участник   | Роль                      | Основные действия                                                |
| ---------- | ------------------------- | ---------------------------------------------------------------- |
| User       | Пользователь              | Вводит данные для регистрации                                    |
| Frontend   | Клиентское приложение     | Отправляет форму регистрации и отображает результат              |
| ApiGateway | Единая точка входа        | Маршрутизирует запрос в `AuthMs`                                 |
| AuthMs     | Владелец учётной записи   | Создаёт пользователя, активирует его и публикует `UserActivated` |
| CustomerMs | Владелец профиля клиента  | Синхронно создаёт профиль пользователя                           |
| Kafka      | Брокер сообщений          | Передаёт событие `UserActivated`                                 |
| BillingMs  | Владелец платёжного счёта | Асинхронно создаёт пустой счёт пользователя                      |
### Предусловия

- Логин пользователя ещё не занят.
- Данные регистрации прошли валидацию.
- `CustomerMs` доступен для синхронного создания профиля.

### Диаграмма

```mermaid
sequenceDiagram
    actor U as User
    participant F as Frontend
    participant G as ApiGateway
    participant AM as AuthMs
    participant CM as CustomerMs
    participant K as Kafka
    participant BM as BillingMs

    autonumber

    U->>F: Заполняет форму регистрации
    F->>G: POST /api/auth/register
    G->>AM: register

    AM->>AM: Создать User(Pending)<br/>Сохранить password hash

    AM->>CM: POST /api/internal/customers
    CM-->>AM: CustomerCreated

    AM->>AM: User.Status = Active

    AM-->>G: 201 Created
    G-->>F: 201 Created
    F-->>U: Регистрация завершена

    AM->>K: publish UserActivated (через Outbox)

    K->>BM: UserActivated
    
    note over BM: Создать пустой Account,<br/>если он отсутствует
```

### Результат

- Учётная запись пользователя создана и активна.
- В `CustomerMs` создан профиль клиента.
- Пользователь может выполнить login через `POST /api/auth/login`.
- `BillingMs` получает `UserActivated` асинхронно и создаёт пустой счёт.
- Если событие ещё не обработано, счёт будет создан лениво при первом обращении в `BillingMs`.