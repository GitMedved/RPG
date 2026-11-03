# M15 — Visual production pipeline

**Status:** pipeline and acceptance rules implemented. The reference image binaries are already stored under `Assets/_Project/Art/Characters/` and `Assets/_Project/Art/Environment/`; production 3D models and authored UI assets are still pending.

## Locked visual direction
Use the supplied Mia, Max and backyard references as the only design authority. Translate the board's Pixar-style 3D target into warm, polished, readable real-time mobile 3D. Do not redesign faces, hairstyles, clothing, coat markings, eye colours or backyard landmark layout.

- Mia: blonde girl, two long braids, green eyes, white short-sleeve T-shirt, blue jeans, white sneakers.
- Max: ginger fluffy tabby cat, green eyes, pink nose, cream muzzle/chest/belly, orange stripes and fluffy tail.
- Backyard: beige two-storey clapboard house, dark roof, white trim, patio/deck, central lawn, white picket fence, mature tree, left-side swing set, sandbox/play bed, shrubs, flowers and pots.
- Preserve the wide establishing view's spatial composition; never mirror the yard.

## Asset intake locations
Copy original, unmodified files into the repository:
- `Reference/Characters/Mia.jpg`
- `Reference/Characters/Max.png`
- `Reference/Environment/backyard_collage.png`

Runtime exports belong in:
- `Assets/_Project/Art/Characters/Source/` (working source files)
- `Assets/_Project/Art/Characters/Runtime/` (FBX, textures, masks)
- `Assets/_Project/Art/Environment/Source/`
- `Assets/_Project/Art/Environment/Runtime/`
- `Assets/_Project/Art/UI/`

Do not commit generated Unity Library, Temp, Logs or Obj folders. Use Git LFS for large source binaries if repository policy enables it. Preserve original references unchanged.

## Production deliverables
**Mia:** clean stylized topology, UVs, humanoid-compatible rig, facial expression support, FBX, textures, source project, idle/walk/run/interact/garden/celebrate/reaction clips.

**Max:** quadruped cat rig with expressive ears/face/tail, FBX, textures, source project, walk/run/jump/sit/sleep/play/eat/reaction clips. Do not force a humanoid rig on Max.

**Backyard:** modular house/patio/fence/foliage/paths/plots/props; matching landmark placement; reusable prefabs; LODs for large assets.

**UI:** authored icon sprites and scalable panels; replace text glyph/emoji symbols only after the icon set is approved.

## Import defaults
- Apply correct scale and forward axis; verify pivots before prefab creation.
- Use shared URP-compatible materials and texture atlases where sensible.
- Enable Read/Write only when required.
- Use platform-appropriate texture compression and mipmaps for 3D textures.
- Use humanoid avatar mapping for Mia only if the rig is genuinely humanoid; use a generic/custom rig for Max.
- Keep source art outside runtime folders when it is not needed in builds.

## Acceptance gate
No production asset is accepted until it matches the supplied reference from front/side/3/4 views, has no missing dependencies, imports without warnings, has verified scale/pivots/materials, and runs on a target mobile device at the agreed performance tier.

## Current limitation
The repository does not yet contain the original reference image binaries or production 3D models. Runtime procedural meshes remain explicitly temporary placeholders. This milestone establishes the production pipeline; it does not pretend to create rigged 3D assets from a text specification.
