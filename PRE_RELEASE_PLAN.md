# Mia & Max Adventures — pre-release plan

**Target:** Android + iOS, stylized 3D, family audience, single-player, offline-first.
**Release principle:** no milestone is accepted from source-code inspection alone. Each gate needs Unity Editor, Play Mode, build and/or device evidence as appropriate.

## Current state (honest snapshot)

The repository contains a playable-prototype foundation: backyard setup, movement/interactions, farming, workshop production, orders, quests/tutorial, progression/upgrades, daily tasks/rewards, collections, two gated annexes, arcade foundations, local JSON saves with backup recovery, accessibility settings, mobile HUD scaffolding, audio/voice playback hooks, texture generation and Editor smoke/audit commands.

Still not release-ready:
- Mia and Max are procedural proxy characters; approved production meshes, UVs, rigs and authored animation clips are not confirmed.
- Environment, crop, prop and UI art are still largely procedural/prototype dressing. M52's 40-surface library is generated locally by a Unity Editor command and must be run, reviewed and assigned; it is not a finished art pack.
- Voice, SFX, music and ambience playback paths exist, but actual licensed recordings are not confirmed.
- Some HUD elements and touch controls remain prototype scaffolding.
- No successful clean Android/iOS player builds or physical-device test evidence is recorded.
- Privacy, child-safety, age-rating, store metadata and legal reviews are open.

## Release phases and acceptance gates

### P0 — Local project verification
- Pull the feature branch and open the project in the agreed Unity 6000.3.x editor.
- Resolve packages; confirm URP asset and renderer are assigned in Graphics and every Quality tier.
- Run texture generation, texture audit, production asset audit, voice audit and core smoke checks.
- Fix every compiler error, missing script, missing reference and recurring runtime exception.
- Save the generated prototype scene and ensure it is included in Build Settings.
**Exit:** clean Editor compile; all core smoke checks pass; scene opens and enters Play Mode from a clean import.

### P1 — Final art and character production
- Import approved Mia and Max models based on the supplied references; validate silhouette, face, proportions and palette.
- Complete UVs and character-specific albedo/normal maps; do not use generic fur/skin surface swatches as final atlases.
- Rig both characters and author Idle, Walk, Run, Interact, Garden, Celebrate, Reaction; Max also needs cat-specific locomotion, sit, sleep, jump and play actions.
- Replace proxy crops, plants, house, fence, stations, annex props and collectibles with coherent modular production assets.
- Run texture audit; tune tiling, texel density, compression, mipmaps, LODs, colliders and shared materials.
**Exit:** no visible greybox/proxy assets in the release path; all key art approved against references; no broken materials or missing dependencies.

### P2 — Gameplay completeness and balance
- Play the full new-player loop: onboarding → move/interact → plant → grow/wait → harvest → craft → order delivery → upgrade → unlock annex → explore → arcade → rewards.
- Confirm all gates, quest conditions, costs, timers, reward caps and tutorial transitions can be completed without softlocks.
- Verify all mini-games have complete start, pause/exit, success/failure, reward and replay flows.
- Tune progression and economy for a relaxed family game; remove dead ends, duplicate rewards and exploit loops.
- Add enough authored quest/order/task/collectible content for the intended first-session and return-session experience.
**Exit:** a fresh player can complete the intended first session and core progression without developer intervention; all reward paths are idempotent and balanced.

### P3 — UI, accessibility and localization
- Replace prototype runtime GUI and placeholder panels with final responsive Canvas UI and approved iconography.
- Review portrait layout on short/tall screens, notches, safe areas, tablets and large text.
- Complete pause/resume, confirmation dialogs, loading/error states, empty states, settings and clear navigation.
- Check touch target sizes, readable contrast, text scaling, reduced motion, volume sliders and vibration toggle.
- Move all user-facing strings to localization keys; finalize English copy and prepare a localization workflow.
**Exit:** no debug/prototype UI remains; all screens are reachable; no clipped text or inaccessible controls on target aspect ratios.

### P4 — Audio, animation and feedback
- Record or generate approved American-English Mia lines and Max vocalizations using a provider/licence that permits game distribution.
- Add SFX for planting, harvesting, crafting, delivery, collecting, upgrades, arcade actions, UI and rewards.
- Add music and location ambience; mix separate Music, Effects and Voice buses.
- Import clips with mobile-friendly settings and verify ducking, interruption, cooldown and mute/volume settings.
- Review animation blending, foot sliding, root motion, facial expression, timing and character voice/action sync.
**Exit:** all critical actions have readable visual/audio feedback; all assets have provenance and distribution rights; no missing voice/SFX references.

### P5 — Technical hardening and data integrity
- Test save creation, repeated saves, backup restore, corrupted primary file, app termination, pause/resume and schema migration.
- Test offline operation, device storage pressure, low-memory resume, audio focus interruption and orientation/safe area changes.
- Add graceful handling for missing optional assets and recoverable failures; remove debug logs and development-only shortcuts.
- Profile CPU/GPU, memory, allocations, draw calls, loading and thermal behaviour on agreed low/mid-tier devices.
- Tune URP quality tiers, texture compression, shadows, render scale, LODs, pooling and particle caps.
**Exit:** no save loss/duplication in regression tests; stable 30 FPS minimum on the agreed low-tier Android device and target 60 FPS where hardware permits; no sustained thermal or memory regression during a 15-minute session.

### P6 — Platform builds and device QA
- Set final company/product identity, Android application ID, iOS bundle ID, version/build numbers, icons and splash screen.
- Configure Android ARM64, target API, Vulkan/OpenGLES fallback and signing; create APK for QA and AAB for Play Console.
- Generate the iOS Xcode project; configure signing/provisioning and build for physical iPhone/iPad.
- Test fresh install, upgrade install, first launch, offline mode, safe areas, touch, audio, background/foreground, save/reload and uninstall/reinstall expectations.
- Capture build logs, device matrix, known issues and reproducible test results.
**Exit:** clean release builds install and pass the smoke matrix on physical Android and iOS devices.

### P7 — Family safety, legal and store submission
- Decide whether the product is child-directed and complete the applicable privacy/data, consent, age-rating and family-policy review for each target store/territory.
- Keep accounts, chat, user-generated content, analytics, ads and real-money purchases out of the initial release unless separately designed and reviewed.
- If monetization is added later, use platform-compliant purchases and parent-gated flows; never expose children to manipulative purchase prompts.
- Publish privacy policy, support contact, store description, screenshots, feature graphic, age rating and content disclosures.
- Prepare crash/support process, version rollback plan and post-release monitoring consistent with privacy commitments.
**Exit:** legal/platform checklist signed off; store metadata complete; submissions accepted by both stores.

## Prioritization

**Must-have for first public release:** stable offline core loop, production Mia/Max, coherent backyard and first annex, completed tutorial, farming/workshop/orders, at least two polished mini-games, save recovery, responsive UI, licensed audio, Android/iOS builds, physical-device QA, privacy/child-safety review.

**Can follow after launch:** additional locations, more mini-games, expanded collection sets, seasonal events, cloud sync, accounts, multiplayer, analytics, ads or real-money monetization. These are not release blockers unless later product decisions make them part of the launch promise.

## Evidence tracker

Record the date, Unity version, device model/OS, build number, test result and link to logs/screenshots for each gate. A green static audit is only a checklist result; it never substitutes for a clean build or a real-device test.

Run **Tools → Mia & Max → Release Readiness Audit** for a first static report, then run the dedicated texture, production, voice and core smoke audits. The release audit deliberately keeps physical-device and legal sign-off open until a human records evidence.
