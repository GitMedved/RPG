# Mia & Max — Backyard Environment Model Production Brief

## Purpose

Replace the 15 primitive backyard placeholders with a cohesive, family-friendly stylized 3D environment. The current placeholder prefabs are layout references only. Do not mark them production-ready or ship them as final art.

## Visual direction

- Bright, warm, inviting stylized 3D; soft rounded forms and readable silhouettes.
- Natural, restrained colors: warm cream house siding, muted charcoal-brown roof, clean warm-white fence, medium natural wood, layered greens, earthy soil and warm sand.
- Avoid neon green grass, hard plastic-looking foliage, excessive gloss, and sharp high-frequency detail.
- Match the supplied backyard reference composition. Preserve the landmark relationships: house and patio at the rear, large lawn in the center, mature tree, swing set, sandbox, perimeter picket fence, planting beds and patio furniture.
- Keep all assets visually consistent in bevel width, roughness, texel density, scale and stylization.

## Asset delivery rules

For each asset deliver:
1. Editable source project (for example Blender .blend) and exported FBX.
2. Textures in PNG or TGA, with packed ORM maps only if the chosen pipeline is documented.
3. One runtime prefab with clean hierarchy and descriptive names.
4. A short README entry recording author, source file, export settings, license/provenance, dimensions, polygon count and texture sizes.
5. One neutral-lit preview render or turntable image.
6. No gameplay scripts, player interaction components or gameplay colliders on visual-only art prefabs. Gameplay roots own interaction and collision.

## Technical targets

These are initial mobile targets, not hard Unity limits; optimize and profile on representative Android and iOS devices before release.

- Environment hero props: target 2,000–8,000 triangles each.
- Medium props: target 500–3,000 triangles each.
- Small props: target 100–1,000 triangles each.
- Prefer one 1K–2K texture set per hero asset; reuse trim sheets/materials for modular fence and wood pieces.
- Use URP/Lit-compatible materials; avoid custom shaders unless justified and tested.
- Apply transforms before export; consistent scale in metres; sensible pivot at ground contact or assembly origin.
- Correct normals, non-overlapping UVs where required, no missing materials, no hidden duplicate geometry.
- Use LODs for the house, mature tree and other assets whose screen size warrants them. Do not add LODs to tiny props without a measured benefit.
- Keep visual prefabs modular and collider-free; gameplay collision remains on separate gameplay roots.

## Required asset list

| Asset | Suggested visual scope | Initial scale envelope (metres) | Priority |
|---|---|---:|---|
| HouseExterior | Two-storey beige/light-cream clapboard house, white trim, dark shingle roof, doors and readable windows | 12 × 7 × 6 | P0 |
| PatioDeck | Deck boards, steps, seating/table and grill as separable pieces | 9 × 0.5 × 4 | P0 |
| FenceStraight | White picket module with rails and shaped picket tops | 3.2 × 1.7 × 0.2 | P0 |
| FenceCorner | Matching corner module with clean junctions | 3.2 × 1.7 × 3.2 | P0 |
| FenceGate | Matching hinged gate, readable opening and posts | 1.8 × 1.8 × 0.3 | P0 |
| MainTree | Mature broad-canopy tree, shaped trunk, layered foliage clusters | 5 × 7 × 5 | P0 |
| SwingSet | Wooden A-frame, crossbeam, ropes/chains and two safe-looking seats | 4 × 3.5 × 2 | P1 |
| Sandbox | Raised rectangular timber frame and detailed sand surface | 4.5 × 0.6 × 3.4 | P1 |
| ShrubCluster | Several varied rounded shrubs with natural silhouette variation | 3.5 × 1.4 × 1.5 | P2 |
| FlowerBed | Raised/edged soil bed with varied flowers and leaves | 3.2 × 1.0 × 1.3 | P2 |
| FlowerPot | Pot, soil, plant and flower as one readable prop | 0.7 × 1.5 × 0.7 | P2 |
| GardenPlot | Raised garden bed with soil furrows; leave crop slots clear for runtime crops | 1.8 × 0.5 × 1.5 | P1 |
| Workshop | Outdoor workbench, legs, toolboard and a few large readable tools | 2.5 × 2.0 × 1.2 | P1 |
| AdventureGate | Distinct wooden gateway to adventure areas | 2.7 × 2.8 × 0.4 | P2 |
| PathSegment | Modular warm gravel/stone path segment with clean edges | 2.4 × 0.15 × 3.5 | P2 |

## Acceptance checklist

- [ ] Silhouette and proportions reviewed against the supplied reference.
- [ ] Correct scale and pivot verified in Unity.
- [ ] No missing meshes, materials or textures.
- [ ] UVs and normals checked.
- [ ] Mobile polygon and texture budgets reviewed.
- [ ] No gameplay scripts or gameplay colliders embedded in visual prefab.
- [ ] LODs added where beneficial and checked for popping.
- [ ] License and source provenance recorded.
- [ ] Visual approval explicitly recorded by the project owner.
- [ ] Runtime integration tested without moving or replacing gameplay roots.

## Current status

All 15 assets currently in `Runtime/Prefabs` are primitive placeholders. The structural placeholder audit passed, but that result is not production-art approval. Production models must be imported and reviewed before runtime integration.
