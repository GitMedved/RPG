# M58 — Shared mobile UI theme

## Changes

- Added `MobileUiTheme` as the single source for the runtime HUD's core palette: ink, cream, forest, leaf, honey, panel and muted states.
- Centralized uGUI button tint states (normal, highlighted, pressed, selected and disabled) in `ApplyButton`.
- Added a shared availability helper for buttons, including the muted disabled appearance.
- Migrated `MobileHudCanvas` to the shared palette and button styling without changing navigation, gameplay callbacks, panel behavior or save data.
- Kept the UI asset-free: no unapproved fonts, sprites or external art were introduced.

## Validation boundary

Source review only. Unity compilation, prefab/scene behavior, visual hierarchy, accessibility contrast, portrait/landscape layout, touch states and physical-device checks have not been run remotely. Run the UI Migration Audit and inspect the HUD in Unity before treating the theme as accepted.
