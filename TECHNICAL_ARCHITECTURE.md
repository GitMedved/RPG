# Mia & Max — MVP Technical Architecture

**Status:** Implemented in Git; Unity Editor and device verification still required.

## Active startup

- Unity 6000.3.0f1, URP 17.3.0, Unity uGUI available.
- Saved startup scene: `Assets/_Project/Scenes/Backyard.unity` (intentionally contains no root GameObjects).
- `TapAdventureWorldBootstrap` owns the temporary 3D world, fixed angled camera, tap hit-testing, context HUD and first-story actions.
- `MobileRuntimeBootstrap` configures mobile timing.
- `GameAudioService` provides optional imported audio with procedural feedback fallbacks.
- The old `BackyardPrototypeBootstrap` is a compatibility stub only. No joystick, movement controller or follow camera starts.
- The old `GameSessionBootstrap` no longer auto-starts, to avoid an unrelated inventory and duplicate save writes.

## MVP state and persistence

One small local `PlayerPrefs` namespace (`MiaMax.TapAdventure.*`) stores:
- twigs, feathers, flowers and stars;
- first-time resource collection flags;
- birdhouse status and Secret Garden unlock;
- a UTC flower-completion timestamp;
- single-claim mini-game and stone rewards.

All state changes call `SaveProgress()`. The MVP's reset command deletes *only* these keys. There is currently no cloud save, synchronization, purchase flow, or migration to the older domain `GameSession` system.

## Interactions

The world builder creates the yard, house, tree, garden, crafting station, pond and gate. Visible action targets carry a `TapAdventureTarget` component and collider. A camera raycast selects objects after excluding HUD rectangles. The user chooses an action with the context panel. The gate changes between two fixed camera compositions; characters are not directly controlled.

## Asset fallback and visuals

The current 3D shapes remain temporary primitives, rendered with real committed URP materials. The original Mia/Max references are included as HUD portraits; they are not rigged animated 3D characters. Optional production SFX and music may be absent; short non-licensed, procedural interaction tones provide basic feedback.

The saved scene contains no produced character prefabs, environment meshes or animation clips. Development tools and legacy domain implementations are retained only as potential future source material.

## Validation

Use **Mia & Max → MVP → 1. Preflight Assets & Scene** before Play and **Mia & Max → MVP → 2. Inspect Running World** during Play. These checks are *not* a substitute for compiling in Unity, manually completing the chapter or testing builds on physical Android and iOS devices.

For the working smoke-test path see `README.md`.
