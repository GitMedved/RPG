# Mia & Max — Backyard Production Asset Handoff

## Purpose

Replace the temporary runtime backyard blockout with a coherent, mobile-ready 3D environment while preserving gameplay interactions and the approved visual reference. The current procedural primitives remain a playable fallback until reviewed production assets are imported.

## Visual source of truth

Use `Assets/_Project/Art/Environment/backyard_collage.png` and `VISUAL_REFERENCE_SPECIFICATION.md`.

Preserve the established layout and orientation:
- Two-storey beige/light-cream clapboard house with dark shingle roof and white trim.
- Patio/deck directly beside the house, outdoor seating and kettle-style grill.
- Broad central lawn.
- White picket fence enclosing the yard.
- Mature leafy tree on the same side as the reference.
- Wooden A-frame swing set on the left.
- Raised rectangular wooden sandbox/play area.
- Shrubs, flower beds and potted plants around the house/fence.
- Bright sunny daytime lighting and warm natural palette.

Do not mirror the layout, move landmarks, or invent major features not shown in the approved board.

## Required runtime prefab set

Place reviewed prefabs in `Assets/_Project/Art/Environment/Runtime/Prefabs/` using these exact names:

| Prefab | Scope |
|---|---|
| HouseExterior | Main two-storey house shell, roof, trim, doors and window frames |
| PatioDeck | Deck/patio surface, steps and modular outdoor furniture |
| FenceStraight | Reusable straight white picket fence segment |
| FenceCorner | Corner fence segment |
| FenceGate | Gate segment that preserves the current gate opening |
| MainTree | Trunk, canopy and optional LODs |
| SwingSet | A-frame, crossbeam, ropes/chains and seats |
| Sandbox | Raised wooden play bed and sand surface |
| ShrubCluster | Reusable shrub grouping |
| FlowerBed | Raised or ground-level planting bed |
| FlowerPot | Patio/house-side potted plant |
| GardenPlot | Farm plot visual; interaction logic remains on the gameplay object |
| Workshop | Workshop station exterior shell |
| AdventureGate | Woodland/Flower Garden entrance visual |
| PathSegment | Reusable garden/woodland path module |

Keep gameplay components (for example `BackyardInteractable`, `FarmPlotPrototype`, portal and progression components) on stable gameplay roots. Art prefabs should be child visuals wherever possible so art replacement does not silently remove interaction, collider, save or progression behaviour.

## Asset folders

- Editable source files: `Assets/_Project/Art/Environment/Runtime/Source/`
- Imported models: `Assets/_Project/Art/Environment/Runtime/Models/`
- Runtime prefabs: `Assets/_Project/Art/Environment/Runtime/Prefabs/`
- Materials: `Assets/_Project/Art/Environment/Runtime/Materials/`
- Textures: `Assets/_Project/Art/Environment/Runtime/Textures/`
- LOD assets: `Assets/_Project/Art/Environment/Runtime/LOD/`

Keep source art and licensed source packages out of Resources unless they are required at runtime. Preserve all Unity `.meta` files.

## Mobile production requirements

- Modular meshes with clean topology and sensible pivots.
- Shared materials and atlases where they reduce draw calls.
- Baked lighting for static environment geometry where practical.
- LODs for the house, main tree and other large silhouettes where useful.
- Avoid excessive transparent foliage, high-cost shaders and unnecessary real-time shadows.
- Use colliders only where needed; avoid mesh colliders on simple interactable props when primitive colliders suffice.
- Record texture sizes, mesh budgets, LOD distances and asset provenance/licence.
- Verify scale and camera composition in the actual gameplay scene on Android and iOS target hardware.

## Integration and acceptance sequence

1. Prepare folders using `Tools > Mia & Max > Prepare Backyard Production Asset Folders`.
2. Import approved model source and exports into Source/Models.
3. Create the required runtime prefabs with the exact names above.
4. Run `Tools > Mia & Max > Audit Backyard Production Assets`.
5. Review every prefab against the approved reference: silhouette, scale, orientation, materials, UVs, textures and mobile budget.
6. Integrate visuals under the existing gameplay roots; preserve interaction scripts, colliders, object IDs, quest gates and save references.
7. Compare the gameplay camera against the reference composition and test movement/interactions around every replaced landmark.
8. Only after visual and device review, switch the release scene from the procedural fallback to the production prefab set.

The audit's `READY*` status checks only that a prefab has a mesh and at least one assigned material. It is not art approval or release approval.

## Current limitation

The repository does not currently contain an approved production mesh set for the backyard. The pipeline prepares folders and checks imported assets; it does not invent final 3D meshes or claim the existing primitive blockout is production art. Keep the current bootstrap fallback until the imported prefabs are reviewed and integration is verified in Unity.
