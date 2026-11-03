# M18 — Visual/audio integration and mobile hardening

**Status:** runtime foundations and checklist implemented; Unity/device validation pending.

## Runtime changes
- `MobileRuntimeBootstrap.cs` sets a conservative 60 FPS target, disables VSync for mobile pacing, and keeps the screen awake during play. It does not force resolution or overwrite project quality tiers.
- `CharacterAnimationBridge.cs` provides the Animator contract for imported character rigs.
- `GameAudioService.cs` provides optional effects, music and voice playback.
- Interaction calls the optional audio cue and player animation trigger.

## Required Editor setup
1. Open the project in the exact Unity Editor version recorded by the project and resolve packages.
2. Assign URP Pipeline Asset and Renderer Data in Graphics and Quality settings.
3. Create Low / Medium / High quality tiers only after profiling; use conservative shadow distance, MSAA and render scale on mobile.
4. Import approved Mia/Max models, rigs, clips, environment prefabs, UI sprites and licensed audio.
5. Assign Animator Controllers and verify the M16 parameter contract.
6. Assign audio clips to Game Audio Service fields in a scene/prefab or add a small scene audio configuration component. The automatically created service is silent by default until clips are assigned.
7. Confirm portrait safe area and landscape behaviour on actual devices.

## Performance targets (initial, to be profiled)
- 30 FPS minimum on the agreed low-tier Android test device; target 60 FPS on mid/high devices where thermals permit.
- Avoid sustained thermal throttling and memory growth during a 15-minute session.
- Pool repeated transient effects and avoid per-frame allocations in gameplay loops.
- Prefer baked lighting, shared materials, LODs, instancing and compressed textures.
- Profile CPU, GPU, memory, draw calls and battery/thermal behaviour; do not infer performance from Editor Play Mode.

## Device matrix
At minimum test one low/mid Android phone, one current iPhone and one smaller-screen device. Test install, first launch, portrait safe areas/notches, touch controls, audio focus, pause/resume, background/foreground, save/reload, low battery mode and offline play.

## Release blockers
- Production models/rigs/animation clips/audio are not yet in the repository.
- Original image references must be copied into Reference/ using the M15 paths.
- Android and iOS signing, provisioning, store compliance and actual builds are not verified.
- Child-directed product privacy, parental controls, accessibility, platform policies and legal review remain release requirements.
- No ads, billing, analytics, cloud services or user accounts are added by M15–M18.

## Definition of done
Unity compile clean; Play Mode smoke tests pass; no missing references; visual and voice assets have provenance; save/pause/resume regression passes; Android and iOS builds each succeed; both are smoke-tested on real devices; performance targets are met. A remote source commit is not proof of any of these checks.
