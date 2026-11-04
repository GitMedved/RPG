# MVP Roadmap and Risk Register

**Planning basis:** one Unity project, Android+iOS, stylized 3D, solo/offline-first. Estimates are rough person-weeks for an experienced small team; they are not calendar promises and exclude major custom character/environment production unless stated.

## 1. Milestones

| Milestone | Scope / exit criteria | Estimate |
|---|---|---:|
| M0 — Repository & editor foundation | Unity version pinned, clean project opens, URP and platform modules verified, folder/asmdef policy, CI/build smoke path | 1–2 weeks |
| M1 — Vertical slice | Backyard greybox, camera, touch/mouse movement, Mia/Max stand-ins, one interaction, basic HUD, scene boot | 2–4 weeks |
| M2 — Core data & persistence | Item definitions, inventory, wallet, XP, versioned local save, tests and recovery | 2–3 weeks |
| M3 — Farming & production | 3 crops, growth/offline calculation, harvest, 3 recipes, queue, order board | 3–5 weeks |
| M4 — Quest & world progression | Main/daily quest framework, reward flow, backyard gate, forest greybox and unlock | 2–4 weeks |
| M5 — Mini-games | Match-3, fishing timing loop, Max runner; shared lifecycle/rewards; tests | 5–8 weeks |
| M6 — Decoration & collections | Place/move/store, starter props, discovery book and completion reward | 2–4 weeks |
| M7 — UX/audio/content pass | Menus, settings, safe areas, localization keys, placeholder audio, tutorial polish | 3–5 weeks |
| M8 — Mobile hardening | Android+iOS builds, device testing, performance, accessibility, save/lifecycle regression | 3–6 weeks |

**Indicative total:** 23–41 person-weeks, before substantial custom art/animation, voice, external QA, store submission and compliance work. Mini-games and art pipeline are the largest uncertainty.

## 2. Recommended sequence
1. Audit repo/editor and agree asset pipeline.
2. Build one polished-enough vertical slice before parallel feature development.
3. Lock domain contracts for economy/inventory/save.
4. Add farming/production/quests.
5. Add each mini-game separately and connect through one reward contract.
6. Add exploration, decoration and collections.
7. Optimize and test on real devices.
8. Only then scope cloud sync, live events or monetisation integrations.

## 3. Risk register
| Risk | Impact | Mitigation |
|---|---|---|
| No supplied 3D character references/models yet | High | Use clearly labelled greybox stand-ins; obtain source references before final modelling/rigging |
| Unity Editor and package baseline not yet validated locally | High | Pin exact Editor patch after opening a clean project; do not guess package versions |
| Broad MVP scope | High | Vertical slice first; freeze acceptance criteria; defer future locations/events |
| Mobile performance of stylized 3D | High | Profile early on low/mid devices; URP, baked lighting, LOD, pooling |
| Save corruption/clock manipulation | High | Versioned DTO, atomic backup, validation and timestamp clamping |
| Mini-game implementation expands | High | Independent prototypes, strict MVP rule sets, shared reward contract |
| Children’s privacy and purchases | High | No data/ads/billing SDKs in MVP; parent gate prototype; specialist review before release |
| Asset licensing/source control | Medium | Track provenance; Git LFS for large binaries if needed; never commit generated Library/Temp |
| Cross-platform UI/input differences | Medium | Safe-area and lifecycle testing from vertical slice |
| Economy exploits | Medium | Central validation, idempotent reward claims, capped mini-game payouts |

## 4. Complexity estimates by MVP system
| System | Complexity | Main unknown |
|---|---|---|
| Backyard + camera | Medium | Art density, camera interaction |
| Character movement/animation | Medium–High | Rig/model/animation availability |
| Farming | Medium | Offline growth and feedback |
| Inventory/economy | Medium | Item rules and transaction invariants |
| Production/orders | Medium | Queue state and balance |
| XP/quests/dailies | Medium | Objective event model and reset rules |
| Match-3 | High | Board generation, cascades, blockers, level design |
| Fishing | Low–Medium | Input feel and catch timing |
| Endless runner | Medium | Procedural lanes, collision, difficulty curve |
| Adventure/forest | Medium–High | Environment content and quest interactions |
| Decoration | High | Placement, collision, persistence and UX |
| Collections | Low–Medium | Content definitions and unlock presentation |
| Save/migrations | High | Reliability, schema evolution and offline timestamps |
| Parent-gated shop mock | Medium | Gate UX and platform policy review |
| Android+iOS hardening | High | Device matrix, signing and actual hardware testing |

## 5. Definition of done per milestone
- Scope and acceptance criteria met.
- Unity compile succeeds in the pinned Editor.
- Automated tests pass for touched domain systems.
- No new console errors or unexplained warnings.
- Save/load and pause/resume tested where relevant.
- Android/iOS readiness reported separately; no platform claimed ready without an actual build and device smoke test.
- Documentation and development log updated.
- Changes reviewed before merging to main.


## 6. Actual branch progress (2026-11-03)

The feature branch has implemented prototypes for the backyard, local save, farming, production/orders, starter quests, three mini-game rule sets and arcade entry point, plus a five-item keepsake collection. This is not equivalent to the roadmap's original milestone numbering. The next unattended work should prioritize readable inventory/HUD and domain tests. A real Unity Editor import/compile and scene/device pass is a hard validation gate; remote source edits cannot replace it.


## 2026-11-03 — Next implementation package

Planned work: persistent onboarding, a compact safe-area-aware activity HUD, expanded arcade feedback, deterministic editor smoke checks, and Android/iOS build-readiness documentation. All new Unity work must be compiled and tested in the editor before it is considered verified.

## 7. Current implementation status — 2026-11-03

The current feature branch includes a generated-at-runtime backyard prototype, player movement and touch joystick, placeholder Mia/Max meshes, farming, production, orders, starter quests, keepsakes, local save, HUD/journal, three mini-game prototypes, a persistent six-step tutorial and two upgrade tracks (garden yield and workshop speed). See the M10 and M11 notes for exact scope and remaining validation.

**Not yet complete:** production-quality responsive UI, a full playable expansion location, full runner level generation, final character models/rigs/animations, decoration placement, automated tests, Editor compile validation after the latest package, and actual Android/iOS builds.

The project must not be described as mobile-ready until the Editor compile, clean Android/iOS builds and physical-device smoke tests pass.


## 8. M12 implementation status — 2026-11-03

Added persistent garden/workshop levels (0–2), validated on load. Garden upgrades cost 60 then 120 coins and add one crop to each future harvest per level. Workshop upgrades cost 60 then 120 coins and reduce newly started production jobs to 85% / 70% of base duration. Existing active jobs are not retroactively modified. Both upgrades are purchased from the Adventure Journal.

This package is source-committed only. Unity compile was confirmed by the user before this package; the new M12 code has not yet been compiled or tested in Play Mode. See M12_GARDEN_WORKSHOP_UPGRADES.md.


## 9. M13 implementation status — 2026-11-03

A small story-gated Flower Garden annex is now generated in the backyard scene. Claiming the final starter quest reveals the fenced annex. Entrance/return portals move the player between the backyard and annex. Three one-time flower resources grant inventory items, XP and coins, and their collection state is saved. This is a prototype blockout and requires Unity compile and Play Mode validation; it is not a separate streamed scene or final art.


## 10. M14 implementation status — 2026-11-03

Added a compact mobile HUD action dock. The status card remains upper-left, Adventure Journal remains upper-right, and Backpack plus contextual Interact controls sit in a bottom dock. The nearby object prompt is unified with the HUD and the existing keyboard E/Space path remains. This is still an IMGUI prototype; final Canvas UI, authored icons, animation, audio and device layout validation remain future work. M14 requires Unity compile and Play Mode validation. See M14_MOBILE_HUD_ACTION_DOCK.md.
 

## 8. M15–M18 implementation update — 2026-11-03

| Milestone | Implemented in branch | Still required before acceptance |
|---|---|---|
| M15 Visual production | Reference-locked art pipeline, asset intake paths, model/environment/UI deliverables and import acceptance documented | Add original reference binaries; import approved production 3D models, materials, environment assets and UI sprites |
| M16 Character animation | `CharacterAnimationBridge` reads movement, drives Speed/Grounded and exposes Interact/Celebrate triggers; player interaction invokes Interact | Import Mia/Max rigs, controllers and authored clips; attach/configure bridge; test animation in Play Mode |
| M17 Audio & voice | `GameAudioService` supports effects, music and voice independently; interaction requests optional SFX; English voice script/direction documented | Record/import licensed SFX, music and character lines; assign clips; test volume, pause and audio focus |
| M18 Integration/hardening | Mobile runtime pacing bootstrap and device test matrix documented | Unity compile, Play Mode, Android+iOS builds, physical-device performance and lifecycle regression |

**Important:** these are integration foundations, not proof of completed art/voice production or mobile release readiness. Unity Editor and physical devices are outside this remote repository workflow.


## 9. M19 — Backyard visual polish — 2026-11-04

Added and integrated a low-cost visual dressing pass for the temporary backyard: stepping stones, layered tree canopy, perimeter flower clusters, fence caps and small house/patio details. The geometry is non-interactive and has no colliders. This improves the greybox presentation but does not complete production art. Unity compile, visual review and mobile profiling remain pending. See M19_BACKYARD_VISUAL_POLISH.md.


## 10. M20 — Production asset readiness audit — 2026-11-04

Added an Editor-only menu utility at `Tools/Mia & Max/Production Asset Audit`. It checks the presence of source references, production models, Animator Controllers, UI sprites, SFX, voice and music folders and reports missing items as pending. The utility does not validate artistic quality, licences, rigging or runtime performance. It must be run in the user's Unity Editor after the branch is updated. See M20_PRODUCTION_ASSET_AUDIT.md.


## 11. M21 — Player settings & accessibility — 2026-11-04

Added persistent audio mix levels (master/effects/music/voice), UI scale (85–115%), vibration preference and reduced-motion preference. The prototype HUD now exposes a Settings panel; volume channels apply to the audio service and saved master volume applies to AudioListener. Vibration and reduced-motion are preferences only until gameplay events/animation systems consume them. Unity compile, Play Mode and device acceptance remain pending. See M21_PLAYER_SETTINGS_ACCESSIBILITY.md.


## 12. M22 — Daily goals & rewards — 2026-11-04

Added three persistent daily goals: harvest two crops, collect one crafted product and deliver one order. Successful domain actions update counters; completion grants 20 coins and 15 XP once per UTC day. Adventure Journal displays progress and claim action. Device clock rollback is ignored to prevent repeated resets. This remains offline/client-clock based and is not tamper-proof. Unity compile, save regression and Play Mode acceptance remain pending. See M22_DAILY_GOALS_REWARDS.md.
