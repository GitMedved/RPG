# Mia & Max — Character Production Brief

**Status: production specification. No 3D character FBX assets are currently present in the repository.**

Reference images are stored in:
- `Mia.jpg`
- `Max.png`

Do not reinterpret the references or add unsupported costume/accessory details. The 3D models must be visually reviewed against those images before approval.

## Mia

- Stylized family-friendly 3D girl, matching the supplied reference.
- Blonde hair in two long braids; green eyes.
- White short-sleeve T-shirt, blue jeans, white sneakers.
- Humanoid skeleton, clean deformation topology, sensible shoulder/hip joints.
- Neutral A-pose or T-pose, feet on ground plane, forward axis +Z, consistent scale around 1.6–1.7 Unity units.
- Separate facial/eye materials where practical; braid geometry must remain readable in silhouette.
- Root motion disabled; locomotion is driven by the game controller.

## Max

- Stylized fluffy ginger cat matching the supplied reference.
- Green eyes, pink nose, cream muzzle and chest, orange tabby markings, fluffy tail.
- Quadruped Generic rig with root, pelvis, spine, chest, neck, head, tail base and tail segments; front and rear leg joints.
- Neutral quadruped pose, paws on ground plane, forward axis +Z, scale proportionate to Mia (roughly knee-height).
- Fur should be represented with optimized geometry/material detail, not expensive real-time hair simulation.
- Root motion disabled; companion follow owns world movement.

## Required animation clips

Create in-place clips for both characters where appropriate:
- Idle
- Walk
- Run
- Interact
- Celebrate
- Garden
- Reaction
- Jump
- Sleep
- Play

Max-specific performance should retain feline gait and body posture; do not retarget Mia's humanoid clips onto Max.

## Import and runtime handoff

- Place one unambiguous base FBX at `Runtime/Mia/Mia.fbx` and `Runtime/Max/Max.fbx`.
- Mia import type: Humanoid, Avatar created from this model.
- Max import type: Generic, Avatar created from this model.
- Keep animation clips in the matching character folder and use the stable names above.
- Run `Tools → Mia & Max → Apply Character FBX Import Profiles`.
- Run `Tools → Mia & Max → Build Character Animator State Machines`.
- Build runtime prefabs only after model/Avatar review; never silently overwrite a reviewed prefab.
- Run `Tools → Mia & Max → Validate Character Runtime Prefabs`.

## Acceptance gates

Visual likeness, facial expression, proportions, braid/tail silhouette, skinning, foot contact, clipping, animation transitions, mobile triangle/material budgets and target-device performance require human review. A valid Avatar or prefab does not equal production approval.
