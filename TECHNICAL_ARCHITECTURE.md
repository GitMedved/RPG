# Technical Architecture

**Status:** Proposed architecture; validate in the selected Unity Editor before implementation.  
**Baseline recommendation:** Unity 6.3 LTS + URP, C#, portrait mobile. Unity identifies 6.3 as its current LTS line with support through December 2027; exact editor patch and package versions must be pinned after installing the Editor. [Unity release page](https://unity.com/releases/unity-6).

## 1. Architecture principles
- Modular monolith: clear boundaries, no microservices or multiplayer abstractions.
- Data-driven content, code-driven rules.
- Composition root owns service construction; avoid global service locators.
- Interfaces only at real seams (save storage, clock, platform features, mini-game lifecycle).
- Events communicate completed domain actions; UI does not mutate economy state.
- Stable IDs and versioned DTOs for persistence.
- Offline-first; network is never required for core play.

## 2. Proposed repository layout
```
Assets/
  _Project/
    Art/{Characters,Environment,Props,Materials,Textures}
    Audio/{Music,SFX,Voice}
    Prefabs/{Characters,World,UI,Gameplay}
    Scenes/{Bootstrap,MainMenu,Backyard,MagicForest,MiniGames}
    ScriptableObjects/{Crops,Recipes,Quests,Items,Locations,Rewards}
    Scripts/
      Core/{Bootstrap,Events,Identifiers}
      Domain/{Economy,Inventory,Progression,Quests}
      Gameplay/{Farming,Production,Orders,Decoration}
      Characters/{Player,Companion}
      World/{Interaction,Camera,Unlocks}
      MiniGames/{Shared,Match3,Fishing,MaxRun}
      Adventure/{Map,Exploration}
      Collections/
      UI/{Navigation,Hud,Views}
      Infrastructure/{Save,Audio,Platform}
      Editor/
    Tests/{EditMode,PlayMode}
    Settings/
Packages/manifest.json
ProjectSettings/
Documentation/
  GAME_DESIGN_DOCUMENT.md
  TECHNICAL_ARCHITECTURE.md
  GAME_ECONOMY.md
  MVP_ROADMAP.md
  ASSET_REQUIREMENTS.md
  CODEX_DEVELOPMENT_LOG.md
```

## 3. Runtime layers
**Presentation:** MonoBehaviours, UI Toolkit or uGUI (choose one after a vertical-slice UX spike), animation and scene adapters.  
**Application:** use cases such as PlantCrop, HarvestCrop, FulfilOrder, ClaimQuestReward and CompleteMiniGame.  
**Domain:** plain C# rules and models for wallet, inventory, progression, crop state, recipes, quests and rewards.  
**Infrastructure:** JSON/file persistence, Unity clock adapter, audio, platform safe-area/haptics adapters.

Unity scene objects should call application use cases. They should not directly add coins, grant XP or write save data.

## 4. Core service boundaries
- `GameSessionService`: session lifecycle and current player state.
- `EconomyService`: validates currency deltas and transaction reasons; rejects negative/invalid transactions.
- `InventoryService`: item definitions, stack limits, add/remove/consume operations.
- `ProgressionService`: XP, levels and unlock events.
- `FarmingService`: plot state, planting rules, growth timestamps, harvest rewards.
- `ProductionService`: recipe validation, input consumption, production completion.
- `QuestService`: objective progress and claimable rewards.
- `OrderService`: order requirements and fulfilment.
- `MiniGameCoordinator`: lifecycle and result contract; mini-games do not own the wallet.
- `AdventureService`: location and chapter unlock state.
- `CollectionService`: discovery state and completion.
- `SaveService`: versioned snapshot, validation, backup/recovery.
- `AudioService`: music/SFX routing and settings.
- `PlatformServices`: safe area, haptics, app lifecycle; later billing/notifications behind interfaces.

## 5. Key domain contracts
Use immutable or controlled state transitions. Example reward flow:
1. Activity emits a result (activity ID, validated outcome, reward definition ID).
2. Application layer validates eligibility and resolves reward configuration.
3. Economy and inventory services apply the transaction.
4. Progression and quest services receive domain events.
5. Save service marks the snapshot dirty and persists at a safe checkpoint.

Never accept currency values from UI or mini-game scene objects. Never persist Unity object references, scene instance IDs or localized display strings.

## 6. Save design
- Local JSON snapshot with schema version and stable IDs.
- Atomic write: write temp file, flush, replace current; keep one previous known-good backup.
- Validate bounds, duplicate IDs, missing definitions and timestamp ranges on load.
- Migrations are explicit sequential functions (v1→v2→v3); unknown future versions fail safely without overwriting the source.
- Save after meaningful transactions, unlocks, scene transitions and app pause; debounce frequent changes.
- Offline growth uses UTC Unix timestamps and a monotonic sanity policy; clamp implausible clock jumps and document the policy.
- Storage path via Unity persistent data path. No cloud provider in MVP.

## 7. Scene flow
Bootstrap → Main Menu → Backyard (hub) ↔ Mini-game scene / Magic Forest. Return results through a typed session result, not static scene globals. Scene loading should be centralized and show loading/error state.

## 8. Rendering and performance
URP is recommended for mobile 3D. Start with conservative shadows, baked lighting where practical, limited real-time lights, texture compression, atlases only where they help, LOD for larger assets, instancing for repeated vegetation and pooling for transient effects. Profile on representative low/mid-range devices before adding expensive effects. Keep quality tiers configurable.

## 9. Input and UI
One input abstraction for touch and Editor mouse simulation. Portrait safe areas, scalable Canvas/UI layout, gesture thresholds, interaction raycasts and explicit pause/resume handling. Avoid mixing multiple UI frameworks without a demonstrated need.

## 10. Tests
EditMode: economy invariants, inventory operations, crop growth, recipe validation, quest objective progress, reward validation, save serialization/migrations.  
PlayMode: scene boot, planting/harvesting interaction, mini-game lifecycle, save/reload, pause/resume and safe-area layout.  
Build validation: Android and iOS build profiles; actual device smoke tests required before claiming platform readiness.

## 11. Dependency policy
Start with Unity built-in systems and URP. Add packages only when a concrete feature needs them. Pin exact package versions compatible with the selected Editor. No ads, analytics, cloud, billing or account SDKs in MVP.

## 12. Open decisions before implementation
- Confirm available Mia/Max reference images, videos, models and license/ownership.
- Confirm target minimum Android/iOS versions and device performance target.
- Choose uGUI vs UI Toolkit after one-screen prototype.
- Confirm whether Unity 6.3 LTS patch is available in the user's Unity Hub.
- Select input package only if needed; prefer the simplest supported path.
- Establish asset licensing and source-control LFS policy for large binaries.
