# M13 — Flower Garden Annex

## What is included

- A gated annex named Flower Garden is generated as part of the backyard prototype.
- The annex remains hidden until the player claims the final starter story quest, which already unlocks the `flower_garden` area ID.
- The entrance gate is interactable from the backyard. Locked interaction explains the story requirement.
- An entrance portal moves Mia into the annex; a return gate moves her back to the backyard.
- Three one-time resource nodes provide pink, yellow and purple flowers.
- Each collected flower adds one matching inventory item, 3 XP and 2 coins.
- Resource collection is stored in the local player save and restored when the scene is rebuilt.

## Acceptance checks in Unity

1. On a fresh save, confirm the annex interior is hidden and the entrance gate reports that the story quest is required.
2. Complete and claim all three starter story quests. The annex should appear after the final reward is claimed.
3. Interact with the entrance gate and confirm Mia appears inside the fenced flower garden.
4. Collect all three flowers. Each node should disappear and inventory should gain the matching flower item.
5. Use the return gate and confirm Mia returns to the backyard.
6. Exit and re-enter Play Mode. The annex should remain unlocked and collected flowers should remain gone; inventory should retain the three flower items.
7. Confirm the existing backyard plots, keepsakes, workshop, order board and arcade remain usable.

## Current limitations

- This is still a generated blockout, not final art.
- The garden is a small annex inside the current backyard scene, not a separately streamed scene.
- No new story dialogue, NPCs, daily respawns, flower crafting recipes or Android/iOS device tests are included.
- Compile and Play Mode validation are pending for this package.
