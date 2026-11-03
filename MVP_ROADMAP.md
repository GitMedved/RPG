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

The current feature branch includes a generated-at-runtime backyard prototype, player movement and touch joystick, placeholder Mia/Max meshes, farming, production, orders, starter quests, keepsakes, local save, HUD/journal, three mini-game prototypes and a persistent six-step tutorial. See the M10 and M11 notes for exact scope and remaining validation.

**Not yet complete:** production-quality responsive UI, a full playable expansion location, full runner level generation, final character models/rigs/animations, decoration placement, automated tests, Editor compile validation after the latest package, and actual Android/iOS builds.

The project must not be described as mobile-ready until the Editor compile, clean Android/iOS builds and physical-device smoke tests pass.
