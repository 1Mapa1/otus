### Название микросервиса
AuthService
### Назначение и зона ответственности
AuthService отвечает за регистрацию аккаунта, аутентификацию пользователя, управление сессиями и ролями доступа.
### Бизнес-контекст
Access Management
### Собственные данные
Сущности
- `Account`
- `AuthSession`
- `Role`
Объекты-значения
- `Login`
- `PasswordHash`
- `SessionToken
Корень агрегата:
- Account
### Высокоуровневая модель данных
#### Account
- `id`
- `login`
- `passwordHash`
- `status`
- `role`
- `createdAt`
- `updatedAt`
- `lastLoginAt`

#### AuthSession
- `id`
- `accountId`
- `refreshTokenHash`
- `expiresAt`
- `createdAt`
- `revokedAt`
#### Role
- `id`
- `name`
- `permissions`

### Состояния
- `PendingVerification`
- `Active`
- `Blocked`
- `Deleted`
### Бизнес-правила
- Логин в системе уникален
- Пароль хранится только в виде хеша
- Один пользователь может иметь несколько активных сессий
- Access token короткоживущий, refresh token отзываемый
- Заблокированный аккаунт не может проходить аутентификацию
### API
#### Пользовательские операции
- **POST** `/auth/register`
- **POST** `/auth/login`
- **POST** `/auth/refresh`
- **POST** `/auth/logout`
- **GET** `/auth/me`
#### Административные операции
- **PATCH** `/auth/accounts/{accountId}/block`
- **PATCH** `/auth/accounts/{accountId}/unblock`
- **PATCH** `/auth/accounts/{accountId}/role`
### Публикуемые события
- `AccountRegistered`
- `AccountVerified`
- `AccountBlocked`
- `SessionStarted`
- `SessionRevoked`
### Потребляемые события
- отсутствуют.
### Зависимости
- отсутствуют.

### Внешние интеграции
Key Store — подпись и валидация токенов
