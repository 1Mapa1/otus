# Postman

Коллекция для проверки сценариев ДЗ 6: регистрация, вход, доступ к `/api/customers/me` по JWT, негативные кейсы.

## Файлы

| Файл | Назначение |
|------|------------|
| `otus-hw6.postman_collection.json` | Коллекция запросов |
| `otus-hw6.postman_environment.json` | Окружение (`baseUrl` и переменные для скриптов) |

Базовый URL по умолчанию: `http://arch.homework` (для кластера с Ingress и записью в hosts).

## Запуск в Newman

Из каталога `Postman`:

```bash
newman run otus-hw6.postman_collection.json -e otus-hw6.postman_environment.json
```

## Результаты прогона

Скриншоты успешного выполнения:

![Результат прогона 1](./Result%201.jpg)

![Результат прогона 2](./Result%202.jpg)

![Результат прогона 3](./Result%203.jpg)
