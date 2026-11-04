# M22 — Daily goals and rewards

## Gameplay loop
The player receives three UTC-day goals:
- Harvest 2 crops.
- Collect 1 finished workshop product.
- Deliver 1 order.

Completing all three unlocks a one-time daily reward of 20 coins and 15 XP. The task counters and claim state are saved locally. The daily date key uses UTC (yyyy-MM-dd) so the reset is consistent across regions and travel. Moving the device clock backwards does not reset progress again.

## Integration
- Harvest progress increments only after a successful harvest.
- Craft progress increments only after a finished product is successfully collected.
- Delivery progress increments only after an order is successfully completed.
- The Adventure Journal shows today's counters and the reward claim action.
- Old saves receive an empty daily task record and start today's goals at zero.

## Limits / future work
- This is a small fixed daily set, not a remotely configured live-ops system.
- No streak bonus, notifications, server clock, cloud sync or rotating task catalogue is included.
- UTC day reset requires the device clock to advance; offline client time is not tamper-proof.
- Daily rewards are intentionally modest and have no paid/randomized element.

## Unity acceptance checklist
1. Compile the project in the pinned Unity Editor.
2. Harvest twice, collect a finished recipe and deliver an order; confirm each counter increments only on successful action.
3. Claim the reward once; verify coins/XP change once and the button reports claimed.
4. Close/reopen the game; confirm progress and claim state persist.
5. Test a save from before M22; confirm it loads and creates daily state without losing inventory or progress.
6. Advance the UTC date in a test save/device environment and verify counters reset once.
7. Confirm a backwards clock change does not reset current progress.

No Unity compile, Play Mode or device test has been performed remotely.