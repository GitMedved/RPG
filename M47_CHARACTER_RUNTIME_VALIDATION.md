# M47 — Production character runtime hardening

## Goal
Make the existing character import pipeline safer to use when approved production Mia and Max FBX assets become available. This milestone improves integration and diagnostics; it does not invent or generate final character art.

## Runtime animation bridge
`CharacterAnimationBridge` now:
- Finds the Animator in the imported model hierarchy.
- Disables root motion so the gameplay CharacterController (Mia) and companion-follow movement (Max) remain authoritative.
- Checks that each expected parameter exists with the correct type before sending it:
  - `Speed` — Float
  - `Grounded` — Bool
  - `Interact` — Trigger
  - `Celebrate` — Trigger
- Rebinds parameter availability if an Animator Controller is assigned after prefab instantiation.
- Respects the existing Reduced Motion setting for interaction and celebration triggers.

## Editor validation
Run:

`Tools → Mia & Max → Validate Character Runtime Prefabs`

The report checks Mia and Max runtime prefabs for:
- Prefab presence at `Assets/Resources/MiaMax/Characters/{Mia|Max}.prefab`
- Animator component
- Valid imported Avatar
- Assigned Animator Controller
- Root Motion disabled
- Complete parameter contract with matching parameter types

A READY result only means the prefab integration contract is present. The validator does not assess mesh quality, rig deformation, clip content, blend trees, state transitions, facial animation, licensing or runtime performance.

## Import conventions
- Mia base model: place one unambiguous base FBX in `Assets/_Project/Art/Characters/Runtime/Mia`, preferably named `Mia.fbx` or `Mia_Model.fbx`. Import as Humanoid.
- Max base model: place one unambiguous base FBX in `Assets/_Project/Art/Characters/Runtime/Max`, preferably named `Max.fbx` or `Max_Model.fbx`. Import as Generic because Max is a quadruped.
- Keep animation-only FBX files separate from the base model. Configure them to copy the Avatar from the base model in the Model Importer.
- Run `Tools → Mia & Max → Apply Character FBX Import Profiles` after importing the base models.
- Review model scale, forward axis, pivot/ground contact, Avatar mapping and deformation manually before accepting the prefabs.

## Acceptance checklist
1. Pull `codex/mia-max-foundation`.
2. Wait for Unity compilation.
3. Run `Tools → Mia & Max → Validate Character Runtime Prefabs`.
4. If no production FBX assets have been imported, PENDING results are expected and are not a prototype failure.
5. When assets exist, verify Mia Humanoid and Max Generic Avatars, in-place movement clips, interaction/celebration states and Max's quadruped gait.
6. Enter Play Mode and confirm root movement is not doubled by animation.
7. Profile animation and skinning on representative Android and iOS devices.

## Current limitation
The Animator contract controllers are scaffolds containing parameters only. No production animation clips, blend trees, transitions, final rigs or voice assets are included by this milestone.

## Verification status
Source-level checks only. Unity compilation, Editor menu execution, FBX import, animation playback and mobile profiling have not been performed remotely.
