# M39 — Garden Visual Pass

## Scope

M39 replaces the single generic sphere used for every planted crop with distinct, low-poly runtime crop silhouettes. The visuals are deliberately temporary procedural meshes and do not claim to be final production art.

## Implemented

- Carrots use an orange root with a leafy crown.
- Tomatoes use a central stem, layered leaves and visible red fruit.
- Strawberries use a low leafy crown, two berries and a small blossom.
- Each crop is rebuilt only when the saved crop ID changes; growth stage updates the plant scale rather than recreating geometry every frame.
- Empty plots hide their crop visual. Harvest/plant interactions, growth timers, inventory, XP, coins and save data remain unchanged.
- Decorative crop child meshes have their colliders removed, so they do not compete with the plot's interaction collider.

## Unity acceptance

1. Pull the feature branch and allow Unity to finish importing.
2. Confirm there are no compile errors.
3. Enter Play Mode and plant each available crop type in separate plots.
4. Confirm carrot, tomato and strawberry silhouettes are visibly different.
5. Confirm plants grow through their stages and can be harvested when ready.
6. Confirm harvesting empties the plot and a newly planted crop replaces the previous visual.
7. Walk around the plots and verify there are no extra interaction targets or collision snags.

## Limitations

- The meshes are simple built-in Unity primitives with flat materials.
- No authored crop models, textures, shaders, particle effects or crop-specific animation clips are included.
- Final crop art should be created from the approved art direction and imported as prefabs in a later asset-production pass.
- Unity compilation, visual appearance, Play Mode and mobile performance have not been tested in this environment.
