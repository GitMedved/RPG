# Mia & Max Adventures — Original English voice and sound integration

## Cast

- **Mia:** Cassidy - Expressive & Imaginative (HeyGen, en-US), cheerful, friendly and curious.
- **Max:** Jalen - Snappy & Charismatic (HeyGen, en-US), playful quick replies and occasional meows.
- This is original English dialogue for Mia & Max; these are provisional production cast choices pending listening QA and provider usage-rights review.

## Six generated speech takes

| Clip ID | Character | Original dialogue | Approx. seconds |
| --- | --- | --- | --- |
| `mia_greeting_01` | Mia | “Hi there! I'm Mia, and this is Max. Shall we build a little birdhouse together?” | 4.73 |
| `mia_birdhouse_ready_01` | Mia | “We did it, Max! A cozy little birdhouse for our new friends. I can't wait to see who moves in!” | 6.23 |
| `max_birdhouse_01` | Max | “Meow! Purr-fect work, Mia! Our next adventure is waiting!” | 3.76 |
| `mia_secret_garden_01` | Mia | “Look, Max! The garden gate is open! I wonder what wonderful things we'll discover inside.” | 6.14 |
| `max_garden_discovery_01` | Max | “Whoa, a golden stone! Meow! This place is full of surprises!” | 4.69 |
| `max_treasure_win_01` | Max | “Meow! We found them all! I knew we could do it!” | 3.33 |

The output URLs are saved in `Assets/_Project/Scripts/Editor/MiaMaxVoiceTakesImporter.cs`
and `.github/workflows/package-mia-max-voices.yml`, both of which include download checks.
Do not rely on the CDN URLs in the shipped game.

## Real cat sounds (installed in Git)

- `Assets/Resources/MiaMax/Audio/SFX/max_meow_01.ogg`
- `Assets/Resources/MiaMax/Audio/SFX/max_meow_02.ogg`

Two actual CC0 kitten recordings from Joseph Sardin, BigSoundBank, mirrored in
[druboni/find-the-cat](https://github.com/druboni/find-the-cat/blob/main/CREDITS.md).
Max's pond/treasure interactions use these clips. The audio manager applies
a minimum 4.5-second cooldown and does not play them over spoken dialogue.

## Offline WAV packaging

- **GitHub Actions**: `.github/workflows/package-mia-max-voices.yml`.
  It retrieves and validates up to six original provider WAVs, filters
  them with FFmpeg (85 Hz high-pass, 10 kHz low-pass),
  targets **-17 LUFS / -1.5 dBTP**, converts to mono PCM and attempts to
  commit them to `Assets/Resources/MiaMax/Audio/Voice/`.
  An unsuccessful job is not proof that any audio was installed.
- **Unity Editor fallback**: **Mia & Max → MVP → 4. Install Generated English Voice Takes**.
  It fetches the six WAVs directly while outside Play Mode and imports them
  into the same Resources directory. Commit the downloaded WAV and .meta
  files after checking quality.

The game always uses local `Resources.Load<AudioClip>`. It does not
call speech providers at runtime and continues playing if voices are absent.

When present, Mia's greeting is played once for a new local game. The
birdhouse scene plays Mia and then Max, the garden unlock/stone scenes
have their own lines, and the sorting mini-game plays Max's victory line.
Character animated mouths respond to the approximate length of the voices;
these are not phoneme-level lip-sync animation clips.

## Sound QA still required

1. Confirm all six actual .wav files exist in Git and import into Unity.
2. Review voice performances for natural US English, no clipped syllables,
   consistent voice timbre and reasonable loudness.
3. Listen on phone speakers and with headphones. Music/ambience are ducked
   under speech; verify that the transitions sound smooth.
4. Apply any final artistic mix, mastering, captions and commercial-use checks.

See `LICENSED_3D_AND_VOICE_PIPELINE.md` for the full art/rig workflow.
