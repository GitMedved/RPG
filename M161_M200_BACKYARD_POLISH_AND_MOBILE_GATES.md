# Mia & Max Adventures — M161–M200 implementation notes

## M161–M170 — Backyard ground-cover pass
- Added four restrained flower pockets at fixed positions away from the starter garden beds and sandbox interaction area.
- Added six low-profile lawn-edge stones.
- Reused the existing generated/fallback foliage, flower and soil materials.
- Decorative primitives continue through the shared helper that removes colliders; stone shadows are disabled.
- No gameplay state, save data, interaction targets or economy rules changed.

## M171–M180 — Static integration guard
- Gameplay & Character Integration Audit now checks that the ground-cover pass is present, uses fixed placement arrays, does not use runtime random placement, and retains decorative collider removal.
- The audit also checks the intended reduced-shadow policy for the small stones.
- This is a source-contract check; it does not instantiate the scene or measure actual draw calls.

## M181–M190 — Mobile performance acceptance
The block intentionally does not force global quality settings without device measurements. Review and record:
1. URP asset and renderer assignment for every quality tier.
2. Shadow distance, cascades and per-object shadow casting.
3. Shared material usage and draw-call count in the backyard camera view.
4. Texture compression, max texture size and mipmaps on Android/iOS.
5. Particle count/lifetime and overdraw in active gameplay.
6. Render scale and frame pacing on agreed low/mid-tier devices.
7. Memory and thermal behaviour during a 15-minute session.

## M191–M200 — Release evidence and handoff
- Keep procedural foliage, stones and all primitive environment elements labelled as prototype dressing until reviewed/replaced by production art.
- After pulling, run the texture generator and its audit, the production asset audit, the gameplay/character integration audit and core smoke checks.
- Open the saved prototype scene from a clean import, enter Play Mode, inspect collision/navigation around plots and sandbox, and test both portrait and landscape layouts.
- Record Unity version, device model/OS, build number, test result and logs/screenshots for each release gate.

## Validation boundary
GitHub source updates and static source checks do not execute Unity. No claim is made here that the project compiles, that the visual composition is approved, that frame-rate targets are met, or that Android/iOS builds pass. Those remain local acceptance gates.
