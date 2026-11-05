# M17 — Audio, sound effects and English character voice

**Status:** runtime audio facade implemented; licensed audio recordings and clips pending.

## Runtime integration
Added `GameAudioService.cs`. It is created automatically and remains silent until clips are assigned. It exposes:
- `Play(GameAudioCue)` for one-shot effects.
- `PlayVoiceLine(AudioClip)` for character voice clips.
- `SetMusic(AudioClip)` for a looping music bed.
- `StopVoice()` for interrupting the current voice line.

The service uses separate effects, music and voice AudioSources. Missing clips are safe and do not block gameplay. `BackyardInteractable` already requests the optional Interact cue.

## Required sound library
- UI: tap, open/close, reward/confirmation, error/denied.
- Backyard: birds, breeze, distant neighbourhood ambience, footsteps on grass/path/deck.
- Farming: soil/plant, watering, harvest.
- Workshop: crafting start, crafting ready, pickup.
- Collection/progression: keepsake discovery, quest completion, level-up.
- Arcade: start, positive result, miss/fail, coin/reward.
- Music: calm backyard loop, garden discovery loop, light arcade loop.

## English voice direction
Warm, friendly, natural, expressive, short and easy to understand for a family audience. Preserve the established Mia and Max voices from the user's existing cartoon reference where usage rights allow. Do not imitate a third-party performer or use voice assets without permission. Max's vocalizations should remain short expressive cat sounds unless the approved cartoon reference establishes spoken words.

## Starter voice script (record only after voice identity approval)
Mia:
- "Hi, Max! Ready for an adventure?"
- "Let's plant something wonderful."
- "Look what we found!"
- "We did it, Max!"
- "One more little adventure?"

Max: short happy meows, curious chirps, purrs, surprised meow and playful effort sounds; no invented spoken dialogue.

## Recording and delivery
Record clean, dry mono voice takes at 48 kHz / 24-bit WAV when practical. Import SFX under `Assets/Resources/MiaMax/Audio/SFX/` using the lowercase cue filenames in the runtime adapter (for example `interact.wav`, `harvest.wav`, `minigame_win.wav`); the service loads these resources automatically. Voice lines and music can be assigned through the service API or a future content configuration asset. Deliver separate takes, no baked-in music/reverb, consistent loudness, clear filenames and written usage rights. Export compressed runtime versions after editing. Keep voice and music volume independently adjustable; add mute controls before release.

## Safety and privacy
No recording of a child's voice or personal data is required for this milestone. Do not upload private voice samples to third-party services without the user's explicit approval and confirmation of rights. No analytics or online voice processing is introduced.

## Acceptance
All clips have documented provenance/licence, no clipping or abrupt tails, correct cue mapping, separate volume controls, pause/background behaviour verified, and audio tested on speakers and headphones. Until clips are imported, the project intentionally remains silent.


## M49 voice identity and playback update
The current approved direction is American English with natural cartoon acting. Mia is bright, sweet and playful. Max is a funny cat with short scripted phrases where appropriate plus expressive meows, chirps and purrs. The first 16-cue audition script and exact recording/import pipeline are documented in `M49_MIA_MAX_VOICE_TEST_AND_UNITY_PIPELINE.md`. Approved individual takes belong in `Assets/Resources/MiaMax/Audio/Voice/` and can be played by stable ID through `GameAudioService.PlayVoiceLine(string)`. The older starter lines above are superseded by the M49 audition set. No final audio recordings are bundled yet; preserve rights/provenance and do not clone or upload private voices without permission.
