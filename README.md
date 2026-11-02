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
