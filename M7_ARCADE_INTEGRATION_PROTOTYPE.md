# M7 — Arcade integration prototype

Status: first playable OnGUI shell and backyard entry point added. Unity validation pending.

## Implemented
- Added an arcade board to the generated backyard blockout.
- Interacting with the board opens a simple menu with Match-3, Fishing, and Max Runner.
- Match-3 uses the standalone `Match3Board` rules engine and awards points for valid matches.
- Fishing exposes a timing target and only grants a reward for a successful catch.
- Max Runner exposes three-lane movement, distance scoring, a bonus-coin action, and a timed run.
- Each started activity receives a unique run ID. Results are passed to `MiniGameRewardService`.
- Mini-game coin and XP changes are now applied together after overflow validation, avoiding partial grants.

## Prototype limitations
- The interface uses Unity `OnGUI`; it is temporary and not production UI.
- Match-3 is a simple text-button grid without animation, special pieces, objectives, or boosters.
- Fishing is a timing check, not a complete casting/bobber/fish encounter.
- Runner is a lane/distance prototype, not yet an obstacle-spawning endless runner; the bonus coin is a manual test action.
- The arcade shell is not yet adapted to touch-safe layout, safe areas, localization, accessibility, or controller input.
- Reward values remain provisional and need balancing.

## Next development
1. Replace the prototype shell with responsive UI prefabs and dedicated scenes.
2. Implement actual runner obstacles, pickups, pooling, and difficulty ramp.
3. Integrate caught fish and mini-game rewards with inventory/collection systems.
4. Add objective-based Match-3 levels and progression.
5. Add automated domain tests for boards, timing sessions, runner sessions, save validation, and reward replay protection.
6. Verify in Unity Editor and on Android/iOS devices.
