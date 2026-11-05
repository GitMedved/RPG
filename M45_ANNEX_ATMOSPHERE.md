# M45 — Adventure annex atmosphere

## What changed

Added `AdventureAnnexAtmospherePrototype`, a lightweight ambient controller attached to each existing story-gated annex content root.

### Flower Garden
- Gentle, low-amplitude swaying of decorative flower stems, leaves and blooms.
- A subtle drifting pollen effect.
- Particle cap: 20; emission: approximately 1.5 particles per second.

### Woodland Trail
- Gentle swaying of secondary canopy clusters and fern fronds.
- A slow, warm firefly-like mote effect.
- Particle cap: 16; emission: approximately 1.1 particles per second.

## Runtime and performance notes
- One controller per annex; foliage transforms are cached once rather than searched every frame.
- Only the cached transforms are updated, with small rotations.
- Particle effects use local simulation, no collision module, no lights and no shadow casting.
- Particle systems are children of the existing gated content roots. They run only while that content is active and stop/clear when it is disabled.
- No changes to portals, area unlocks, resources, rewards, economy, save data or player controls.
- The effects use procedural primitives and generic particle materials; they are atmospheric prototypes, not final authored VFX.

## Unity acceptance checklist
1. Pull `codex/mia-max-foundation` and allow Unity to compile.
2. Unlock Flower Garden; confirm flowers move gently and pollen is subtle rather than distracting.
3. Unlock Woodland Trail; confirm canopy/ferns sway and motes remain sparse.
4. Exit each annex; confirm its ambient effect stops while the content is hidden.
5. Check that resource interactions and walking routes remain unchanged.
6. Test portrait and landscape Game views, then profile on a representative Android and iOS device.

## Verification status
Only source-level structural checks were performed. Unity compilation, Play Mode, visual quality and mobile profiling have not been run remotely.
