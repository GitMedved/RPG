# M3 — Farming Loop Prototype

## Scope implemented
- Added persistent farm plot data to the player save.
- Added a domain-level farming service with three crop IDs: carrot, tomato and strawberry.
- Added three interactive garden plots to the runtime backyard blockout.
- New profiles receive three starter seeds of each crop.
- Planting consumes one matching seed, grants 2 XP and stores a UTC planting timestamp.
- Crops advance through three visible placeholder growth stages using elapsed real time. Prototype growth targets: carrot 1 minute, tomato 2 minutes, strawberry 3 minutes.
- Harvesting a mature crop grants 2 produce, 8 XP and 3 coins; plot state is cleared and saved.
- Existing local save notifications persist plot state without requiring network access.

## Interaction
Approach a plot until the interaction prompt appears, then press E/Space in Editor or use the existing touch interaction area on device. The prototype currently plants the first available seed in carrot → tomato → strawberry order. A mature crop can be harvested through the same interaction.

## Limitations
- Crop visuals are simple placeholder spheres and do not represent final Mia & Max art direction.
- There is no crop picker UI, inventory UI, watering, fertilizer, weather, offline time balancing or production recipes yet.
- Growth uses UTC wall-clock time; device clock manipulation is not protected against.
- No Unity Editor compilation, Play Mode test or Android/iOS build has been run in this environment. Treat the code as an implementation draft pending Editor validation.
