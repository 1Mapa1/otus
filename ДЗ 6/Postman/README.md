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

В GUI Postman: импорт коллекции и окружения, выбрать окружение, выполнить коллекцию (Run collection).

## Результаты прогона

Скриншоты успешного выполнения:

- [Result 1.jpg](./Result%201.jpg)
- [Result 2.jpg](./Result%202.jpg)
- [Result 3.jpg](./Result%203.jpg)
