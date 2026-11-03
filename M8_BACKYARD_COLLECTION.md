# M8 — Backyard Keepsake Collection

## Implemented prototype
- Five persistent backyard discoveries: red ball, blue feather, old button, yellow leaf and tiny shell.
- Each keepsake is a world object with a one-time interaction; discovered items disappear on subsequent loads.
- The collection book reports progress and grants a one-time completion reward of 25 coins and 30 XP.
- Discovery IDs and reward-claimed state are stored in the local player save.

## Prototype limitations
- Keepsakes are colored primitive meshes and are deliberately temporary.
- Interaction and collection progress are logged through the current prototype interaction system; there is no polished collection screen, audio feedback, collectible animation or inventory gallery.
- The five item placements are provisional and need visual/playability review in Unity.
- Save schema remains additive at version 1; fields default safely for existing JSON saves.

## Validation still required
Open the project in the recorded Unity Editor baseline, allow script import, inspect the generated backyard, verify each pickup and save/reload, and test touch interaction on Android/iOS. No Unity compile or device test has been run in this environment.
