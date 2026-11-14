# Mia & Max — Prototype Foundation Audit

**Audit date:** 2026-11-14  
**Repository:** `GitMedved/RPG`  
**Branch audited:** `codex/mia-max-foundation`  
**Purpose:** choose a practical foundation for a fixed-camera, tap-led 3D family adventure without replacing the approved game direction.

## Executive decision

Do **not** replace the current project wholesale with a third-party template, and do not buy a template yet.

Keep this repository as the product project because its game design, save/economy/progression code and mini-game prototypes are already tailored to Mia & Max. Use Unity's free **Adventure Game | 3D Sample Project** as an isolated reference project for interaction architecture and example scene patterns. It is the closest verified 3D starting point found that is explicitly compatible with the project's exact recorded Unity editor version (6000.3.0f1) and URP. Its default point-and-click character movement is not part of Mia & Max and must not be carried over.

The current blocker is not a missing quest system. It is the absence of a functioning presentation scene and production art. Importing another large gameplay framework before fixing that would add integration risk without solving the blank-screen problem.

## 1. Repository facts verified

### Unity project
- Editor recorded in `ProjectSettings/ProjectVersion.txt`: Unity 6000.3.0f1.
- URP package pinned to 17.3.0 in `Packages/manifest.json`.
- uGUI package is present (`com.unity.ugui` 2.0.0).

### Scene and startup
- `Assets/_Project/Scenes/Backyard.unity` is only 3,539 bytes and its serialized `SceneRoots` list is empty.
- `BackyardPrototypeBootstrap.Start()` calls `TapAdventureWorldBootstrap.Build()`; the old primitive backyard builder is explicitly bypassed.
- `TapAdventureWorldBootstrap.BuildWorld()` tries to load `Resources/MiaMax/Art/BackyardScene`, creates/configures a camera, sets ambient lighting and logs that the illustrated world was created.
- The bootstrap's own `OnGUI()` draws the background only when that texture loaded successfully. If it is missing, there is no world image behind the UI.
- `Assets/Resources/MiaMax/Art/` contains only a README. The required `BackyardScene.png` and `WorldMaterialAtlas` are not present there.
- The hotspot input is screen-space and uses hard-coded normalized positions. It is not a 3D scene raycast interaction system.
- The legacy `BackyardPrototypeBootstrap` still contains a large primitive blockout builder, but its startup path no longer invokes that builder.

**Conclusion at audit time:** the game launched without meaningful world visuals because the builder never called its own world-geometry methods, the camera was at ground height looking horizontally, and the only optional image backdrop was missing. The follow-up implementation now calls the 3D world builders, uses the existing authored URP materials, creates a fixed angled portrait camera and selects objects by collider raycasts. The world is now a runtime primitive-based greybox, not final art; Unity Play Mode still needs to confirm the rendered result.

### Art and prefab availability
- Approved reference images are committed: `Assets/_Project/Art/Characters/Mia.jpg`, `Max.png`, and `Assets/_Project/Art/Environment/backyard_collage.png` plus the night-room collage.
- These are visual references, not production runtime character models.
- The character handoff explicitly says there are no approved production FBX models or animation clips.
- `Assets/_Project/Prefabs/` contains only a placeholder `.gitkeep`.
- `Assets/Resources/Environment/` does not exist on the audited branch, although `BackyardProductionEnvironmentRuntime` attempts to load prefabs from that Resources folder.
- The environment runtime source folder currently contains production briefs, not the referenced FBX/OBJ/GLB model library.

**Conclusion:** previous environment-audit and prefab-builder work must not be mistaken for committed, runtime-loadable production prefab assets. The required prefab outputs are not present at the paths used by the runtime loader.

## 2. External prototype comparison

| Candidate | Strengths | Mismatch / risk | Decision |
|---|---|---|---|
| **Unity Adventure Game — 3D Sample Project** (free, 78.7 MB) | Fully commented 3D point-and-click sample; object interactions, inventory, conditions, persistence and scene/game-state management; Asset Store lists URP compatibility for Unity 6000.3.0f1. | Includes point-and-click character movement, which conflicts with the fixed-camera/no-locomotion design. Older sample architecture needs review before copying. | **Best free reference project.** Import in a separate scratch project first; port only suitable systems/patterns. |
| **Out of Circulation — Game Accessibility 3D Sample** (free, 220.3 MB) | Official Unity narrative point-and-click sample; interaction highlighting, inventory/settings, dialogue/navigation and accessibility patterns; URP compatibility listed for Unity 6000.3.0f1. | Accessibility-focused case study, not a cozy world-building/crafting game; much larger download. | Secondary reference for accessibility and interaction feedback, not the base. |
| **Beasty Visual Novel** ($15.99 minimum at time of audit) | Unity 6000.2+; complete demo with rooms, tap/click interactables, quests, inventory, conditions, saves and localization. | Primarily a 2D visual-novel/free-roam room system; placeholder 2D art; paid; minigames/3D world presentation would still be custom. | Do not purchase for this project. Borrow its data-driven story/quest ideas only if needed. |
| **Survival Hero — Hyper Casual Simulator** ($49.99) | 3D cartoon assets, gathering, building, land unlocks, inventory and progression; URP support is listed. | Built around a touch/drag top-down character controller and harvesting loop; different game feel; older listed Unity version (2023.2.18f1); paid. | Reject as the foundation. It is closer to a farming/idle survival game than Mia & Max. |
| **Cozy Quest UI** ($19.99) | Portrait UGUI screens for inventory, quests, rewards, settings and menus; Unity 6/URP demo. | UI kit only; does not provide the world, interactions or game loop; uses URP 2D demo scenes. | Do not buy now. Existing project already has UI foundations; first establish a working world and visual direction. |

## 3. Recommended integration strategy

### Keep in Mia & Max
Retain the current product-specific systems where their implementation passes review:
- player save and progression data;
- item/resource identifiers and reward validation;
- quest/story progression;
- mini-game rule prototypes and reward adapter;
- accessibility settings and audio service boundaries;
- approved Mia & Max visual references and design documents.

These systems still need compilation, play-mode and save/reload tests. Their existence in source is not proof that the complete game loop is currently playable.

### Study in Unity's free Adventure Game sample
Use a separate scratch project with Unity 6000.3.0f1 and URP. Verify the sample opens and plays before copying anything. Inspect:
- interactive-object contracts and selection feedback;
- inventory item definitions and UI separation;
- conditional action availability;
- scene state and persistence patterns.

Do not port:
- character navigation/pathfinding or player movement;
- camera-follow/player-controller assumptions;
- unrelated sample story, characters or art;
- sample save data formats without a migration plan.

### Mia & Max interaction contract
The production flow should be:

1. Tap a visible object in the fixed 3D scene.
2. Show a contextual card with one or more valid actions.
3. Validate prerequisites and resource costs.
4. Confirm the action.
5. Play a short Mia/Max reaction or object animation.
6. Apply the transaction once, update the world object and quest state.
7. Autosave at the transaction boundary.

The player never steers Mia or Max around the scene. Character movement, where needed for a short cinematic, is authored animation rather than player locomotion.

## 4. Required vertical slice before adding more content

Build one complete, visually readable backyard scene and one story chain:

- Fixed portrait-friendly camera framing the house, lawn, fence, mature tree and patio consistently with the supplied backyard reference.
- Actual imported environment meshes/materials; no visible primitive cubes/spheres as final art.
- Mia and Max visible using approved models when available; until then, do not pretend a reference image or generic proxy is a production character.
- Three reliable tap targets: fallen twigs, blue feather, crafting bench.
- Context panel and inventory display with correct counts.
- Birdhouse recipe: 3 twigs + 1 feather.
- A visible birdhouse state change after crafting; later bird arrival is persisted.
- One short Max treasure-sorting mini-game, with validated one-time reward.
- Save, quit/re-enter, and verify all world/resource/story states restore correctly.

Only after this slice works should the project expand to the Secret Garden, additional mini-games, decoration and later chapters.

## 5. Art and asset intake

The project already documents free CC0 asset sources and Unity import menu commands in `Assets/_Project/Art/Environment/FREE_ASSET_INTEGRATION.md`. The intended process is:
1. Import one coherent nature/environment pack, not several unrelated packs.
2. Generate the review scene.
3. Select only models that match the backyard reference and mobile budget.
4. Create URP materials and production prefabs in the exact Resources paths used by runtime loaders.
5. Validate scale, grounding, normals, UVs, colliders, draw calls and texture memory.
6. Integrate the selected models into the actual startup scene and visually inspect them in Play Mode.

Do not report an asset pack as integrated merely because a downloader/importer or prefab-builder script exists. The imported model files, prefab outputs, material references and runtime load paths must all be present.

For Mia and Max, the next genuine production milestone is approved rigged FBX models and character-specific animation clips. The existing handoff document defines the import and animation acceptance criteria.

## 6. Immediate work order

1. **Completed in the follow-up code change:** call the world geometry builders during startup, use authored URP material assets, use a fixed angled portrait camera and replace screen-space hard-coded hotspots with world-object collider raycasts.
2. **Completed in the follow-up code change:** persist one-time resource collection and one-time mini-game/garden-stone rewards to prevent repeatable reward exploits; safely parse the saved flower-growth timestamp.
3. Replace temporary primitive greybox objects with reviewed environment prefabs from one coherent CC0 pack.
4. Add Mia and Max as approved rigged production models and wire their authored short reaction animations.
5. Connect the current birdhouse/flower/secret-garden chain to the canonical inventory/save/progression services rather than leaving it isolated in PlayerPrefs.
6. Import and inspect the free Unity Adventure Game sample in isolation; port only interaction/inventory patterns that fit the tap-only contract.
7. Run Unity compile, Play Mode, save/reload and portrait safe-area checks. Then build Android and iOS separately.

## 7. Release gate

A repository commit is not a release. Do not call the prototype playable or release-ready until all of the following are true:
- Unity 6000.3.0f1 imports packages without errors.
- The actual startup scene visibly renders the intended world.
- All three world interactions and the birdhouse story chain work in Play Mode.
- Mini-game reward cannot be claimed repeatedly by replaying the same run.
- Save/reload preserves inventory, object states and progression.
- Portrait layouts work on at least one narrow and one tall aspect ratio.
- Android and iOS builds complete; each receives a device smoke test.
- Missing production character models/animations, audio, licensing and child-safety/platform compliance are either completed or explicitly excluded from the release scope.

## Sources checked

- Unity Adventure Game sample: https://assetstore.unity.com/packages/templates/packs/adventure-game-3d-sample-project-76216
- Unity Game Accessibility sample: https://assetstore.unity.com/packages/templates/packs/game-accessibility-3d-sample-project-223056
- Beasty Visual Novel: https://beastycomponents.itch.io/beasty-visual-novel
- Beasty House Demo documentation: https://bauflowbeasty.github.io/docs/beasty-visual-novel/getting-started/house-demo/
- Survival Hero: https://assetstore.unity.com/packages/templates/packs/survival-hero-hyper-casual-simulator-325375
- Cozy Quest UI: https://assetstore.unity.com/packages/2d/gui/cozy-quest-ui-mobile-rpg-idle-game-interface-396734
