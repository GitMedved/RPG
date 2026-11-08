# M121–M130 — Garden/workshop feedback and mobile upgrade UX

## Implemented

- FarmingService now exposes `GrowthProgress01(plot)` and `SecondsUntilHarvest(plot)`. Growth stage derives from the normalized progress, keeping visuals, prompt and harvest readiness aligned.
- FarmPlotPrototype refreshes at 0.25-second intervals instead of querying plot state every rendered frame. Crop placeholders scale smoothly from seedling through mature stages; interaction text shows remaining time or harvest readiness.
- ProductionService exposes `Progress01`, clamped to the 0–1 range. ProductionStationPrototype shows remaining seconds and percent complete while crafting, and indicates the current workshop level bonus while idle.
- Adventure Journal upgrade details show garden level, current crop yield, next-level yield, coin cost and whether the player can afford it. Workshop affordability is also visible.
- Mobile journal upgrade buttons use the live affordability state, preventing taps on purchases that cannot currently succeed.
- Smoke coverage verifies growth stage/progress/countdown and normalized workshop progress within the existing 16 scenarios. Gameplay integration audit checks the new presentation links.

## Player-facing behavior

- Empty plot: "Plant a seed".
- Growing plot: "Growing · Ns" with a smoothly scaling crop placeholder.
- Mature plot: "Harvest crop".
- Workshop in progress: remaining seconds and percentage complete.
- Upgrade journal: current and next garden yield, price and affordability; unavailable upgrade buttons are disabled.

## Validation status

- Static source review and structural checks only.
- Unity compilation and the 16-scenario smoke suite have not been run in this environment.
- Play Mode, portrait/landscape layout review, save/reload and Android/iOS device QA remain manual gates.
- Existing save schema is unchanged; no final character models, rigs, or authored animation clips were added in this block.
