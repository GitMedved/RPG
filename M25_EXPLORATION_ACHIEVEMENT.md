# M25 — Exploration achievement and journal progression

## Changes
- Added the eighth lifetime achievement, **Trailblazer**, for finding all three Woodland Trail resources.
- The achievement reads the persisted `CollectedTrailResources` list, so progress survives reloads and cannot be double-counted by repeated interaction.
- Reward: 30 coins and 25 XP, claimable once through the existing Adventure Journal flow.
- Expanded the Journal achievement section to show all eight achievements rather than only the first three. The scrollable panel was extended to fit the complete list and claim feedback.

## Validation
Source-level structural checks only. Unity Editor compilation, Play Mode, save/load persistence and device UI layout checks remain pending.
