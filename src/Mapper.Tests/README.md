# Mapper.Tests

Проект юнит-тестов для приложения Mapper.

## Структура

### Context Factories

Фабрики контекста базы данных для тестов находятся в `Common/ContextFactories/`:

- **GeoMapsContextFactory** - создает контекст с тестовыми GeoMap для тестирования операций над картами
- **GeoMarksContextFactory** - создает контекст с тестовыми GeoMap и различными типами GeoMark (TransitionMark, WorkplaceMark, CameraMark)
- **EmployeesContextFactory** - создает контекст с тестовыми GeoMap, WorkplaceMark и Employee для тестирования операций над сотрудниками

### Test Fixtures

Query Test Fixtures используются для тестов запросов (Queries) с использованием xUnit Collection Fixtures:

- **GeoMapsQueryTestFixture** - фикстура для тестов запросов GeoMaps
- **GeoMarksQueryTestFixture** - фикстура для тестов запросов GeoMarks
- **EmployeesQueryTestFixture** - фикстура для тестов запросов Employees

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

### Тесты запросов (Queries)

#### GeoMaps
- `GetGeoMapListQueryHandlerTests` - тестирование получения списка карт
- `GetGeoMapByIdQueryHandlerTests` - тестирование получения карты по ID с кэшированием

#### GeoMarks
- `GetGeoMarksListQueryHandlerTests` - тестирование получения списка меток с фильтрацией по типу

#### Employees
- `GetEmployeeListQueryHandlerTests` - тестирование получения списка сотрудников с фильтрацией
- `GetEmployeeDetailsQueryHandlerTests` - тестирование получения деталей сотрудника

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

## Покрытие тестами

### Команды (30 тестов)
- **GeoMaps (6 тестов):**
  - CreateGeoMap: 2 теста
  - UpdateGeoMap: 3 теста
  - DeleteGeoMap: 2 теста

- **GeoMarks (16 тестов):**
  - CreateGeoMark: 4 теста (по типам)
  - UpdateTransitionMark: 3 теста
  - UpdateWorkplaceMark: 3 теста
  - UpdateCameraMark: 4 теста
  - DeleteGeoMark: 3 теста

- **Employees (8 тестов):**
  - CreateEmployee: 3 теста
  - UpdateEmployee: 4 теста
  - DeleteEmployee: 2 теста

### Запросы (17 тестов)
- GetGeoMapList: 2 теста
- GetGeoMapById: 2 теста
- GetGeoMarks: 3 теста
- GetEmployeeList: 4 теста
- GetEmployeeDetails: 3 теста

**Всего: 47 тестов**

## Запуск тестов

Из корня решения:
```
dotnet test src\Mapper.Tests\Mapper.Tests.csproj
```

Или через Visual Studio Test Explorer.
