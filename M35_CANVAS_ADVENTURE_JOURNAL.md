# M35 — Canvas Adventure Journal

## Goal

Finish moving the primary player-facing detail panels away from IMGUI so the core progression loop can be used through touch-friendly Unity uGUI controls.

## Implemented

- Migrated the Adventure Journal into the existing responsive Canvas detail panel.
- Journal content is scrollable and summarizes story quests, keepsakes, workshop production, active orders, garden/workshop upgrade levels, daily goals, lifetime achievements and woodland exploration.
- Added Canvas actions for:
  - Claiming the next story quest reward.
  - Claiming the keepsake completion reward.
  - Starting carrot soup, strawberry juice and wooden toy production.
  - Collecting a finished product and delivering the next ready order.
  - Upgrading the garden and workshop.
  - Claiming the daily goal reward and next achievement reward.
- Journal actions call the existing domain services through `PrototypeHud`; the economy, reward values and save format remain unchanged.
- Action feedback is shown in the journal summary and refreshed while the panel is open.
- The Canvas close button now closes all detail panels consistently.
- Removed the Canvas-mode IMGUI detail-panel rendering path. The IMGUI HUD remains only as a fallback for scenes that do not attach `MobileHudCanvas`.

## Current UI migration status

- Canvas: main status HUD, daily gift, navigation, backpack, world map, settings and adventure journal.
- Legacy fallback: `PrototypeHud.OnGUI` remains for scenes without the runtime Canvas component.
- Arcade board UI remains on its separate responsive prototype renderer.
- The Canvas still uses runtime-created panels and built-in Arial; authored prefabs, final icons, typography and localization are future art/UI tasks.

## Unity acceptance checklist

1. Confirm no compile errors after pulling M35.
2. Open Adventure Journal and scroll its content.
3. Test each journal action. Actions that are not yet available should show a helpful message and must not corrupt inventory or rewards.
4. Confirm a successful action updates the journal summary and the player's coins/XP/inventory.
5. Close Journal with both the top navigation button and panel Close button.
6. Confirm Backpack, World Map and Settings still open one at a time.
7. Confirm no duplicate IMGUI status bar, action dock or Journal panel appears while Canvas HUD is active.
8. Check touch targets and scroll behavior in portrait and landscape Game views.

## Not validated here

No Unity Editor compilation, Play Mode, save/reload, Android or iOS device test was run in this environment. Source-level checks do not replace Unity validation.
