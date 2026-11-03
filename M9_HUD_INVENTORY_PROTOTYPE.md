# M9 — Prototype HUD and inventory viewer

## Added
- Safe-area-aware runtime HUD showing player level, current XP and coins.
- Expandable backpack panel showing item names and quantities.
- Friendly labels for current crop, seed, production and wood item IDs.
- HUD is created with the runtime backyard bootstrap and uses Unity IMGUI to avoid an additional UI package dependency.

## Limitations
- Prototype-only UI: no item use, sorting, item icons, category filters, scrolling/pagination or localization table.
- The panel displays at most 12 stacks.
- Styling uses Unity's default IMGUI skin and requires a proper UI/UX pass.
- Safe-area scaling is a first pass; device orientation, notches and small screens must be checked in Unity on hardware.

## Validation
Unity compilation, Play Mode, touch interaction and Android/iOS device layout remain unverified.
