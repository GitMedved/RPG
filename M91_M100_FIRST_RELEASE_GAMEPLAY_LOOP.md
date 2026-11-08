# M91–M100 — First-release gameplay loop integration

## Scope

This package connects the authorable backyard catalog to production, order delivery and the opening story progression. It keeps the existing local save schema and built-in content fallback.

## Implemented

- **M91 — Catalog-backed recipes:** recipe IDs, ingredient IDs/quantities, output IDs/quantities, XP and production duration are read from BackyardContentCatalog. Existing static recipes remain the fallback when a catalog entry is absent.
- **M92 — Catalog-backed order board:** starter orders use catalog item requirements and payouts; additional authored order entries are discoverable without adding hard-coded definitions. Order completion consumes the configured stack and grants coins/XP in one observable session transaction.
- **M93 — Story prerequisites:** catalog entries can reference a required StoryGoal ID. Catalog validation reports missing or self-referencing prerequisites.
- **M94 — World-area progression:** the starter quest chain is sequential. Claiming its configured story goal records the area unlock; Flower Garden content remains unavailable until that unlock is saved, and Woodland Trail follows the next story milestone.
- **M95 — Inventory reward operations:** GameSession exposes preflighted item-plus-reward and item-consumption-plus-reward operations, protecting against quantity and integer overflow.
- **M96 — Atomic production lifecycle:** starting a job consumes ingredients and records the job before one state notification; collecting a job grants output/XP and clears the active job before notifying save listeners.
- **M97 — Area-gated content:** catalog recipes and orders assigned to a locked area are not available until QuestService reports that area unlocked.
- **M98 — Compatibility:** built-in recipe/order definitions remain fallback paths; no PlayerSaveData schema migration was introduced.
- **M99 — First-session acceptance scenario:** the Editor smoke suite covers the opening quest, crafting, delivery, follow-up quest reward and local save/reload.
- **M100 — Integration audit:** source checks cover runtime catalog use, prerequisite metadata, atomic transactions and the first-loop smoke scenario.

## Local Unity acceptance

1. Pull branch codex/mia-max-foundation.
2. Run Tools → Mia & Max → Create or Update Starter Content Catalog and review the generated asset.
3. Run Tools → Mia & Max → Run Core Smoke Checks. Expected result: **13/13**.
4. Run Tools → Mia & Max → Gameplay & Character Integration Audit.
5. In Play Mode, verify craft → collect → order delivery → quest claim, then save/reload and confirm inventory, coins, XP, completed orders and unlocked areas.
6. Verify Flower Garden and Woodland Trail content is unavailable before its quest unlock and available afterwards.

The source-level audit and deterministic Editor smoke checks do not replace Play Mode, physical-device, Android/iOS build, art, animation, audio or release acceptance. Unity has not been run by this implementation.
