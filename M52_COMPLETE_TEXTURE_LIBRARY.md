# M52 — Complete procedural texture library

## Scope
Adds an editor-only texture authoring tool for the current backyard, Flower Garden, Woodland Trail, workshop/stations, props, collectibles and reusable future environment kits. The library is intentionally built from shared tileable surfaces rather than one-off large textures, keeping memory and draw-call budgets manageable on mobile.

## Run in Unity
1. Pull the feature branch `codex/mia-max-foundation`.
2. Wait for Unity to finish compiling.
3. Open **Tools → Mia & Max → Generate Complete Texture Library**.
4. Wait for PNG import and material creation to finish.
5. Run **Tools → Mia & Max → Audit Generated Texture Library** and inspect the Console report.

The generator creates:
- 40 stylized surfaces;
- 40 Base Color maps (512×512);
- 40 tangent-space Normal maps;
- 40 grayscale Height maps;
- 40 matching URP Lit materials (Standard fallback if URP Lit is unavailable).

## Runtime integration
Generated materials are stored in `Assets/Resources/MiaMax/Materials/` so they are included in player builds. The backyard bootstrap loads the generated lawn, plaster, roof, fence, wood, foliage, soil, path and bark materials when present, and retains its original colour-only fallback when the library has not yet been generated.

## Coverage
Grass/lawn, garden soil, warm wood, tree bark, cream plaster, roof shingles, patio stone, play sand, leaves, canvas fabric, ceramic glaze, painted metal, paving brick, pond water, flower petals, vegetable greens/reds/oranges/purples, gravel path, white fence paint, brown leather, ginger and cream fur starters, warm skin, blonde hair, blue denim, white cotton, green eyes, toy plastic, illustrated paper, interior wallpaper, wood and tile flooring, wicker, soft rubber, mossy stone, mushroom caps, berry skin and pond reeds.

Maps are generated deterministically, use periodic noise and repeat wrapping, and are compressed with mipmaps. Normal maps are imported as Unity normal maps. Materials use conservative smoothness/metallic values.

## Important limits
- This is a complete **starter surface library**, not a claim that every scene object has already been UV-unwrapped and assigned a final material.
- Mia and Max production skin/face/eye/clothing/fur atlases cannot be correctly authored until final production meshes and UV layouts exist. The ginger/cream fur surfaces are reusable material starters, not final character texture atlases.
- The generator is source-controlled; PNG and .mat assets are generated inside the user's Unity project when the menu command is run. GitHub's text-file API cannot commit generated binary PNGs through this workflow.
- Review visual tiling, colour response, shader compatibility and performance in the actual project and on target devices before release.
