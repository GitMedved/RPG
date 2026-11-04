# M19 — Backyard visual polish pass

**Status:** procedural dressing pass implemented; Unity visual review still required.

## What changed
- Added `BackyardVisualPolishPrototype`, a separate lightweight dressing layer for the current temporary backyard.
- Integrated it into `BackyardPrototypeBootstrap` after the existing landmark detail pass.
- Added a stepping-stone route from the patio toward the play area.
- Added layered foliage to soften the central tree canopy.
- Added four small flower clusters around the yard perimeter, keeping the central movement area open.
- Added fence post caps, a front fascia strip and a patio edge detail.
- Dressing primitives have no colliders, so they do not interfere with player movement or interaction.
- Materials are created as shared per-pass assets and use URP/Lit with a Standard fallback.

## Intentional limitations
This is a visual improvement to the playable greybox, not final production art. It does not replace the approved reference-matched house, tree, fence, swing, sandbox, flower beds or character models. The production environment should eventually replace these primitives with authored meshes, textures, LODs and prefabs.

## Unity acceptance
1. Compile with no errors.
2. Enter Play Mode and inspect the yard from the normal follow camera.
3. Confirm the stepping stones sit on the lawn and do not obstruct plots or keepsakes.
4. Confirm foliage clusters do not obscure the player, garden gate or important interactables.
5. Walk through the yard and verify decorative objects do not block movement.
6. Check the scene at the target mobile aspect ratios and profile the additional draw calls on a representative device.

## Next visual production dependency
The next meaningful quality jump requires real 3D assets rather than more primitive geometry: Mia and Max production models/rigs, a reference-matched house and modular yard kit, plus authored UI sprites. Keep the procedural pass available as a fallback until those assets are approved.
