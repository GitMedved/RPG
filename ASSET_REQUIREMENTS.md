# Asset Requirements and Art Pipeline

## 1. Source of truth and approved references

The approved visual references are committed in this repository:
- `Assets/_Project/Art/Characters/Mia.jpg`
- `Assets/_Project/Art/Characters/Max.png`
- `Assets/_Project/Art/Environment/backyard_collage.png`
- `Assets/_Project/Art/Environment/room_night_collage.png`

The written visual specification is `VISUAL_REFERENCE_SPECIFICATION.md`. These assets and the supplied character/video references are authoritative. Preserve the originals. Do not invent unsupported character, clothing, face, proportion or environment details.

Until production models are approved and integrated, procedural characters and environment pieces must remain clearly identified as placeholders. Do not describe the current prototype proxies as final art.

## 2. Approved reference lock

- **Max:** match the committed reference image for face, proportions, ginger fur, markings, eyes, muzzle, ears and tail. Do not redesign him.
- **Mia:** match the committed reference image for face, proportions, hair, hairstyle, eyes, clothing and colours. Do not redesign her.
- **Backyard:** use the wide establishing reference as the master composition. Preserve the relative placement and orientation of the house, patio, lawn, fence, tree, swing, play area, planting and other visible landmarks. Do not mirror the layout.
- **Interior:** use the committed room reference for the living-room look and palette where that environment is used.

## 3. Required source package

The current committed images are sufficient to begin a first modelling pass. Before final approval, collect additional orthographic views or a turntable only where the existing references do not resolve a modelling decision:
- Mia: front, side, back and 3/4 views; face and outfit details.
- Max: front, side, back and 3/4 views; fur pattern, face, paws and tail.
- Character turntable or movement video if available.
- Ownership/licensing notes for every third-party asset and any voice samples.

Do not block the first modelling pass by requesting redundant references. Record unresolved visual assumptions and obtain approval before finalizing them.

## 4. Character production requirements

**Mia**
- Mobile-ready stylized humanoid mesh with clean topology and UVs.
- Humanoid-compatible skeleton where practical; stable bone names and bind pose.
- Facial expression support appropriate to the approved reference.
- In-place clips: idle, walk, run, interact, garden, celebrate, reaction and required mini-game actions.

**Max**
- Mobile-ready stylized quadruped cat mesh with clean topology and UVs.
- Dedicated quadruped rig with expressive face, ears, paws and tail; do not force humanoid retargeting.
- In-place clips: idle, walk, run, jump, sit, sleep, play, eat, reaction and required fishing/adventure actions.

**Deliverables for each character**
- Editable source project and interchange/FBX export.
- Mesh, skeleton/rig, UVs, textures and animation clips.
- Material setup and Unity import notes.
- Polygon/texture budgets, licence and provenance.
- A preview/turntable and a clip list identifying each action and loop/exit behaviour.

## 5. Unity runtime integration

Use the documented runtime layout:
- `Assets/_Project/Art/Characters/Runtime/Mia/`
- `Assets/_Project/Art/Characters/Runtime/Max/`
- `Assets/_Project/Art/Characters/Runtime/Controllers/`
- `Assets/Resources/MiaMax/Characters/Mia.prefab`
- `Assets/Resources/MiaMax/Characters/Max.prefab`

Follow `CHARACTER_PRODUCTION_HANDOFF.md` for import profiles, Avatar/rig inspection, Animator state machines, runtime prefab validation and gameplay integration.

Important: an Editor utility can configure import settings and assemble prefabs, but it cannot create production mesh topology, a skeleton, skin weights, UVs, facial rigging, authored animation or voice recordings. Do not mark a character complete based only on file presence.

## 6. Environment and props

MVP backyard kit: ground/terrain, house exterior, fence, garden beds and crop stages, trees/shrubs/flowers, pond, workshop exterior, adventure gate, paths, starter decorations and interactable props. Woodland kit: ground/foliage, trees, bridge, landmarks and collectible props.

Use modular pieces, shared materials and reusable prefabs. Prioritize readable silhouettes and interaction clarity over micro-detail. Replace prototype proxies in the release path with a coherent, reference-matched art set.

## 7. UI and iconography

The temporary runtime-drawn joystick and interaction prompt are test scaffolding, not final UI. Replace them with responsive portrait mobile UI prefabs and approved iconography.

Required screens include HUD, inventory, quest panel, order board, map, mini-game entry/results, collection book, settings and pause. Include safe-area handling, scalable panels, readable touch targets and localization-ready English text keys. Keep any shop mock-up parent-gated and non-functional until a separate monetization/privacy review is approved.

## 8. Audio

Required replaceable assets: Mia dialogue, Max vocalizations, music loops, backyard/woodland ambience, UI feedback, planting/harvesting/crafting/delivery/reward effects and mini-game cues.

Record the source, licence and distribution rights for every recording. Voice direction remains American English: Mia bright, sweet and playful; Max funny cat voice with short spoken phrases and meows. Do not treat playback hooks or empty folders as completed audio content.

## 9. Technical budgets

- Prefer shared materials and texture atlases where they reduce draw calls.
- Use LOD for large environment assets and instancing for repeated vegetation.
- Keep texture sizes appropriate to on-screen importance; use platform compression.
- Bake lighting for static scenery where practical.
- Avoid expensive transparent overdraw and excessive real-time shadows.
- Profile on a low/mid-range Android device and a supported iPhone before finalizing budgets.
- Keep source files outside runtime folders when they are not needed in builds.

## 10. Naming and asset acceptance

Use PascalCase names. Keep editable source art documented and runtime exports in the relevant Art subfolder. Prefabs must not depend on scene-only objects. Every placeholder must include `Placeholder` in its name and be listed in the development log.

An asset is accepted only when:
- It matches the approved references and style guide.
- Scale, orientation, pivot, topology, UVs, materials and textures are reviewed.
- Rig deformation and every required animation are previewed.
- Unity import has no missing dependencies or warnings requiring action.
- Mobile performance is reviewed.
- Licence/provenance is recorded.
- It has been integrated and visually tested in the actual game scene.

## 11. Generated shared texture library

M52 adds `Tools/Mia & Max/Generate Complete Texture Library`, which generates a reusable surface set for backyard, Flower Garden, Woodland Trail, stations and props. Each surface has Base Color, Normal and Height PNG maps plus a matching material. See `M52_COMPLETE_TEXTURE_LIBRARY.md` and `M52_TEXTURE_MANIFEST.csv`.

The generated library is a shared environment starter kit, not final character texture atlases. Mia's skin/face/eyes/clothing and Max's face/eyes/fur maps must be authored against the final character meshes and UV layouts. Material assignment and visual/device validation remain separate acceptance steps.
