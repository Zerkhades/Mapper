# Mapper

Интерактивная карта для предприятия (Front + Back)

## Описание

Данный проект представляет собой решение, использующее ASP.NET Core WebAPI с использованием ORM для работы с базой данных. В проекте реализована архитектура CQRS + Mediator, что позволяет разделить логику обработки команд и запросов для улучшения поддержки и масштабируемости. В качестве базы данных использует MSSQL Express 

## Технологии

- **.NET 8**
- **ASP.NET Core WebAPI**
- **Entity Framework (ORM)**
- **CQRS + Mediator**
- **MSSQL Express**
- **React as web front-end** (in progress)
- **Avalonia as desktop front-end** (in progress)

## Установка

1. Клонируйте репозиторий:
   ```bash
   git clone https://github.com/Zerkhades/Mapper
   ```
2. Откройте решение в Visual Studio/Rider или VS Code.

3. Восстановите зависимости:
	```bash
	dotnet restore
	```
4. Обновите строку подключения в appsettings.json для подключения к вашей базе данных MSSQL Express:
	```json
	"ConnectionStrings": {
	  "DefaultConnection": "Server=localhost;Database=YourDatabase;Trusted_Connection=True;"
	}
	```
# Установка с помощью Docker-compose
Для запуска проекта с помощью Docker выполните следующие шаги:

**Для разворачивания контейнеров с помощью IDE достаточно выбрать docker-compose как основной проект и запустить его, дальнейших пункты не требуются**

1. **Клонируйте репозиторий**:
   ```bash
   git clone https://github.com/your-username/your-project-name.git
   ```

2. **Создайте и запустите контейнеры из образа**
	```powershell
	docker-compose up
	```
# Архитектура
Проект построен на архитектуре CQRS с использованием MediatR для обработки запросов и команд. Каждый слой имеет свою ответственность:

Mapper.Application: Содержит бизнес-логику и обработчики команд/запросов.

Mapper.Domain: Содержит модели данных и бизнес-объекты.

Mapper.WebApi: Обработчик API-запросов, предоставляющий интерфейс для взаимодействия с клиентом.

Mapper.Persistence: Содержит реализацию работы с базой данных через Entity Framework.

Mapper.Tests: Юнит-тесты для проверки бизнес-логики.

Mapper, Mapper.Desktop: Содержит в себе логику для отображения на платформе desktop

# Структура папок
```plaintext
├── Mapper.Backend
    ├── Core
        ├── Mapper.Application/       # Логика приложения (команды/запросы)
        ├── Mapper.Domain/            # Модели и бизнес-логика
    ├── Infrastructure
        ├── Mapper.Persistence/       # Доступ к данным (EF Core)
    ├── Presentation
        ├── Mapper.WebApi/            # API контроллеры
    ├── Tests
        ├── Mapper.Tests/             # Тесты
    ├── Docker-compose                # Контейнеризация WebAPI проекта
├── Mapper                            # Avalonia 
├── Mapper.Desktop                    # Avalonia desktop front-end
├── Mapper.sln                        # Решение
```

# Как использовать API
Все эндпоинты описаны в swagger: localhost:port/index.html / localhost:port/swagger

# Разработчики
Zerkhades – maintainer

# Лицензия
Данный проект распространяется под лицензией MIT. См. LICENSE файл для подробностей.