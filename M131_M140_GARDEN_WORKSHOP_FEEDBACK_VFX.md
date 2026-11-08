# M131–M140 — Garden, Workshop and Interaction Feedback

## Implemented on `codex/mia-max-foundation`

- Added `BackyardFeedbackVfx`, a lightweight shared ParticleSystem utility for planting, harvesting, workshop start, progress milestones and completed production.
- Integrated planting and harvest bursts into `FarmPlotPrototype`; successful actions only emit feedback.
- Added restrained crop sway while a crop is visible and disabled animated crop sway and particle bursts when Reduced Motion is enabled.
- Integrated workshop start/completion bursts and three intermediate progress puffs, driven by normalized production progress rather than per-frame particle spawning.
- Extended the Gameplay & Character Integration Audit to check the VFX utility and both gameplay integrations.
- Kept effects procedural and asset-free so the project does not depend on missing textures, prefabs, packages or audio clips.

## Visual and technical constraints

- Effects are one-shot, capped to a small particle count, use a shared unlit material and destroy their GameObjects when complete.
- Existing gameplay/domain reward transactions are unchanged; effects are presentation-only and cannot grant items or currency.
- No new character design assumptions were introduced. The approved Mia/Max reference images remain the source of truth for production models.
- Procedural character meshes remain clearly labelled proxies. Final rigged models, authored animation clips and recorded voice takes are still separate production tasks.

## Validation status

- Source-level checks: verify that the new utility is referenced by garden/workshop presentation and checked by the integration audit.
- Not run here: Unity compilation, Editor Smoke Checks, Gameplay & Character Integration Audit, Play Mode, visual comparison, portrait/landscape layout, Android/iOS builds or physical-device profiling.
- If the earlier local workshop upgrade assertion still reports that level one does not change 180 seconds to 153, first pull the current branch and rerun the smoke suite: the current balance formula is 180 × 85% = 153 and the upgrade service reads the persisted workshop level. Do not treat a source inspection as a successful Unity test.
