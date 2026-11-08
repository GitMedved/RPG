# M81–M85 — Progression and reward transaction hardening

## M81 — Bounded level progression
- Centralized XP application in GameSession.
- Player progression now has a level-100 cap; XP thresholds are clamped to a safe range.
- Excess XP at the cap is bounded below the next-level threshold rather than overflowing an integer.

## M82 — Atomic reward grants
- Added GameSession.TryGrantRewards for combined coin/XP grants.
- The operation validates both balances before changing either value and emits one state-change notification.
- QuestService now refuses a claim if the complete reward cannot be applied; a failed grant does not mark the quest as claimed.

## M83 — Deterministic regression coverage
- Added a ninth grouped Editor smoke check for quest reward overflow rejection, one-time claim behavior and the level cap.
- Expected result is 9/9 when run in the local Unity Editor; it has not been executed remotely.

## M84 — Integration audit
- Gameplay & Character Integration Audit now checks that the atomic reward API is present and used by QuestService.
- This is a source-contract check only; it does not execute a quest or validate a save on device.

## M85 — Acceptance notes
- Documented the new reward boundary and updated the release roadmap.
- Existing save schema remains version 1; no serialized fields were added or changed.

## Local validation
After pulling the feature branch, run **Tools → Mia & Max → Run Core Smoke Checks** and confirm 9/9. Then run **Tools → Mia & Max → Gameplay & Character Integration Audit**. Play Mode must still verify claiming each story reward, level progression, annex unlocks and save/reload. No Unity compilation, Editor execution, device build or runtime acceptance is claimed by these source changes.
