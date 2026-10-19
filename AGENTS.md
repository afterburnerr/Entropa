# Entropa — обязательные правила для агентов

Этот файл читается автоматически в каждом сеансе. Он задаёт порядок работы с Unity и C#.
Не начинай редактировать код, сцену или ассеты, пока не выполнил раздел 1.

## 0. Проверенный контекст проекта

- Проект: `/home/afterburner/Projects/unity/Entropa`
- Unity Editor: `6000.6.6f1`
- Render pipeline: URP `17.6.0`
- Input System: `1.20.0`, uGUI `2.6.0`
- Unity Pipeline: `com.unity.pipeline` `0.8.0-exp.1` (даёт live-команды редактора)
- Player: `Mono2x`, API Compatibility Level `NET_Standard_2_0`, target `StandaloneLinux64`
- Живой Editor подключён: `unity status` → `127.0.0.1:7800`, state `ready`
- Работа с Editor — только через Unity CLI (`unity command`); MCP намеренно не используется
- Git: проект под git, перед изменениями проверяй `git status` и не трогай чужие незакоммиченные изменения

## 1. Железное правило: сначала skills, потом код

Перед любым ответом или действием по Unity/C# загрузи нужные skills через инструмент `skill`.

Минимум для любой задачи, где есть Unity или C#:

1. `unity-development` — всегда;
2. `csharp-best-practices` — всегда, если в задаче есть C#;
3. профильные skills из матрицы ниже;
4. `unity-cli` — если задача касается Unity Editor, сцен, ассетов, сборки, тестов или CLI.

В первом прогрессе явно напиши: `Skills loaded: ...`.
Если инструмент `skill` недоступен, скажи об этом и работай по правилам этого файла.
Молча пропускать skills запрещено.

## 2. Матрица загрузки skills

| Задача | Обязательные skills |
|---|---|
| Любая Unity/C# задача | `unity-development`, `csharp-best-practices` |
| Новый gameplay-скрипт, MonoBehaviour, ScriptableObject, архитектура скриптов | + `unity-scriptdesign` |
| Inspector, EditorWindow, PropertyDrawer, AssetImporter, UI Toolkit/IMGUI, prefab tooling | + `unity-editor-tooling` |
| Edit Mode / Play Mode тесты, CI-тесты Unity | + `unity-testing` |
| Аудит тестов, flaky/tautological tests | + `test-anti-patterns` |
| Профилирование, GC, память, frame time, Jobs/Burst/DOTS | + `unity-performance`, при необходимости `performance-optimization` |
| Рефакторинг C# без изменения поведения | + `csharp-refactoring` |
| Управление живым Editor, сценой, prefab, ассетами, сборкой через CLI | + `unity-cli` |
| Новая игровая механика, фича, жанр, «с чего начать» | + `gamedev-router`, затем профильные game-* skills |
| HUD, меню, инвентарь, экранные состояния | + `game-ui-ux` |
| Камера, follow, orbit, shake | + `camera-systems` |
| Управление, ребайнд, геймпад, deadzone, input buffering | + `input-systems` |
| Физика, timestep, jitter, tunneling, collision layers | + `physics-tuning` |
| Уровни, blockout, pacing, encounter design | + `level-design` |
| Сохранения, save slots, миграции, autosave | + `save-systems` |
| Game feel, juice, screenshake, hit-stop, squash/stretch | + `game-feel`, `game-juice` |
| Спрайты, тайлсеты, фоны, UI-арт, 3D-ассеты | + `create-game-assets` |
| Аудио, микшер, ducking, adaptive music | + `audio-design` |
| Новая крупная фича | + `brainstorming` до реализации, затем `writing-plans` |
| Выполнение письменного плана | + `executing-plans` |
| Баг, падение, неверное поведение | + `systematic-debugging` |
| Разработка через тесты | + `test-driven-development` |
| Перед заявлением «готово» | + `verification-before-completion` |
| Код-ревью | + `requesting-code-review` / `receiving-code-review` |
| Независимые параллельные задачи | + `dispatching-parallel-agents` или `subagent-driven-development` |
| Изоляция ветки | + `using-git-worktrees` |
| Завершение ветки | + `finishing-a-development-branch` |

Не используй в Unity Phaser/PixiJS/three.js skills: `phaser-*`, `pixijs-*`, `threejs-*`,
`game-setup-and-config`, `loading-assets`, `scenes`, `events-system`, `scale-and-responsive`,
`v3-to-v4-migration`, `v4-new-features`, `audio-and-sound`, `yandex-games-dev`.
Они относятся к веб-движкам. Исключение — пользователь явно просит веб-прототип.

Старый `~/.claude/skills/unity-development/SKILL.md` описывает CoplayDev `@unityMCP` и
устаревшие имена инструментов. В opencode действует override из
`~/.config/opencode/skills/unity-development/SKILL.md`; не следуй старым именам `@unityMCP`.

Полный каталог skills с путями и краткими правилами: `UNITY_SKILLS.md`.

## 3. Живой Unity Editor через CLI

Если Editor открыт, управляй им через CLI, а не правь `.unity`, `.prefab` и `.asset` вручную.

```bash
unity status --format json
unity command --format json                 # список команд и их схем
unity command --format json get_scene_hierarchy
unity command --format json console         # ошибки компиляции и логи
unity command eval 'return Application.unityVersion;'
unity command editor_play
unity command editor_stop
unity command recompile
unity command --format json recompile_status
```

Синтаксис:

- глобальные флаги (`--format json`, `--project-path`, `--timeout`) ставь до `command`;
- параметры команды передавай как `--name value`;
- если у команды один позиционный аргумент (например `eval`), передавай его строкой;
- перед вызовом незнакомой команды прочитай её схему:
  `unity command --format json --query <command_name>`.

Примеры live-операций:

```bash
unity command create_gameobject --name Player --primitive capsule
unity command find_gameobjects --name Player
unity command delete_gameobject --target '/Player'
unity command add_component --target '/Player' --type Rigidbody
unity command save_scene
```

MCP не используем: единственный канал управления Editor — `unity command`. Проверяй реальные
имена и схемы команд через `unity command --format json`, не выдумывай их.

Если CLI не подключается, хотя Editor запущен:

1. выполни `unity pipeline list --format json`;
2. проверь Safe Mode (`data.summary.instancesInSafeMode`);
3. при Safe Mode исправь ошибки компиляции в C# и перезапусти Editor;
4. не запускай второй Editor на уже открытом проекте.

## 4. Инженерные правила Unity/C#

### Версия и совместимость

- Перед кодом читай `ProjectSettings/ProjectVersion.txt`, `Packages/manifest.json`,
  `Packages/packages-lock.json`, затронутые `.asmdef`/`.asmref` и настройки Player.
- Не предполагай, что десктопный .NET SDK равен компилятору Unity.
- Для этого проекта целевые `Mono2x` и `.NET Standard 2.0`; не вводи API и синтаксис,
  которые не поддерживаются этим профилем.
- Документацию сверяй с версией Editor/пакета, а не с общими примерами.

### MonoBehaviour и жизненный цикл

- Не создавай MonoBehaviour через `new`; используй `AddComponent` или `Instantiate` prefab.
- В `Awake` инициализируй собственные ссылки; не рассчитывай на порядок `Awake` у чужих объектов.
- Подписки на события — в `OnEnable`, отписки — в `OnDisable`; учитывай pooling и disabled domain reload.
- Изменения Rigidbody — в `FixedUpdate`; ввод читай по правилам установленного Input System и потребляй в симуляции.
- Follow-камеру обновляй в `LateUpdate`.
- Coroutine не является фоновым потоком; отключение MonoBehaviour не останавливает его coroutine.
- Async: владей `CancellationToken`, не используй `.Result`/`.Wait()`, возвращайся на main thread
  перед вызовами Unity API. `ConfigureAwait(false)` — только в контекстно-независимых библиотеках.

### Данные и сериализация

- Разделяй authoring-time, composition-time и runtime-mutable состояние.
- ScriptableObject — общие authoring-данные; не храни в нём per-instance runtime state.
- Держи `.meta` рядом с ассетом, сохраняй GUID при перемещении/переименовании.
- При переименовании сериализованных полей используй `FormerlySerializedAs`, если версия поддерживает.
- Проверяй последствия для prefab overrides, сцен, UnityEvent method names, animation events,
  reflection и assembly-qualified type names.
- Не редактируй сгенерированные `.csproj` как источник конфигурации Unity.
- Не коммить `Library`, `Temp`, `Logs`, `obj` и build-кэши.
- Editor-код держи в Editor-only assembly или под корректной conditional compilation.

### Качество C#

- Соблюдай ownership: коллекции не выставляй изменяемыми без необходимости.
- Для `UnityEngine.Object` учитывай lifetime-aware equality; `?.`, `??`, `is null` не заменяют её.
- Строковые идентификаторы сравнивай ordinal-сравнением; для пользовательского текста — с нужной культурой.
- Коллекции выбирай по паттерну доступа; LINQ не тащи в hot path без измерения.
- Исключения — для исключительных ситуаций; не глотай ошибки, сохраняй stack trace через `throw;`.
- Освобождай `IDisposable` через `using`/`finally`; отписывайся от событий по lifetime.
- Не добавляй абстракции, DI-контейнер, event bus или singleton «по умолчанию»: это trade-off.

### Editor tooling

- Inspector-правки — через `SerializedObject`/`SerializedProperty`, с `Update` и
  `ApplyModifiedProperties`; учитывай multi-object editing и mixed values.
- Перед мутацией записывай правильный `Undo`; для структурных операций используй парные Undo API.
- Не выполняй тяжёлые операции и сетевые вызовы в `OnGUI`, repaint или PropertyDrawer.
- `OnValidate` держи лёгким и безопасным.
- Импорт большими партиями — `StartAssetEditing`/`StopAssetEditing` в `try/finally`.
- UI Toolkit (UXML/USS) и IMGUI — форматы Unity, не произвольный HTML/CSS.

### Тесты

- Используй Unity Test Framework и NUnit, не xUnit/MSTest.
- Edit Mode — для синхронной логики; Play Mode — для player loop, lifecycle, physics и сцен.
- Очищай созданные объекты, сцены, подписки и ассеты в `SetUp`/`TearDown`.
- Восстанавливай time scale, physics settings, static state и random state.
- Избегай wall-clock sleeps и неограниченного polling; используй ограниченное число кадров.
- Проверяй свежий XML-результат и логи, а не только exit code.

### Рефакторинг

- Сначала классифицируй операцию: behavior-preserving refactor, feature, bug fix или contract change.
- Не смешивай рефакторинг и изменение поведения в одном шаге.
- Ищи привязки, а не текстовые совпадения; учитывай partial-объявления и сгенерированный код.
- Для Unity дополнительно проверяй сериализацию, `.meta`, GUID, prefab/scene references и UnityEvent.

### Производительность

- Сначала измеряй: Profiler, Memory Profiler, Frame Debugger, Profile Analyzer.
- Не добавляй pooling, Jobs, Burst, DOTS или смену pipeline без конкретной причины и замеров.
- Отделяй Editor overhead от player behavior; сравнивай одинаковые условия.
- Не заменяй ясный cold-path код ради гипотетической аллокации.

## 5. Обязательная проверка перед словом «готово»

1. Если менялся C#: `unity command recompile`, затем `unity command --format json recompile_status`
   до завершения.
2. `unity command --format json console` — `compilationFailed: false`, 0 ошибок.
3. Если менялась логика: запусти релевантные Edit Mode/Play Mode тесты через Unity.
4. Если менялась сцена/UI/визуал: сделай `capture_scene_view` или `capture_game_view`.
5. Если менялись player settings, сборка или платформенный код: выполни сборку через `unity build`.
6. В финальном ответе укажи точные команды и наблюдаемый результат.
   Если проверка не запускалась — прямо скажи, что именно осталось непроверенным.

Не заявляй об успехе на основании намерения или старого результата. Нужны свежие доказательства.
