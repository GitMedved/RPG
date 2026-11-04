# M35 — Canvas Backpack, World Map and Feedback

## Implemented

- Migrated Backpack presentation from IMGUI to a scrollable Unity uGUI panel.
- Migrated World Map overview to the same Canvas panel.
- Both panels use the existing saved inventory and quest unlock state; no duplicate game data or new save schema was introduced.
- Added a close action that explicitly closes either Canvas detail panel without accidentally opening the other.
- Added a Canvas feedback toast for short gameplay messages, including daily gift claims.
- Journal and Settings remain on the legacy IMGUI renderer for now; their controls and service actions are unchanged.
- Arcade continues to use its own responsive prototype interface.

## Acceptance checklist

1. Pull the feature branch and allow Unity to recompile.
2. Confirm no compile errors.
3. Open Backpack: verify current item names and quantities; scroll if the inventory exceeds the viewport.
4. Close Backpack using the panel close button and bottom dock; verify the panel closes.
5. Open World Map: verify area unlock states and discovery counts match progression.
6. Close World Map and confirm Backpack does not open as a side effect.
7. Claim a daily gift or trigger an interaction message and verify the Canvas toast appears and disappears.
8. Verify Journal and Settings still open and their existing controls still work.
9. Check portrait and landscape Game views and touch scrolling.

## Known migration boundary

The Journal and Settings panels are still IMGUI. The Canvas detail panel uses temporary colors and built-in Arial, not final Mia & Max artwork, iconography or typography. No Unity Editor compile, Play Mode, physical-device, Android or iOS test was run here.
