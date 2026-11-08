# M151–M160 — Backyard production material integration

## Scope
- Connect the existing generated world-material library to the established backyard environment dressing.
- Use reviewed generated URP materials when present, while preserving color-material fallbacks for a clean first launch.
- Prevent duplicate environment dressing if the build entry point is invoked more than once.
- Add safe shader fallback handling and a source-level integration audit.

## Material bindings
- House cream plaster: Plaster_Cream
- White trim/fence: Fence_WhitePaint
- Warm timber/door: Wood_Warm
- Garden soil: Soil_Garden
- Planter foliage: Leaf_Green
- Pink blooms: Flower_Petal

The generated library still must be created locally with Tools → Mia & Max → Generate Complete Texture Library. Yellow blooms, glass, brass and terracotta retain dedicated color fallbacks so unrelated surface textures are not misapplied.

## Local acceptance
1. Pull the feature branch.
2. Run Generate Complete Texture Library.
3. Run Audit Generated Texture Library and confirm all 40 surfaces are complete.
4. Run Gameplay & Character Integration Audit.
5. Enter Play Mode and inspect the house trim, door, garden beds, stepping stones and patio planters.
6. Confirm there is only one M40_Backyard_Environment_Dressing root after scene initialization.

## Validation boundary
Source changes and repository structure were reviewed. Unity compilation, asset generation/import, material appearance, Play Mode and Android/iOS performance have not been executed in this environment. Generated PNG and Material files are created locally by the Editor menu and are not committed by this source-only change.
