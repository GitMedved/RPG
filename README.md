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
- [M10 Tutorial & Arcade Feedback](M10_TUTORIAL_AND_ARCADE_FEEDBACK.md)
- [M11 Android & iOS Build Readiness](M11_MOBILE_BUILD_READINESS.md)
- [M12 Garden & Workshop Upgrades](M12_GARDEN_WORKSHOP_UPGRADES.md)
- [M13 Flower Garden Annex](M13_FLOWER_GARDEN_ANNEX.md)
- [M14 Mobile HUD & Action Dock](M14_MOBILE_HUD_ACTION_DOCK.md)
- [M15 Visual Production Pipeline](M15_VISUAL_PRODUCTION_PIPELINE.md)
- [M16 Character Rigs & Animation](M16_CHARACTER_RIGS_AND_ANIMATION.md)
- [M17 Audio & Voice](M17_AUDIO_AND_VOICE.md)
- [M18 Integration & Mobile Hardening](M18_INTEGRATION_AND_MOBILE_HARDENING.md)
- [M19 Backyard Visual Polish](M19_BACKYARD_VISUAL_POLISH.md)
- [M20 Production Asset Audit](M20_PRODUCTION_ASSET_AUDIT.md)
- [M21 Player Settings & Accessibility](M21_PLAYER_SETTINGS_ACCESSIBILITY.md)
- [M22 Daily Goals & Rewards](M22_DAILY_GOALS_REWARDS.md)
- [M34 Canvas Mobile HUD](M34_CANVAS_MOBILE_HUD.md)

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
- No production 3D models, authored rigs, animation clips, final UI art or platform builds are included yet. Reference images are stored under `Assets/_Project/Art/`.
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


## M10 — Tutorial and arcade feedback

The prototype now includes a persistent six-step tutorial tied to successful gameplay actions. Match-3 reshuffles when no legal swaps remain, fishing has a player-controlled aim, and Max Runner has timed lane obstacles and lane-specific coins. These are still prototype systems and require a Unity compile/Play Mode pass. See [M10 Tutorial & Arcade Feedback](M10_TUTORIAL_AND_ARCADE_FEEDBACK.md).

## M11 — Mobile build readiness

Android/iOS module, signing, build and physical-device validation steps are documented in [M11 Mobile Build Readiness](M11_MOBILE_BUILD_READINESS.md). No mobile build has been produced or certified yet.


## M12 — Garden & workshop upgrades

The Adventure Journal now includes two persistent upgrade tracks. Garden upgrades increase the number of crops received per harvest; workshop upgrades reduce the duration of newly started production jobs. Upgrade levels are saved locally, capped at level 2, and purchased with earned coins. Existing production jobs keep the duration recorded when they were started. See [M12 Garden & Workshop Upgrades](M12_GARDEN_WORKSHOP_UPGRADES.md). The feature still requires Unity Play Mode and save/reload validation.


## M13 — Flower Garden Annex

The starter story quest now leads to a tangible gated garden annex in the backyard prototype. Once the final starter quest reward is claimed, the garden becomes visible and accessible through an entrance gate. Three one-time flower resources can be collected for inventory items, XP and coins; discoveries persist in the local save. See [M13 Flower Garden Annex](M13_FLOWER_GARDEN_ANNEX.md). This package requires Unity compilation and Play Mode acceptance testing.


## M15–M18 — Production integration foundations

- M15 locks the supplied reference-led art direction and defines source/runtime asset intake, model, environment and UI acceptance requirements. Original reference binaries and production 3D assets still need to be imported.
- M16 adds an optional Animator bridge with a documented parameter contract and interaction trigger. Authored rigs and animation clips remain external deliverables.
- M17 adds an automatically-created, silent-by-default audio service with separate effects/music/voice channels. Licensed sound effects, music and English voice recordings remain to be supplied and assigned.
- M18 adds conservative mobile runtime pacing defaults and a device-hardening checklist. Android/iOS builds and real-device performance remain unverified.

See the four milestone documents for setup and acceptance gates. M14 was reported by the user as OK; M15–M18 source changes have not yet been compiled in Unity in this environment.


### M19 — Backyard visual polish
Added a low-cost procedural dressing pass (stepping stones, layered tree foliage, perimeter flower clusters and small architectural details). This remains temporary greybox art pending production models and must be visually checked in Unity.

### M20 — Production asset audit
Added a Unity Editor menu command to report reference, model, Animator Controller, UI sprite and audio asset readiness. Missing assets are reported as pending rather than compile failures.

## M21 — Player settings & accessibility

Added a persistent Settings panel for independent audio levels, UI scale, vibration preference and reduced-motion preference. Settings are saved locally and restored on startup. This is still prototype UI; haptics/reduced-motion hooks and real audio clips need integration and device verification. See [M21 Player Settings & Accessibility](M21_PLAYER_SETTINGS_ACCESSIBILITY.md).


## M22 — Daily goals & rewards

Three UTC-based daily goals track successful harvesting, collecting a finished craft and delivering an order. Completing all three grants 20 coins and 15 XP once per day. Progress and claim state persist locally and appear in the Adventure Journal. This is a fixed offline prototype, not a server-backed live-ops system. See [M22 Daily Goals & Rewards](M22_DAILY_GOALS_REWARDS.md).


## M23 — Lifetime achievements

Added seven persistent achievements tied to harvesting, crafting, deliveries, keepsakes, flower-garden resources, mini-game rewards and player level. Each achievement grants a one-time coin/XP reward claimable from the Adventure Journal. See `M23_ACHIEVEMENTS_PROGRESS.md`. Unity compile and runtime acceptance remain pending.


## M24 — Woodland Trail expansion

A fourth story quest now unlocks a new same-scene Woodland Trail annex after the three Flower Garden resources are collected. Explore and collect an acorn, mushroom and wild berries for persistent inventory items and small XP/coin rewards. See `M24_WOODLAND_TRAIL_EXPANSION.md`. The annex is still built from prototype geometry; final art and Unity runtime validation remain pending.


## M25 — Exploration achievement

Added the `Trailblazer` lifetime achievement for finding all three Woodland Trail resources (30 coins + 25 XP, one-time claim). The Adventure Journal now lists all eight achievements in its scrollable panel. See `M25_EXPLORATION_ACHIEVEMENT.md`.


## M26 — Exploration and inventory feedback

The Adventure Journal now shows Woodland Trail unlock/discovery progress, and the backpack uses friendly names for flower and woodland resources. See `M26_EXPLORATION_INVENTORY_FEEDBACK.md`.


## M27 — Interaction feedback

Added a short contextual toast when Mia interacts with a world object. The message is visible independently of the Adventure Journal and Backpack, uses unscaled time, and leaves gameplay/save state unchanged. This remains prototype IMGUI feedback; Unity runtime and device checks are required. See `M27_INTERACTION_FEEDBACK.md`.


## M28 — Accessibility feedback integration

Connected the saved vibration preference to world interaction feedback on supported mobile platforms. The reduced-motion preference now suppresses one-shot Interact/Celebrate Animator triggers while leaving locomotion parameters intact. Reduced-motion coverage is partial; camera, UI and mini-game effects are not yet adapted. See `M28_ACCESSIBILITY_FEEDBACK.md`.


## M29 — Daily exploration goal

Expanded daily goals to include one new resource discovery in the Flower Garden or Woodland Trail. Progress is saved with the existing daily task data; old saves default to 0/1. The reward remains 20 coins + 15 XP after all four goals. See `M29_DAILY_EXPLORATION_GOAL.md`.


## M30 — Daily visit streak and gifts

Added a persistent UTC visit streak with a repeating seven-day coin/XP gift track, one claim per UTC day and a compact claim control in the HUD. Current and best streaks persist; missed days reset only the current streak. This is offline/client-clock based, with no server validation or notifications. See `M30_DAILY_VISIT_STREAK.md`.


## M31 — Daily arcade reward cap

Added a shared limit of five successful mini-game reward claims per UTC day across Match-3, Fishing and Max Runner. The count persists with daily task data, resets at the existing daily boundary and is shown in the Arcade menu. Existing per-run replay protection remains in place. See `M31_DAILY_ARCADE_REWARD_CAP.md`.


## M32 — World Map overview

Added a compact World Map HUD panel showing the Backyard, Flower Garden and Woodland Trail, their unlock states and resource discovery counts. Locked areas explain the story requirement. Journal, Settings, Backpack and Map panels now close one another to prevent overlap. This is a text-first progression overview, not a minimap or fast-travel system. See `M32_WORLD_MAP_OVERVIEW.md`.


## M33 — Responsive Arcade HUD

The Arcade panel now scales from the device safe area and offsets for safe-area insets, while retaining the existing mini-game layout and rules. This is still an IMGUI prototype; touch targets and aspect-ratio behaviour require Unity/device validation. See `M33_RESPONSIVE_ARCADE_HUD.md`.


## M34 — Canvas mobile HUD foundation

Started the transition from IMGUI to Unity uGUI. A safe-area-aware Screen Space Overlay Canvas now owns the main status card, daily gift action, Journal/Settings/Map shortcuts and bottom Backpack/Interact dock. Existing game services and panel state are preserved; the detailed Backpack, Journal, Map and Settings contents remain on the legacy IMGUI renderers for this first migration slice. The UI is still assembled from temporary panels and built-in font assets. Unity Editor compile, Play Mode, aspect-ratio and device checks are required. See [M34 Canvas Mobile HUD](M34_CANVAS_MOBILE_HUD.md).
