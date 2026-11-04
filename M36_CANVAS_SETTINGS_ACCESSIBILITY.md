# M36 — Canvas Settings and Accessibility Controls

## Implemented

- Migrated the Settings panel presentation to the shared Canvas detail panel.
- Added Canvas controls for Master, Effects, Music, Voice and UI scale adjustments.
- Added Canvas toggles for vibration and reduced motion.
- All controls call the existing `GameSettingsService`; settings remain persisted in the existing player save data.
- The IMGUI Settings panel is no longer rendered when the Canvas HUD is active.
- Backpack and World Map continue to use the same shared scrollable Canvas panel.
- Journal remains on IMGUI for the next migration slice.

## Acceptance checklist

1. Open Settings and confirm the Canvas panel appears without a duplicate IMGUI panel.
2. Adjust all five values in both directions; verify displayed values update and persist after restarting Play Mode.
3. Toggle vibration and reduced motion; verify labels update and values persist.
4. Confirm Backpack and World Map still open and close correctly.
5. Confirm Journal still opens and its existing quest, production, order and reward actions remain available.
6. Check portrait and landscape layouts and touch targets.

## Not validated here

No Unity Editor compile, Play Mode, save/reload or physical-device test was run. The UI uses temporary panels and built-in Arial; final visual assets and typography remain pending.
