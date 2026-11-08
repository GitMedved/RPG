# Mia & Max — Production Character Handoff

This document defines the handoff from approved character references to Unity runtime assets. It is an import/validation contract, not a claim that production meshes or animation clips are already present.

## Source of truth

- Keep the approved Mia and Max reference images in `Assets/_Project/Art/Characters/` and preserve the originals.
- Do not use screenshots of procedural greybox characters as production references.
- Model proportions, silhouette, face, hair/fur markings, clothing and palette must be reviewed against the approved images before rigging is accepted.

## Unity folder layout

Run `Tools → Mia & Max → Prepare Character Asset Pipeline` to create:

```
Assets/_Project/Art/Characters/Runtime/
  Mia/
    Mia.fbx
    idle / walk / run / interact / celebrate / garden / reaction clips
  Max/
    Max.fbx
    idle / walk / run / interact / celebrate / garden / reaction / jump / sleep / play clips
  Controllers/
    Mia_AnimatorContract.controller
    Max_AnimatorContract.controller
Assets/Resources/MiaMax/Characters/
  Mia.prefab
  Max.prefab
```

Clip files can be embedded in the base FBX or supplied as separate imported animation assets in the corresponding character folder. Keep names recognizable so the Animator builder can resolve them.

## Rig and animation policy

| Character | Rig | Movement ownership | Import policy |
|---|---|---|---|
| Mia | Humanoid | CharacterController | In-place clips; root motion off |
| Max | Generic quadruped | Companion-follow | In-place clips; root motion off |

- Use a consistent bind pose and stable bone names.
- Keep the mesh and skeleton facing Unity's expected forward direction.
- Avoid root translation/rotation in locomotion clips; the gameplay controller owns world movement.
- Idle, walk and run should loop cleanly. Action clips should return to the locomotion/idle state.
- Max's tail, ears and paws should deform cleanly; avoid a humanoid retargeting profile for the quadruped.
- Use production UV layouts before authoring character-specific albedo/normal/roughness maps. Generic material-library maps are not final character textures.

## Import and validation sequence

1. Add reviewed source FBX files to the matching Runtime character folder.
2. Run `Tools → Mia & Max → Apply Character FBX Import Profiles`.
3. Inspect each imported Avatar and mesh deformation manually.
4. Add and review the required animation clips.
5. Run `Tools → Mia & Max → Build Character Animator State Machines`.
6. Run `Tools → Mia & Max → Validate Character Runtime Prefabs`.
7. Run `Tools → Mia & Max → Production Asset Audit` and `Gameplay & Character Integration Audit`.
8. Enter Play Mode and verify movement, interactions, Max follow behaviour, animation transitions and Reduced Motion.

The runtime prefab builder adds the CharacterAnimationBridge, assigns the imported Animator, disables root motion and creates a prefab under Resources. It does not generate a mesh, skeleton, facial rig, UV map, animation clip or voice recording.

## Release acceptance

Do not mark a character production-ready until:
- model silhouette and proportions are approved against references;
- mesh topology, skin weights and deformation are reviewed;
- Avatar/rig type is valid;
- all required clips exist and have reviewed loop/exit behaviour;
- controller transitions are previewed;
- prefab geometry and bridge parameters pass validation;
- the character is tested in both portrait and landscape layouts and on target devices.
