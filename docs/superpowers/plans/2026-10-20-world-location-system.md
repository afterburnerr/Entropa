# World Location System

## Goal

Streamed additively-loaded locations under a persistent bootstrap scene, with preloaded door transitions, per-location DI scopes, and persisted world state. Extensible: a new location is one scene plus one `LocationDefinition` asset.

## Non-goals (this iteration)

- Addressables/remote content, save files, seamless visual portals, moving elevator cabins.
- Interior streaming cells (design supports it; demo locations fit in one scene each).

## Architecture

- `Bootstrap` scene stays loaded forever: `GameScope` (root VContainer), player, camera rig.
- Locations are additive scenes authored in shared world coordinates.
- `SceneLoader` (singleton in root scope) owns `LoadSceneAsync`/activation/unload state per location.
- Door zones: `TransitionPreloader` trigger calls `PreloadAsync` on approach; `SlidingDoor` opens on `LocationReady`; `TransitionExecutor` trigger calls `TransitionAsync`.
- `TransitionService` sequence: ensure preloaded -> activate -> locate `TransitionPoint` in target scene -> move `IPlayerAnchor` -> set `WorldState.CurrentLocation` -> unload previous -> notify.
- `LocationLifetimeScope` (child of root scope via `FindParent`) registers location-local components.
- `WorldStartup` (VContainer entry point) performs the initial transition so gameplay code never sees an empty world.
- World state (phases, flags, cleared enemies) lives in `WorldState`; locations reconcile themselves on load in later iterations.

## Assemblies

`Assets/Game/World/Entropa.World.asmdef` (`Entropa.World`), refs: `Entropa.Core`, `UniTask`, `VContainer`.
`Entropa.Gameplay` gains `Entropa.World`; `Entropa.Bootstrap` gains `Entropa.World`; `Entropa.Tests.EditMode` gains `Entropa.World`, `UniTask`.

## Files

World (`Assets/Game/World/`):

- `LocationDefinition.cs` — SO: `_locationId`, `_displayName`, `_sceneName`, `_musicId`; public getters.
- `LocationRoot.cs` — scene marker holding `LocationDefinition`.
- `TransitionPoint.cs` — spawn point with `_pointId`.
- `IPlayerAnchor.cs` — `Transform Root { get; }`.
- `ISceneLoader.cs` — `PreloadAsync(LocationDefinition)`, `ActivateAsync`, `UnloadAsync(LocationDefinition)`, `IsActivated`, `TryGetLoaded`.
- `SceneLoader.cs` — implements; `LoadSceneAsync(..., allowSceneActivation: false)`, waits progress 0.9, `ToUniTask`; `LocationReady`/`LocationActivated`/`LocationUnloaded` events; disposes pending operations.
- `IWorldState.cs` / `WorldState.cs` — `CurrentLocation`, `SetLocation`, `IsFlagSet`/`SetFlag`, `LocationChanged` event.
- `ITransitionPointLocator.cs` / `SceneTransitionPointLocator.cs` — scans target scene root objects.
- `ITransitionService.cs` / `TransitionService.cs` — concurrency guard, transition sequence, `TransitionStarted`/`TransitionCompleted`.
- `TransitionPreloader.cs` — trigger, fires preload once per location, `Ready` event.
- `TransitionExecutor.cs` — trigger, runs transition, re-arms after exit.
- `SlidingDoor.cs` — UniTask lerp of a panel on `Ready`.
- `LocationLifetimeScope.cs` — child scope, registers location components; skips build with clear error when no root scope.
- `WorldRoot.cs` — static root scope holder for scene-to-scene parent resolution.
- `WorldStartupSettings.cs` — serializable startup location + point id.
- `WorldStartup.cs` — `IStartable`, fires first transition.

Gameplay (`Assets/Game/Gameplay/PlayerAnchorBehaviour.cs`): MonoBehaviour implementing `IPlayerAnchor`.

Bootstrap (`Assets/Game/Bootstrap/GameLifetimeScope.cs`): adds `_startupLocation`, `_startupPointId`; registers world services and `PlayerAnchorBehaviour`; sets `WorldRoot.RootScope`; registers `WorldStartup`.

## Scenes

- `Assets/Scenes/Bootstrap.unity` — `GameScope` (GameLifetimeScope), `Gameplay` root with `PlayerCharacter` instance (+`PlayerAnchorBehaviour` scene instance override), `Cameras` root with `CameraRig` instance.
- `Assets/Scenes/Playground.unity` — becomes `Location_Playground`: `LocationRoot` + `LocationLifetimeScope`; spawn `Default` (0,0,-12); gate on east wall at x≈18, z=-12 (pillars, lintel, wall segments, sliding panel); `Preload_ToTower` zone x∈[16,18.2]; `Exit_ToTower` executor at x≈18.35; `FromTower` point (16.8,0,-12) facing west; `SlidingDoor` wired to preloader.
- `Assets/Scenes/TwoTowers.unity` (new) — `Location_TwoTowers`: 12x12 room at (60,0,0), doorway at west wall x=54, `FromPlaza` point (55.8,0,0) facing east; `Preload_ToPlaza` x∈[54.6,55.3]; `Exit_ToPlaza` executor x≈54.05; own directional light + floor/wall materials; `SlidingDoor` wired.
- `Assets/Config/Locations/Location_Playground.asset`, `Location_TwoTowers.asset`.
- Build settings: Bootstrap first, then locations.

## Tests (EditMode)

- `WorldStateTests` — defaults, set/flag, event.
- `TransitionServiceTests` — fakes for loader/anchor/locator; transition sets state, moves anchor, unloads previous (plain C# service, no Unity loop).
- `TransitionPointLocatorTests` — finds point in active scene, missing id returns null.

## Verification

- `run_tests --mode EditMode --async_tests true` (all green), PlayMode still green.
- Play `Bootstrap`: screenshot plaza; hold `W` through gate -> tower screenshot; hold `A` west through tower door -> plaza screenshot; console error-free; player position follows spawns.

## Order

1. Plan commit.
2. World assembly + code.
3. Gameplay anchor + GameLifetimeScope + compile.
4. Tests, run EditMode.
5. Location assets + scenes + build settings via editor script.
6. Smoke play + screenshots + console.
7. Commit/push (only this feature's files).
