# M6 — Mini-game foundations

Status: gameplay rules prototypes added; Unity presentation and device validation remain pending.

## Shared rewards
- `MiniGameRewardService` is the single reward entry point for `match3`, `fishing`, and `max_run`.
- Run IDs are recorded in the local save to prevent replaying the same run for rewards.
- Rewards are bounded by score bands; the claim history is capped at 500 entries.
- `LocalSaveService.Validate` initializes and sanitizes the claim list when loading saves.

## Match-3
- Pure C# 8x8 board rules engine with five gem kinds by default.
- Initial boards avoid immediate matches; only adjacent swaps that create matches consume a move.
- Cascades clear, collapse, refill, and award score.
- Detects whether a legal move remains and attempts to shuffle dead boards.
- Not yet implemented: Unity scene/UI, animations, goals, boosters, tutorial, and automated tests.

## Fishing
- Timing-session rules provide a moving progress value, randomized target zone, catch result, and three quality levels.
- Not yet implemented: cast/bite presentation, input UI, fish inventory integration, animation, and automated tests.

## Max Runner
- Three-lane rules session supports lane movement, distance scoring, coins, obstacle avoidance, and collision end state.
- Not yet implemented: procedural track spawning, character animation, camera, pooling, touch controls, UI, and automated tests.

## Integration and validation still required
- Add mini-game entry points to the backyard and route results through the shared reward service.
- Build the actual mini-game scenes and connect earned items to inventory.
- Verify reward atomicity, replay protection, save migration, and score edge cases.
- Open the project in Unity, resolve packages, compile, run Play Mode, and test Android/iOS builds.
- All rewards and balance values are provisional prototype values.
