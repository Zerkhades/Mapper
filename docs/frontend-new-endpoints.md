# Новые endpoints для фронтенда

Базовый префикс: `/api/v1`.

Даты передавайте в ISO 8601 с timezone, например `2026-05-07T18:00:00Z`.
Поля JSON возвращаются в `camelCase`.

## Авторизация

API настроен на JWT Bearer токены от Keycloak.

Текущее состояние в коде: для новых endpoints из этого документа нет `[Authorize]`, и глобальная fallback policy тоже не включена. Это значит, что сейчас они технически доступны без токена, если инфраструктура перед API не ограничивает доступ отдельно.

Для фронтенда все равно стоит сразу поддержать авторизованный вызов, потому что аутентификация уже подключена в WebApi и может быть включена на контроллерах без изменения контрактов.

Header для защищенных запросов:

```http
Authorization: Bearer <access_token>
```

Ожидаемые scopes:

| Scope | Назначение |
| --- | --- |
| `openid` | OIDC login. |
| `profile` | Базовый профиль пользователя. |
| `api` | Доступ к Mapper API. |

Рекомендуемый flow для браузерного фронтенда: Authorization Code + PKCE.

Keycloak endpoints для realm `mapper`:

| Endpoint | Назначение |
| --- | --- |
| `/auth/realms/mapper/protocol/openid-connect/auth` | Начать login flow. |
| `/auth/realms/mapper/protocol/openid-connect/token` | Обменять authorization code на tokens. |
| `/auth/realms/mapper/protocol/openid-connect/userinfo` | Получить профиль, если нужен UI. |
| `/auth/realms/mapper/protocol/openid-connect/logout` | Logout flow. |

Клиент для фронтенда/Swagger в текущей конфигурации:

| Поле | Значение |
| --- | --- |
| `client_id` | `mapper.swagger` |
| `grant_type` | Authorization Code |
| PKCE | обязательно |
| client secret | не требуется |
| scopes | `openid profile api` |

Локальные redirect origins, которые уже есть в Keycloak:

| Origin | Комментарий |
| --- | --- |
| `http://localhost:5001` | WebApi/Swagger local. |
| `https://localhost:7193` | HTTPS local variant. |
| `http://localhost:8080` | Docker compose HTTP. |
| `https://localhost:8081` | Docker compose HTTPS. |

Если фронтенд запускается на другом origin, его нужно добавить в `webOrigins`/redirect URIs клиента Keycloak и в `Cors:AllowedOrigins` WebApi для non-development окружений.

Обработка ошибок авторизации на фронтенде:

| HTTP status | Что делать |
| --- | --- |
| `401 Unauthorized` | Токена нет, он истек или не принят API. Запустить login/refresh flow. |
| `403 Forbidden` | Пользователь аутентифицирован, но прав или scope недостаточно. Показать экран недостаточных прав. |

Для destructive endpoint `POST /retention/archive/cleanup` UI должен требовать отдельное подтверждение пользователя даже при наличии токена. Безопасный режим: сначала `dryRun=true`, затем после подтверждения `dryRun=false` и `confirm=true`.

## Operator Dashboard

`GET /api/v1/dashboard/operator`

Сводка для операторского экрана: камеры, алерты, архивы, активность и аномалии движения.

Query:

| Параметр | Тип | По умолчанию | Описание |
| --- | --- | --- | --- |
| `from` | `DateTimeOffset?` | последние 24 часа | Начало периода. |
| `to` | `DateTimeOffset?` | текущее UTC-время | Конец периода. |
| `top` | `number` | `5` | Сколько top/problem элементов вернуть. |

Пример:

```http
GET /api/v1/dashboard/operator?from=2026-05-06T00:00:00Z&to=2026-05-07T00:00:00Z&top=5
```

Ответ:

```json
{
  "periodStart": "2026-05-06T00:00:00Z",
  "periodEnd": "2026-05-07T00:00:00Z",
  "totalCameras": 12,
  "onlineCameras": 10,
  "offlineCameras": 2,
  "unknownStatusCameras": 0,
  "motionAlerts": 34,
  "unresolvedMotionAlerts": 4,
  "highSeverityUnresolvedAlerts": 1,
  "archivedVideos": 120,
  "averageMotionPercentage": 18.4,
  "topActiveCameras": [
    {
      "cameraMarkId": "11111111-1111-1111-1111-111111111111",
      "cameraTitle": "Main entrance",
      "motionAlerts": 9,
      "unresolvedAlerts": 1,
      "averageMotionPercentage": 44.2
    }
  ],
  "anomalies": [
    {
      "cameraMarkId": "11111111-1111-1111-1111-111111111111",
      "cameraTitle": "Main entrance",
      "currentAlertCount": 9,
      "baselineAverageAlertCount": 2,
      "zScore": 3.5,
      "activityRatio": 4.5,
      "reason": "Motion activity is significantly above baseline."
    }
  ],
  "problemCameras": [
    {
      "cameraMarkId": "22222222-2222-2222-2222-222222222222",
      "cameraTitle": "Warehouse",
      "isOnline": false,
      "status": "Offline",
      "changedAt": "2026-05-06T21:10:00Z",
      "reason": "NetworkTimeout",
      "responseTimeMs": null
    }
  ]
}
```

## Map Graph Analytics

`GET /api/v1/dashboard/map-graph`

Аналитика связности карт и переходов: изолированные карты, слабые компоненты, узкие места, кратчайший путь.

Query:

| Параметр | Тип | По умолчанию | Описание |
| --- | --- | --- | --- |
| `sourceGeoMapId` | `Guid?` | `null` | Начальная карта для расчета пути. |
| `targetGeoMapId` | `Guid?` | `null` | Целевая карта для расчета пути. |
| `top` | `number` | `5` | Сколько bottleneck-узлов вернуть. |

Пример:

```http
GET /api/v1/dashboard/map-graph?sourceGeoMapId=aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa&targetGeoMapId=bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb&top=5
```

Ключевые поля ответа:

| Поле | Описание |
| --- | --- |
| `mapCount` | Количество карт. |
| `transitionCount` | Количество переходов между картами. |
| `isolatedMapCount` | Карты без входящих и исходящих переходов. |
| `weakComponentCount` | Количество слабосвязных компонент. |
| `directedReachabilityRatio` | Доля достижимости в направленном графе. |
| `nodes` | Карты с входящими/исходящими переходами и reachability. |
| `edges` | Переходы между картами. |
| `weakComponents` | Группы карт, связанных хотя бы без учета направления. |
| `bottlenecks` | Карты с наибольшим влиянием на навигацию. |
| `shortestPaths` | Кратчайший путь, если переданы `sourceGeoMapId` и `targetGeoMapId`. |

## Indoor Routes

Маршруты строятся поверх изображения карты через редактируемый навигационный граф:

- `route node` - точка, через которую может проходить маршрут.
- `route edge` - связь между двумя точками.
- `calculate` - расчет линии маршрута от точки А до точки Б.

Координаты `x` и `y` используют ту же систему координат, что и метки карты. Frontend должен рисовать `segments[].points` поверх изображения карты как polyline.

Важная модель поведения:

- Backend пока не анализирует изображение карты автоматически.
- Frontend или редактор маршрутов создает узлы и ребра вручную.
- Узел можно привязать к существующей метке через `geoMarkId`.
- Если маршрут начинается/заканчивается в произвольной точке `x/y`, backend привяжет эту точку к ближайшему route node.
- Если маршрут начинается/заканчивается в `geoMarkId`, backend сначала ищет route node с таким `geoMarkId`; если его нет, берет ближайший route node к координатам метки.
- Если пути нет, ответ будет успешным, но `points` будет пустым, а расстояние `0`.

### Route Nodes

#### Получить узлы маршрута

`GET /api/v1/geomaps/{geoMapId}/routes/nodes`

Пример:

```http
GET /api/v1/geomaps/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/routes/nodes
```

Ответ:

```json
[
  {
    "id": "11111111-1111-1111-1111-111111111111",
    "geoMapId": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
    "geoMarkId": "22222222-2222-2222-2222-222222222222",
    "x": 120.5,
    "y": 240.0,
    "title": "Main corridor"
  }
]
```

Поля:

| Поле | Тип | Описание |
| --- | --- | --- |
| `id` | `Guid` | Идентификатор route node. |
| `geoMapId` | `Guid` | Карта, на которой находится узел. |
| `geoMarkId` | `Guid?` | Опциональная привязка к метке карты: рабочему месту, камере или переходу. |
| `x` | `number` | X-координата на изображении карты. |
| `y` | `number` | Y-координата на изображении карты. |
| `title` | `string?` | Название для редактора маршрутов. |

#### Создать узел маршрута

`POST /api/v1/geomaps/{geoMapId}/routes/nodes`

Body:

```json
{
  "x": 120.5,
  "y": 240.0,
  "title": "Main corridor",
  "geoMarkId": null
}
```

Ответ:

```json
"11111111-1111-1111-1111-111111111111"
```

#### Обновить узел маршрута

`PUT /api/v1/geomaps/{geoMapId}/routes/nodes/{routeNodeId}`

Body:

```json
{
  "x": 130.0,
  "y": 245.0,
  "title": "Main corridor updated",
  "geoMarkId": "22222222-2222-2222-2222-222222222222"
}
```

Ответ: `204 No Content`.

#### Удалить узел маршрута

`DELETE /api/v1/geomaps/{geoMapId}/routes/nodes/{routeNodeId}`

Ответ: `204 No Content`.

Важно: при удалении узла backend удаляет связанные route edges.

### Route Edges

#### Получить ребра маршрута

`GET /api/v1/geomaps/{geoMapId}/routes/edges`

Ответ:

```json
[
  {
    "id": "33333333-3333-3333-3333-333333333333",
    "geoMapId": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
    "fromNodeId": "11111111-1111-1111-1111-111111111111",
    "toNodeId": "44444444-4444-4444-4444-444444444444",
    "costOverride": null,
    "isBidirectional": true,
    "isDisabled": false,
    "description": "Main corridor segment"
  }
]
```

Поля:

| Поле | Тип | Описание |
| --- | --- | --- |
| `id` | `Guid` | Идентификатор route edge. |
| `geoMapId` | `Guid` | Карта, на которой находится связь. |
| `fromNodeId` | `Guid` | Начальный узел. |
| `toNodeId` | `Guid` | Конечный узел. |
| `costOverride` | `number?` | Пользовательская стоимость пути. Если `null`, используется геометрическое расстояние. |
| `isBidirectional` | `boolean` | `true` - можно идти в обе стороны; `false` - только `fromNodeId -> toNodeId`. |
| `isDisabled` | `boolean` | Временно выключить связь без удаления. |
| `description` | `string?` | Комментарий для редактора. |

#### Создать ребро маршрута

`POST /api/v1/geomaps/{geoMapId}/routes/edges`

Body:

```json
{
  "fromNodeId": "11111111-1111-1111-1111-111111111111",
  "toNodeId": "44444444-4444-4444-4444-444444444444",
  "isBidirectional": true,
  "costOverride": null,
  "description": "Main corridor segment"
}
```

Ответ:

```json
"33333333-3333-3333-3333-333333333333"
```

`costOverride` полезен, когда геометрически линия короткая, но фактически проход сложнее: лестница, турникет, опасная зона, медленный участок.

#### Обновить ребро маршрута

`PUT /api/v1/geomaps/{geoMapId}/routes/edges/{routeEdgeId}`

Body:

```json
{
  "isBidirectional": false,
  "costOverride": 300,
  "isDisabled": false,
  "description": "One-way corridor"
}
```

Ответ: `204 No Content`.

#### Удалить ребро маршрута

`DELETE /api/v1/geomaps/{geoMapId}/routes/edges/{routeEdgeId}`

Ответ: `204 No Content`.

### Calculate Route

`POST /api/v1/geomaps/{geoMapId}/routes/calculate`

Строит маршрут внутри одной карты. Сейчас межкартовые маршруты через `TransitionMark` не склеиваются автоматически; для UI это лучше показывать как следующий этап: сначала маршрут до перехода, затем маршрут на целевой карте.

Body:

```json
{
  "start": {
    "nodeId": null,
    "geoMarkId": "22222222-2222-2222-2222-222222222222",
    "x": null,
    "y": null
  },
  "end": {
    "nodeId": null,
    "geoMarkId": null,
    "x": 640.0,
    "y": 360.0
  },
  "walkingSpeed": 1.4
}
```

`start` и `end` задаются одним из способов:

| Вариант | Пример | Когда использовать |
| --- | --- | --- |
| По route node | `{ "nodeId": "11111111-1111-1111-1111-111111111111" }` | Редактор маршрутов, точный старт из графа. |
| По метке карты | `{ "geoMarkId": "22222222-2222-2222-2222-222222222222" }` | Маршрут от рабочего места, камеры, перехода. |
| По координатам | `{ "x": 640.0, "y": 360.0 }` | Пользователь кликнул произвольную точку на карте. |

`walkingSpeed` - скорость движения в единицах координат карты в секунду. По умолчанию `1.4`. Если координаты карты заданы в пикселях, фронту лучше воспринимать `estimatedSeconds` как условную оценку, пока нет калибровки “пиксели -> метры”.

Ответ:

```json
{
  "segments": [
    {
      "geoMapId": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
      "points": [
        { "x": 120.5, "y": 240.0 },
        { "x": 180.0, "y": 250.0 },
        { "x": 300.0, "y": 260.0 },
        { "x": 640.0, "y": 360.0 }
      ],
      "distance": 537.42,
      "estimatedSeconds": 384
    }
  ],
  "totalDistance": 537.42,
  "estimatedSeconds": 384
}
```

Поля ответа:

| Поле | Тип | Описание |
| --- | --- | --- |
| `segments` | `RouteSegment[]` | Сегменты маршрута. Сейчас обычно один сегмент на текущей карте. |
| `segments[].geoMapId` | `Guid` | Карта, на которой нужно рисовать polyline. |
| `segments[].points` | `{ x, y }[]` | Готовые точки линии маршрута. |
| `segments[].distance` | `number` | Длина сегмента в координатных единицах карты. |
| `segments[].estimatedSeconds` | `number` | Оценка времени для сегмента. |
| `totalDistance` | `number` | Общая длина маршрута. |
| `estimatedSeconds` | `number` | Общая оценка времени. |

### UI Flow

Рекомендуемый сценарий для frontend:

1. Открыть карту и загрузить обычные marks.
2. Загрузить `GET /routes/nodes` и `GET /routes/edges`.
3. В режиме просмотра дать пользователю выбрать точку А и точку Б.
4. Отправить `POST /routes/calculate`.
5. Нарисовать `segments[0].points` как polyline поверх изображения.
6. Если `points` пустой, показать состояние “маршрут не найден”.

Рекомендуемый сценарий для редактора маршрутов:

1. Оператор ставит route nodes на проходах, поворотах, дверях, лестницах, лифтах.
2. Соединяет соседние узлы route edges.
3. Для односторонних проходов ставит `isBidirectional=false`.
4. Для временно закрытых проходов ставит `isDisabled=true`.
5. Для сложных участков задает `costOverride`, если короткая линия не должна считаться быстрым путем.

### Error Handling

Типовые ошибки:

| HTTP status | Причина | Что делать в UI |
| --- | --- | --- |
| `404` | Карта, узел, ребро или метка не найдены. | Обновить данные карты, убрать устаревшую выбранную точку. |
| `400` | Невалидный body, например нет `start`/`end` или `walkingSpeed <= 0`. | Подсветить форму/состояние выбора. |
| `200` с пустым `points` | Граф есть, но пути между точками нет. | Показать “маршрут не найден” и предложить выбрать другую точку. |

### Notes For Future Image Analysis

Текущий контракт уже готов к полуавтоматической генерации графа по изображению карты. Когда появится анализ картинки, frontend сможет использовать те же `RouteNode` и `RouteEdge`: backend или отдельный инструмент будет создавать черновой граф, а оператор будет подтверждать и редактировать его через эти endpoints.

## Smart Notifications

`GET /api/v1/notifications/smart`

Список приоритетных уведомлений для UI: offline-камеры, high motion alerts, всплески активности.

Query:

| Параметр | Тип | По умолчанию | Описание |
| --- | --- | --- | --- |
| `from` | `DateTimeOffset?` | последние 24 часа | Начало периода анализа. |
| `to` | `DateTimeOffset?` | текущее UTC-время | Конец периода анализа. |
| `offlineGraceMinutes` | `number` | `5` | Не показывать offline, пока камера offline меньше этого времени. Допустимый диапазон нормализуется до `1..1440`. |
| `top` | `number` | `50` | Максимум уведомлений. Допустимый диапазон нормализуется до `1..200`. |

Типы уведомлений:

| `type` | `severity` | Когда появляется |
| --- | --- | --- |
| `CameraOffline` | `Warning` или `Critical` | Камера offline дольше `offlineGraceMinutes`; `Critical` после 30 минут. |
| `HighMotion` | `Critical` | Есть нерешенный high motion alert. |
| `MotionSpike` | `Warning` | Текущая активность сильно выше baseline за предыдущий равный период. |

Пример ответа:

```json
[
  {
    "id": "camera-offline:22222222-2222-2222-2222-222222222222:2026-05-07T17:30:00.0000000Z",
    "type": "CameraOffline",
    "severity": "Critical",
    "title": "Camera is offline",
    "message": "Warehouse has been offline for 42 minutes.",
    "geoMapId": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
    "cameraMarkId": "22222222-2222-2222-2222-222222222222",
    "cameraTitle": "Warehouse",
    "relatedEntityId": null,
    "occurredAt": "2026-05-07T17:30:00Z",
    "generatedAt": "2026-05-07T18:12:00Z",
    "context": {
      "reason": "NetworkTimeout",
      "offlineMinutes": "42"
    }
  }
]
```

UI-подсказка: используйте `id` как стабильный ключ списка, `severity` для цвета/сортировки, `relatedEntityId` для перехода к связанному alert/video, если поле заполнено.

## Audit Events

`GET /api/v1/audit/events`

Лента действий для админского UI или карточки сущности.

Query:

| Параметр | Тип | По умолчанию | Описание |
| --- | --- | --- | --- |
| `entityType` | `string?` | `null` | Фильтр по типу сущности. |
| `entityId` | `Guid?` | `null` | Фильтр по конкретной сущности. |
| `userId` | `Guid?` | `null` | Фильтр по пользователю. |
| `from` | `DateTimeOffset?` | `null` | Начало периода. |
| `to` | `DateTimeOffset?` | `null` | Конец периода. |
| `skip` | `number` | `0` | Смещение для пагинации. |
| `take` | `number` | `100` | Размер страницы. |

Пример:

```http
GET /api/v1/audit/events?entityType=GeoMap&entityId=aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa&skip=0&take=50
```

Ответ:

```json
[
  {
    "id": "33333333-3333-3333-3333-333333333333",
    "action": "UpdateGeoMapCommand",
    "entityType": "GeoMap",
    "entityId": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
    "userId": "44444444-4444-4444-4444-444444444444",
    "occurredAt": "2026-05-07T18:10:00Z",
    "metadataJson": "{\"requestName\":\"UpdateGeoMapCommand\"}"
  }
]
```

## Archive Retention Preview

`GET /api/v1/retention/archive/preview`

Безопасный просмотр видеоархивов, которые подпадают под retention policy. Ничего не удаляет.

Query:

| Параметр | Тип | По умолчанию | Описание |
| --- | --- | --- | --- |
| `cameraMarkId` | `Guid?` | `null` | Ограничить preview одной камерой. |
| `now` | `DateTimeOffset?` | текущее UTC-время | Опорное время расчета возраста. Обычно не передавать. |
| `motionVideoRetentionDays` | `number` | `90` | Retention для видео с движением. |
| `noMotionVideoRetentionDays` | `number` | `7` | Retention для видео без движения. |
| `archivedVideoRetentionDays` | `number` | `365` | Retention для архивированных видео. |
| `take` | `number` | `100` | Максимум кандидатов. На сервере ограничивается до `1..500`. |

Ответ содержит:

| Поле | Описание |
| --- | --- |
| `candidateCount` | Сколько кандидатов вернул endpoint. |
| `reclaimableBytes` | Потенциально освобождаемый объем. |
| `candidates[]` | Видео-кандидаты с причиной попадания под policy. |

`candidates[].reason` может быть:

| Значение | Смысл |
| --- | --- |
| `Motion video exceeded motion retention policy.` | Видео с движением старше `motionVideoRetentionDays`. |
| `No-motion video exceeded short retention policy.` | Видео без движения старше `noMotionVideoRetentionDays`. |
| `Archived video exceeded archived retention policy.` | Архивированное видео старше `archivedVideoRetentionDays`. |

## Archive Retention Cleanup

`POST /api/v1/retention/archive/cleanup`

Запускает cleanup по тем же правилам, что preview.

Важно: по умолчанию `dryRun=true`, поэтому endpoint только возвращает кандидатов. Реальное удаление возможно только при `dryRun=false` и `confirm=true`.

Body:

```json
{
  "cameraMarkId": null,
  "now": null,
  "motionVideoRetentionDays": 90,
  "noMotionVideoRetentionDays": 7,
  "archivedVideoRetentionDays": 365,
  "take": 100,
  "dryRun": true,
  "confirm": false
}
```

Безопасный вызов для UI:

```http
POST /api/v1/retention/archive/cleanup
Content-Type: application/json

{
  "take": 100,
  "dryRun": true,
  "confirm": false
}
```

Реальное удаление:

```http
POST /api/v1/retention/archive/cleanup
Content-Type: application/json

{
  "take": 100,
  "dryRun": false,
  "confirm": true
}
```

Ответ:

```json
{
  "executedAt": "2026-05-07T18:20:00Z",
  "dryRun": false,
  "confirmed": true,
  "candidateCount": 2,
  "deletedCount": 2,
  "reclaimableBytes": 2048000,
  "candidates": [
    {
      "videoArchiveId": "55555555-5555-5555-5555-555555555555",
      "cameraMarkId": "22222222-2222-2222-2222-222222222222",
      "videoPath": "camera/222/video.mp4",
      "thumbnailPath": "camera/222/thumb.jpg",
      "recordedAt": "2025-12-01T10:00:00Z",
      "fileSizeBytes": 2048000,
      "hasMotionDetected": false,
      "isArchived": false,
      "ageDays": 157,
      "retentionDays": 7,
      "reason": "No-motion video exceeded short retention policy."
    }
  ]
}
```

UI-подсказка: для destructive action сначала вызывайте preview или cleanup с `dryRun=true`, показывайте пользователю `candidateCount` и `reclaimableBytes`, и только после явного подтверждения отправляйте `dryRun=false`, `confirm=true`.
