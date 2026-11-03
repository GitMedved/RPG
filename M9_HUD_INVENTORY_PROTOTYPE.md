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


## M9.2 — Journal activity controls
- Adventure Journal now exposes quest reward claiming and collection completion reward claiming.
- Added recipe selection for carrot soup, strawberry juice and wooden toy.
- Added collection of completed production jobs and delivery of the next available order.
- Order status now reports how many deliveries are complete and which products are ready.
- These controls reuse the existing persistent domain services; no new save fields were introduced.

## Current validation status
- The user has confirmed the scene, inventory, journal, quest reward button and save/reload flow work in their Unity Editor.
- The newly added recipe/order/collection controls have only been checked in source and GitHub; they still require Unity Play Mode verification.
- The panel is still IMGUI prototype UI. Check vertical fit, text wrapping and touch targets on a narrow phone before treating it as mobile-ready.
