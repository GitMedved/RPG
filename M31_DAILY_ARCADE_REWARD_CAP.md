# M31 — Daily Arcade Reward Cap

## Changes
- Added a five-claim-per-UTC-day limit shared by Match-3, Fishing and Max Runner.
- The limit applies to successful mini-game reward claims, not to starting or playing a run.
- Daily claim count is persisted in `DailyTaskSaveData` and resets with the existing UTC daily task reset.
- Existing per-run duplicate protection and score-banded reward calculation remain unchanged.
- The Arcade menu displays today's reward count. When the cap is reached, reward collection reports that the daily limit is reached and invites the player to return tomorrow.
- Existing saves initialize the new count as zero; save validation clamps it to the supported range. No schema version bump.

## Acceptance checks in Unity
1. Claim rewards from different mini-games and confirm the shared count increments.
2. Claim five successful run rewards; confirm the menu shows 5/5.
3. Finish a sixth run; confirm no coins/XP are granted and the cap message appears.
4. Restart the app on the same UTC date; confirm 5/5 remains and rewards stay blocked.
5. Simulate the next UTC date; confirm the counter resets and rewards become available again.
6. Confirm replaying the same run ID is still rejected and does not consume another daily claim.

## Balance note
The limit is deliberately generous for the prototype and should be tuned using actual play-session data. It is an offline economy guard, not an anti-cheat boundary.

## Validation status
Source-level checks only. Unity compile, Play Mode, save/load, UTC rollover and arcade UX tests remain pending.
