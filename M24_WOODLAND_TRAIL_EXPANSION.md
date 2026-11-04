# M24 — Woodland Trail expansion

## Player-facing flow
1. Complete the starter order chain and claim “A Busy Little Backyard” to unlock the Flower Garden.
2. Collect the three persistent flower resources.
3. Claim “A Path Beyond the Garden” in the Adventure Journal. This grants 40 coins, 45 XP and unlocks the Woodland Trail.
4. Use the new gate at the left/front edge of the backyard to enter the trail.
5. Explore the small woodland annex and collect an acorn, a mushroom and wild berries. Each is a one-time discovery that grants one inventory item, 5 XP and 3 coins.
6. Use the return gate to go back to the backyard.

## Implementation
- Added `CollectedTrailResources` to the local save and validation.
- Added a fourth story quest, with completion based on all three Flower Garden resources.
- Extended the existing portal component with configurable area IDs while retaining the Flower Garden defaults.
- Added a reusable gated-area root and a persistent collectible resource component.
- Built a small same-scene woodland annex with a ground platform, path, simple tree silhouettes, three resources and a return gate.
- Existing character/camera and interaction systems are reused.

## Scope and limitations
- This is a same-scene prototype annex made from runtime primitives, not final environment art.
- Resource IDs are `acorn`, `wild_mushroom` and `wild_berries`; they use the existing generic inventory stack system.
- No new save schema version is required: the new list is initialized and validated when old saves are loaded.
- Unity Editor compile, Play Mode, save persistence and mobile device tests remain required.
