# Mia & Max Adventures — Release Candidate freeze and execution checklist

## Release policy

The next target is a **first playable Release Candidate**, not a feature-complete long-term vision. Freeze new features. Only accept changes that fix a release blocker, improve stability/accessibility, or complete required launch content. Do not add accounts, multiplayer, ads, analytics or online services to this offline-first family MVP.

## Repository facts observed on 2026-11-08

- Unity editor version recorded by the repository: 6000.3.0f1.
- The feature branch is `codex/mia-max-foundation`.
- Commit `ee76ae427e031452973ad7fab798fa124ac69310` includes the saved `Assets/_Project/Scenes/Backyard.unity` scene and `ProjectSettings/EditorBuildSettings.asset`.
- The saved Backyard scene is the sole enabled Build Settings scene in the developer-reported local audit. `UntitledBackup.unity` is retained but is not a build scene.
- No GitHub Actions workflow is currently committed.
- Character image references, backyard and room reference collages, and a 40-material / 40-map-per-channel generated surface library are present.
- The repository snapshot contains no production Mia/Max FBX models, runtime character prefabs, authored character animation clips, production UI sprites, voice recordings, SFX, or music/ambience clips.
- The project is a playable prototype foundation, not a release candidate. A clean-clone open/build check is still required.

## Latest developer-reported validation (2026-11-08)

- Core Smoke Checks passed **16/16** in the developer's Unity Editor session. These cover deterministic domain/save paths only; they do not cover Play Mode, scene interaction, UI layout, FBX import, or mobile builds.
- The local Release Readiness Audit reported one valid enabled scene, persisted Build Settings, configured Android/iOS identifiers, company/product identity, release version 1.0, URP assignments and all 40 generated materials plus 40 Albedo, 40 Normal and 40 Height maps.
- The audit reported **11 / 22 static checks ready** and **11 open blockers**: production Mia model, production Max model, runtime character prefabs, animation clips, UI production sprites, voice recordings, sound effects, music/ambience, session smoke-check evidence, privacy/child-safety/store review, and physical-device validation.
- The local smoke-check pass is session-scoped convenience, not portable or commit-bound QA evidence; retain the Console log with the tested commit SHA.
- Git push of commit `ee76ae427e031452973ad7fab798fa124ac69310` to `origin/codex/mia-max-foundation` succeeded. The branch is now published.

## RC0 — Reproducible Unity project

**Repository-content portion: completed in commit `ee76ae4`.** The saved scene, its metadata and Build Settings are committed.

Remaining clean-clone verification:
1. Pull `codex/mia-max-foundation`.
2. Open with Unity 6000.3.0f1 and wait for package resolution/import.
3. Confirm clean Editor compilation and no missing scripts/references.
4. Close Unity, reopen the project from the committed branch and confirm `Assets/_Project/Scenes/Backyard.unity` loads as the startup scene without relying on an unsaved editor state.
5. Enter Play Mode and verify the saved scene starts correctly.

**Exit evidence:** clean clone/import; clean Editor compile; saved scene opens and enters Play Mode without missing references.

## RC1 — Functional acceptance

Run and record:
- Core Smoke Checks.
- Gameplay & Character Integration Audit.
- Production Asset Audit.
- Voice Asset Audit.
- Texture Library generation and audit.
- Missing Script References audit.
- Full first-session playthrough: onboarding → movement/interactions → plant → harvest → workshop → order delivery → upgrade → unlock/explore → arcade → save/reload.
- Fresh profile and corrupted-save recovery.
- Pause/resume, offline launch and repeated save/reload.

**Exit evidence:** logs from the exact commit, with every critical test passing and no reproducible blocker.

## RC2 — Content acceptance

The current prototype proxies are not release-quality characters. Before public release:
- Import approved Mia and Max production meshes based on the supplied references.
- Complete rigs, avatars, required animation clips and runtime prefabs.
- Replace or explicitly approve all visible placeholder environment, crop, station and UI art.
- Supply licensed American-English Mia/Max voice assets, SFX, music and ambience, or make a deliberate content-scope decision to ship without any missing audio.
- Verify all asset licences permit commercial game distribution.

**Exit evidence:** visual sign-off, animation review, asset provenance/licence record and no visible unintended placeholder assets in the release path.

## RC3 — Mobile build and device QA

- Confirm Android and iOS Build Support modules are installed in Unity Hub.
- Configure product identity, unique application ID and bundle ID with values controlled by the publisher.
- Configure Android ARM64, signing, target API and an agreed graphics API fallback.
- Configure iOS signing/team and generate an Xcode project.
- Build Android APK for QA and Android App Bundle for store testing; build and install iOS on a physical device.
- Test install, first launch, safe areas, touch, audio interruption, background/resume, offline operation, save/reload and uninstall/reinstall expectations.
- Profile CPU/GPU, memory, thermal behaviour and frame pacing on agreed low/mid-tier devices.

**Exit evidence:** build logs, device/OS matrix, screenshots/video, profiler captures and no release-blocking defects.

## RC4 — Store and family-safety sign-off

Publisher must supply/approve:
- Legal publisher/company identity and support contact.
- Privacy policy and applicable child-directed privacy/data assessment.
- Age rating and content questionnaire answers.
- Store description, screenshots, icons, feature graphic and localized metadata.
- Rights/licences for every third-party asset, voice and music recording.
- Store account access, signing credentials and final release approval.

Do not claim compliance or store acceptance based only on source code. No public submission should happen before the publisher/legal review is complete.

## What the automated Release Readiness Audit can prove

The audit checks static project configuration and asset presence. It cannot prove likeness quality, animation quality, legal rights, gameplay fun, build success, device stability, store compliance or publisher approval. Manual gates remain explicit.

## Immediate next action

The next highest-value step is production content creation and integration, starting with approved Mia and Max character models, rigs, animation clips and runtime prefabs. The developer's reference images are already committed; no further reference upload is needed for this stage. In parallel, the developer can complete the clean-clone startup-scene verification and manual Play Mode acceptance.
