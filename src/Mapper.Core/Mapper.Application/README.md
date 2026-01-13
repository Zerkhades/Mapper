# Mapper.Application

Слой приложения, содержащий бизнес-логику и CQRS команды/запросы.

## Структура проекта

```
Mapper.Application/
??? Common/                          # Общие компоненты
?   ??? Behaviours/                  # MediatR Pipelines
?   ?   ??? LoggingBehaviour.cs     # Логирование запросов
?   ?   ??? ValidationBehaviour.cs  # Валидация через FluentValidation
?   ??? Exceptions/                  # Исключения приложения
?   ?   ??? AlreadyExistsException.cs
?   ?   ??? NotFoundException.cs
?   ??? Mappings/                    # AutoMapper профили
?       ??? AssemblyMappingProfile.cs
?       ??? GeoMapProfile.cs
?       ??? IMapWith.cs
?
??? Features/                        # Основная бизнес-логика (CQRS)
?   ??? DTOs/                        # Data Transfer Objects
?   ?   ??? GeoMapDtos.cs           # GeoMapDetailsDto, GeoMarkDto
?   ?
?   ??? GeoMaps/                     # Функциональность карт
?   ?   ??? Commands/
?   ?   ?   ??? CreateGeoMap/
?   ?   ?       ??? CreateGeoMapCommand.cs  # Команда + Валидатор + Обработчик
?   ?   ??? Queries/
?   ?       ??? GetGeoMapById/
?   ?           ??? GetGeoMapByIdQuery.cs   # Запрос + Обработчик
?   ?
?   ??? GeoMarks/                    # Функциональность меток
?       ??? Commands/
?           ??? AddGeoMark/
?           ?   ??? AddGeoMarkCommand.cs        # Универсальная команда добавления метки
?           ??? AddTransitionMark/
?           ?   ??? AddTransitionMarkCommand.cs # Типизированная команда для метки-перехода
?           ??? AddWorkplaceMark/
?           ?   ??? AddWorkplaceMarkCommand.cs  # Типизированная команда для метки-рабочего места
?           ??? AddCameraMark/
?               ??? AddCameraMarkCommand.cs     # Типизированная команда для метки-камеры
?
??? Interfaces/                      # Интерфейсы для внешних зависимостей
?   ??? ICacheService.cs
?   ??? ICameraAdapter.cs
?   ??? ICurrentUserService.cs
?   ??? IMapImageStorage.cs
?   ??? IMapperDbContext.cs
?   ??? IMapRealtimeNotifier.cs
?
??? DependencyInjection.cs          # Регистрация сервисов

```

## Принципы организации

### 1. CQRS Pattern
- **Commands** - изменяют состояние системы (Create, Update, Delete, Archive)
- **Queries** - только читают данные

### 2. Структура Features
Каждая фича организована по принципу:
```
Feature/
  Commands/CommandName/
    CommandNameCommand.cs      # Команда (record)
    CommandNameValidator.cs    # Валидатор FluentValidation (в том же файле)
    CommandNameHandler.cs      # Обработчик MediatR (в том же файле)
```

**Все в одном файле**: Команда, валидатор и обработчик находятся в одном файле для удобства навигации и поддержки.

### 3. Naming Conventions
- Commands: `{Action}{Entity}Command` (например, `CreateGeoMapCommand`)
- Queries: `Get{Entity}{Filter}Query` (например, `GetGeoMapByIdQuery`)
- Handlers: `{CommandName}Handler`
- Validators: `{CommandName}Validator`

### 4. Зависимости
Проект использует:
- **MediatR** - CQRS паттерн
- **FluentValidation** - валидация команд
- **AutoMapper** - маппинг между доменными моделями и DTO
- **EF Core** - доступ к данным через IMapperDbContext

## Использование

### Пример команды:
```csharp
var command = new CreateGeoMapCommand(
    Name: "Офис",
    Description: "План первого этажа",
    ImageStream: stream,
    FileName: "office.png",
    ContentType: "image/png",
    ImageWidth: 1920,
    ImageHeight: 1080
);

var mapId = await mediator.Send(command, cancellationToken);
```

### Пример запроса:
```csharp
var query = new GetGeoMapByIdQuery(mapId);
var dto = await mediator.Send(query, cancellationToken);
```

## Валидация
Все команды автоматически валидируются через `ValidationBehavior` pipeline.
FluentValidation правила определены в классах-валидаторах рядом с командами.

## Логирование
Все запросы автоматически логируются через `LoggingBehaviour` pipeline с использованием Serilog.
