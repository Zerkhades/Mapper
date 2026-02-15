# Mapper

Интерактивная карта для предприятия с интеграцией систем видеонаблюдения

## 🎯 Описание

Mapper - это современное корпоративное решение для управления интерактивными картами предприятия с интегрированной системой видеонаблюдения. Проект построен на **ASP.NET Core 8 WebAPI** с использованием архитектуры **CQRS + Mediator** и **Clean Architecture**, что обеспечивает высокую масштабируемость, тестируемость и поддерживаемость.

## ✨ Основной функционал

### 🗺️ Управление картами и метками
- **Интерактивные карты предприятия** с поддержкой масштабирования и навигации
- **Метки на картах**: Workplace (рабочие места), Transition (переходы), Camera (камеры)
- **Управление сотрудниками** с привязкой к рабочим местам
- **Real-time обновления** через SignalR Hub
- **API версионирование** с поддержкой нескольких версий API

### 📹 Система видеонаблюдения
- **Архив видеозаписей** с метаданными и таймлайном
- **Детектирование движения** с автоматическими алертами (Low/Medium/High severity)
- **Автоматические снимки** с камер по расписанию
- **Масштабирование изображений** (zoom 1.0-10.0x)
- **История статусов камер** с логированием всех изменений (15+ типов событий)
- **Фоновые задачи** для автоматической обработки видеопотоков

[👉 Подробная документация по системе видеонаблюдения](./CAMERA_FEATURES.md)

### 🔐 Безопасность и мониторинг
- **OAuth 2.0 + OpenID Connect** аутентификация через IdentityServer
- **JWT Bearer токены** для API
- **Health Checks** endpoints (liveness и readiness рrоbes)
- **Distributed Tracing** через OpenTelemetry
- **Структурированное логирование** с Serilog

## 🛠️ Технологический стек

### Backend
- **.NET 8** с C# 12
- **ASP.NET Core WebAPI** с API версионированием (Asp.Versioning)
- **Entity Framework Core** - ORM для работы с БД
- **MediatR** - реализация паттерна Mediator для CQRS
- **PostgreSQL** - основная база данных
- **FluentValidation** - валидация моделей
- **AutoMapper** - маппинг между объектами

### Инфраструктура
- **Redis** - распределённое кеширование
- **MinIO (S3-совместимое)** - хранилище изображений и видео
- **SignalR** - real-time двусторонняя коммуникация
- **Hangfire** - планирование и выполнение фоновых задач
- **FFmpeg** - обработка видео и изображений с камер

### Observability (Наблюдаемость)
- **OpenTelemetry** - трейсы и метрики
  - ASP.NET Core инструментация
  - HTTP Client инструментация
  - Entity Framework Core инструментация
- **Prometheus** - сбор и хранение метрик
- **Grafana** - визуализация метрик и дашборды
- **Jaeger** - распределённая трассировка (distributed tracing)
- **Elasticsearch + Kibana** - поиск и аналитика логов
- **Seq** - структурированное логирование
- **Serilog** - библиотека логирования

### Аутентификация и авторизация
- **В данный момент авторизация полностью отключена**
- **IdentityServer** - OAuth 2.0 / OpenID Connect провайдер
- **JWT Bearer** - токены для API аутентификации

### DevOps & Контейнеризация
- **Docker** - контейнеризация приложений
- **Docker Compose** - оркестрация мульти-контейнерных приложений

### Front-end
- **React** - https://github.com/Zerkhades/Mapper.Frontend
- **Avalonia** - кроссплатформенное desktop приложение

## 📚 Документация

- [**CAMERA_FEATURES.md**](./CAMERA_FEATURES.md) - Полное описание функций видеонаблюдения
- [**IMPLEMENTATION_SUMMARY.md**](./IMPLEMENTATION_SUMMARY.md) - Резюме технической реализации
- [**USAGE_EXAMPLES.md**](./USAGE_EXAMPLES.md) - Примеры использования API
- [**INSTALLATION.md**](./INSTALLATION.md) - Руководство по установке и конфигурации

## 🚀 Быстрый старт

### Предварительные требования

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (для контейнерной установки)
- [PostgreSQL 14+](https://www.postgresql.org/download/) (для локальной установки)
- [Redis](https://redis.io/download) (для локальной установки)
- [FFmpeg](https://ffmpeg.org/download.html) (для работы с камерами)

### Установка с помощью Docker Compose (рекомендуется)

**Для разворачивания через IDE:**
1. Откройте решение в Visual Studio 2022+
2. Выберите **docker-compose** как startup project
3. Нажмите F5 или Start Debugging

**Для разворачивания через командную строку:**

1. Клонируйте репозиторий:
   ```bash
   git clone https://github.com/Zerkhades/Mapper.git
   cd Mapper
   ```

2. Запустите все сервисы:
   ```bash
   docker-compose up -d
   ```

3. Дождитесь запуска всех контейнеров (обычно 1-2 минуты)

4. Доступ к сервисам:
   - **WebAPI Swagger**: http://localhost:5001
   - **IdentityServer**: http://localhost:5002
   - **Reverse Proxy**: http://localhost:8080
   - **Hangfire Dashboard**: http://localhost:5001/hangfire
   - **MinIO Console**: http://localhost:9001 (minioadmin/minioadmin)
   - **Seq Logs**: http://localhost:5341 (admin/admin123!)
   - **Kibana**: http://localhost:5601
   - **Jaeger UI**: http://localhost:16686
   - **Prometheus**: http://localhost:9090
   - **Grafana**: http://localhost:3000 (admin/admin)

### Локальная установка

1. Клонируйте репозиторий:
   ```bash
   git clone https://github.com/Zerkhades/Mapper.git
   cd Mapper
   ```

2. Восстановите зависимости:
   ```bash
   dotnet restore
   ```

3. Обновите `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=MapperDB;Username=postgres;Password=yourpassword"
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
       "Authority": "http://localhost:5002",
       "Audience": "api"
     },
     "Otel": {
       "Endpoint": "http://localhost:4317"
     },
     "Camera": {
       "FfmpegPath": "C:\\ffmpeg\\bin\\ffmpeg.exe",
       "Jobs": {
         "MotionCron": "*/5 * * * *",
         "VideoCron": "*/30 * * * *",
         "StatusCron": "*/1 * * * *",
         "SnapshotCron": "*/1 * * * *"
       }
     }
   }
   ```

4. Примените миграции базы данных:
   ```bash
   cd src/Mapper.Core/Mapper.WebApi
   dotnet ef database update --project ../Mapper.Persistence
   ```

5. Запустите приложение:
   ```bash
   dotnet run --project src/Mapper.Core/Mapper.WebApi
   ```

## 🏗️ Архитектура

Проект следует принципам **Clean Architecture** и **Domain-Driven Design (DDD)** с явным разделением на слои:

```
┌─────────────────────────────────────────────┐
│          Presentation Layer                 │
│     (WebAPI, Controllers, SignalR)          │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│         Application Layer                   │
│  (CQRS Handlers, Validation, DTOs)          │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│           Domain Layer                      │
│    (Entities, Value Objects, Rules)         │
└─────────────────────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│        Infrastructure Layer                 │
│  (EF Core, Redis, S3, Cameras, Jobs)        │
└─────────────────────────────────────────────┘
```

### Слои приложения

#### Core (Ядро)
- **Mapper.Domain**: Доменные модели, entities, value objects, бизнес-правила
- **Mapper.Application**: Прикладная логика, CQRS команды/запросы, валидация, интерфейсы
  - Commands & Queries (CreateGeoMap, GetEmployeeById и т.д.)
  - Validators (FluentValidation)
  - DTOs и Mapping Profiles (AutoMapper)

#### Infrastructure (Инфраструктура)
- **Mapper.Persistence**: Data Access Layer с Entity Framework Core
  - DbContext, миграции, конфигурации сущностей
- **Mapper.Infrastructure**: Реализация инфраструктурных сервисов
  - `BackgroundJobs/` - Hangfire задачи для камер
  - `Caching/` - Redis кеширование
  - `Cameras/` - адаптеры для работы с камерами (SimpleCameraAdapter, FakeCameraAdapter)
  - `Realtime/` - SignalR хабы для real-time обновлений
  - `Storage/` - S3-совместимое хранилище (MinIO)

#### Presentation (Представление)
- **Mapper.WebApi**: REST API контроллеры, middleware, конфигурация
  - Controllers с API версионированием
  - Swagger/OpenAPI документация
  - JWT аутентификация
  - Health checks endpoints

#### Authentication
- **Mapper.IdentityServer**: OAuth 2.0 / OpenID Connect сервер

#### Tests
- **Mapper.Tests**: Юнит и интеграционные тесты
  - Domain тесты
  - Command/Query handler тесты
  - Validator тесты
  - Integration тесты

## 📁 Структура проекта

```plaintext
Mapper/
├── src/
│   ├── Mapper.Core/
│   │   ├── Mapper.Application/              # Application Layer
│   │   │   ├── CommandsAndQueries/
│   │   │   │   ├── Employee/                # Employee CQRS
│   │   │   │   ├── GeoMap/                  # Map CQRS
│   │   │   │   ├── GeoMark/                 # Mark CQRS
│   │   │   │   └── CameraArchive/           # Camera archive CQRS
│   │   │   ├── Common/
│   │   │   │   ├── Mappings/                # AutoMapper profiles
│   │   │   │   ├── Behaviors/               # MediatR behaviors
│   │   │   │   └── Exceptions/              # Custom exceptions
│   │   │   ├── Interfaces/                  # Application interfaces
│   │   │   └── DependencyInjection.cs
│   │   ├── Mapper.Domain/                   # Domain Layer
│   │   │   ├── Employee.cs
│   │   │   ├── GeoMap.cs
│   │   │   ├── GeoMark.cs (abstract)
│   │   │   ├── WorkplaceMark.cs
│   │   │   ├── TransitionMark.cs
│   │   │   ├── CameraMark.cs
│   │   │   ├── CameraVideoArchive.cs
│   │   │   ├── CameraMotionAlert.cs
│   │   │   └── CameraStatusHistory.cs
│   │   ├── Mapper.Persistence/              # Data Access Layer
│   │   │   ├── MapperDbContext.cs
│   │   │   ├── Migrations/
│   │   │   └── Configurations/              # EF Core configurations
│   │   ├── Mapper.Infrastructure/           # Infrastructure Layer
│   │   │   ├── BackgroundJobs/
│   │   │   │   ├── DetectCameraMotionJob.cs
│   │   │   │   ├── RecordCameraVideoJob.cs
│   │   │   │   ├── PollCameraStatusAndLogHistoryJob.cs
│   │   │   │   └── FetchCameraSnapshotsJob.cs
│   │   │   ├── Caching/
│   │   │   │   └── RedisCacheService.cs
│   │   │   ├── Cameras/
│   │   │   │   ├── SimpleCameraAdapter.cs   # Real camera integration
│   │   │   │   └── FakeCameraAdapter.cs     # Testing adapter
│   │   │   ├── Realtime/
│   │   │   │   ├── MapHub.cs                # SignalR Hub
│   │   │   │   └── MapRealtimeNotifier.cs
│   │   │   └── Storage/
│   │   │       └── S3/
│   │   │           ├── S3MapImageStorage.cs
│   │   │           └── S3ObjectStorage.cs
│   │   └── Mapper.WebApi/                   # Presentation Layer
│   │       ├── Controllers/
│   │       │   ├── EmployeesController.cs
│   │       │   ├── GeoMapController.cs
│   │       │   ├── GeoMarkController.cs
│   │       │   └── CameraTelemetryController.cs
│   │       ├── Middleware/
│   │       ├── Services/
│   │       │   ├── CurrentUserService.cs
│   │       │   └── HangfireDashboardAuthorizationFilter.cs
│   │       ├── Models/
│   │       ├── Program.cs
│   │       ├── Startup.cs
│   │       ├── appsettings.json
│   │       └── Dockerfile
│   ├── Mapper.IdentityServer/               # Authentication
│   ├── Mapper.ReverseProxy/                 # Reverse Proxy
│   └── Mapper.Tests/                        # Tests
│       ├── Domain/
│       ├── Integration/
│       ├── Validators/
│       ├── Mappings/
│       └── CameraArchive/
├── ops/
│   └── prometheus.yml                       # Prometheus config
├── docker-compose.yml                       # Production compose
├── docker-compose.override.yml              # Development overrides
└── Mapper.sln
```

## 🌐 API Endpoints

API документация доступна через **Swagger UI**:
- Локально: http://localhost:5001
- Через Reverse Proxy: http://localhost:8080

### API версионирование

API поддерживает версионирование через URL:
- `GET /api/v1/GeoMap` - версия 1.0
- `GET /api/v2/GeoMap` - версия 2.0 (если доступна)

### Основные группы endpoints

#### 🗺️ GeoMap (Карты)
- `GET /api/v1/GeoMap` - получить список карт
- `GET /api/v1/GeoMap/{id}` - получить карту по ID
- `POST /api/v1/GeoMap` - создать новую карту
- `PUT /api/v1/GeoMap` - обновить карту
- `DELETE /api/v1/GeoMap/{id}` - удалить карту

#### 📍 GeoMark (Метки)
- `GET /api/v1/GeoMark` - получить все метки
- `GET /api/v1/GeoMark/{id}` - получить метку по ID
- `POST /api/v1/GeoMark` - создать метку (Workplace/Transition/Camera)
- `PUT /api/v1/GeoMark` - обновить метку
- `DELETE /api/v1/GeoMark/{id}` - удалить метку

#### 👥 Employees (Сотрудники)
- `GET /api/v1/Employees` - получить список сотрудников
- `GET /api/v1/Employees/{id}` - получить сотрудника по ID
- `POST /api/v1/Employees` - создать сотрудника
- `PUT /api/v1/Employees` - обновить сотрудника
- `POST /api/v1/Employees/{id}/archive` - архивировать сотрудника

#### 📹 CameraTelemetry (Телеметрия камер)
- `GET /api/v1/CameraTelemetry/status` - получить статусы всех камер
- `GET /api/v1/CameraTelemetry/{id}/history` - история статусов камеры
- `GET /api/v1/CameraTelemetry/{id}/motion-alerts` - алерты движения
- `GET /api/v1/CameraTelemetry/{id}/video-archive` - архив видео

### Health Checks

- `GET /health/live` - Liveness probe (приложение запущено)
- `GET /health/ready` - Readiness probe (приложение готово принимать запросы)

### Мониторинг

- `GET /metrics` - Prometheus метрики
- `GET /hangfire` - Hangfire Dashboard (требуется аутентификация)

## 🔄 Real-time обновления (SignalR)

Подключение к SignalR Hub для получения обновлений в реальном времени:

**Endpoint**: `http://localhost:5001/hubs/map`

**Поддерживаемые события**:
- `MapUpdated` - карта обновлена
- `MarkCreated` - создана новая метка
- `MarkUpdated` - метка обновлена
- `MarkDeleted` - метка удалена
- `EmployeeUpdated` - сотрудник обновлен
- `CameraStatusChanged` - изменился статус камеры

**Пример подключения (JavaScript)**:
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5001/hubs/map?access_token=YOUR_JWT_TOKEN")
    .build();

connection.on("MapUpdated", (mapId) => {
    console.log(`Map ${mapId} was updated`);
});

await connection.start();
```

## 🔐 Аутентификация

### OAuth 2.0 / OpenID Connect Flow

1. **Получение токена** через IdentityServer:
   ```
   POST http://localhost:5002/connect/token
   Content-Type: application/x-www-form-urlencoded
   
   grant_type=authorization_code&
   client_id=mapper.swagger&
   code=AUTHORIZATION_CODE&
   redirect_uri=http://localhost:5001/swagger/oauth2-redirect.html&
   code_verifier=CODE_VERIFIER
   ```

2. **Использование токена** в API запросах:
   ```
   GET http://localhost:5001/api/v1/GeoMap
   Authorization: Bearer YOUR_JWT_TOKEN
   ```

### Swagger OAuth

Swagger UI настроен для OAuth2 Authorization Code Flow с PKCE:
- Client ID: `mapper.swagger`
- Scopes: `api`, `openid`, `profile`

Нажмите **Authorize** в Swagger UI и следуйте инструкциям.

## 📊 Мониторинг и наблюдаемость

### Логирование

**Serilog** настроен для записи логов в несколько sink'ов:
- Console (разработка)
- Seq (структурированные логи)
- Elasticsearch (поиск и аналитика)

**Просмотр логов**:
- Seq: http://localhost:5341 (admin/admin123!)
- Kibana: http://localhost:5601

### Распределённая трассировка

**OpenTelemetry** экспортирует трейсы в Jaeger:
- Jaeger UI: http://localhost:16686

Трейсы включают:
- HTTP requests (ASP.NET Core)
- HTTP client calls
- Database queries (EF Core)

### Метрики

**Prometheus** собирает метрики с endpoint `/metrics`:
- Prometheus UI: http://localhost:9090
- Grafana dashboards: http://localhost:3000

Метрики включают:
- ASP.NET Core метрики (requests, latency)
- HTTP Client метрики
- .NET Runtime метрики (GC, threads, exceptions)

## 🎯 Фоновые задачи (Hangfire)

Настроены следующие recurring jobs для автоматизации работы с камерами:

| Job | Schedule | Описание |
|-----|----------|----------|
| `detect-camera-motion` | `*/5 * * * *` | Каждые 5 минут детектирует движение на камерах |
| `record-camera-video` | `*/30 * * * *` | Каждые 30 минут записывает видео с камер |
| `poll-camera-status` | `*/1 * * * *` | Каждую минуту проверяет статус камер |
| `fetch-camera-snapshots` | `*/1 * * * *` | Каждую минуту делает снимки с камер |

Schedule можно настроить в `appsettings.json`:
```json
"Camera": {
  "Jobs": {
    "MotionCron": "*/5 * * * *",
    "VideoCron": "*/30 * * * *",
    "StatusCron": "*/1 * * * *",
    "SnapshotCron": "*/1 * * * *"
  }
}
```

**Hangfire Dashboard**: http://localhost:5001/hangfire

## 🧪 Тестирование

Проект включает comprehensive test suite:

```bash
# Запустить все тесты
dotnet test

# Запустить тесты с покрытием
dotnet test /p:CollectCoverage=true
```

**Тестовые проекты**:
- Domain тесты (Employee, GeoMap, CameraMark и т.д.)
- Application тесты (Command/Query handlers)
- Validator тесты (FluentValidation)
- Integration тесты
- Infrastructure тесты (FakeCameraAdapter)

## 🚢 Развёртывание

### Docker Production Build

```bash
# Build production images
docker-compose -f docker-compose.yml build

# Run in production mode
docker-compose -f docker-compose.yml up -d
```

### Environment Variables

Основные переменные окружения для production:

```bash
# Database
ConnectionStrings__DefaultConnection="Host=postgres;Port=5432;Database=MapperDB;Username=postgres;Password=STRONG_PASSWORD"

# Redis
Redis__ConnectionString="redis:6379"

# S3 (MinIO)
S3__ServiceUrl="http://minio:9000"
S3__AccessKey="MINIO_ACCESS_KEY"
S3__SecretKey="MINIO_SECRET_KEY"
S3__Bucket="mapper"

# JWT
Jwt__Authority="http://identityserver:5002"
Jwt__Audience="api"

# OpenTelemetry
Otel__Endpoint="http://jaeger:4317"

# FFmpeg
Camera__FfmpegPath="/usr/bin/ffmpeg"
```

## 🔧 Troubleshooting

### Частые проблемы

**1. Ошибка подключения к базе данных**
```
Solution: Проверьте ConnectionString и убедитесь, что PostgreSQL запущен
```

**2. Hangfire jobs не запускаются**
```
Solution: Проверьте логи в Seq/Kibana. Возможно, схема Hangfire не создана (PrepareSchemaIfNecessary=true)
```

**3. Camera jobs завершаются с ошибкой**
```
Solution: Убедитесь, что FFmpeg установлен и Camera:FfmpegPath настроен правильно
```

**4. SignalR не работает**
```
Solution: Проверьте CORS настройки и access_token в query string
```

## 👨‍💻 Разработка

### Требования

- Visual Studio 2022+ или Rider 2023+
- .NET 8 SDK
- Docker Desktop (опционально)

### Рекомендуемые расширения Visual Studio

- ReSharper / Rider
- EF Core Power Tools
- Docker Tools

### Соглашения по коду

- Следуйте C# Coding Conventions
- Используйте async/await для I/O операций
- Пишите юнит-тесты для новой бизнес-логики
- Документируйте публичные API с XML комментариями

## 🤝 Вклад в проект

Если вы хотите внести вклад в проект:

1. Fork репозитория
2. Создайте feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit изменения (`git commit -m 'Add some AmazingFeature'`)
4. Push в branch (`git push origin feature/AmazingFeature`)
5. Откройте Pull Request

## 📝 Лицензия

Этот проект распространяется под лицензией MIT. См. файл `LICENSE` для подробностей.

## 📧 Контакты

**Maintainer**: Zerkhades

**GitHub**: https://github.com/Zerkhades/Mapper

---

⭐ Если проект был полезен, поставьте звезду на GitHub!
