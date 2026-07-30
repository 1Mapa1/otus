# Main

Полный E2E-сценарий проекта: восемь микросервисов, Kafka, Redis, Saga,
идемпотентность, компенсация и уведомления.

Запуск из PowerShell:

```powershell
newman run .\electronics-store.postman_collection.json `
  -e .\electronics-store.postman_environment.json `
  --delay-request 300 `
  --reporters cli
```
