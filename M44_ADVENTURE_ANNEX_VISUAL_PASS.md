# M44 — Flower Garden & Woodland Trail visual pass

## Scope
Added two themed environment dressing passes to the existing story-gated annexes.

### Flower Garden
- Three framed raised planting beds with visible soil and pale edging.
- Additional flower clusters in pink, yellow, purple and white.
- Small bird-bath bowl with water surface.
- Stepping stones and a low shrub border along the rear fence.

### Woodland Trail
- Layered secondary canopy clusters around the existing trees.
- Mossy boulders with green moss caps.
- Fern-like foliage placed along the outer edges, leaving the main path open.
- Fallen log, small berry cluster and two decorative mushrooms.

## Integration and safety
- Added `AdventureAnnexVisualPass` under `Assets/_Project/Scripts/World`.
- Called from the existing Flower Garden and Woodland Trail builders, using their current unlocked content roots.
- The existing quest unlocks, portals, resource nodes, item IDs, rewards, save data and player positions are unchanged.
- Decorative objects are children of the gated content and are created with their primitive colliders removed. Existing interactable resource roots and portal colliders are untouched.
- No new lights or runtime update loops are introduced.

## Unity acceptance
1. Pull `codex/mia-max-foundation` and allow Unity to compile.
2. Unlock Flower Garden and inspect flower beds, clusters, bird bath and path stones.
3. Unlock Woodland Trail and inspect layered trees, rocks, ferns, log and mushrooms.
4. Collect all existing resources and confirm their original interactions/rewards still work.
5. Walk through both areas and check that decorative objects do not block movement.
6. Check Console and inspect portrait/landscape Game views.

## Limitations
All additions use procedural Unity primitives and simple materials. They are prototype dressing, not final authored foliage, terrain, textures, particles, lighting or production environment assets. Unity compilation, Play Mode and mobile-device performance have not been run remotely.
