# Mia & Max — Release Candidate Workflow

This project now has a consolidated Unity Editor entry point.

## 1. Generate the production asset packs

Run:

**Tools → Mia & Max → Release → Generate All Production Asset Packs**

It invokes the environment, original audio and UI sprite generators. Each generator writes its own report and preserves the existing gameplay placeholder system. Review all three reports for errors.

Generated asset groups:
- 15 optimized backyard environment prefabs, meshes, materials and textures
- 11 gameplay SFX, 4 ambience loops and 3 original music themes
- 10 UI sprites used by the mobile HUD and reusable cards

## 2. Run the read-only audit sequence

Run:

**Tools → Mia & Max → Release → Run All Read-Only Release Audits**

This invokes the backyard production prefab audit, surface asset audit, audio audit, voice audit, character runtime validation, missing-script audit and release readiness audit.

The sequence produces individual Console reports and does not build the game or approve visual quality, device behaviour, licensing, privacy/child-safety compliance or store submission.

## 3. Remaining manual gates

The current repository contains Mia and Max reference images, but no production FBX character models or voice recordings. Those are explicit release blockers. The draft voice lines and character production brief are provided for the next art/audio handoff.

After assets are ready, visual likeness, rig deformation, animation, UI scaling, sound quality, gameplay in Play Mode, Android/iOS builds and real-device performance must be reviewed. Legal/privacy/child-safety and store requirements also need human sign-off.

The assistant cannot run Unity Editor or physical-device tests on the developer's computer. The workflow is designed to reduce final verification to two menu commands plus the required human/device reviews.
