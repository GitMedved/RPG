# M5 — Story Quests & World Progression Prototype

## Scope

This milestone adds a first persistent progression layer on top of the existing farming, production and order loops. It deliberately stays small: three one-time story quests, a simple backyard adventure journal interaction and one future-area unlock.

## Quest chain

1. **A New Adventure** — unlocks as soon as the player reaches level 2. Reward: 10 coins and 15 XP.
2. **Helping the Neighbours** — unlocks after the first order is completed. Reward: 15 coins and 20 XP.
3. **A Busy Little Backyard** — unlocks after all three starter orders are completed. Reward: 30 coins and 35 XP; permanently unlocks the `flower_garden` world-area ID.

Quest completion is derived from existing saved progression and order data. Claim records are persisted by quest ID, so a reward cannot be claimed twice. The adventure journal reports the number of claimed rewards and currently claimable rewards.

## Implementation

- `QuestProgressData` stores quest ID and claim timestamp.
- `PlayerSaveData` now stores claimed quests and unlocked world-area IDs.
- `LocalSaveService` initializes and validates the new collections. The save schema remains v1 because the additions are optional fields and old saves receive empty collections during validation.
- `QuestService` owns quest conditions, reward claiming and area-unlock queries.
- `QuestBoardPrototype` connects the service to an interactable adventure journal in the backyard blockout.
- No new area scene is fabricated; `flower_garden` is a persistent unlock flag for a later map/scene milestone.

## Validation status and limitations

- Source files were reviewed through GitHub after changes.
- Unity Editor compilation, Play Mode, save migration/recovery and Android/iOS builds have not been run in this environment.
- Quest feedback is currently logged and shown through the existing prototype interaction label; there is no polished quest list, reward popup, quest markers or actual flower-garden scene yet.
- Quest definitions and rewards are currently hardcoded starter content. They should move to data assets when the content volume grows.
