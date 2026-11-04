# M40 — Backyard Environment Art Pass

## Scope

M40 adds a cohesive decorative presentation layer to the existing backyard greybox. It builds on the approved backyard layout and M39 crop silhouettes; it does not redesign the environment or introduce new gameplay systems.

## Implemented

- Added a readable front entrance to the house: framed door, inset panels, handle, doorstep, two divided windows, sills and warm facade trim.
- Added slim timber edging around the three existing interactive garden plots without covering their crop visuals or changing their colliders.
- Added a short stepping-stone spur between the patio and garden plots.
- Added two small patio planters with simple foliage and colorful blooms.
- Added a clean decorative rim and a small toy block to the existing sandbox.
- Grouped all M40 pieces under `M40_Backyard_Environment_Dressing` for easier scene inspection.
- Decorative primitives have their colliders removed. Existing interactables, farm plots, movement and save data are not modified.

## Unity acceptance

1. Pull the feature branch and wait for Unity package/import activity to finish.
2. Confirm the Console has no compile errors.
3. Enter Play Mode and inspect the house front: door, windows and trim should sit on the yard-facing wall.
4. Check that the doorstep does not block the patio route.
5. Plant carrot, tomato and strawberry crops; confirm the new garden edging does not cover plants.
6. Walk around the patio, planters, plots and sandbox; confirm no new interaction targets or collision obstacles appear.
7. Check the scene in portrait and landscape Game views for visual readability.

## Limitations

- All additions are runtime-generated Unity primitives with simple materials.
- This is a composition/polish pass, not final environment art. No authored meshes, texture maps, foliage assets, lighting bake, VFX or environment animations are included.
- The supplied backyard reference remains authoritative; any final art replacement must preserve its house, lawn, fence, tree, swing, sandbox and patio landmarks.
- Unity compilation, Play Mode appearance and Android/iOS performance have not been tested in this environment.
