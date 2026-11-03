# Mia & Max Adventures

**Little Moments, Big Adventures!**

Cross-platform 3D family adventure game for Android and iOS, built with Unity and C#.

## Project status

Pre-production Unity prototype. The repository began as a README-only project; the current feature branch now contains the Unity scaffold, a generated-at-runtime backyard greybox, mobile input and interaction prototypes, plus temporary stylized character proxies.

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

Unity baseline currently recorded as 6000.3.0f1 with URP package 17.3.0. Confirm the exact Editor patch and resolve packages locally before treating this as a validated project. Do not commit generated `Library/`, `Temp/`, `Logs/` or `Obj/` folders.

## Current limitations

Production character models, authored rigs, animation clips, final environment art, audio and committed scene assets are not present yet. Runtime procedural character and environment proxies are temporary greybox art only.

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


## M2 — Character & World Prototype

The runtime greybox now includes temporary stylized Mia and Max proxies with reference-led visual cues (Mia's blonde braids, white top, blue jeans and green eyes; Max's ginger coat, cream muzzle/belly and green eyes), simple movement-driven limb motion, Max's swaying tail, and a more detailed backyard facade/patio/garden pass.

- New implementation: `StylizedCharacterPrototype.cs`, `BackyardDetailPrototype.cs`.
- These are procedural placeholder meshes, not final 3D models or production animation rigs.
- See [M2 Character & World Prototype](M2_CHARACTER_WORLD_PROTOTYPE.md) for scope and validation checklist.
- Unity compilation, visual review, Play Mode and Android/iOS device testing remain unverified in this environment.


## Core data and local persistence

The first offline player-state foundation is now present:
- GameSession validates coin, item-stack and XP operations.
- PlayerSaveData uses stable item IDs and schema version 1.
- LocalSaveService stores JSON under Unity's persistent data path, keeps a backup and validates loaded values.
- GameSessionBootstrap loads once, autosaves after state changes and saves on app pause/quit.

This is an infrastructure milestone only: there is not yet a production inventory UI, crop/recipe/order content, migration chain beyond schema v1, or automated test suite. Unity compilation and device lifecycle behaviour still require local Editor/device validation.


## M3 — Farming loop prototype

Three interactive backyard plots are now connected to persistent farm data. New profiles receive starter carrot, tomato and strawberry seeds; planting, timed growth and harvesting update inventory, XP and coins. Crop visuals remain temporary placeholders. See [M3 Farming Loop Prototype](M3_FARMING_LOOP_PROTOTYPE.md). Unity compilation and device testing are still pending.

## M4 — Production and orders prototype

The backyard now includes an interactive workshop and order board. Three persistent recipes (carrot soup, strawberry juice, wooden toy) use real-time production jobs; three one-time orders consume crafted products and reward coins/XP. See [M4 Production & Orders Prototype](M4_PRODUCTION_ORDERS_PROTOTYPE.md). Recipe selection and order presentation are still temporary interaction-only flows. Unity compilation and device testing remain pending.

## M5 — Story quests & world progression

A first story progression layer is now connected to the backyard prototype. The adventure journal offers three one-time quests tied to player level and the three starter orders. Claiming the final quest unlocks the persistent `flower_garden` area flag for a future location. Quest claims and unlocked areas are saved locally. See [M5 Quests & World Progression](M5_QUESTS_WORLD_PROGRESSION.md). The quest journal is still a prototype interaction, not a finished UI, and Unity compilation/device validation remains pending.


## M6 — Mini-game foundations

Pure C# rules prototypes are available for Match-3, fishing timing and Max Runner. Mini-game rewards use unique run IDs, bounded reward bands and persisted replay-claim records. See [M6 Mini-game Foundations](M6_MINIGAME_FOUNDATIONS.md).

## M7 — Arcade integration prototype

The generated backyard now contains an arcade board that opens a temporary interface for all three mini-games. Results flow through the shared reward service into the player profile. See [M7 Arcade Integration Prototype](M7_ARCADE_INTEGRATION_PROTOTYPE.md). This is an interaction prototype, not final mobile UI; Unity compilation and device validation are still pending.


## M8 — Backyard keepsake collection

Five one-time keepsakes can now be discovered around the backyard and are persisted in the local save. The collection book tracks progress and awards 25 coins + 30 XP once all five are found. See [M8 Backyard Keepsake Collection](M8_BACKYARD_COLLECTION.md). The objects and interaction feedback remain prototype placeholders; Unity compilation, save/reload and device testing are still pending.


## M9 — Prototype HUD and inventory viewer

A temporary safe-area-aware HUD now displays level, XP and coins, with an expandable backpack list. This is a functional prototype viewer only: item use, sorting, icons, pagination and final UI styling are not implemented. Unity compile and device layout validation remain pending.
