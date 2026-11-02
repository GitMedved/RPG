# M2 — Character & World Prototype

## Goal

Move the M1/M1.2 scene from plain primitive blockout toward a recognizable first playable Mia & Max backyard, without pretending that procedural proxies are final art.

## Implemented in this branch

### Mia — temporary stylized proxy
- Blonde hair cap and front fringe, two long braid silhouettes.
- Green eyes and simple face markers.
- White shirt, blue denim legs and light sneakers.
- Simple movement-driven arm/leg rotation.

### Max — temporary stylized proxy
- Ginger body/head, cream muzzle and belly, green eyes, pink nose.
- Upright ears with inner-ear colour, four paw shapes and a tail.
- Tail sway and movement-driven paw motion while following Mia.

### Backyard — detail pass
- House entrance, front door glass, two framed windows and steps.
- Patio table, chairs and a simple grill silhouette.
- Raised flower beds, flower heads and potted plants.
- Existing tree, swing, sandbox, fence, lawn and house blockout remain.

## Technical notes

- All character and prop meshes are created at runtime from Unity primitives. This keeps the prototype self-contained while original model assets are not yet in the repository.
- The character proxies are not skinned meshes, have no facial rig and do not use Animator controllers.
- The movement cycle is a lightweight transform animation driven by root displacement; it is not a polished walk/run animation.
- The world detail pass is intentionally low-poly and uses shared temporary materials per builder.
- Keep final character design locked to the supplied Mia/Max reference images. Do not treat proxy geometry as approval of new facial or body proportions.

## Local validation checklist

1. Open with the recorded Unity 6000.3.x Editor and let Package Manager resolve URP 17.3.0.
2. Run **Mia & Max → Setup Backyard Prototype Scene** if the scene asset is missing.
3. Open `Assets/_Project/Scenes/Backyard.unity` and enter Play Mode.
4. Check Mia's face/outfit colour cues and Max's silhouette from the gameplay camera.
5. Walk in all directions; check limb motion, Max follow distance and tail movement.
6. Check that the new house facade and patio props do not block the intended movement path.
7. Test the portrait layout on a physical Android device and a supported iPhone; verify touch joystick reach, interaction tap area, camera framing and frame rate.
8. Record screenshots and any compile/runtime errors before moving to production asset import.

## Not included / not verified

- Original reference image binaries are still not stored in this repository.
- No final Mia/Max model, topology, UVs, rig, facial blendshapes or authored animation clips.
- No final environment meshes, materials, textures, lighting bake, VFX or audio.
- No Unity Editor compile, Play Mode session, Android build or iOS build was run by the remote repository workflow.
- URP Pipeline Asset and Renderer Data still need to be generated and assigned in the Unity Editor as described in `Assets/_Project/URP_SETUP.md`.

## Next step after review

Import approved, licensed character and environment source assets into the repository, then replace these procedural proxies with production-ready prefabs. Only after the actual models and rigs are available should we retarget authored idle/walk/run/interact animations and tune Max's companion behaviours.
