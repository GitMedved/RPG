# M12 — Garden & Workshop Upgrades

## Goal

Give earned coins a useful long-term purpose and make backyard progression tangible without adding a new location or changing the existing save schema.

## Garden upgrade

- Levels: 0–2.
- Cost: 60 coins for level 1, then 120 coins for level 2.
- Effect: base harvest output is 2 crops; each garden level adds 1 crop to every successful harvest.
- Applies only to future harvests.

## Workshop upgrade

- Levels: 0–2.
- Cost: 60 coins for level 1, then 120 coins for level 2.
- Effect on newly started production jobs:
  - Level 0: 100% of recipe duration.
  - Level 1: 85% of recipe duration.
  - Level 2: 70% of recipe duration.
- A job already in progress keeps the duration stored when it was started.

## Persistence and validation

- Levels are stored in PlayerSaveData and clamped to 0–2 during save validation.
- The fields are additive; the current schema version remains 1.
- Upgrade purchases use the existing validated coin-spending operation.
- Upgrade controls and current costs are displayed in the Adventure Journal.

## Unity Play Mode acceptance checklist

1. Open the Journal and confirm both upgrades start at level 0.
2. With fewer than 60 coins, attempt each upgrade; neither level should change.
3. Earn at least 60 coins, buy each upgrade once, and confirm the wallet decreases by 60.
4. Plant and harvest after the Garden level-up; harvest should produce 3 items at level 1 and 4 at level 2.
5. Start a recipe before and after a Workshop level-up; the new job duration should be reduced, while an already active job should not change.
6. Exit Play Mode and enter again; both levels and the wallet must persist.
7. Confirm the Journal remains usable on the target phone aspect ratio and safe area.

## Scope limits

- No new playable area, decoration placement, production queue, premium currency, ads or billing SDKs are included.
- The UI remains a prototype IMGUI panel.
- No Unity compile, Play Mode, Android/iOS build or device test is claimed for this package until completed locally.
