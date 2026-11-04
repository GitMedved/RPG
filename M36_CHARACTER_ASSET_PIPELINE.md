# M36 — Character FBX Import Pipeline

## Goal

Prepare a predictable, character-specific import workflow before production Mia and Max models are added. This milestone does not invent replacement character designs and does not claim to provide finished models, rigs or animation clips.

## Editor tools

After Unity imports the new Editor script, use:

- `Tools → Mia & Max → Prepare Character Asset Pipeline`
  - Creates `Assets/_Project/Art/Characters/Runtime/Mia`
  - Creates `Assets/_Project/Art/Characters/Runtime/Max`
  - Creates `Assets/_Project/Art/Characters/Runtime/Controllers`
  - Creates `Mia_AnimatorContract.controller` and `Max_AnimatorContract.controller` if they do not already exist.
  - Adds the agreed Animator parameters: `Speed` (float), `Grounded` (bool), `Interact` (trigger), `Celebrate` (trigger).
- `Tools → Mia & Max → Apply Character FBX Import Profiles`
  - The selected base Mia FBX uses Humanoid animation type and Create From This Model avatar setup.
  - The selected base Max FBX uses Generic animation type for the quadruped rig.
  - Animation import is enabled on each base model; embedded cameras and lights are disabled.
  - Only the selected base FBX is modified. Separate animation-only FBX files are not automatically changed; configure them manually with Copy From Other Avatar and the base model's Avatar after import.
  - Creates runtime prefabs in `Assets/Resources/MiaMax/Characters/Mia.prefab` and `Max.prefab` when a base model FBX is available.
  - If several FBX files exist, the base model must be named `Mia.fbx` / `Max.fbx`, `Mia_Model.fbx` / `Max_Model.fbx`, or `Mia_Character.fbx` / `Max_Character.fbx`; otherwise profile application and prefab generation are skipped rather than guessing which file is the base model.

## Character-specific requirements

**Mia**
- Use a humanoid skeleton only when the supplied model has a valid, correctly mapped human rig.
- Validate Avatar configuration, proportions, skin weights, foot contact and retargeted locomotion.
- Use in-place locomotion while the prototype CharacterController owns movement unless a specific action is deliberately synchronized.

**Max**
- Keep Generic/custom quadruped rig; do not force Humanoid retargeting.
- Preserve the supplied ginger-cat silhouette and proportions.
- Validate four-legged locomotion and separate tail/ear secondary motion.

## Animator contract scaffold

The generated controller files contain the shared parameter contract only. They do not contain final motion clips, blend trees, transitions or authored animation. The controllers must not be considered production-ready until real clips are assigned and tested.

The generated Resources prefabs attach the contract controller to the imported model Animator when available. At runtime, `StylizedCharacterPrototype.BuildMia/BuildMax` first loads these prefabs. If they are absent, the existing greybox proxies remain as a safe fallback. This allows the gameplay root, CharacterController, follow logic and interaction bridge to stay intact while the visual child model is replaced.

## Asset placement

- Mia model and animation FBX files: `Assets/_Project/Art/Characters/Runtime/Mia/`
- Max model and animation FBX files: `Assets/_Project/Art/Characters/Runtime/Max/`
- Animator controller contracts: `Assets/_Project/Art/Characters/Runtime/Controllers/`

Use legally cleared assets and preserve the existing Mia/Max reference images as the visual source of truth. Do not replace or redesign character details without approval.

## Unity acceptance checklist

1. Run Prepare Character Asset Pipeline and confirm the folder/controller assets appear.
2. Run Production Asset Audit; contract files are scaffolds, not proof that final animation is ready.
3. Import the approved Mia FBX into the Mia folder and apply the Humanoid profile.
4. Import the approved Max FBX into the Max folder and apply the Generic profile.
5. Inspect Avatar/rig import warnings and preview all available clips.
6. Confirm no model import settings were applied to unrelated assets.

## Not validated here

The Editor tools were source-authored but not executed in Unity. No FBX assets, production rigs, animation clips or prefabs were imported or validated. Unity compilation and import tests remain pending.


## Runtime fallback behavior

- Without a generated Mia/Max Resources prefab, the game continues to display the current procedural proxy.
- With a generated prefab, the imported model is instantiated under the existing gameplay root.
- The gameplay root remains responsible for movement, collision, interaction and companion following; the visual model is a child.
- Review model scale and pivot in Unity before acceptance. The pipeline does not guess scale corrections or retarget custom skeletons.


### Separate animation FBX files

The import-profile tool intentionally changes only the selected base model FBX. For separate animation-only FBX files, configure the Model Importer to use the character's matching rig type and `Copy From Other Avatar`, then assign the Avatar from the base model. Do not use `Create From This Model` on animation-only files unless they contain a complete independent skeleton and Avatar.
