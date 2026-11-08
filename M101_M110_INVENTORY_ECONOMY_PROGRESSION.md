# M101–M110 — Inventory, economy and progression consistency

## Implemented

- **M101 — InventoryService:** added one domain facade for inventory quantity queries, availability checks, validated add/consume operations and detached read snapshots.
- **M102 — Inventory integration:** production exposes CanStart/CanCollect and checks ingredients through InventoryService; the order board checks required quantities through the same facade. Final mutations remain in GameSession transaction methods.
- **M103 — Shared XP pipeline:** daily goals and achievements now use GameSession.TryGrantRewards, the same bounded level/XP logic used by quest rewards.
- **M104 — Daily reward consistency:** daily-goal completion is marked before the one reward notification and rolled back if the validated reward cannot be applied.
- **M105 — Achievement reward consistency:** achievement claims use the shared transaction and roll back the claim marker if reward application fails.
- **M106 — Atomic upgrades:** garden/workshop purchases deduct coins and increment the corresponding level in one GameSession operation and one notification.
- **M107 — Production affordances:** added CanStart(recipeId) and CanCollect so presentation can disable unavailable actions before attempting them.
- **M108 — Save compatibility:** no PlayerSaveData schema changes; inventory snapshots are detached and do not expose mutable save stacks.
- **M109 — Smoke coverage:** added inventory facade and shared progression transaction tests, including level-100 cap behavior and single-notification upgrade/reward assertions.
- **M110 — Integration audit:** added checks for InventoryService, inventory query integration, shared reward calls, atomic upgrades and the expanded smoke suite.

## Unity acceptance

1. Pull branch codex/mia-max-foundation.
2. Run Tools → Mia & Max → Run Core Smoke Checks. Expected result: **15/15**.
3. Run Tools → Mia & Max → Gameplay & Character Integration Audit.
4. In Play Mode, check recipe buttons before/after collecting ingredients; verify Collect only becomes available when the timer finishes.
5. Purchase Garden and Workshop upgrades and verify wallet/level after save/reload.
6. Complete daily goals and an achievement near level 100; confirm no level overflow and no duplicate reward after reload.

The code was reviewed statically in the repository. Unity compilation, Editor smoke execution, Play Mode, device builds and physical-device testing have not been run by this implementation.
