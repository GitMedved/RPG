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

## Current art / UX architecture

- `TapAdventureWorldBootstrap` creates the authoritative hidden gameplay collider blockout, camera, save state and adventure actions.
- `TapAdventureArtWorld` disables the primitive renderers and builds an independent visible stage with sculpted rounded meshes, low-poly organic meshes, UVs/normals, shared URP materials, house/roof/window details, garden beds, an apple tree, water garden, fence and secret-garden assets. Its lightweight touch colliders preserve the gameplay interactions.
- `TapAdventureCharacterRig` draws Mia and Max as animated stylized 3D **development proxies**. Breathing/idle sway, gestures, eyes, tail and approximate voice-timed talking are implemented; final models matching approved original references, skinning and animation clips remain future art work.
- `TapAdventureHud` uses a scalable uGUI Canvas, nine-sliced rounded interface panels, adaptive safe-area layouts, original cropped character portraits, chapter hints, inventory counters, contextual actions, a voice toggle and the treasure sorting modal. IMGUI remains a fallback.
- `GameAudioService` produces small feedback tones when production WAVs are absent; real English dialogue takes were generated via HeyGen, with their optional explicit Editor importer in `MiaMaxVoiceTakesImporter`. The download is only for local asset installation; the game itself is offline.
- The original 40 URP surface materials remain reusable. The previous character movement/camera setup is not launched.

The C# procedural meshes are not production FBX assets and should not be described as final reference-matched characters. The voice binaries have not yet been installed into the Git repository.

## Validation

Use **Mia & Max → MVP → 1. Preflight Assets & Scene** before Play and **Mia & Max → MVP → 2. Inspect Running World** during Play. These checks are *not* a substitute for compiling in Unity, manually completing the chapter or testing builds on physical Android and iOS devices.

For the working smoke-test path see `README.md`.
