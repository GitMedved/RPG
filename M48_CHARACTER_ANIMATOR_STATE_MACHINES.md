# M48 — Character Animator State Machines

## Goal

Provide a repeatable Editor-side state-machine scaffold for imported Mia and Max clips without inventing animation assets or changing gameplay movement.

## Editor command

Run **Tools → Mia & Max → Build Character Animator State Machines** after **Prepare Character Asset Pipeline**.

The builder uses the existing controller contracts in `Assets/_Project/Art/Characters/Runtime/Controllers` and searches each character's runtime source folder for imported AnimationClip assets.

Recognized clip-name keywords (case-insensitive):
- `idle`
- `walk`
- `run`
- `interact`
- `celebrate`

Exact keyword matches are preferred, then the first name containing the keyword. Use clear, unique clip names such as `Mia_Idle`, `Mia_Walk`, `Max_Idle`, etc. Review every assigned clip in Unity; naming alone cannot verify its content or rig compatibility.

## Generated behavior

- Idle is the default state. If no Idle clip exists, it is an empty placeholder.
- Idle ↔ Walk uses Speed threshold 0.08.
- Walk ↔ Run uses Speed threshold 0.65.
- If Walk is absent but Run exists, Idle ↔ Run is wired.
- Interact and Celebrate states are created only when matching clips exist. Any State trigger transitions enter them; they return to Idle near clip completion.
- Root motion remains disabled by the runtime pipeline; gameplay roots own movement.

The builder clears and recreates the generated Base Layer states each time it runs. It does not change imported FBX files or create clips. Re-running it is safe for the generated layer but replaces any custom states/transitions previously added to that same Base Layer. Keep bespoke animation work in a separate layer/controller or rebuild it after running this tool.

## Important limitations

- No production animation clips are included in the repository. Without imported clips the command reports missing clips and creates only the empty Idle placeholder.
- Max still requires a properly authored quadruped Generic rig and clips. Do not use Mia's Humanoid clips for Max.
- Grounded remains part of the shared parameter contract for future jump/fall states; this first scaffold does not add jump/fall transitions.
- No Unity Editor compilation, clip preview, Play Mode animation, Avatar deformation, or Android/iOS profiling is claimed. These require local Unity validation.

## Acceptance

1. Import approved Mia and Max FBX assets into their respective folders.
2. Confirm Mia is Humanoid and Max is Generic with valid Avatars.
3. Run the builder and inspect Console for missing clip names.
4. Open both Animator Controllers and preview each assigned state.
5. Enter Play Mode: verify idle/walk/run respond to movement and Interact/Celebrate triggers play once and return to Idle.
6. Confirm gameplay movement is unchanged and Root Motion remains disabled.
