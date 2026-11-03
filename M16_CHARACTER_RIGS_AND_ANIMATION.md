# M16 — Character rigs and animation integration

**Status:** runtime adapter implemented; authored character rigs and clips pending import.

## Runtime integration
Added `CharacterAnimationBridge.cs`. Attach it to the root of Mia's player prefab after the production rig is imported. It reads root movement and optional CharacterController grounding, then drives an Animator when one is assigned. Without an Animator Controller it safely does nothing.

Animator Controller contract:
- `Speed` — float, normalized 0–1 (idle to run blend).
- `Grounded` — bool.
- `Interact` — trigger.
- `Celebrate` — trigger.

`PlayerInteraction.TryInteract()` triggers `Interact` on the player bridge before executing the world interaction. Do not use these parameter names in the Animator unless the controller follows this contract.

## Required animation set
**Mia:** idle, walk, run, turn-in-place, interact, plant, harvest, craft, celebrate, surprised/concerned reaction and mini-game poses.

**Max:** idle, walk, run, jump, sit, sleep, play, sniff, eat, tail/ear reactions, happy and surprised reactions. Keep the cat's quadruped locomotion and silhouette faithful to the supplied reference.

## Retargeting and import
- Mia: Humanoid only after avatar mapping and retarget tests.
- Max: Generic/custom quadruped rig; validate root motion and tail/ear secondary motion separately.
- Prefer in-place locomotion while the current CharacterController owns movement.
- Disable root motion unless a specific authored action is deliberately synchronized with gameplay.
- Keep interaction/action clips short and interruptible.
- Use animation events only for cosmetic timing; gameplay rewards and inventory mutations must remain in domain services.

## Acceptance
Idle/walk/run blend is stable at 30/60 FPS; no foot sliding beyond agreed tolerance; character faces movement direction; interaction trigger plays once; no Animator errors when a clip/controller is absent; Max's follow movement does not fight root motion.
