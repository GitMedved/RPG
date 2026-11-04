# M29–M33 — Consolidated Unity Acceptance Pass

Run this checklist after pulling the latest `codex/mia-max-foundation` branch. It consolidates the new daily progression, visit gifts, arcade economy, world map and responsive arcade UI.

## 1. Import and compilation
- Open the existing Unity project; do not create a new project.
- Wait for script import and Package Manager resolution to finish.
- Confirm the Console has no red compile errors.
- Warnings can be recorded separately; do not treat ordinary gameplay Debug.Log messages as errors.

## 2. Daily goals and exploration (M29)
- Open Adventure Journal and confirm the four goals: Harvest 0/2, Craft 0/1, Deliver 0/1 and Explore 0/1.
- Successfully collect a new Flower Garden or Woodland Trail resource and confirm Explore reaches 1/1.
- Confirm an already-collected resource cannot increment the counter again.
- Complete all four goals and claim the existing +20 coins / +15 XP reward once.
- Close/reopen the game and confirm daily progress and claim state persist.

## 3. Daily visit streak and gift (M30)
- Confirm the top-left HUD shows a visit streak and a Day 1 daily gift on a new profile.
- Claim the gift and verify +10 coins / +5 XP.
- Confirm the button changes to CLAIMED and cannot grant a second reward that UTC day.
- Restart the game and confirm the claimed state persists.
- Do not change the device clock on a production save just to test streak rollover.

## 4. Arcade daily reward limit (M31)
- Open the Arcade and confirm the menu shows daily rewards as 0/5 (or the saved current count).
- Claim mini-game rewards from any combination of Match-3, Fishing and Max Runner.
- Confirm the count is shared across all three games.
- At 5/5, finish another run and confirm no additional reward is granted and the limit message appears.
- Restart on the same UTC date and confirm the count remains.

## 5. World map and panel behaviour (M32)
- Open World Map and confirm Backyard is always available.
- Confirm Flower Garden and Woodland Trail reflect the current saved unlock flags.
- Check the resource discovery counts against the inventory/world state.
- Open Journal, Settings and Backpack; confirm each closes World Map and no panels overlap.
- Unlock an area through the normal story quest flow and confirm its map status updates.

## 6. Arcade responsive layout (M33)
- In portrait Game view, verify the Arcade menu, daily reward count and three game buttons fit and remain tappable.
- Verify the full Match-3 board fits inside its panel.
- Verify Fishing slider/buttons and Max Runner controls remain visible.
- Switch to landscape and repeat.
- Check touch/mouse mapping after the safe-area GUI transform.

## 7. Report back
Please send:
- Whether compilation completed with zero errors.
- Any red Console errors (copy the full text).
- Whether the daily gift, arcade cap and map panel worked.
- A screenshot only if a panel is clipped or controls overlap.

## Important
These changes have only received source-level review in this workflow. No Unity Editor compile, Play Mode, Android build, iOS build or physical-device test is being claimed as complete. Keep your existing save; do not delete save files during this acceptance pass.
