# Каталог Unity/C# skills на этой машине

Составлено 2026-10-19 для проекта Entropa. Здесь перечислены все установленные
skills, которые относятся к Unity, C# и геймдеву, с путями, назначением и ключевыми
правилами. `AGENTS.md` ссылается на этот файл и задаёт матрицу загрузки.

Обозначения:

- **Dedicated** — skills из набора `unity-csharp` в `~/.config/opencode/skills/`.
- **Official** — установлен из Unity CLI (`unity skill install`).
- **Game** — engine-neutral геймдев-навыки из `~/.claude/skills/`.
- **Process** — процессные навыки из `~/.claude/skills/`.

## 1. Выделенные Unity/C# skills (обязательный минимум)

| Skill | Путь | Назначение | Ключевые правила |
|---|---|---|---|
| `unity-development` | `~/.config/opencode/skills/unity-development/SKILL.md` | Версия Unity/C#, архитектура, lifecycle, сериализация, сборка, безопасная работа с проектом | Сначала читай `ProjectVersion.txt`, `manifest.json`, lock-файл, asmdef и настройки. Не путай десктопный .NET с компилятором Unity. Создавай MonoBehaviour только через Unity API. Следи за `Awake`/`OnEnable`/`FixedUpdate`/`LateUpdate`. Сохраняй `.meta` и GUID. Не редактируй сгенерированные `.csproj`. Editor-код — в Editor assembly. Проверяй через Editor/CLI, а не только `dotnet build`. |
| `csharp-best-practices` | `~/.config/opencode/skills/csharp-best-practices/SKILL.md` | Корректность C#, ownership, null, коллекции, async, cancellation | Соблюдай границы компилятора Unity. Модель инварианты явно. Выбирай класс/структуру осознанно. Для `UnityEngine.Object` используй Unity equality. Ordinal для идентификаторов. Не тащи LINQ в hot path. Не используй `.Result`/`.Wait()`. Владей `CancellationToken`. |
| `unity-scriptdesign` | `~/.config/opencode/skills/unity-scriptdesign/SKILL.md` | Дизайн-ревью скриптов: ответственность, связи, lifecycle, Inspector, тестируемость | Один скрипт — одна работа. Явные зависимости вместо глобалов. Для каждого поля определи lifecycle: authoring, composition или runtime. Общайся через прямые ссылки, интерфейсы или события по владению. Кэшируй ссылки, не ищи каждый кадр. Держи serialized-поля приватными и сгруппированными. |
| `unity-editor-tooling` | `~/.config/opencode/skills/unity-editor-tooling/SKILL.md` | Inspector, PropertyDrawer, EditorWindow, UI Toolkit/IMGUI, импортеры, prefab tooling | Используй `SerializedObject`/`SerializedProperty` с `Update`/`ApplyModifiedProperties`. Учитывай multi-object edit и prefab overrides. Записывай `Undo` перед мутацией. Не грузи работу в `OnGUI`/repaint. `OnValidate` — лёгкий. Батчи импорта — `StartAssetEditing`/`StopAssetEditing` в `try/finally`. Проверяй реальный рендер UI, а не только XML. |
| `unity-performance` | `~/.config/opencode/skills/unity-performance/SKILL.md` | Профилирование frame time, GC, памяти, загрузки, сборки | Сначала симптом, железо, сценарий и бюджет. Профилируй representative player/device. Отделяй Editor overhead. Иди за bottleneck: main thread, GC.Alloc, Instantiate/Destroy, rendering CPU, GPU, memory growth. Jobs/Burst/DOTS — только под подходящую нагрузку и после чтения версий пакетов. Доказывай изменение сравнением замеров. |
| `unity-testing` | `~/.config/opencode/skills/unity-testing/SKILL.md` | NUnit, Edit Mode/Play Mode, regression, CI, XML/логи | Сначала изучи версию Test Framework, существующие тесты и asmdef. Edit Mode — синхронная логика, Play Mode — player loop/lifecycle/physics/scenes. Проверяй внешнее поведение, а не реализацию. Чисти ресурсы в `SetUp`/`TearDown`. Никаких wall-clock sleeps. Проверяй свежий XML и логи. |
| `csharp-refactoring` | `~/.config/opencode/skills/csharp-refactoring/SKILL.md` | Безопасный рефакторинг C# без изменения поведения | Сначала классифицируй запрос: refactor, feature, bug fix, upgrade, contract change. Не смешивай refactor и behavior change. Ищи привязки, а не текст. Учитывай partial/generated/conditional. Сохраняй serialization/reflection/DI-имена. Для Unity — `.meta` GUID, UnityEvent, prefab/scene references. Проверяй компиляцией и тестами. |
| `test-anti-patterns` | `~/.config/opencode/skills/test-anti-patterns/SKILL.md` | Аудит тестов: пустые проверки, flaky, изоляция, дубли | Проверяй, что тесты реально ловят падение. Ищи tautological assertions, проглоченные исключения, зависимость от порядка, magic values. Для Unity применяй NUnit/UnityTest-семантику. |

Источник набора: `~/.config/opencode/skill-packs/unity-csharp/manifest.json`.
`unity-development` здесь намеренно переопределяет старый Claude-skill; удаление
opencode-версии вернёт старую.

## 2. Официальный Unity CLI skill

| Skill | Путь | Назначение | Ключевые правила |
|---|---|---|---|
| `unity-cli` | `~/.claude/skills/unity-cli/SKILL.md` | Работа с Unity CLI: управление Editor, сценами, GameObject, ассетами, сборкой, тестами, установками Editor | Если Editor открыт — управляй им live через `unity command`, а не правь YAML вручную. Проверяй `unity status` (state `ready`). Команды и схемы — `unity command` / `unity list`. При Safe Mode (`unity pipeline list`) исправь ошибки компиляции и перезапусти Editor. Не запускай второй Editor на том же проекте. |

Референсы: `~/.claude/skills/unity-cli/references/` — auth/license, build/run/test,
editors/install, projects/templates, integration-advanced, config/hub, diagnostics.

## 3. Game-навыки, применимые к Unity

| Skill | Путь | Когда использовать |
|---|---|---|
| `gamedev-router` | `~/.claude/skills/gamedev-router/SKILL.md` | Первый шаг для любой игровой задачи: определить движок и нужный профильный skill |
| `game-feel` | `~/.claude/skills/game-feel/SKILL.md` | Сочность действий: shake, hit-stop, tween/easing, squash&stretch, knockback |
| `game-juice` | `~/.claude/skills/game-juice/SKILL.md` | Ощущение игры: screenshake, particles, timing звука, слои feedback |
| `game-ui-ux` | `~/.claude/skills/game-ui-ux/SKILL.md` | HUD, меню, overlays, anchor-вёрстка, scaling, safe area, focus navigation |
| `input-systems` | `~/.claude/skills/input-systems/SKILL.md` | Action mapping, rebinding, gamepad, deadzone, input buffering, coyote time |
| `camera-systems` | `~/.claude/skills/camera-systems/SKILL.md` | 2D/3D follow, deadzone, look-ahead, smoothing, bounds, orbit, shake |
| `level-design` | `~/.claude/skills/level-design/SKILL.md` | Blockout/whitebox, метрики, pacing, gating, encounter design |
| `save-systems` | `~/.claude/skills/save-systems/SKILL.md` | Что сериализовать, формат, слоты, atomic writes, версии и миграции, autosave |
| `physics-tuning` | `~/.claude/skills/physics-tuning/SKILL.md` | Fixed vs variable timestep, интерполяция, mass/gravity/drag, CCD, jitter, collision layers |
| `performance-optimization` | `~/.claude/skills/performance-optimization/SKILL.md` | Общая методика: измерить, найти bottleneck, pooling, batching, аллокации, GC |
| `create-game-assets` | `~/.claude/skills/create-game-assets/SKILL.md` | Арт-дирекшн, style bible, спрайты, тайлсеты, фоны, UI-арт, 3D-брифы |
| `audio-design` | `~/.claude/skills/audio-design/SKILL.md` | Шины/микшер, decibels, ducking, adaptive music, вариации SFX, beat sync |
| `brainstorming` | `~/.claude/skills/brainstorming/SKILL.md` | До любой творческой работы: требования, intent, дизайн |
| `writing-plans` | `~/.claude/skills/writing-plans/SKILL.md` | Письменный план для многошаговой задачи до кода |
| `executing-plans` | `~/.claude/skills/executing-plans/SKILL.md` | Выполнение плана с чекпойнтами |
| `test-driven-development` | `~/.claude/skills/test-driven-development/SKILL.md` | Реализация фичи/багфикса через тесты |
| `systematic-debugging` | `~/.claude/skills/systematic-debugging/SKILL.md` | Любой баг/падение/неожиданное поведение до фикса |
| `verification-before-completion` | `~/.claude/skills/verification-before-completion/SKILL.md` | Перед заявлением «готово»: свежие доказательства |
| `requesting-code-review` | `~/.claude/skills/requesting-code-review/SKILL.md` | Перед merge/завершением крупной работы |
| `receiving-code-review` | `~/.claude/skills/receiving-code-review/SKILL.md` | При получении ревью |
| `dispatching-parallel-agents` | `~/.claude/skills/dispatching-parallel-agents/SKILL.md` | 2+ независимые задачи |
| `subagent-driven-development` | `~/.claude/skills/subagent-driven-development/SKILL.md` | План с независимыми задачами в текущей сессии |
| `using-git-worktrees` | `~/.claude/skills/using-git-worktrees/SKILL.md` | Изоляция работы до выполнения плана |
| `finishing-a-development-branch` | `~/.claude/skills/finishing-a-development-branch/SKILL.md` | Merge/PR/cleanup после завершения |

## 4. Не использовать в Unity

Это skills веб-движков и веб-платформ; применять только если пользователь явно
просит веб-прототип:

`phaser-core`, `phaser-arcade-physics`, `events-system`, `scenes`, `loading-assets`,
`game-setup-and-config`, `scale-and-responsive`, `audio-and-sound`,
`v3-to-v4-migration`, `v4-new-features`, `pixijs-rendering`, `threejs-*`,
`yandex-games-dev`, `web-build-universal`, `web-ship-it`.

`aspnet-core` тоже про C#, но это веб-стек ASP.NET Core, а не Unity. Не смешивать.

## 5. Конфликты и приоритеты

- `unity-development` существует в двух местах. Для opencode приоритет у
  `~/.config/opencode/skills/unity-development/SKILL.md` (override). Старая версия
  `~/.claude/skills/unity-development/SKILL.md` упоминает CoplayDev `@unityMCP` и
  устаревшие имена инструментов — не следовать ей в этом проекте.
- MCP в этом проекте не используется: Editor управляется через Unity CLI (`unity command`),
  а не через CoplayDev `@unityMCP`.
- Для Editor-команд источник истины — `unity command --format json`, а не память о
  названиях инструментов. Схему каждой незнакомой команды читай перед вызовом.
- Если skill недоступен — фиксируй это в ответе и следуй правилам `AGENTS.md`.
