# M111–M120 — Economy balance and garden/workshop progression

## Implemented

- Centralized first-release economy tuning in `EconomyBalance`: upgrade prices (60/120 coins), maximum upgrade level (2), garden yield (2/3/4 crops), harvest rewards (3 coins and 8 XP), planting XP (2), and workshop speed (0/15/30 percent faster).
- `UpgradeService` now uses the shared rules and exposes `CanUpgradeGarden`, `CanUpgradeWorkshop`, `NextGardenHarvestYield`, and `NextWorkshopDuration(baseSeconds)` for UI presentation.
- Production duration is calculated with integer arithmetic and ceiling division. This is deterministic and avoids floating-point boundary errors.
- Farming harvest preflights item, coin and XP capacity. The mature plot is cleared before the single reward notification; if the transaction is rejected, the plot is restored and no crop/reward is granted.
- Planting and harvesting use centralized XP/reward values.
- Core smoke coverage is now 16 scenarios. The added scenario checks economy curves, upgrade previews, level-one harvest yield, rewards and overflow rollback.
- Integration audit checks the new balance service and atomic harvest path.

## Balance reference

| System | Level 0 | Level 1 | Level 2 |
|---|---:|---:|---:|
| Upgrade price for next level | 60 coins | 120 coins | Max level |
| Garden harvest yield | 2 crops | 3 crops | 4 crops |
| Workshop production time | 100% | 85% | 70% |

Harvest reward: 3 coins + 8 XP. Planting reward: 2 XP.

## Validation status

- Static source review and structural checks only.
- Unity compilation and the 16-scenario smoke suite have not been run in this environment.
- Play Mode, save/reload, visual UI integration, Android/iOS builds and physical-device tests remain manual release gates.
- Existing save schema is unchanged.
