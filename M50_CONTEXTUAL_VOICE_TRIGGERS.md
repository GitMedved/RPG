# M50 — Contextual Mia & Max voice triggers

## Scope
M50 connects the M49 voice playback contract to successful gameplay outcomes. It does not generate or fabricate audio recordings.

## Implemented behaviour
- Successful planting plays Mia's `mia_plant_01` line.
- Successful harvesting plays Mia's `mia_harvest_01` line.
- Successfully starting a workshop recipe plays Mia's `mia_interact_01` line.
- Successfully collecting a finished workshop product plays Mia's `mia_success_01` line.
- Discovering a previously uncollected keepsake plays Max's `max_meow_curious_01` vocalization.
- `GameVoiceDirector` centralizes calls to the M49 resource-based audio service.
- Each cue has a 2.5-second same-ID cooldown. Missing clips remain silent and do not block the underlying action.

## Design constraints
- Voice is triggered only after the corresponding gameplay service confirms success; failed actions do not speak.
- No voice is played every frame or merely because the player is near an object.
- The dedicated voice AudioSource remains separate from music and sound effects.
- The director does not change save data, rewards, tutorial progression, economy or interaction cooldowns.
- Voice IDs map directly to files under `Assets/Resources/MiaMax/Audio/Voice/`.

## Audio still required
Import the approved WAV files from the M49 take manifest. Until then, trigger calls safely return without audible output; the runtime does not create empty placeholder clips. Max's meow needs a genuine cat-like sound-effect take, not a forced human TTS voice.

## Acceptance checklist
1. Import `mia_plant_01`, `mia_harvest_01`, `mia_interact_01`, `mia_success_01` and `max_meow_curious_01`.
2. Plant, harvest, start a recipe, collect a completed recipe and discover a keepsake; verify the matching cue each time.
3. Attempt invalid actions; verify they do not trigger a voice cue.
4. Repeat the same cue rapidly; verify the 2.5-second cooldown.
5. Set Voice Volume to zero; verify gameplay remains functional and voice is silent.
6. Verify that a missing clip only logs the existing audio warning and never blocks the action.

Unity compilation, audible playback, save/reload, Play Mode and Android/iOS device checks have not been run in this environment.
