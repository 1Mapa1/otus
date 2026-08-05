# C4-модель

Контейнерная диаграмма показывает пользователей, микросервисы, основные синхронные вызовы, доменные события и внешние интеграции решения «I'll Have the BLT».

## Запуск Structurizr Local

Из PowerShell:

```powershell
docker run --rm -it `
  --name structurizr-hw1 `
  -p 8080:8080 `
  -v "${PWD}:/usr/local/structurizr" `
  structurizr/structurizr local
```

Команду необходимо выполнять из папки `ДЗ 1/Архитектура/C4`. После запуска открыть <http://localhost:8080>.
