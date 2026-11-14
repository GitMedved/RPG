# M42 — Tap Adventure Runtime Repair

**Branch:** `codex/mia-max-foundation`  
**Status:** code committed; Unity validation pending.

## What changed

The startup path previously logged that an illustrated world had been created, but it did not call any of its own world-building methods. Its camera was also positioned at ground level, looking horizontally across the yard. The optional `BackyardScene.png` backdrop was absent, so the app could launch into an effectively empty screen.

The runtime bootstrap now:
- calls the existing ground/path, house, tree/birdhouse, flower bed, workshop, pond/decor and gate builders;
- loads the existing authored URP materials from `Resources/MiaMax/Materials` (grass, wood, plaster, patio stone, leaves, petals, roof and pond water) rather than depending on a missing atlas;
- creates a fixed, angled, portrait-oriented orthographic camera and a warm directional light;
- selects actual 3D objects by camera raycast against their colliders; hard-coded screen-space hotspot coordinates have been removed;
- does not stretch the optional concept-art image over the live 3D world;
- safely handles a malformed saved flower-growth timestamp;
- persists one-time collection flags for fallen twigs and the feather, and one-time reward flags for the sorting mini-game and golden garden stone;
- creates the Secret Garden clearing when its unlock recipe is completed.

## First playable interaction chain

1. Tap the old tree to collect 2 twigs (once).
2. Tap the fallen twigs to collect 3 more (once).
3. Tap the blue feather to collect it (once).
4. Tap the birdhouse and build it for 3 twigs + 1 feather.
5. Water the flower bed; wait one minute; return to collect 3 flowers.
6. At the crafting bench, spend 1 twig + 1 flower to unlock the Secret Garden.
7. Tap the treasure basket to play Max's three-step sorting mini-game. Its reward can be claimed only once.

The tree and fallen-twig pickups are separate discoveries; together they provide enough twigs for the birdhouse and leave one twig for the keepsake recipe.

## Local Unity smoke test

1. Pull the latest `codex/mia-max-foundation` branch.
2. Open the project in Unity 6000.3.0f1 and wait for package import/compilation to finish.
3. Run **Mia & Max → Prototype → Validate Tap Adventure Materials** and confirm all 8 authored materials are found. If you need a clean first-time run, use **Mia & Max → Prototype → Reset Tap Adventure Progress** (it deletes only this prototype's PlayerPrefs keys).\n4. Open `Assets/_Project/Scenes/Backyard.unity` and press Play. The scene file is intentionally lightweight; the runtime bootstrap creates the tap world after scene load.
4. Confirm the yard, house, tree, path, flower bed, workbench, pond and gate are visible from the fixed angled camera.
5. Tap the tree, fallen twigs, feather, birdhouse, flower bed, workbench and treasure basket. Confirm each selection opens the matching contextual card.
7. Complete the birdhouse recipe and confirm the resource counts decrease exactly once.
8. Water the flower bed and wait 60 seconds. Confirm the blooms visibly enlarge and can then be collected.
9. Complete the keepsake recipe and confirm the Secret Garden clearing appears.
10. Finish the sorting mini-game, reopen it and confirm it no longer grants another reward.
11. Stop Play Mode, press Play again and confirm inventory, crafted objects, unlocks and one-time rewards persist.

Use the prototype-specific reset menu above when you need to repeat the first-time sequence. It preserves all other application preferences.

## Known limitations — not release-ready

- The world is still a **temporary primitive-based greybox**, albeit now textured with the authored URP material assets. This repair does not magically turn the greybox into approved final 3D environment art.
- Mia and Max production FBX models, character rigs and animation clips are still missing; the runtime scene does not yet show approved animated hero models.
- The tap-adventure prototype currently stores its small story-chain state in its own PlayerPrefs keys. It has not yet been migrated to the canonical `GameSession` / inventory / save / quest services.
- The third-party Unity Adventure Game sample has not been downloaded or imported; it remains an isolated reference candidate, not a dependency.
- No Unity Editor compile, Play Mode session, Android build, iOS build or physical-device smoke test can be run through the GitHub repository editor. These are still mandatory before describing this as a verified playable prototype.

## Commits

- `57b7ea7` — restore 3D runtime world, camera and raycast selection.
- `746d5d3` — prevent repeatable resource and mini-game reward exploits.
- `06eeb7c` — use authored URP material assets.
- `cf2f90e` — tune portrait camera and flower-growth feedback.
- `81fb35b` — remove obsolete hotspot tables.
