# M29 — Daily Exploration Goal

## Changes
- Expanded the offline UTC daily task set from three goals to four.
- Added an exploration goal: discover one previously uncollected resource in either the Flower Garden or Woodland Trail.
- Persisted the daily exploration counter in `DailyTaskSaveData`. Existing saves deserialize the new integer as zero; no schema bump is required.
- Successful resource pickups record exploration progress only after the item is successfully added to inventory.
- The Adventure Journal now allocates additional scroll space for the fourth progress line and the shifted achievement section.
- The existing daily reward remains 20 coins + 15 XP and can still be claimed only once per UTC day after all four goals are complete.

## Acceptance checks in Unity
1. Open the Adventure Journal and confirm four daily objectives are visible.
2. Collect one new Flower Garden resource; confirm Explore changes to 1/1.
3. Collect a Woodland Trail resource on a different day or reset test save; confirm it also counts.
4. Attempt to interact with an already collected resource; confirm it cannot increment the daily counter again.
5. Complete harvest, craft, delivery and exploration; claim the reward and confirm +20 coins / +15 XP.
6. Save, restart and confirm daily progress and claimed state persist.
7. Confirm a legacy save loads with Explore at 0/1 and no migration exception.

## Limitations
- Daily tasks are local/offline and use the device UTC clock; there is no server validation or anti-clock-tampering.
- Exploration currently means collecting a new flower or woodland resource. Visiting an area or re-collecting an item does not count.
- Reward amount is unchanged.

## Validation status
Source-level checks only. Unity compile, Play Mode, old-save migration, UTC rollover and mobile layout validation remain to be performed locally.
