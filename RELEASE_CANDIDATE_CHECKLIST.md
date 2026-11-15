# Mia & Max Adventures — MVP Validation Checklist

**Status:** MVP implementation committed. **Not a release candidate.**

## Local Unity requirements

- [ ] Clone/pull the latest `codex/mia-max-foundation` branch.
- [ ] Open in Unity Editor 6000.3.0f1 and resolve all package/compile errors.
- [ ] Open the saved `Assets/_Project/Scenes/Backyard.unity`.
- [ ] Run **Mia & Max → MVP → 1. Preflight Assets & Scene** with no failed items.
- [ ] Press Play and run **Mia & Max → MVP → 2. Inspect Running World** with no failed items.
- [ ] Verify portraits, textures, HUD, scene camera and all tap targets on a portrait-sized Game view.
- [ ] Verify illustrated models replace greybox renderers; inspect cottage roof, fence, garden, character lighting and gate from the fixed camera.
- [ ] Confirm all 8 Gobkit CC0 GLBs are genuinely imported and rendered (not only present as source files).
- [ ] Confirm both skinned human candidate GLBs import and expose animation clips; do not automatically declare either the finished Mia.
- [ ] If selecting the Vuena cat, import its official FREE CC0 ZIP, build MaxRig prefab, inspect skinned mesh, facial/jaw movement, and 12 source animations.
- [ ] Confirm authentic Max meow clips play at safe level without overlapping character dialogue.
- [ ] Confirm all 6 original English dialogue WAVs exist and import locally, whether packaged by GitHub Actions or downloaded via Unity Editor.
- [ ] Confirm native rig controller can return Idle after React/Talk without camera or character locomotion.
- [ ] Verify animated Mia and Max are visible: blinking, body sway, Max tail and contextual gestures.
- [ ] Confirm HUD chapter changes, touch hit areas, safe-area margins, portrait cropping and voice mute control on several screen aspect ratios.
- [ ] Optional voice import: run **Mia & Max → MVP → 4. Install Generated English Voice Takes**, then replay birdhouse, keepsake and stone scenes for speech and talking-mouth checks.

## Gameplay acceptance

- [ ] Fresh progress: tree and fallen twigs grant resources only once.
- [ ] Feather pickup grants once; the birdhouse requires 3 twigs + 1 feather.
- [ ] Building the birdhouse updates the visible object, rewards once and persists.
- [ ] Watering the flowers starts a roughly 60-second UTC timer; harvest works after waiting.
- [ ] Keepsake crafting costs 1 twig + 1 flower and requires the birdhouse.
- [ ] The gate switches camera composition and the **Back to yard** control returns.
- [ ] The golden stone reward is granted once.
- [ ] Max's treasure sorting game grants a reward only once, with a working leave button.
- [ ] Restarting Play Mode restores progress correctly; reset tool clears only MVP keys.
- [ ] No joystick, follow camera or direct character locomotion is present.

## Device checks

- [ ] Build/install on Android and iOS with correct identifiers and signing.
- [ ] Check camera composition, tap areas and safe-area layout on small and tall phones.
- [ ] Verify sound cues, pause/resume, timer completion and persistence after app termination.
- [ ] Check frame rate, memory, cold start and Unity console/device logs.

## Not met for commercial release

- [ ] Final environment 3D models and prefabs with coherent art direction.
- [ ] Recognizable rigged, animated Mia and Max in the 3D scene.
- [ ] Final licensed audio/voice and at least one polished extra mini-game.
- [ ] Store age/privacy compliance, child-facing UX review, promotional materials.
- [ ] Regression tests and signed release builds verified on real devices.

Source-of-truth run instructions: `README.md`. This checklist deliberately avoids the obsolete free-roam and farm-management milestones.
