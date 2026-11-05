# M42 — Backyard station visual pass

## Scope
Replaced the visible cube faces of five interactive backyard stations with distinct procedural 3D dressing:
- Workshop: timber workbench, cooking pot, rim, herbs and recipe book.
- Order board: framed notice board with paper slips and a blue emblem.
- Adventure journal board: warm framed board, parchment notes and a red emblem.
- Arcade: freestanding cabinet with marquee, screen, control deck, joystick and buttons.
- Collection: raised display pedestal and a large keepsake book with cover/spine/emblem.

## Implementation
- Added `BackyardStationVisuals` under `Assets/_Project/Scripts/World`.
- The visual pass is called immediately after each station's existing gameplay components are configured.
- The original root objects remain in place with their original colliders and gameplay scripts. Only their root renderers are hidden.
- New primitive decorations are children of the existing roots and their colliders are destroyed, so they do not add interaction blockers.
- No service logic, reward values, inventory IDs, save data, or progression rules changed.

## Acceptance in Unity
1. Pull `codex/mia-max-foundation` and let Unity recompile.
2. Enter Play Mode and inspect the Workshop, Order Board, Adventure Journal Board, Arcade cabinet and Collection Book.
3. Walk up to each station and verify the existing interaction prompt/action still works.
4. Confirm the decorative parts do not block Mia or Max, and inspect both portrait and landscape Game views.
5. Check Console for compile/runtime errors.

## Limitations
All geometry is generated from Unity primitives and simple materials. These are improved prototype visuals, not final authored models, textures, typography, animation or production art. Unity compilation, Play Mode and mobile-device performance have not been run remotely.
