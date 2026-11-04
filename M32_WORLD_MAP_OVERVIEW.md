# M32 — World Map Overview

## Changes
- Added a World Map shortcut to the HUD.
- The map presents the Backyard as the home base, then shows Flower Garden and Woodland Trail as a simple progression route.
- Area state is read from the existing saved world-unlock flags; discovery counts are read from the persisted resource collections.
- Locked-area copy explains which story action unlocks the next location.
- The map, Journal, Settings and Backpack panels now close one another when opened, avoiding overlapping IMGUI panels.

## Acceptance checks in Unity
1. Open World Map from the top-right HUD.
2. Confirm Backyard is always shown as the home base.
3. Before unlocking Flower Garden, confirm it is marked LOCKED and explains the required story reward.
4. Claim the relevant quest and confirm Flower Garden changes to UNLOCKED.
5. Collect flowers and confirm the discovery count updates.
6. Unlock Woodland Trail and confirm its status and discovery count update.
7. Open Journal, Settings and Backpack in turn; confirm the map closes and panels do not overlap.
8. Test portrait, landscape and safe-area layouts.

## Limitations
- This is a compact text-first progression map, not a rendered world map or minimap.
- It does not implement fast travel, navigation arrows in the 3D world or current-player location tracking.
- It uses existing same-scene area unlocks and resource collections; no save schema change.

## Validation status
Source-level checks only. Unity compile, Play Mode, progression state and device layout validation remain pending.
