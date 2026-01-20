# Mapper.Tests

Проект юнит-тестов для приложения Mapper.

## Структура

### Context Factories

Фабрики контекста базы данных для тестов находятся в `Common/ContextFactories/`:

- **GeoMapsContextFactory** - создает контекст с тестовыми GeoMap для тестирования операций над картами
- **GeoMarksContextFactory** - создает контекст с тестовыми GeoMap и различными типами GeoMark (TransitionMark, WorkplaceMark, CameraMark)

### Test Fixtures

Query Test Fixtures используются для тестов запросов (Queries) с использованием xUnit Collection Fixtures:

- **GeoMapsQueryTestFixture** - фикстура для тестов запросов GeoMaps
- **GeoMarksQueryTestFixture** - фикстура для тестов запросов GeoMarks

### Тесты команд (Commands)

#### GeoMaps
- `CreateGeoMapCommandHandlerTests` - тестирование создания GeoMap
- `DeleteGeoMapCommandHandlerTests` - тестирование мягкого удаления GeoMap

#### GeoMarks
- `CreateGeoMarkCommandHandlerTests` - тестирование создания различных типов меток (Transition, Workplace, Camera)
- `DeleteGeoMarkCommandHandlerTests` - тестирование мягкого удаления GeoMark

### Тесты запросов (Queries)

#### GeoMaps
- `GetGeoMapListQueryHandlerTests` - тестирование получения списка карт
- `GetGeoMapByIdQueryHandlerTests` - тестирование получения карты по ID с кэшированием

#### GeoMarks
- `GetGeoMarksListQueryHandlerTests` - тестирование получения списка меток с фильтрацией по типу

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
- `IMapRealtimeNotifier` - для AddGeoMarkCommand
- `ICacheService` - для GetGeoMapByIdQuery

## Запуск тестов

Из корня решения:
```
dotnet test src\Mapper.Tests\Mapper.Tests.csproj
```

Или через Visual Studio Test Explorer.
