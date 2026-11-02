# Asset Requirements and Art Pipeline

## 1. Source-of-truth rule
The supplied references are now available in this conversation: Max.png, Mia.jpg and backyard_collage.png. Their appearance and layout are authoritative. A written specification is in VISUAL_REFERENCE_SPECIFICATION.md. The images have not yet been committed as binary files in this repository, so Codex/Unity cannot load them from the repo until they are copied into Reference/. Do not invent character design details. Until production models are created, use clearly labelled greybox placeholders.

## 2. Approved reference lock\n- Max: ginger fluffy tabby cat, large green eyes, pink nose, cream muzzle/chest, orange striping and fluffy tail. Preserve his supplied face, palette and proportions.\n- Mia: blonde girl with two long braids, green eyes, white short-sleeve T-shirt, blue jeans and white sneakers. Preserve her supplied face, hairstyle and outfit.\n- Backyard: beige two-storey clapboard house, dark roof, white trim, adjoining patio/deck with outdoor seating and black kettle grill, central lawn, white picket fence, mature tree, left-side A-frame swing set, raised sandbox/play bed, shrubs, flowers and potted plants. Sunny daytime lighting. Preserve relative landmark placement from the wide establishing view.\n- Do not mirror the layout. The wide board is the master composition; side/detail panels are the same location.\n\n## 2. Required source package
Request/collect:
- Mia: front, side, back, 3/4 views; neutral and expressive face references; outfit details.
- Max: front, side, back, 3/4 views; fur colour/pattern and face references.
- Character turntable or short movement video if available.
- House/backyard/environment images used in the cartoon.
- Any existing voice samples only if the user has rights to use them in the game.
- Ownership/licensing notes for every third-party asset.

## 3. Current prototype status\nM2 adds procedural runtime proxy meshes for Mia and Max plus simple backyard detail proxies. They are temporary low-detail placeholders, not approved character models, final proportions, production rigs or authored animations. Do not use these proxies as final art direction.\n\n## 4. Character production requirements
**Mia:** mobile-ready stylized humanoid model, clean topology, UVs, humanoid-compatible rig where practical, facial expression support and animation retargeting plan. Required animation list: idle, walk, run, interact, garden, celebrate, reaction and mini-game actions.

**Max:** stylized quadruped cat model, expressive face/ears/tail, rig suitable for walk/run/jump/sit/sleep/play/eat/reaction/fishing/adventure animations. Ensure silhouette and ginger colouring match references.

Deliverables per character: source project, FBX or supported interchange export, textures, rig, animation clips, material setup notes, polygon/texture budgets, licence/provenance and Unity import settings.

## 5. Environment and props
MVP backyard kit: ground/terrain, house exterior, fence, garden beds, crop growth stages, trees/shrubs/flowers, pond, workshop exterior, adventure gate, paths, starter decorations and interactable props. Forest kit: ground/foliage, trees, bridge, landmarks, collectible props and friendly-animal placeholder/approved model.

Use modular pieces, shared materials and reusable prefabs. Prioritize readable interaction silhouettes over micro-detail.

## 6. UI and iconography

Prototype note: a temporary circular touch joystick and a basic interaction prompt currently use runtime GUI drawing. They are test scaffolding only and must be replaced with approved portrait mobile UI art and scalable Unity UI prefabs before production.
Portrait layouts: HUD, inventory, quest panel, order board, map, mini-game entry/results, collection book, settings, pause and parent-gated mock shop. Provide scalable vector/source assets where possible, 9-slice panels and icon exports at multiple resolutions. All visible text must be localization-ready English keys.

## 7. Audio
Replaceable music loops, ambient backyard/forest beds, UI feedback, planting/harvesting/production/reward effects, mini-game cues and character vocal placeholders. Record source, licence and usage rights. Do not bake audio assumptions into gameplay logic.

## 8. Technical budgets (initial targets; validate by profiling)
- Prefer shared materials and texture atlases where they reduce draw calls.
- Use LOD for large environment assets and instancing for repeated vegetation.
- Keep texture sizes appropriate to on-screen importance; use platform compression.
- Use baked lighting for static scenery where possible.
- Avoid expensive transparent overdraw and excessive real-time shadows.
- Profile on a low/mid-range Android device and a supported iPhone before finalizing budgets.
- Keep source files outside runtime folders when they are not needed in builds.

## 9. Naming and folder conventions
PascalCase asset names; descriptive prefixes only where they improve search. Keep source art in a documented source location and runtime exports in the relevant Art subfolder. Prefabs must not depend on scene-only objects. Every placeholder asset must include `Placeholder` in its name and be listed in the development log.

## 10. Asset acceptance checklist
- Matches approved reference and style guide.
- Correct scale/orientation/pivot.
- Clean import, materials and textures.
- Mobile performance reviewed.
- Animation clips and avatar mapping tested.
- No missing dependencies.
- Licence/provenance recorded.
- Placeholder status clearly identified.
