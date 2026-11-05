# M49 — Mia & Max voice identity, first test set and Unity pipeline

## Locked voice bible

- Language/accent: American English.
- Performance style: natural cartoon acting; conversational timing, clear articulation, emotionally truthful rather than exaggerated announcer delivery.
- Mia: bright, sweet, playful; youthful feminine character voice, warm and curious, expressive but not shrill.
- Max: funny cat character; short spoken phrases only when deliberately scripted, plus expressive meows, chirps, purrs and playful effort sounds. Keep the cat identity audible; do not turn him into a generic human child voice.
- Keep the same selected voice identities, pronunciation, relative pitch, pacing and recording chain across game and YouTube. Do not clone a real person's voice or upload private reference recordings without permission.

## First audition script

Record each ID as its own dry mono WAV. Parenthetical direction is not spoken.

| ID / filename | Character | Spoken line / sound | Performance direction |
|---|---|---|---|
| mia_greeting_01 | Mia | Hi, Max! Ready for an adventure? | Bright greeting, smile in voice |
| mia_curiosity_01 | Mia | Hmm... I wonder what's over there. | Curious, thinking aloud |
| mia_discovery_01 | Mia | Oh! Look what I found! | Small surprised lift, then delight |
| mia_plant_01 | Mia | Let's plant something wonderful. | Warm encouragement |
| mia_harvest_01 | Mia | We grew this together! | Proud, delighted |
| mia_interact_01 | Mia | Let's see what we can do here. | Friendly, inviting |
| mia_quest_01 | Mia | Max, I think someone needs our help. | Gentle concern, optimistic |
| mia_success_01 | Mia | We did it, Max! | Genuine happy celebration |
| mia_encourage_01 | Mia | Come on, Max. One more try! | Playful encouragement |
| mia_goodbye_01 | Mia | What a lovely day. See you on our next adventure! | Soft, affectionate ending |
| max_meow_happy_01 | Max | Happy meow | Short bright rising meow |
| max_meow_curious_01 | Max | Curious chirp-meow | Two-note questioning chirp |
| max_meow_surprised_01 | Max | Surprised meow | Quick, light surprise |
| max_purr_01 | Max | Gentle purr | Warm, relaxed, 1–2 seconds |
| max_effort_01 | Max | Playful effort sound | Tiny playful grunt/chirp, not distressed |
| max_triumph_01 | Max | Happy trill-meow | Short celebratory trill |

### Audition notes
- Produce two or three alternate takes for Mia greeting, discovery and success; select one canonical take for each.
- For Max, record several natural cat-like variants; avoid forcing human words into meows.
- The lines are original starter copy for testing, not locked story canon. Review pronunciation, age-appropriateness and voice consistency before final approval.
- If the selected TTS provider cannot make convincing cat vocalizations, create Max's nonverbal sounds as separate sound-design assets rather than using spoken TTS.

## Audio generation / recording specification

1. Generate/record one line per file; do not combine speakers or lines.
2. Master source: WAV, mono, 48 kHz, 24-bit PCM where supported. Keep clean/dry: no music, reverb, ambience, compression pumping or background noise baked in.
3. Leave a short natural head/tail; trim long silence. Avoid clipping. Keep perceived loudness consistent across takes; do final loudness matching in the audio editor, not by flattening expressive dynamics.
4. Retain the provider/project source and a text record of voice/model, generation date, settings, licence/usage terms and approved take. Confirm commercial game and YouTube usage rights before release.
5. Create Unity runtime copies as WAV or compressed Vorbis/quality-appropriate AudioClips. Voice assets should be mono for centered dialogue; use platform compression after listening tests.

## Unity folder and naming contract

Place approved files here (without changing the IDs):
`Assets/Resources/MiaMax/Audio/Voice/`

Examples:
- `Assets/Resources/MiaMax/Audio/Voice/mia_greeting_01.wav`
- `Assets/Resources/MiaMax/Audio/Voice/max_meow_happy_01.wav`

Unity loads clips with `Resources.Load<AudioClip>("MiaMax/Audio/Voice/" + lineId)`. The M49 runtime integration adds `GameAudioService.PlayVoiceLine(string lineId, float volume = 1f)`. It stops the previous voice line before playing the next, uses the existing dedicated voice AudioSource and respects the existing Voice Volume mix setting. Missing files return false and produce a clear Console warning; gameplay does not fail.

Example call from a gameplay component:
```csharp
GameAudioService.Instance?.PlayVoiceLine("mia_greeting_01");
GameAudioService.Instance?.PlayVoiceLine("max_meow_happy_01");
```

To stop current dialogue:
```csharp
GameAudioService.Instance?.StopVoice();
```

Do not put music or sound effects into the Voice folder. Existing SFX and ambience paths remain unchanged.

## First Unity integration test

1. Import the approved WAV files into the exact Resources path above.
2. Let Unity finish importing; select each clip and verify it is not 3D/spatialized, does not loop, and uses suitable compression.
3. Temporarily call the example line from a test interaction or use a small temporary test component; do not add permanent auto-play at scene start.
4. Test two Mia lines and two Max sounds in sequence; verify the second line cleanly replaces the first.
5. Change Settings → Voice Volume to 0 and confirm voice is silent; restore a comfortable level.
6. Test Android and iOS device speakers/headphones, app pause/resume and repeated interactions.
7. Confirm no line is cut off by a UI/SFX source; dialogue and SFX use separate AudioSources.

## Current scope and limitations

- M49 locks the voice direction, provides original test copy and adds resource-based runtime playback.
- No generated or recorded WAV files are included in this commit. The project remains silent until approved audio is imported.
- No provider voice ID has been selected yet; do not treat a temporary TTS voice as the final canonical voice until auditioned and approved.
- The playback method is ready to be called by gameplay, but existing interactions are not yet assigned dialogue triggers.
- Unity compilation, audio import, audible playback and device testing remain pending.
