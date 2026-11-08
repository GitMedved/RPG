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


## 13. M23 — Lifetime achievements — 2026-11-04

Added seven lifetime achievements with persisted progress and one-time rewards. Successful gameplay actions update harvest/craft/delivery counters; collection, flower, arcade and level milestones are derived from existing saved state. Adventure Journal displays achievement progress and allows claiming the next available reward. This is offline local progression, not server-authoritative. Unity compile, Play Mode and save/load regression remain pending. See `M23_ACHIEVEMENTS_PROGRESS.md`.


## 14. M24 — Woodland Trail expansion — 2026-11-04

Added a fourth story quest after Flower Garden resource collection. Claiming it unlocks a new same-scene woodland annex through a configurable area portal. The annex contains three one-time persistent resources (acorn, mushroom, wild berries), a return gate and prototype environment dressing. Local save validation handles the new collection list without a schema bump. Unity compile, Play Mode, save/load and mobile acceptance remain pending. See `M24_WOODLAND_TRAIL_EXPANSION.md`.


## 15. M25 — Exploration achievement — 2026-11-04

Added the eighth lifetime achievement, Trailblazer, derived from the three persisted Woodland Trail discoveries. One-time reward: 30 coins and 25 XP. Expanded the scrollable Adventure Journal to show the full eight-achievement list and reward claim. Unity compile, Play Mode, save/load and mobile layout validation remain pending. See `M25_EXPLORATION_ACHIEVEMENT.md`.


## 16. M26 — Exploration and inventory feedback — 2026-11-04

Added a live Woodland Trail unlock/discovery status line in the Adventure Journal and friendly inventory names for flower and woodland resources. Increased scroll content to accommodate the new line. Mobile aspect-ratio text wrapping still needs Unity Editor/device validation. See `M26_EXPLORATION_INVENTORY_FEEDBACK.md`.


## 17. M27 — Interaction feedback — 2026-11-04

Added a small HUD toast for world interactions. The interactable publishes its configured name/action through a static event; the HUD subscribes only while enabled and displays the message for 2.6 seconds using unscaled time. No save schema or gameplay reward logic changed. Unity compile, Play Mode and mobile layout acceptance remain required. See `M27_INTERACTION_FEEDBACK.md`.


## 18. M28 — Accessibility feedback integration — 2026-11-04

World interaction feedback now requests vibration through the existing settings service, respecting the saved toggle and mobile-platform guard. CharacterAnimationBridge suppresses Interact/Celebrate one-shot triggers when Reduced Motion is enabled. This is partial accessibility coverage, not a project-wide reduced-motion implementation. Unity compile, Animator and device haptic checks remain pending. See `M28_ACCESSIBILITY_FEEDBACK.md`.


## 19. M29 — Daily exploration goal — 2026-11-04

Expanded the daily task set to four goals: harvest 2, collect a finished craft, deliver an order and discover one new Flower Garden/Woodland Trail resource. Added a persisted exploration counter and connected successful one-time resource pickups. Existing saves default the new field to zero without a schema bump. Reward remains 20 coins + 15 XP. Unity compile, old-save, UTC rollover and mobile journal layout checks remain pending. See `M29_DAILY_EXPLORATION_GOAL.md`.


## 20. M30 — Daily visit streak and gifts — 2026-11-04

Added persistent last-visit/last-claim UTC dates, current/best streaks and a repeating seven-day reward track (10–60 coins, 5–30 XP). A next-day visit increments the streak; a missed UTC day resets current streak to one. Added a HUD claim button and confirmation toast, with one reward claim per UTC date and overflow checks. Existing saves initialize the new data without a schema bump. Offline/client-clock limitation applies. Unity compile, save/load, UTC boundary and mobile HUD tests remain pending. See `M30_DAILY_VISIT_STREAK.md`.


## 21. M31 — Daily arcade reward cap — 2026-11-04

Added a shared five-successful-reward-claims-per-UTC-day limit across all three mini-games. The persisted count resets with daily tasks; Arcade menu displays usage and reports when the cap is reached. Per-run duplicate protection and score-banded reward values remain unchanged. This is an offline economy guard, not server-side anti-cheat. Unity compile, save/load, date rollover and arcade Play Mode checks remain pending. See `M31_DAILY_ARCADE_REWARD_CAP.md`.


## 22. M32 — World Map overview — 2026-11-04

Added a HUD World Map panel with the Backyard home base and story-gated Flower Garden/Woodland Trail, live unlock status and discovery counts. Locked-area text describes the next story requirement. Enforced mutual exclusion across Map, Journal, Settings and Backpack panels. No save schema change. Unity compile, progression state, safe-area and device layout checks remain pending. See `M32_WORLD_MAP_OVERVIEW.md`.


## 23. M33 — Responsive Arcade HUD — 2026-11-04

Wrapped the arcade IMGUI panel in a safe-area-aware transform, scaling from safe width/height with a 0.72–1.2 clamp and offsetting from top/side insets. Existing game controls and logical layout remain unchanged. Unity Game view, touch mapping, portrait/landscape and device checks remain pending. See `M33_RESPONSIVE_ARCADE_HUD.md`.


## 24. M34 — Canvas mobile HUD foundation — 2026-11-04

Added a runtime uGUI Screen Space Overlay Canvas with CanvasScaler, normalized safe-area anchors, status card, daily gift, top navigation and bottom action dock. The Canvas calls the existing PrototypeHud state/actions; detailed Backpack, Journal, Map and Settings contents remain on the legacy IMGUI renderers for this first migration slice. Added the `com.unity.ugui` package dependency. Unity package resolution, compilation, Play Mode, portrait/landscape and touch-device acceptance remain pending. See `M34_CANVAS_MOBILE_HUD.md`.


## 25. M35 — Canvas Backpack, World Map and feedback — 2026-11-04

Migrated Backpack and World Map presentation to a scrollable uGUI Canvas panel; added Canvas feedback toast and explicit panel close behavior. Journal and Settings remain on IMGUI pending their action-by-action migration. Unity compile, Play Mode, scrolling, aspect-ratio and touch validation remain pending. See `M35_CANVAS_BACKPACK_MAP_FEEDBACK.md`.


## 26. M36 — Canvas Settings & Accessibility — 2026-11-04

Migrated Settings to the shared Canvas detail panel. Audio channels, UI scale, vibration and reduced-motion preferences call the existing settings service and retain save persistence. Journal remains on IMGUI pending migration. Unity compile, save/reload, Play Mode, aspect-ratio and touch validation remain pending. See `M36_CANVAS_SETTINGS_ACCESSIBILITY.md`.


## 25. M35 — Canvas Adventure Journal — 2026-11-04

Migrated the Adventure Journal and its progression/economy actions to the responsive Canvas detail panel. The Canvas now owns the main HUD, backpack, map, settings and journal. Legacy IMGUI remains as a fallback only when a scene has no MobileHudCanvas. Unity compile, Play Mode, save/reload and device checks remain pending. See `M35_CANVAS_ADVENTURE_JOURNAL.md`.


## 26. M36 — Character FBX Import Pipeline — 2026-11-04

Added Editor tools for character asset folders, Animator parameter-contract scaffolds and character-specific FBX import profiles. Mia is configured for Humanoid only with a valid human rig; Max remains Generic/quadruped. The audit distinguishes scaffolds from finished animation and checks imported clips. Production models, clips and Unity validation remain pending. See `M36_CHARACTER_ASSET_PIPELINE.md`.


### M36 runtime character integration

Added a prefab generation step for approved base FBX files and runtime Resources loading with proxy fallback. Mia/Max movement, interaction and companion logic remain on their existing gameplay roots. Model scale, Avatar mapping, clips and animation quality still require Unity review.


## 27. M37 — Core Editor smoke checks — 2026-11-04

Added an Editor menu command, **Tools → Mia & Max → Run Core Smoke Checks**, covering five grouped checks: session economy/inventory, upgrades, production/orders and daily counters, daily reward idempotency, and local-save validation/backup recovery. Save tests use a unique temporary directory and clean it up. This improves repeatability for deterministic domain paths but is not Unity Test Framework coverage and does not replace scene/Play Mode/device testing. The command must be run in the local Unity Editor; no remote Unity compile or execution is claimed. See `M37_CORE_EDITOR_SMOKE_CHECKS.md`.


## 28. M38 — Tutorial flow repair — 2026-11-04

Fixed two missing tutorial progression hooks: successful planting now advances the guide from movement to harvesting, and successful harvesting advances it to crafting. Failed actions do not advance progress. Tutorial hints show the current step number. Expanded Editor smoke checks with a full six-step in-memory tutorial scenario (movement, planting, harvesting, crafting, delivery, arcade reward). Expected smoke result is 6/6. Unity compile, Editor execution, Play Mode and device checks remain pending. See `M38_TUTORIAL_FLOW_REPAIR.md`.


## 29. M39 — Garden visual pass — 2026-11-04

Replaced the generic crop sphere with distinct low-poly carrot, tomato and strawberry runtime silhouettes. Visuals follow saved crop IDs and growth stages without changing farming rules, rewards or persistence. Decorative parts have no colliders. The result is still procedural placeholder art; production crop prefabs/textures and final visual review remain future work. Unity compile, Play Mode and mobile performance checks are pending. See `M39_GARDEN_VISUAL_PASS.md`.


## 30. M40 — Backyard environment art pass — 2026-11-04

Added a coherent procedural dressing layer to the existing backyard: house door/windows/facade trim, garden plot edging, stepping-stone spur, patio planters and sandbox trim. Decorative colliders are removed and existing gameplay objects remain unchanged. This is still prototype geometry, not final authored environment art. Unity compilation, Play Mode visual/collision checks and mobile performance remain pending. See `M40_BACKYARD_ENVIRONMENT_ART_PASS.md`.


## 31. M41 — Backyard lighting & atmosphere — 2026-11-04

Added a warm daylight setup for the runtime prototype using the existing camera and directional sun: soft solid-color sky, three-tone ambient lighting, warm sunlight and softer shadows. No additional lights or gameplay changes. Final skybox, post-processing, baked lighting and mobile performance validation remain pending. See `M41_BACKYARD_LIGHTING_ATMOSPHERE.md`.


## 32. M42 — Backyard station visual pass — 2026-11-05

Added recognizable procedural dressing for the workshop, order board, Adventure Journal board, arcade cabinet and collection book. Original interactive roots/colliders and gameplay components are retained; added decorative colliders are removed. Unity compile, Play Mode interaction/collision, aspect-ratio and device performance checks remain pending. See `M42_BACKYARD_STATION_VISUAL_PASS.md`.


## 33. M43 — Character proxy animation polish — 2026-11-05

Added lightweight idle breathing, head motion, periodic blinking, speed-responsive gait and Max tail sway to procedural character proxies only. Production prefab loading, Animator contracts and gameplay movement remain unchanged. Unity compilation, Play Mode animation review and mobile performance checks remain pending. See `M43_CHARACTER_PROXY_ANIMATION_POLISH.md`.


## 34. M44 — Flower Garden & Woodland Trail visual pass — 2026-11-05

Added themed procedural environment dressing to both gated annexes while preserving story unlocks, portals, resource interactions and progression. All new decorative primitive colliders are removed. Unity compilation, Play Mode traversal/interaction and device performance checks remain pending. See `M44_ADVENTURE_ANNEX_VISUAL_PASS.md`.

## 35. M45 — Annex atmosphere and ambient motion — 2026-11-05

Added low-amplitude foliage sway and capped ambient pollen/firefly-like particles to the two story-gated annexes. Each annex uses one controller; effects activate only with the unlocked content root, use no extra lights/colliders and do not change gameplay or save data. Unity compilation, Play Mode and device performance checks remain pending. See `M45_ANNEX_ATMOSPHERE.md`.

## 36. M46 — Main backyard ambient life — 2026-11-05

Added cached, low-amplitude wind motion for decorative foliage in the main yard. The motion respects Reduced Motion. Optional wind and bird loops load from Resources and respect the effects-volume setting; actual sound files remain pending. No gameplay colliders, save data or interaction behavior changed. See `M46_BACKYARD_AMBIENCE.md`.

## M47 — Production character runtime hardening — 2026-11-05

- Hardened Animator parameter binding and guarded missing parameters.
- Disabled root motion on generated runtime character prefabs to prevent conflicts with gameplay controllers.
- Added an Editor validator for Mia/Max runtime prefabs, imported Avatar validity, assigned controller, root-motion setting and parameter contract.
- Production FBX models, authored clips, blend trees, state transitions, final rig review and device animation profiling remain pending.


## 37. M48 — Character Animator state machines — 2026-11-05

Added an Editor builder for the Mia/Max Animator contract controllers. It maps available imported clips to Idle/Walk/Run/Interact/Celebrate and wires movement/action transitions; missing clips remain clearly reported. The tool does not generate animation assets, validate animation quality or replace the need for local Animator/Play Mode review. It rebuilds the generated Base Layer. Unity compile, imported clip review, Play Mode and device checks remain pending. See `M48_CHARACTER_ANIMATOR_STATE_MACHINES.md`.


## 38. M50 — Contextual character voice triggers — 2026-11-05

Connected successful farming, workshop and keepsake outcomes to stable Mia/Max voice IDs with a centralized 2.5-second same-cue cooldown. Missing recordings remain silent and do not block gameplay. See `M50_CONTEXTUAL_VOICE_TRIGGERS.md`.

## 39. M51 — Voice asset readiness audit — 2026-11-05

Added a read-only Editor command that checks the 16 named M49 audition clips in their exact Resources folder and reports available/missing assets. Corrected the general asset audit folder. The tool does not create placeholders or certify licensing/quality. See `M51_VOICE_ASSET_AUDIT.md`.


## 41. M53 — Release readiness audit and pre-release plan — 2026-11-05

Added a read-only Editor release audit covering Build Settings scenes, Android/iOS identifiers, product identity/version, URP asset presence, production character models/prefabs/animation, generated texture sets, UI sprites and audio assets. The audit intentionally reports manual blockers for executed smoke tests, physical-device validation and privacy/child-safety/store review; it does not claim to build or certify the game. Added `PRE_RELEASE_PLAN.md` with phased acceptance gates from local verification through production art, gameplay, UI, audio, technical hardening, Android/iOS QA and store submission. Source review only; Unity compilation and audit execution remain pending.


## 42. M54 — Mobile joystick safe-area hardening — 2026-11-05

Made the prototype touch joystick safe-area-aware and responsive to screen width. Touch sensitivity now matches its rendered radius, and movement resets on app pause/focus loss or component disable to prevent stuck input. The control remains an IMGUI prototype; final Canvas art and device validation are still required. See `M54_MOBILE_JOYSTICK_HARDENING.md`.


## 43. M55 — Canvas joystick and UI migration audit — 2026-11-05

Migrated the prototype joystick visual from IMGUI to a dedicated Screen Space Overlay Canvas. Touch handling, safe-area hit region, dead zone and movement vector remain unchanged; the Canvas does not intercept HUD button input and adapts to screen/safe-area/Canvas-scale changes. Added an Editor audit for remaining OnGUI entry points. PrototypeHud still contains an intentional IMGUI fallback if MobileHudCanvas is missing, so the prototype UI is not yet completely free of IMGUI. Unity compilation, audit execution, portrait/landscape, touch and device checks remain pending. See M55_CANVAS_JOYSTICK_AND_UI_AUDIT.md.


## 44. M56 — Responsive Canvas detail panels — 2026-11-05

Made the shared Canvas detail sheet fit the active safe-area viewport by clamping its width and height, retaining the current design dimensions when the display allows and shrinking on landscape/shorter layouts. Extended the UI migration audit to check this responsive path. Panel interaction, scrolling, portrait/landscape layout and phone/tablet behavior still require local Unity validation. See M56_RESPONSIVE_CANVAS_DETAIL_PANELS.md.


## 45. M57 — Compact responsive detail panels — 2026-11-06

Adjusted the shared detail sheet's internal layout for landscape and shorter displays. Settings and journal action rows compress to fit a dedicated bottom action zone, while the scroll viewport is resized to the actual remaining panel area. All existing callbacks are preserved. The UI audit checks for the new responsive path. Local Unity compile, visual and touch validation remain required. See M57_COMPACT_RESPONSIVE_DETAIL_PANELS.md.


## 46. M58 — Shared mobile UI theme — 2026-11-06

Centralized the runtime HUD palette and button interaction states in `MobileUiTheme`; migrated the Canvas HUD's button construction, panel tokens and enabled/disabled feedback to the shared theme. Extended the UI audit to verify the integration. This is still prototype styling without final approved fonts/sprites. Unity compilation, visual acceptance, accessibility contrast and device checks remain required. See M58_SHARED_MOBILE_UI_THEME.md.


## 47. M59 — Mobile UI contrast and readable labels — 2026-11-06

Added contrast-aware button label colors, adjusted the leaf-green action token and extended the Editor UI audit with a 4.5:1 contrast check for core button backgrounds. This is a focused palette check only; complete accessibility and device validation remain required. See M59_MOBILE_UI_CONTRAST.md.

 
## 48. M60–M70 — UI, gameplay content and character integration foundations — 2026-11-06

Implemented reusable Canvas card/state components, a route navigation stack, shared button typography, read-only quest snapshots, an authorable backyard content catalog, expanded character animation actions and a consolidated Editor integration audit. Existing domain economy/reward systems remain unchanged and authoritative. This is an engineering foundation package, not acceptance of finished screens, balanced content, final art or animation. See `M60_M70_IMPLEMENTATION_PACKAGE.md`.

**Still required:** migrate each screen to the shared components/navigation; author and review the content catalog; tune economy through playtesting; import approved Mia/Max production meshes and clips; run all Editor audits and Unity compile/Play Mode checks; test save/reload and physical devices.


## 49. M71–M75 — Save resilience and content authoring pipeline — 2026-11-06

Improved interrupted-write recovery and save payload normalization, added an additive starter content catalog builder for known crops/recipes/orders/quests/mini-games, and expanded the integration audit to check all 16 known IDs. Added smoke coverage for save normalization, temporary-file recovery and duplicate-safe catalog authoring. The expected Editor smoke suite is now 8/8, but has not yet been run locally. See `M71_M75_SAVE_AND_CONTENT_PIPELINE.md`.

**Next:** run the local smoke suite and create/review the catalog asset; then wire catalog-backed presentation only where it can remain consistent with authoritative gameplay services. Continue production art/animation/audio work and complete full gameplay/save/device acceptance.


## M76–M80 — Content catalog reliability — 2026-11-08

The authoring catalog now has indexed ID lookup, case-insensitive duplicate protection, kind-filtered queries and stricter validation. Core smoke checks cover lookup semantics and required item references; the gameplay audit validates all 16 starter IDs against their expected content kinds. The catalog remains an authoring/index layer; domain services remain authoritative for economy and progression. Local Unity validation is required before wiring catalog data into runtime screens. See M76_M80_CONTENT_CATALOG_RELIABILITY.md.


## M81–M85 — Progression and reward transaction hardening

- Player level progression is bounded at level 100, including normalization of legacy saves; XP thresholds are safe from integer overflow.
- Combined coin/XP rewards are validated before either balance changes; quest claims cannot be recorded when the reward is rejected.
- Core Editor smoke coverage expanded to nine grouped checks; local Unity execution and Play Mode acceptance remain required.
- No save schema change. See `M81_M85_PROGRESSION_REWARD_HARDENING.md`.


## M86–M90 — Runtime content catalog integration

- Story quest names, displayed rewards and payout values now resolve from the runtime Resources catalog, with safe built-in fallback.
- Starter catalog builder writes to Resources and preserves/migrates the older prototype asset.
- Editor smoke coverage expanded to 10 scenarios; runtime and build validation remain pending.
- No save schema change. See `M86_M90_RUNTIME_CONTENT_CATALOG.md`.


## M91–M100 — First-release gameplay loop integration

- Recipes and order deliveries now consume authored catalog definitions, including configurable requirements, outputs, rewards and durations; built-in definitions remain the fallback.
- Story goals support validated prerequisites. The opening quest chain is sequential and claims unlock Flower Garden, then Woodland Trail.
- Catalog recipes and orders respect their area's unlock state.
- Inventory, production start/collection and order delivery use preflighted state transitions so save listeners observe completed transactions.
- Editor smoke suite expanded to 13 scenarios, including custom catalog recipes/orders, area gates and a fresh-profile craft/deliver/quest/save loop.
- No save schema change. Source changes and static review only; Unity compile, smoke execution, integration audit, Play Mode and device acceptance remain pending. See M91_M100_FIRST_RELEASE_GAMEPLAY_LOOP.md.


## M101–M110 — Inventory, economy and progression consistency

- Added InventoryService as the query/mutation facade with detached snapshots; production and orders use its quantity checks.
- Added production CanStart/CanCollect presentation affordances.
- Daily-task and achievement rewards now use GameSession's shared bounded XP/reward transaction.
- Garden and Workshop upgrades deduct currency and increment their level atomically in one state notification.
- Expanded Core Smoke Checks to 15 scenarios and extended Gameplay & Character Integration Audit.
- No save schema change. Static source review only; Unity compile, smoke execution and Play Mode remain pending. See M101_M110_INVENTORY_ECONOMY_PROGRESSION.md.


## M111–M120 — Economy balance and garden/workshop progression

- Added EconomyBalance as the single source of truth for upgrade prices, garden harvest yield, harvest rewards and workshop production-time multipliers.
- UpgradeService now exposes affordability checks and next-level previews for garden yield and workshop production duration; the maximum upgrade level comes from the shared balance rules.
- Workshop duration uses deterministic integer percentage arithmetic, avoiding floating-point rounding errors.
- Harvest now preflights inventory/currency/XP capacity, clears the plot and grants crop plus rewards as one observable transaction; rejected rewards preserve the mature crop.
- Plant and harvest XP/coin values are centralized. No save schema change.
- Core Editor Smoke Checks expanded to 16 scenarios, including economy curves, upgrade preview, successful upgraded harvest and overflow rollback.
- Gameplay & Character Integration Audit now checks the economy balance service and atomic harvest contract.
- Static source review only. Unity compile, 16/16 smoke execution, audit, Play Mode and device QA remain pending. See M111_M120_ECONOMY_AND_GARDEN_UPGRADES.md.


## M121–M130 — Garden/workshop feedback and mobile upgrade UX

- FarmingService exposes normalized crop growth progress and a seconds-until-harvest countdown, using the same growth timing as harvest eligibility.
- FarmPlotPrototype updates on a bounded 0.25-second cadence, smoothly scales crop visuals across growth stages and displays contextual planting/growth/harvest prompts.
- ProductionService exposes normalized workshop progress; the workshop interaction prompt shows remaining time and percentage, plus the current workshop speed level when idle.
- Adventure Journal upgrade summaries show current garden yield, next-level yield, upgrade cost and affordability; workshop upgrade affordability is also shown.
- Mobile journal upgrade actions visually reflect whether the player can currently afford the purchase.
- Smoke coverage now checks crop growth progress/countdown and normalized production progress within the existing 16 scenarios. Integration audit covers the new runtime/presentation contracts.
- No save schema change. Static source review only; Unity compile, smoke execution, Play Mode and device QA remain pending. See M121_M130_GARDEN_WORKSHOP_FEEDBACK.md.


## M131–M140 — Garden/workshop interaction feedback (implemented in source)
- Added low-cost, one-shot particle feedback for planting, harvest and workshop state changes.
- Added restrained crop idle sway with Reduced Motion support.
- Extended static integration audit to verify VFX wiring.
- Unity compilation, editor checks, Play Mode and device validation remain manual gates.


## M141–M150 — Production character pipeline and asset validation
- Runtime prefab pipeline now binds the animation bridge to imported Animator components and validates render geometry plus the complete parameter contract.
- Character bridge refreshes its parameter cache when an Animator Controller is replaced.
- Production asset audit handles absent folders without avoidable AssetDatabase missing-folder warnings.
- Added `CHARACTER_PRODUCTION_HANDOFF.md` with the approved-reference, FBX, rig, clip and validation handoff contract.
- Production FBX models, UV-specific textures, authored animation clips and device validation remain outstanding release gates.


### M131–M140 follow-up — compile repair
- Corrected the VFX script's domain/bootstrap namespace imports and explicitly enabled Unity's built-in Particle System module in the package manifest.
- Integration audit now verifies those compile dependencies, not only VFX call-site presence.
- Local Unity package resolution and recompilation remain required after pull.
