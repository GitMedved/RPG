# Mia & Max Adventures

**Little Moments, Big Adventures!**

Cross-platform 3D family adventure game for Android and iOS, built with Unity and C#.

## Project status

Pre-production foundation. The repository initially contained only this README; no existing Unity project, assets, scenes or build configuration were available during the first audit.

## Approved direction

- Portrait-first, single-player, offline-capable.
- Stylized 3D, family-friendly, English-first.
- Cozy backyard development + gentle exploration RPG + arcade mini-games.
- Mia and Max's supplied visual references are the source of truth.
- No combat, multiplayer, forced ads, gambling or paid randomized rewards in MVP.

## Documentation

- [Game Design Document](GAME_DESIGN_DOCUMENT.md)
- [Technical Architecture](TECHNICAL_ARCHITECTURE.md)
- [Game Economy](GAME_ECONOMY.md)
- [MVP Roadmap](MVP_ROADMAP.md)
- [Asset Requirements](ASSET_REQUIREMENTS.md)
- [Visual Reference Specification](VISUAL_REFERENCE_SPECIFICATION.md)
- [Codex Development Log](CODEX_DEVELOPMENT_LOG.md)

## Unity project scaffold

The Unity Editor version and package manifest have deliberately not been fabricated. Open the project in the agreed Unity 6 LTS Editor and let Unity generate valid `ProjectSettings/` and `Packages/` files before pinning the exact editor/package versions. Do not commit generated `Library/`, `Temp/`, `Logs/` or `Obj/` folders.

## Current limitations

No character models, rigs, animation clips, environments, audio, scenes or gameplay code are present yet. Any greybox assets created later must be clearly labelled as placeholders until approved Mia/Max references are imported.

## Development workflow

Work on feature branches, keep milestones small, run EditMode/PlayMode tests and validate Android/iOS builds separately. Never claim a platform build is ready without a successful build and device smoke test.


## M1 backyard prototype

The branch now includes a minimal Unity scaffold and runtime greybox prototype. Open with Unity 6000.3.x (baseline recorded as 6000.3.0f1), let the Editor import scripts, then use **Mia & Max → Setup Backyard Prototype Scene** if the scene is not created automatically. Open `Assets/_Project/Scenes/Backyard.unity` and press Play.

- Editor controls: WASD / arrow keys.
- Character and environment meshes are temporary primitive placeholders.
- No URP configuration, original reference image binaries, final art, animations or platform builds are included yet.
- Unity compile and device builds must be verified in the Editor; they have not been run in this environment.

## M1.2 — mobile interaction foundation

- URP 17.3.0 package dependency is pinned; create and assign the URP Pipeline Asset/Renderer Data in Unity Editor using [URP_SETUP.md](Assets/_Project/URP_SETUP.md).
- A temporary touch joystick is connected to Mia's movement controller.
- Nearby interactions are available at the tree, swing and sandbox (press E in Editor; touch action is a prototype).
- These systems remain greybox quality and have not been compiled or tested in Unity in this environment.
