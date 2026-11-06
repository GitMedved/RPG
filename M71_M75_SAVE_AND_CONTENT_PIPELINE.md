# M71–M75 — Save recovery and content authoring pipeline

**Branch:** `codex/mia-max-foundation`  
**Date:** 2026-11-06  
**Scope:** source-level implementation. Unity compilation, Editor execution and device acceptance are not claimed.

## M71 — Interrupted-write save recovery
- Extended `LocalSaveService.LoadOrCreate` to recover a complete temporary save when the primary file is absent, as can happen after the primary was rotated to backup but before the temp file was promoted.
- The temporary copy is promoted only when its `UpdatedAtUnix` is at least as recent as the known-good backup.
- A valid primary always wins; stale temporary files are removed. A corrupt primary continues to restore from the known-good backup.
- Unsupported schema versions remain explicit errors and are not silently overwritten.

## M72 — Save payload normalization
- Duplicate inventory stacks merge into one stack with overflow clamping.
- Farm plots with missing/duplicate IDs are removed; incomplete crop state is reset.
- Unknown or incomplete production jobs are cleared.
- Duplicate completed orders, claimed quests and unlocked area IDs are removed.
- Bounded collection sizes remain in place; no save schema version change was made.

## M73 — Starter content authoring
- Added `Tools → Mia & Max → Create or Update Starter Content Catalog`.
- Creates `Assets/_Project/ScriptableObjects/BackyardContentCatalog.asset` with the known starter crop, recipe, order, story quest and mini-game IDs/rewards.
- The builder is additive and idempotent: existing IDs are never overwritten, so manual tuning is preserved.
- Added `BackyardContentCatalog.TryAdd` for duplicate-safe insertion.
- Added a deterministic Editor smoke check for duplicate rejection and catalog validation.

## M74 — Catalog coverage audit
- Extended **Gameplay & Character Integration Audit** to check for the authoring tool and coverage of 16 known starter IDs across crops, recipes, orders, quests and mini-games.
- Coverage is separate from structural validation: a structurally valid but incomplete catalog is still reported as pending.
- The audit remains static and does not prove runtime content consumption or balanced progression.

## M75 — Local acceptance handoff
- Consolidated the exact Unity Editor steps below and documented the remaining release evidence.
- The content catalog remains an authoring/index layer; current gameplay services continue to own inventory, timing, rewards and unlocks.
- No catalog asset is auto-created remotely, and no Unity Editor command has been run by this change.

## Local Unity acceptance
1. Pull `codex/mia-max-foundation`.
2. Wait for Unity compilation to finish.
3. Run **Tools → Mia & Max → Run Core Smoke Checks**. Expected static/domain suite: 8/8.
4. Run **Tools → Mia & Max → Create or Update Starter Content Catalog**. Review the generated asset in the Inspector; the command preserves existing IDs and values.
5. Run **Tools → Mia & Max → Gameplay & Character Integration Audit**. Confirm the starter catalog coverage row is ready.
6. Run **Tools → Mia & Max → Release Readiness Audit** and retain the complete report.
7. Test a disposable profile: plant/harvest, craft/collect, deliver an order, claim a quest and arcade reward, close/reopen, and verify persistence. Do not delete or overwrite the real save.
8. Test backup recovery only in a copied test save directory, not the live player save.
9. Confirm portrait/landscape layout and run Android/iOS device tests when production assets and builds are available.

**Acceptance status:** source changes committed; Unity compile, 8/8 smoke execution, catalog asset creation, audits, Play Mode save/reload, backup recovery in Editor, physical device builds and QA remain pending.
