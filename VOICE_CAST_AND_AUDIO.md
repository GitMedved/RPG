# Mia & Max Adventures — English voice cast, direction and asset integration

## Cast and performance

- **Mia:** HeyGen `Cassidy - Expressive & Imaginative`, en-US. Bright, warm, playful and reassuring; natural American English. Original reference image remains the source of truth for her appearance.
- **Max:** HeyGen `Jalen - Snappy & Charismatic`, en-US. Lively, playful, short cat companion replies, including occasional natural-sounding “meow.” These are provisional voice choices pending a listening test with the real game.

All lines are original dialogue for *Mia & Max Adventures*, generated on **2026-11-15**.

## Generated WAV takes

| Unity Resources clip ID | Character | Original dialogue | Approx. duration |
| --- | --- | --- | --- |
| `mia_birdhouse_ready_01` | Mia | “We did it, Max! A cozy little birdhouse for our new friends. I can't wait to see who moves in!” | 6.23 s |
| `max_birdhouse_01` | Max | “Meow! Purr-fect work, Mia! Our next adventure is waiting!” | 3.76 s |
| `mia_secret_garden_01` | Mia | “Look, Max! The garden gate is open! I wonder what wonderful things we'll discover inside.” | 6.14 s |
| `max_garden_discovery_01` | Max | “Whoa, a golden stone! Meow! This place is full of surprises!” | 4.69 s |

The generated audio links are retained in `Assets/_Project/Scripts/Editor/MiaMaxVoiceTakesImporter.cs`. They are not a permanent asset hosting solution and may expire.

## One-click installation in Unity

1. Open the project in Unity 6000.3.0f1, leave Play Mode.
2. Run **Mia & Max → MVP → 4. Install Generated English Voice Takes**.
3. Confirm the download. The importer checks the RIFF/WAV header and writes the four files to `Assets/Resources/MiaMax/Audio/Voice/`.
4. Wait for Unity to import the audio clips. A Console summary reports success/failure per take.
5. Play the game and finish the birdhouse, keepsake and garden-stone actions. Mia speaks after the birdhouse and Max responds; during relevant lines each on-screen proxy character has simple animated mouth movement.
6. After reviewing sound and rights for commercial use, commit the imported WAV files and their Unity `.meta` files on the feature branch.

The game plays **offline**. The importer connects to HeyGen only when you explicitly request installation in the Unity Editor.

## Quality acceptance criteria

- Mia sounds friendly and curious, not robotic, and remains intelligible at mobile-speaker volume.
- Max sounds playful and cat-like without replacing every word with a meow.
- Speech never clips or stacks; dialogue and short chimes do not compete at excessive volume.
- Character mouth animation tracks the approximate speaking duration; precise phoneme lip-sync is a separate production task.
- A final mix needs normalization, accessible sound/music sliders, ambience, and quiet-mode checks on physical devices.
- Voice cast and any CDN licenses must be verified for commercial distribution.

## Current limitations

The WAV binaries are **not yet in GitHub**. This repository contains the verified text IDs and the Unity importer, not a self-contained shipped voice pack. A project clone will play short original procedural SFX but will not speak until the WAV files are installed locally. Do not ship with remote speech URLs; ship verified local WAVs.
