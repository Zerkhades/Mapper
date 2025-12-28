# Mapper

Интерактивная карта для предприятия (API only)

## Описание

Данный проект представляет собой решение, использующее ASP.NET Core WebAPI с использованием ORM для работы с базой данных. В проекте реализована архитектура CQRS + Mediator, что позволяет разделить логику обработки команд и запросов для улучшения поддержки и масштабируемости. В качестве базы данных используется PostgreSQL.

## Технологии

### Backend
- **.NET 8**
- **ASP.NET Core WebAPI**
- **Entity Framework Core (ORM)**
- **CQRS + Mediator (MediatR)**
- **PostgreSQL**
- **FluentValidation**
- **AutoMapper**

### Infrastructure
- **Redis** - кеширование
- **MinIO (S3)** - хранилище изображений карт
- **SignalR** - real-time уведомления
- **Reverse Proxy** - маршрутизация запросов

### Observability & Monitoring
- **Elasticsearch** - поиск и аналитика логов
- **Kibana** - визуализация данных Elasticsearch
- **Seq** - структурированное логирование
- **Jaeger** - distributed tracing
- **Prometheus** - метрики
- **Grafana** - визуализация метрик
- **Serilog** - логирование

### Authentication & Authorization
- **IdentityServer** - аутентификация и авторизация

### Planned Front-end
- **React as web front-end** (in progress)
- **Avalonia as desktop front-end** (in progress)

## Установка

### Локальная установка

1. Клонируйте репозиторий:
   ```bash
   git clone https://github.com/Zerkhades/Mapper
   ```

2. Откройте решение в Visual Studio/Rider или VS Code.

3. Восстановите зависимости:
	```bash
	dotnet restore
	```

4. Обновите строку подключения в `appsettings.json` для подключения к вашей базе данных PostgreSQL:
	```json
	"ConnectionStrings": {
	  "DefaultConnection": "Host=localhost;Port=5432;Database=MapperDB;Username=postgres;Password=yourpassword"
	}
	```

5. Настройте подключения к сервисам инфраструктуры (Redis, MinIO, и т.д.) в `appsettings.json`:
	```json
	"Redis": { "ConnectionString": "localhost:6379" },
	"S3": {
	  "ServiceUrl": "http://localhost:9000",
	  "AccessKey": "minioadmin",
	  "SecretKey": "minioadmin",
	  "Bucket": "mapper"
	},
	"Elastic": { "Url": "http://localhost:9200" },
	"Seq": { "ServerUrl": "http://localhost:5341" },
	"Otel": { "Endpoint": "http://localhost:4317" }
	```

### Установка с помощью Docker Compose

Для запуска проекта с помощью Docker выполните следующие шаги:

**Для разворачивания контейнеров с помощью IDE достаточно выбрать docker-compose как основной проект и запустить его, дальнейшие пункты не требуются**

1. **Клонируйте репозиторий**:
   ```bash
   git clone https://github.com/Zerkhades/Mapper.git
   ```

2. **Создайте и запустите контейнеры**:
	```bash
	docker-compose up -d
	```

3. **Доступ к сервисам**:
   - WebAPI: http://localhost:5001
   - IdentityServer: http://localhost:5002
   - Reverse Proxy: http://localhost:8080
   - MinIO Console: http://localhost:9001
   - Kibana: http://localhost:5601
   - Seq: http://localhost:5341 (admin/admin123!)
   - Jaeger UI: http://localhost:16686
   - Prometheus: http://localhost:9090
   - Grafana: http://localhost:3000

## Архитектура

Проект построен на архитектуре CQRS с использованием MediatR для обработки запросов и команд. Каждый слой имеет свою ответственность:

### Core
- **Mapper.Application**: Содержит бизнес-логику и обработчики команд/запросов (CQRS handlers, валидация, интерфейсы)
- **Mapper.Domain**: Содержит модели данных и бизнес-объекты (entities, value objects)

### Infrastructure
- **Mapper.Persistence**: Содержит реализацию работы с базой данных через Entity Framework Core (DbContext, миграции, репозитории)
- **Mapper.Infrastructure**: Содержит реализации инфраструктурных сервисов (кеширование, хранилище файлов, адаптеры камер, real-time уведомления, фоновые задачи)

### Presentation
- **Mapper.WebApi**: Обработчик API-запросов, предоставляющий интерфейс для взаимодействия с клиентом (контроллеры, middleware, конфигурация)

### Authentication
- **Mapper.IdentityServer**: Сервис аутентификации и авторизации

### Tests
- **Mapper.Tests**: Юнит-тесты для проверки бизнес-логики

## Структура папок

```plaintext
├── src/
│   ├── Mapper.Core/
│   │   ├── Mapper.Application/       # Логика приложения (команды/запросы, валидация)
│   │   │   ├── CommandsAndQueries/   # CQRS команды и запросы
│   │   │   │   ├── Employee/         # Управление сотрудниками
│   │   │   │   ├── GeoMap/           # Управление картами
│   │   │   │   └── GeoMark/          # Управление метками на картах
│   │   │   ├── Features/             # Feature handlers
│   │   │   ├── Interfaces/           # Интерфейсы приложения
│   │   │   └── DependencyInjection.cs
│   │   ├── Mapper.Domain/            # Модели и бизнес-логика
│   │   ├── Mapper.Persistence/       # Доступ к данным (EF Core, миграции)
│   │   ├── Mapper.Infrastructure/    # Инфраструктурные сервисы
│   │   │   ├── BackgroundJobs/       # Фоновые задачи (Quartz.NET)
│   │   │   ├── Caching/              # Redis кеширование
│   │   │   ├── Cameras/              # Адаптеры для камер
│   │   │   ├── Realtime/             # SignalR для real-time уведомлений
│   │   │   └── Storage/              # S3 хранилище (MinIO)
│   │   └── Mapper.WebApi/            # API контроллеры и конфигурация
│   │       ├── Controllers/
│   │       └── Dockerfile
│   ├── Mapper.IdentityServer/        # Сервис аутентификации
│   ├── Mapper.ReverseProxy/          # Reverse proxy
│   └── Mapper.Tests/                 # Тесты
├── ops/
│   └── prometheus.yml                # Конфигурация Prometheus
├── docker-compose.yml                # Оркестрация контейнеров
├── docker-compose.override.yml       # Override конфигурация
├── .dockerignore
└── Mapper.sln                        # Решение
```

## Как использовать API

Все эндпоинты описаны в Swagger UI:
- Локально: `http://localhost:5001/swagger`
- Через Reverse Proxy: `http://localhost:8080/swagger`

### Основные эндпоинты

- **GeoMap** - управление географическими картами
  - GET /api/GeoMap - получение списка карт
  - GET /api/GeoMap/{id} - получение карты по ID
  - POST /api/GeoMap - создание новой карты
  - PUT /api/GeoMap - обновление карты
  - DELETE /api/GeoMap/{id} - удаление карты

- **GeoMark** - управление метками на картах
  - Создание, обновление, удаление меток
  
- **Employee** - управление сотрудниками
  - Создание, обновление, архивирование сотрудников

## Real-time уведомления

Проект поддерживает real-time уведомления через SignalR Hub:
- Подключение: `http://localhost:5001/mapHub`
- События: обновления карт и меток в реальном времени

## Мониторинг и логирование

- **Логи**: доступны в Seq (http://localhost:5341) и Elasticsearch через Kibana (http://localhost:5601)
- **Трейсинг**: распределённый трейсинг в Jaeger (http://localhost:16686)
- **Метрики**: Prometheus + Grafana для мониторинга метрик приложения

## Разработчики

Zerkhades – maintainer

## Лицензия

Данный проект распространяется под лицензией MIT. См. LICENSE файл для подробностей.
