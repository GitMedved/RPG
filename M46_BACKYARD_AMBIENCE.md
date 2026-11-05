# M46 — Main backyard ambient life

## What changed

Added `BackyardAmbientPrototype` to the runtime backyard after its existing visual dressing has been created.

### Gentle wind animation
- Central tree canopy clusters move subtly.
- Lawn-edge shrubs and decorative flower details sway gently.
- Patio planter stems, leaves and blooms have small independent movement.
- Decorative transforms are discovered and cached once; no scene-wide search occurs in Update.
- The animation respects the persisted Reduced Motion setting. When enabled, animated parts return to their authored base rotations.

### Optional ambient audio
The controller looks for two optional looping clips:
- `Assets/Resources/MiaMax/Audio/Ambience/backyard_wind`
- `Assets/Resources/MiaMax/Audio/Ambience/backyard_birds`

The expected asset type is AudioClip. The loops are intentionally quiet and use the saved Effects Volume preference. No sound recordings are included in this change; without those clips, the game remains silent in the backyard. Use original or properly licensed audio.

## Safety and performance
- Only objects with the known decorative names are animated.
- Player, Max, farm plots, stations, portals, collectibles and gameplay colliders are not modified.
- No new colliders, lights, particle systems, save fields or gameplay rules.
- Wind animation is disabled visually by Reduced Motion.
- Audio sources are created only when the corresponding AudioClip is found.

## Unity acceptance checklist
1. Pull `codex/mia-max-foundation` and wait for Unity to compile.
2. Enter Play Mode and inspect the central tree, flowers, shrubs and patio planters.
3. Toggle Settings → Reduced Motion; foliage should stop moving and return to its base pose.
4. If ambience AudioClips are available under the paths above, verify wind and birds play quietly.
5. Set Effects Volume to zero and confirm ambient loops become silent.
6. Check that Mia, Max, garden plots, stations and collectibles behave exactly as before.
7. Profile on representative Android and iOS devices.

## Verification status
Source-level structural checks only. Unity compilation, Play Mode, audio playback and mobile profiling have not been run remotely.
