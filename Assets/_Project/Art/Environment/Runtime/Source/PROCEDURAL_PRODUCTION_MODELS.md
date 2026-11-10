# Mia & Max — Procedural Backyard Production Models

## What is generated

The Unity Editor tool `Tools/Mia & Max/Backyard/Generate All 15 Production Models` creates a separate, editable set of 15 stylized environment prefabs. It builds custom meshes (not Unity primitive GameObjects), UV0 coordinates, mesh assets, generated albedo PNGs and URP/Lit materials. A post-build optimization combines visual meshes by material and LOD level to reduce renderer/draw-call pressure while preserving material separation and the house/tree LOD groups.

Generated outputs:

- `Runtime/ProductionPrefabs/` — 15 visual-only prefabs.
- `Runtime/ProductionMeshes/` — individual editable mesh assets.
- `Runtime/ProductionMaterials/` — generated materials.
- `Runtime/ProductionTextures/` — generated albedo textures.

The original placeholder prefabs remain in `Runtime/Prefabs/`. The production generator only resets its dedicated `Production*` output folders, so the original placeholders and gameplay scene are preserved.

## Generate and inspect

1. Pull the project branch `codex/mia-max-foundation`.
2. Wait for Unity to finish compiling.
3. Run `Tools > Mia & Max > Backyard > Generate All 15 Production Models`.
4. Read the generation report in Console. Stop and report any compilation or generation errors.
5. Run `Tools > Mia & Max > Backyard > Audit Production Prefabs (Read Only)`. The audit now also enforces targets of no more than 8 renderers and 12,000 triangles per prefab. Repeated ellipsoid details use a deliberately low-poly mesh to reduce foliage, flower and path costs.
6. Run `Tools > Mia & Max > Backyard > Create Production Yard Preview`.
7. Open `Runtime/Scenes/BackyardProductionLayoutPreview.unity` from the Project window and inspect it in Scene/Game view.

## Asset list

HouseExterior, PatioDeck, FenceStraight, FenceCorner, FenceGate, MainTree, SwingSet, Sandbox, ShrubCluster, FlowerBed, FlowerPot, GardenPlot, Workshop, AdventureGate and PathSegment.

## Important limitations

These are procedural stylized starting models with custom geometry and generated surface patterns. They are not hand-sculpted final art and have not been visually approved, profiled on target mobile devices, or validated against the reference image in Unity. The procedural textures are generated albedo patterns, not authored PBR texture sets. LODs are supplied for the house and main tree as simple distant representations.

All prefabs are visual-only: no gameplay scripts or gameplay colliders are added. The optimization targets are at most 8 renderers and 12,000 triangles per prefab; these are structural budgets, not substitutes for Unity Frame Debugger or target-device profiling. The gameplay bootstrap, gameplay roots, interactions, save data and Build Settings are not modified. Do not connect these prefabs to the playable scene until the owner reviews the preview and mobile performance is checked.
