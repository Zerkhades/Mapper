# Mapper

Mapper - backend для интерактивной карты предприятия с метками рабочих мест, переходов и камер. API построен на ASP.NET Core, использует CQRS через MediatR, PostgreSQL, Redis, S3-совместимое хранилище, SignalR, Hangfire и Keycloak.

Репозиторий сейчас ориентирован на .NET 10 и локальный запуск через Docker Compose. Фронтенд живет отдельно: https://github.com/Zerkhades/Mapper.Frontend.

## Что есть в проекте

- Карты предприятия и метки: рабочие места, переходы между картами, камеры.
- Управление сотрудниками и привязка сотрудников к рабочим местам.
- Камерная телеметрия: статусы, snapshots, zoom snapshots, история статусов.
- Видеоархив камер, motion alerts и задачи очистки архива по retention policy.
- Операторская аналитика, граф связности карт, smart notifications и аудит команд.
- Real-time обновления через SignalR hub `/hubs/map`.
- Swagger/OpenAPI с OAuth2 Authorization Code + PKCE через Keycloak.
- Health checks, Prometheus metrics, OpenTelemetry traces, Serilog + Seq.

## Стек

- .NET 10, ASP.NET Core Web API
- Entity Framework Core 10, PostgreSQL 16
- MediatR, FluentValidation, AutoMapper
- Redis, MinIO/S3, Hangfire
- SignalR
- Keycloak 26
- OpenTelemetry, Prometheus, Grafana, Jaeger, Seq
- xUnit, Moq, coverlet.collector
- YARP reverse proxy

## Структура

```text
src/
  Mapper.Core/
    Mapper.Domain/          доменные сущности и бизнес-правила
    Mapper.Application/     CQRS handlers, DTO, validators, behaviours, interfaces
    Mapper.Persistence/     EF Core DbContext, configurations, migrations
    Mapper.Infrastructure/  Redis, S3, cameras, SignalR, Hangfire jobs
    Mapper.WebApi/          API host, controllers, middleware, health checks
  Mapper.ReverseProxy/      YARP reverse proxy для API и Keycloak
  Mapper.Tests/             xUnit tests
docs/
  auth-keycloak.md          локальная схема Keycloak
  frontend-new-endpoints.md актуальные endpoint-контракты для фронтенда
ops/
  keycloak/mapper-realm.json
  prometheus.yml
```

В корне также есть старые папки `Mapper.Application`, `Mapper.Domain`, `Mapper.Persistence`, `Mapper.WebApi`, `Mapper.Tests`. Актуальные проекты подключены в `Mapper.sln` из `src/`.

## Быстрый старт через Docker Compose

Требования:

- Docker Desktop
- .NET 10 SDK, если нужно собирать и тестировать локально без контейнера

Запуск:

```bash
docker compose up --build -d
```

Основные URL:

| Сервис | URL |
| --- | --- |
| Reverse Proxy / Swagger | `http://localhost:8080` |
| WebApi direct Swagger | `http://localhost:5001` |
| Keycloak admin через proxy | `http://localhost:8080/auth/admin` |
| Keycloak direct debug | `http://localhost:5002/auth/admin` |
| Hangfire Dashboard | `http://localhost:5001/hangfire` |
| MinIO Console | `http://localhost:9001` |
| Seq | `http://localhost:5341` |
| Jaeger | `http://localhost:16686` |
| Prometheus | `http://localhost:9090` |
| Grafana | `http://localhost:3000` |
| go2rtc | `http://localhost:1984` |

Локальные учетные данные:

| Сервис | Логин | Пароль |
| --- | --- | --- |
| Keycloak admin | `admin` | `admin` |
| Keycloak test user | `mapper.admin` | `mapper-admin` |
| MinIO | `minioadmin` | `minioadmin` |
| Seq | `admin` | `admin123!` |
| Grafana | `admin` | `admin` |

При запуске compose для `mapper.webapi` включены startup tasks:

- `StartupTasks__ApplyMigrations=true` - применить EF Core migrations.
- `StartupTasks__EnsureS3Bucket=true` - создать S3 bucket `mapper`, если его нет.
- `StartupTasks__SeedDatabase=false` - не сидировать базу по умолчанию.

## Локальный запуск API без контейнера

Инфраструктуру удобнее поднять через compose, а сам API запустить из IDE или `dotnet run`.

```bash
docker compose up -d db keycloakdb keycloak redis minio seq jaeger prometheus grafana
dotnet restore Mapper.sln
dotnet run --project src/Mapper.Core/Mapper.WebApi/Mapper.WebApi.csproj
```

Для запуска API вне Docker проверьте `src/Mapper.Core/Mapper.WebApi/appsettings.Development.json` или user secrets. Минимально нужны:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=MapperDB;Username=postgres;Password=pass"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "S3": {
    "ServiceUrl": "http://localhost:9000",
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin",
    "Bucket": "mapper"
  },
  "Jwt": {
    "Authority": "http://localhost:5002/auth/realms/mapper",
    "Audience": "api"
  },
  "SwaggerOAuth": {
    "Authority": "http://localhost:5002/auth/realms/mapper",
    "ClientId": "mapper.swagger"
  }
}
```

Если миграции не применяются автоматически, используйте:

```bash
dotnet ef database update -p src/Mapper.Core/Mapper.Persistence -s src/Mapper.Core/Mapper.WebApi
```

## Основные команды

```bash
dotnet restore Mapper.sln
dotnet build Mapper.sln --configuration Release
dotnet test src/Mapper.Tests/Mapper.Tests.csproj
dotnet test src/Mapper.Tests/Mapper.Tests.csproj --collect:"XPlat Code Coverage" --results-directory ./coverage
dotnet format Mapper.sln --verify-no-changes --no-restore
```

## API

Базовый префикс версионированных endpoints: `/api/v1`.

Основные группы:

| Группа | Префикс |
| --- | --- |
| Карты | `/api/v1/geomaps` |
| Метки карты | `/api/v1/geomaps/{geoMapId}/marks` |
| Сотрудники | `/api/v1/employees` |
| Камерная телеметрия | `/api/v1/geomaps/{geoMapId}/marks/camera/{markId}` |
| Архив камеры | `/api/v1/geomaps/{geoMapId}/marks/camera/{cameraMarkId}/archive` |
| Dashboard | `/api/v1/dashboard` |
| Smart notifications | `/api/v1/notifications/smart` |
| Audit events | `/api/v1/audit/events` |
| Retention | `/api/v1/retention` |
| Files | `/api/files/{key}` |

Служебные endpoints:

| Endpoint | Назначение |
| --- | --- |
| `/health/live` | liveness probe |
| `/health/ready` | readiness probe: PostgreSQL, Redis, S3 |
| `/metrics` | Prometheus scraping endpoint |
| `/hangfire` | Hangfire Dashboard |
| `/hubs/map` | SignalR hub |

Подробные контракты новых frontend endpoints: [docs/frontend-new-endpoints.md](docs/frontend-new-endpoints.md).

## Аутентификация

API настроен на JWT Bearer tokens от Keycloak. Swagger использует public client `mapper.swagger` и Authorization Code + PKCE со scopes:

```text
openid profile api
```

В локальном Docker Compose WebApi валидирует issuer через публичный адрес:

```text
Jwt__Authority=http://localhost:8080/auth/realms/mapper
Jwt__MetadataAddress=http://keycloak:8080/auth/realms/mapper/.well-known/openid-configuration
Jwt__Audience=api
SwaggerOAuth__Authority=http://localhost:8080/auth/realms/mapper
SwaggerOAuth__ClientId=mapper.swagger
```

Больше деталей и пример service token: [docs/auth-keycloak.md](docs/auth-keycloak.md).

Важно: JWT authentication подключена, но не все контроллеры закрыты `[Authorize]`. Если endpoint должен стать приватным, добавляйте авторизацию явно или вводите fallback policy осознанно.

## SignalR

Hub доступен по адресу:

```text
/hubs/map
```

Клиент может подписаться на группу конкретной карты:

```javascript
await connection.invoke("JoinMap", geoMapId);
```

Группы имеют формат `map:{geoMapId}`. Уведомления отправляются через `IMapRealtimeNotifier` из infrastructure layer.

## Фоновые задачи

Hangfire планирует recurring jobs при старте WebApi:

| Job | Настройка cron | По умолчанию |
| --- | --- | --- |
| `detect-camera-motion` | `Camera:Jobs:MotionCron` | `*/5 * * * *` |
| `record-camera-video` | `Camera:Jobs:VideoCron` | `*/30 * * * *` |
| `poll-camera-status` | `Camera:Jobs:StatusCron` | `*/1 * * * *` |
| `fetch-camera-snapshots` | `Camera:Jobs:SnapshotCron` | `*/1 * * * *` |
| `cleanup-archive-retention` | `Retention:ArchiveCleanup:Cron` | `0 3 * * *` |

Retention cleanup по умолчанию работает в безопасном режиме `DryRun=true`. Реальное удаление требует `DryRun=false` и `Confirm=true`.

## Конфигурация

Часто используемые переменные окружения:

```text
ConnectionStrings__DefaultConnection=Host=db;Database=MapperDB;Username=postgres;Password=pass
Redis__ConnectionString=redis:6379
S3__ServiceUrl=http://minio:9000
S3__AccessKey=minioadmin
S3__SecretKey=minioadmin
S3__Bucket=mapper
Seq__ServerUrl=http://seq:80
Otel__Endpoint=http://jaeger:4317
Jwt__Authority=http://localhost:8080/auth/realms/mapper
Jwt__MetadataAddress=http://keycloak:8080/auth/realms/mapper/.well-known/openid-configuration
Jwt__Audience=api
SwaggerOAuth__Authority=http://localhost:8080/auth/realms/mapper
SwaggerOAuth__ClientId=mapper.swagger
Cors__AllowedOrigins__0=http://localhost:3000
```

Секреты, реальные пароли и machine-specific пути не коммитим.

## Разработка

- Целевой framework актуальных проектов: `net10.0`.
- Nullable включен.
- Версии NuGet централизованы в `Directory.Packages.props`.
- Clean Architecture остается основной границей: Domain не зависит от Application, Persistence и Infrastructure подключаются через интерфейсы Application.
- Новые features лучше добавлять вертикально: command/query, validator, DTO/mapping, controller, tests.
- Для новых handler/validator/persistence-sensitive изменений добавляйте тесты рядом с существующими группами в `src/Mapper.Tests`.

## Документация

- [docs/auth-keycloak.md](docs/auth-keycloak.md) - Keycloak, клиенты, test user, token examples.
- [docs/frontend-new-endpoints.md](docs/frontend-new-endpoints.md) - актуальные endpoint-контракты для frontend.
- [src/Mapper.Tests/README.md](src/Mapper.Tests/README.md) - заметки по тестовому проекту, если нужны детали по тестам.

## Troubleshooting

**Swagger не может авторизоваться через Keycloak.**
Проверьте, что открываете Swagger через тот же origin, который есть в realm import и настройках OAuth. Для compose основной путь: `http://localhost:8080`.

**API в контейнере не видит Keycloak metadata.**
В compose должен быть задан `Jwt__MetadataAddress=http://keycloak:8080/auth/realms/mapper/.well-known/openid-configuration`, а `Jwt__Authority` должен оставаться публичным issuer `http://localhost:8080/auth/realms/mapper`.

**Readiness probe возвращает unhealthy.**
Проверьте PostgreSQL, Redis и MinIO: `/health/ready` включает все три проверки.

**Hangfire jobs не появились.**
Проверьте подключение к PostgreSQL и dashboard `/hangfire`. Планирование выполняется при старте WebApi.

**S3 bucket отсутствует.**
В compose включен `StartupTasks__EnsureS3Bucket=true`. При локальном запуске без контейнера включите `StartupTasks:EnsureS3Bucket` или создайте bucket `mapper` в MinIO вручную.
