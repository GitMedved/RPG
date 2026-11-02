# Asset Requirements and Art Pipeline

## 1. Source-of-truth rule
The user's supplied Mia and Max images/videos are authoritative. The current repository contained no character or environment assets at audit time. Do not invent character design details. Until references are added, use neutral greybox shapes only and label them as temporary.

## 2. Required source package
Request/collect:
- Mia: front, side, back, 3/4 views; neutral and expressive face references; outfit details.
- Max: front, side, back, 3/4 views; fur colour/pattern and face references.
- Character turntable or short movement video if available.
- House/backyard/environment images used in the cartoon.
- Any existing voice samples only if the user has rights to use them in the game.
- Ownership/licensing notes for every third-party asset.

## 3. Character production requirements
**Mia:** mobile-ready stylized humanoid model, clean topology, UVs, humanoid-compatible rig where practical, facial expression support and animation retargeting plan. Required animation list: idle, walk, run, interact, garden, celebrate, reaction and mini-game actions.

**Max:** stylized quadruped cat model, expressive face/ears/tail, rig suitable for walk/run/jump/sit/sleep/play/eat/reaction/fishing/adventure animations. Ensure silhouette and ginger colouring match references.

Deliverables per character: source project, FBX or supported interchange export, textures, rig, animation clips, material setup notes, polygon/texture budgets, licence/provenance and Unity import settings.

## 4. Environment and props
MVP backyard kit: ground/terrain, house exterior, fence, garden beds, crop growth stages, trees/shrubs/flowers, pond, workshop exterior, adventure gate, paths, starter decorations and interactable props. Forest kit: ground/foliage, trees, bridge, landmarks, collectible props and friendly-animal placeholder/approved model.

Use modular pieces, shared materials and reusable prefabs. Prioritize readable interaction silhouettes over micro-detail.

## 5. UI and iconography
Portrait layouts: HUD, inventory, quest panel, order board, map, mini-game entry/results, collection book, settings, pause and parent-gated mock shop. Provide scalable vector/source assets where possible, 9-slice panels and icon exports at multiple resolutions. All visible text must be localization-ready English keys.

## 6. Audio
Replaceable music loops, ambient backyard/forest beds, UI feedback, planting/harvesting/production/reward effects, mini-game cues and character vocal placeholders. Record source, licence and usage rights. Do not bake audio assumptions into gameplay logic.

## 7. Technical budgets (initial targets; validate by profiling)
- Prefer shared materials and texture atlases where they reduce draw calls.
- Use LOD for large environment assets and instancing for repeated vegetation.
- Keep texture sizes appropriate to on-screen importance; use platform compression.
- Use baked lighting for static scenery where possible.
- Avoid expensive transparent overdraw and excessive real-time shadows.
- Profile on a low/mid-range Android device and a supported iPhone before finalizing budgets.
- Keep source files outside runtime folders when they are not needed in builds.

## 8. Naming and folder conventions
PascalCase asset names; descriptive prefixes only where they improve search. Keep source art in a documented source location and runtime exports in the relevant Art subfolder. Prefabs must not depend on scene-only objects. Every placeholder asset must include `Placeholder` in its name and be listed in the development log.

## 9. Asset acceptance checklist
- Matches approved reference and style guide.
- Correct scale/orientation/pivot.
- Clean import, materials and textures.
- Mobile performance reviewed.
- Animation clips and avatar mapping tested.
- No missing dependencies.
- Licence/provenance recorded.
- Placeholder status clearly identified.
