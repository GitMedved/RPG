# M43 — Character proxy animation polish

## Scope
Added lightweight procedural presentation animation to the existing Mia and Max greybox character models:
- subtle idle breathing for Mia's torso and Max's body;
- small idle head motion;
- periodic eye blinks (including Max's pupils so they remain aligned);
- a slightly varied, speed-responsive limb gait;
- a softer idle tail sway for Max.

## Boundaries
- The changes run only on the procedural fallback created by `StylizedCharacterPrototype`.
- If imported production prefabs exist in Resources, they are loaded as before and this proxy animation script is not attached to them.
- No character reference design, proportions, gameplay movement, collision, save data, Animator parameter contract or rewards were changed.
- This is presentation polish, not a production rig, authored animation clips, facial rig or final character model.

## Unity acceptance
1. Pull `codex/mia-max-foundation` and wait for compilation.
2. In Play Mode, observe Mia and Max standing still for at least 10 seconds: breathing/head movement and occasional blinks should be subtle.
3. Move Mia and check the walk cycle responds to movement without excessive limb rotation.
4. Observe Max's tail and facial alignment while idle and moving.
5. Check Console and confirm interactions and character movement remain unchanged.

## Limitations
Animation is procedural and frame-driven; there are no imported clips or skinned meshes in this milestone. Unity compile, Play Mode visual review and Android/iOS performance have not been run remotely.
