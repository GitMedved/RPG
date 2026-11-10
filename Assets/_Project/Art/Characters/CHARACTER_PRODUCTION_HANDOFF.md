# Mia & Max — 3D Character Production Handoff

**Status: specification only. No production FBX has been supplied or approved.** The 2D references are the visual source of truth; do not substitute generic procedural meshes for recognisable characters.

## Artist deliverables
For each character: editable source scene/project; clean production FBX; separate texture maps/material list; rigged source and exported skeleton; ten separate in-place clips (Idle, Walk, Run, Interact, Celebrate, Garden, Reaction, Jump, Sleep, Play); neutral front/side/back and 3/4 renders; animation preview; asset manifest (units, forward axis, frame rate, clip ranges, polygon count, texture sizes, material slots, rig type and rights statement).

## Mia — non-negotiable likeness
- Blonde girl, two long braids, green eyes.
- White short-sleeve T-shirt, blue jeans, white sneakers.
- Match approved reference face, hairline, braid placement, proportions, colours and silhouette.
- Humanoid deformation rig; clean shoulder, elbow, wrist, hip, knee, ankle and neck deformation.
- Braids remain readable and do not intersect torso during turns, jumps or celebration.
- No unsupported accessories, logos, costume details or props.

## Max — non-negotiable likeness
- Fluffy ginger cat, green eyes, pink nose, cream muzzle/chest, orange tabby markings, fluffy tail.
- Match reference head shape, eye spacing, muzzle, body proportions, stripe pattern and tail silhouette.
- Quadruped Generic rig with independent front/rear legs, spine/chest/neck/head and segmented tail.
- Preserve feline posture and gait; never retarget Mia's humanoid clips.
- Optimised geometry/material fur; no expensive real-time fur simulation.

## Unity import contract
- Metres; forward +Z; ground contact Y=0.
- Mia: Humanoid, Avatar created from this model.
- Max: Generic, Avatar created from this model.
- In-place clips; root motion disabled.
- Initial mobile target: <=25k triangles per character, <=4 material slots, one skinned renderer where practical; 2K textures only if justified. Profile on devices and reduce if needed.
- No hidden scene dependencies, external linked textures or unsupported shaders.
- FBX filenames Mia.fbx and Max.fbx; use paths in CHARACTER_PRODUCTION_BRIEF.md.

## Animation acceptance
Each clip must have correct character-specific motion; no foot sliding in planted portions, joint collapse, clipping, tail/braid penetration or pose pops. Idle loops seamlessly. Walk/run loops have consistent stride/contact. Interact/Garden/Play communicate action without root translation. Celebrate/Reaction have anticipation and settle. Jump has takeoff, airborne and landing. Sleep breathes calmly and loops. Transitions do not snap/freeze. Max locomotion is quadrupedal/feline.

## Review gates
1. Verify source and commercial rights.
2. Compare front/side/back/3/4 likeness to approved references.
3. Import FBX; inspect rig, Avatar, materials, scale and axis.
4. Build and inspect character review scene from all angles.
5. Preview all ten clips and transitions in Unity.
6. Run runtime prefab validation and release audits.
7. Profile CPU/GPU, skinning, draw calls and memory on Android and iOS.

Do not close character blockers until FBX files and clips exist, runtime prefabs/controllers validate, likeness and animation are visually approved, and device performance is recorded.
