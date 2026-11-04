# M20 — Production asset readiness audit

**Status:** Unity Editor audit utility committed; execution in the user's Unity Editor is pending.

## Run it
After pulling the feature branch and allowing Unity to compile, use:
`Tools → Mia & Max → Production Asset Audit`

The tool prints a report to the Console and shows a short summary dialog. Missing production assets are marked PENDING; the tool does not break prototype startup or claim that an asset is approved.

## Checks
- Mia, Max and backyard reference images.
- Mia and Max production FBX/model assets.
- Mia and Max Animator Controllers.
- UI sprites.
- Sound effects under `Assets/Resources/MiaMax/Audio/SFX`.
- Voice recordings under `Assets/_Project/Audio/Voice`.
- Music under `Assets/_Project/Audio/Music`.

The audit checks presence only. It cannot validate artistic similarity, rig deformation, animation quality, audio rights, mix quality, platform compression or runtime performance. Those require human review and Unity/device testing.

## Notes
The project already contains original reference images in `Assets/_Project/Art/`. Production models, animator controllers, final UI sprites and audio recordings remain pending until imported.


## M36 audit clarification

The Animator Controller entries now specifically search for the generated `Mia_AnimatorContract` and `Max_AnimatorContract` scaffolds. These contain the agreed parameter contract only and are not evidence of completed animation. The audit also checks for imported Mia and Max AnimationClip assets in their respective Runtime folders. See `M36_CHARACTER_ASSET_PIPELINE.md`.
