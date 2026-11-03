# M10 — First-session tutorial and arcade feedback

## Implemented in source
- Added a persistent six-step tutorial: movement, planting, harvesting, crafting, order delivery and arcade play.
- Tutorial progress is stored in PlayerSaveData and survives local save/reload.
- The tutorial advances only after the matching gameplay action succeeds.
- The HUD displays the current contextual instruction within the screen safe area.
- Match-3 reshuffles after a cascade when no valid swaps remain.
- Fishing aim is player-controlled instead of defaulting to the target centre.
- Max Runner now presents lane obstacles and collectible lane targets; successful dodges and collisions affect the run.

## Important limitations
- These changes have been committed to the feature branch but have **not** been compiled or exercised in Unity after this package.
- Tutorial is currently a single persistent linear sequence; no skip, replay, localization table or illustrated callouts yet.
- Arcade interface remains IMGUI prototype UI. Match-3 uses glyphs; fishing uses a slider; runner uses lane buttons and timed lane events.
- Runner obstacle/coin spawning is a simple prototype, not a procedural level/difficulty system.
- Mini-game reward balance and tutorial progression require Play Mode testing.
- Small-screen overlap and actual touch ergonomics remain unverified.

## Unity validation checklist
1. Open the project in the pinned Unity Editor and wait for import.
2. Confirm no compiler errors.
3. Enter Backyard and move Mia; tutorial should advance to planting.
4. Plant, wait for maturity, harvest; verify each step advances only after success.
5. Start a recipe, wait, collect it; verify the crafting step advances.
6. Deliver a ready order; verify the delivery step advances.
7. Finish a mini-game and claim its reward; verify tutorial completes.
8. Stop Play Mode and enter again; verify tutorial state remains.
9. Test all screens on at least one small Android/iOS viewport and check safe areas.
10. Inspect Console and test pause/resume/save reload.
