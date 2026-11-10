# Mia & Max — Release Candidate Workflow

This project now has a consolidated Unity Editor entry point.

## 1. Apply the mobile application identity

Run:

**Tools → Mia & Max → Release → Apply Mobile App Identity**

This sets company `AleksDevelop`, product `Mia & Max Adventures`, version `1.0`, and the same approved identifier `com.aleksdevelop.miamax` for Android and iOS. The release audit now requires this exact identifier rather than accepting any syntactically plausible bundle ID. It also checks whether Android/iOS build-support modules are installed, whether Android is configured for IL2CPP with ARM64 included, and whether iOS uses IL2CPP. These are configuration checks only; they do not prove that either platform can produce a successful signed build.

## 2. Generate the production asset packs

Run:

**Tools → Mia & Max → Release → Generate All Production Asset Packs**

It invokes the environment, original audio and UI sprite generators. Each generator writes its own report and preserves the existing gameplay placeholder system. Review all three reports for errors.

Generated asset groups:
- 15 optimized backyard environment prefabs, meshes, materials and textures
- 11 gameplay SFX, 4 ambience loops and 3 original music themes
- 10 UI sprites generated for mobile HUD and reusable cards; visual integration still requires review

## 3. Run the read-only audit sequence

Run:

**Tools → Mia & Max → Release → Run All Read-Only Release Audits**

This schedules the backyard production prefab audit, surface asset audit, audio audit, voice audit, character runtime validation, missing-script audit and release readiness audit for the next Unity Editor update. The deferred sequence refreshes the AssetDatabase first, allowing WAV/FBX imports to settle before checking imported clips. Review the individual Console reports and does not build the game or approve visual quality, device behaviour, licensing, privacy/child-safety compliance or store submission.

## 4. Character animation pipeline

After importing the approved Mia and Max FBX models and their ten named clips per character, run:

**Tools → Mia & Max → Build Production Animator Controllers From Clips**

The builder requires one uniquely identifiable clip for each action: `Idle`, `Walk`, `Run`, `Interact`, `Celebrate`, `Garden`, `Reaction`, `Jump`, `Sleep` and `Play`. It creates separate `Mia_Production.controller` and `Max_Production.controller` assets, with locomotion speed transitions and action triggers. It does not overwrite an existing production controller or modify existing runtime prefabs. The FBX import-profile command only creates runtime prefabs after the matching production controller exists; it will not create a release prefab that uses the empty parameter-only scaffold. After the controllers are built, run **Tools → Mia & Max → Apply Character FBX Import Profiles** to create missing runtime prefabs.

## 5. Remaining manual gates

The current repository contains Mia and Max reference images, but no production FBX character models or voice recordings. Those are explicit release blockers. The draft voice lines and character production brief are provided for the next art/audio handoff.

After assets are ready, visual likeness, rig deformation, animation, UI scaling, sound quality, gameplay in Play Mode, Android/iOS builds and real-device performance must be reviewed. Legal/privacy/child-safety and store requirements also need human sign-off.

The assistant cannot run Unity Editor or physical-device tests on the developer's computer. The editor workflow automates generation and static audits, but it cannot replace the character-art handoff, human visual/audio review, Play Mode verification, platform builds, physical-device testing or legal/store sign-off.
