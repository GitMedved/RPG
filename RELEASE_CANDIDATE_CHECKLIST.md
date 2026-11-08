# Mia & Max Adventures — Release Candidate freeze and execution checklist

## Release policy

The next target is a **first playable Release Candidate**, not a feature-complete long-term vision. Freeze new features. Only accept changes that fix a release blocker, improve stability/accessibility, or complete required launch content. Do not add accounts, multiplayer, ads, analytics or online services to this offline-first family MVP.

## Repository facts observed on 2026-11-08

- Unity editor version recorded by the repository: 6000.3.0f1.
- The feature branch contains the prototype bootstrap and gameplay systems.
- The branch tree currently has no committed `.unity` scene asset.
- The branch tree currently has no `ProjectSettings/EditorBuildSettings.asset`.
- No GitHub Actions workflow is currently committed.
- Character image references and the backyard reference collage are present.
- Final Mia/Max production models, rigged animation clips, final UI artwork and licensed audio recordings are not established by the repository snapshot.
- These facts mean the repository is not yet a reproducible mobile release candidate.

## RC0 — Create a reproducible Unity project

1. Pull `codex/mia-max-foundation`.
2. Open with Unity 6000.3.0f1 and wait for package resolution/import.
3. Resolve every compiler error and missing script/reference.
4. Run `Mia & Max > Setup Backyard Prototype Scene`.
5. Save the generated scene under `Assets/_Project/Scenes/` with a stable name such as `MiaMax_Backyard`.
6. Add that saved scene to Build Settings and make it the first enabled scene.
7. Save project settings and commit the scene, its `.meta`, and `ProjectSettings/EditorBuildSettings.asset`.
8. Close Unity, reopen the project from a clean pull/import and confirm the saved scene loads without relying on an unsaved editor state.

**Exit evidence:** committed scene and Build Settings; clean Editor compile; no missing references.

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

## Immediate next action for the developer

Complete RC0 locally. The release audit now explicitly checks for a saved enabled scene and persisted Build Settings. After RC0, paste the full Console output of Release Readiness Audit and Core Smoke Checks; address the concrete blockers in that report rather than adding more features.
