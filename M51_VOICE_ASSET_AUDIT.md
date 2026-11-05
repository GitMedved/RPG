# M51 — Voice asset readiness audit

## Added
- Editor command: **Tools → Mia & Max → Voice Asset Audit**.
- Checks all 16 stable take IDs from the M49 voice audition set.
- Looks for WAV files under the exact runtime path `Assets/Resources/MiaMax/Audio/Voice/`.
- Reports each take as available or missing, with basic imported clip metadata for available files.
- Read-only: does not generate placeholder files, alter import settings or modify gameplay.
- Corrected the general Production Asset Audit to inspect the runtime Resources voice folder.

## IDs checked
Mia: `mia_greeting_01`, `mia_curiosity_01`, `mia_discovery_01`, `mia_plant_01`, `mia_harvest_01`, `mia_interact_01`, `mia_quest_01`, `mia_success_01`, `mia_encourage_01`, `mia_goodbye_01`.

Max: `max_meow_happy_01`, `max_meow_curious_01`, `max_meow_surprised_01`, `max_purr_01`, `max_effort_01`, `max_triumph_01`.

## Acceptance
1. Pull the feature branch and open the project in Unity.
2. Run **Tools → Mia & Max → Voice Asset Audit**.
3. Confirm the report matches the WAVs actually imported under the Resources folder.
4. Listen to every available take and review voice identity, pronunciation, performance and commercial usage rights separately.

The audit checks presence and basic clip metadata only. It does not prove the file is audible, correctly licensed, emotionally suitable or mixed consistently. Unity compilation and Editor execution have not been run in this environment.
