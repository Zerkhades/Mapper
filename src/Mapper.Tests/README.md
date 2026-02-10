# Mapper.Tests

Проект юнит-тестов для приложения Mapper.

## Структура

### Context Factories

Фабрики контекста базы данных для тестов находятся в `Common/ContextFactories/`:

- **GeoMapsContextFactory** - создает контекст с тестовыми GeoMap для тестирования операций над картами
- **GeoMarksContextFactory** - создает контекст с тестовыми GeoMap и различными типами GeoMark (TransitionMark, WorkplaceMark, CameraMark)
- **EmployeesContextFactory** - создает контекст с тестовыми GeoMap, WorkplaceMark и Employee для тестирования операций над сотрудниками
- **CameraArchiveContextFactory** - создает контекст с тестовыми Camera, CameraStatusHistory и архивом видео/событий

### Test Fixtures

Query Test Fixtures используются для тестов запросов (Queries) с использованием xUnit Collection Fixtures:

- **GeoMapsQueryTestFixture** - фикстура для тестов запросов GeoMaps
- **GeoMarksQueryTestFixture** - фикстура для тестов запросов GeoMarks
- **EmployeesQueryTestFixture** - фикстура для тестов запросов Employees

### Общие тесты и базовые классы

- `QueryTestFixture` - базовая фикстура для тестов запросов
- `TestCommandBase` - базовый класс для тестов команд
- `PaginatedListTests` - тесты общих компонентов пагинации
- `ExceptionsTests` - тесты доменных исключений

### Тесты команд (Commands)

#### GeoMaps
- `CreateGeoMapCommandHandlerTests` - тестирование создания GeoMap
- `UpdateGeoMapCommandHandlerTests` - тестирование обновления GeoMap (название и описание)
- `DeleteGeoMapCommandHandlerTests` - тестирование мягкого удаления GeoMap

#### GeoMarks
- `CreateGeoMarkCommandHandlerTests` - тестирование создания различных типов меток (Transition, Workplace, Camera)
- `UpdateTransitionMarkCommandHandlerTests` - тестирование обновления TransitionMark
- `UpdateWorkplaceMarkCommandHandlerTests` - тестирование обновления WorkplaceMark
- `UpdateCameraMarkCommandHandlerTests` - тестирование обновления CameraMark
- `DeleteGeoMarkCommandHandlerTests` - тестирование мягкого удаления GeoMark

#### Employees
- `CreateEmployeeCommandHandlerTests` - тестирование создания сотрудника
- `UpdateEmployeeCommandHandlerTests` - тестирование обновления сотрудника
- `DeleteEmployeeCommandHandlerTests` - тестирование архивации сотрудника

#### Camera Archive
- `CameraMotionAlertCommandHandlerTests` - тестирование создания событий движения
- `CameraVideoArchiveCommandHandlerTests` - тестирование записи видеоархива

### Тесты запросов (Queries)

#### GeoMaps
- `GetGeoMapListQueryHandlerTests` - тестирование получения списка карт
- `GetGeoMapByIdQueryHandlerTests` - тестирование получения карты по ID с кэшированием

#### GeoMarks
- `GetGeoMarksListQueryHandlerTests` - тестирование получения списка меток с фильтрацией по типу

#### Employees
- `GetEmployeeListQueryHandlerTests` - тестирование получения списка сотрудников с фильтрацией
- `GetEmployeeDetailsQueryHandlerTests` - тестирование получения деталей сотрудника

### Domain, DTO, Mappings, Validators

- `Domain/*Tests` - тесты доменных моделей (GeoMap, GeoMark, Employee, Camera)
- `DTOs/CameraDtoTests` - тесты DTO объектов камер
- `Mappings/CameraArchiveMappingTests` - тесты профилей маппинга
- `Validators/*ValidatorTests` - тесты валидаторов команд

### Интеграционные и инфраструктурные тесты

- `Integration/CameraIntegrationTests` - интеграционные тесты камеры
- `Infrastructure/FakeCameraAdapterTests` - тесты адаптера камеры

## Зависимости

- **xUnit** - фреймворк для тестирования
- **Moq** - библиотека для создания mock-объектов
- **Microsoft.EntityFrameworkCore.InMemoryDatabase** - для тестирования с in-memory БД

## Особенности

### Работа с private setters

Поскольку доменные модели используют `private init` для ID, в тестах используется рефлексия для установки ID:

```csharp
var map = new GeoMap("Name", "/path", 1920, 1080, "Description");
typeof(GeoMap).GetProperty("Id")!.SetValue(map, desiredId);
```

### Mock-объекты

Для тестирования используются mock-объекты для зависимостей:
- `IMapImageStorage` - для CreateGeoMapCommand
- `IMapRealtimeNotifier` - для AddGeoMarkCommand и всех Update команд для меток
- `ICacheService` - для GetGeoMapByIdQuery и всех Update команд
- `ICameraAdapter` - для тестов интеграции камеры и инфраструктуры

## Покрытие тестами

- **Команды**: GeoMaps, GeoMarks, Employees, Camera Archive
- **Запросы**: GeoMaps, GeoMarks, Employees
- **Доменные модели**: GeoMap, GeoMark, Employee, Camera
- **DTO и маппинги**: CameraDto, CameraArchiveProfile
- **Валидация**: CreateGeoMap, CreateEmployee
- **Инфраструктура и интеграция**: FakeCameraAdapter, CameraIntegration
- **Общие компоненты**: пагинация и доменные исключения

## Запуск тестов

Из корня решения:
```
dotnet test src\Mapper.Tests\Mapper.Tests.csproj
```

Или через Visual Studio Test Explorer.
