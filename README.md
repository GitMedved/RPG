# Mia & Max Adventures — Tap Adventure MVP

A portrait-first, single-player Unity 6 / URP prototype for Android and iOS. The player taps objects; Mia and Max are **not** controlled with a joystick or follow camera.

## Current status

**MVP code integrated in branch `codex/mia-max-foundation`; Unity Play Mode and device builds have NOT yet been verified.** This is a small interactive game prototype, not release-quality 3D art.

The committed code creates an interactive backyard automatically at runtime from the saved, intentionally empty scene `Assets/_Project/Scenes/Backyard.unity`.

### What can be played

1. Tap the apple tree and fallen twigs to gather materials.
2. Find the feather and build the birdhouse (3 twigs + 1 feather).
3. Water the flower bed, wait approximately 60 seconds, and collect flowers.
4. Craft a keepsake at the workbench (1 twig + 1 flower), after building the birdhouse.
5. Enter the Secret Garden through the gate. The camera switches to a second fixed view.
6. Discover its golden stone, then return to the yard with **Back to yard**.
7. Play Max's three-step treasure sorting mini-game (one-time reward).
8. Close and reopen the game; progress persists in the MVP's local PlayerPrefs save.

The rebuilt uGUI HUD displays the original character portraits, story objectives, inventory, a context card and the interactive mini-game. Two **animated, stylized 3D proxy characters** appear in the yard with blinking, gestures, idle motion, tail sway and limited speaking-mouth motion. These are NOT yet approved 3D likenesses of the reference images. The previously visible greybox is hidden behind custom-generated rounded/organic meshes for a cottage, roof, window frames, fence, apple tree, workbench, flowerbed, water garden, crafting props, secret-garden gate and clearing. The greybox colliders remain for reliable interaction.

Basic interaction feedback is procedurally synthesized. Four original English spoken dialogue WAVs were generated with HeyGen; they are not tracked as binary assets yet. To install locally from the editor run **Mia & Max → MVP → 4. Install Generated English Voice Takes** while outside Play Mode. This operation requires internet access and will download the clips to `Assets/Resources/MiaMax/Audio/Voice/`. The gameplay remains offline afterward. Commit those WAVs before building/releasing from another machine. Generated source URLs may expire.

## Run it on Mac

1. Pull this exact branch:
   ```bash
   cd ~/Desktop/RPG
   git switch codex/mia-max-foundation
   git pull origin codex/mia-max-foundation
   ```
   If your repository is in another folder, substitute that folder for `~/Desktop/RPG`.
2. In Unity Hub, open the repository root using Unity Editor **6000.3.0f1**. Wait for packages and script compilation.
3. Open `Assets/_Project/Scenes/Backyard.unity`.
4. Run **Mia & Max → MVP → 1. Preflight Assets & Scene**; inspect Console.
5. If you want dialogue, use **Mia & Max → MVP → 4. Install Generated English Voice Takes** once outside Play Mode (internet required). This is optional and the game works without voices.
6. Press **Play**; run **Mia & Max → MVP → 2. Inspect Running World** to confirm the camera, animated characters, Canvas HUD and tap targets.
7. Tap the target objects with the mouse (mobile builds use touch).

To start fresh: **Mia & Max → MVP → 3. Reset Local Progress**. This deletes only the standalone prototype's local PlayerPrefs progress; restart Play Mode afterwards. Do not use it if you want to keep the current local MVP save.

If your local Unity workspace already contains imported Kenney models, keep a backup before any clean reimport; the assets are not included in this Git branch.

## The kept architecture

- `Assets/_Project/Scripts/Core/TapAdventureWorldBootstrap.cs`: complete active MVP world and tap interaction loop
- `Assets/_Project/Scripts/Infrastructure/Audio/GameAudioService.cs`: audio cues with generated fallbacks
- `Assets/_Project/Scripts/Infrastructure/Mobile/MobileRuntimeBootstrap.cs`: mobile lifecycle defaults
- `Assets/_Project/Scripts/Editor/TapAdventureMvpAudit.cs`: in-Editor preflight and running-world checks
- `Assets/_Project/Scripts/Editor/TapAdventurePrototypeTools.cs`: local progress reset
- `Assets/Resources/MiaMax/Materials/`: 40 URP materials
- `Assets/Resources/MiaMax/Art/MiaPortrait.jpg` and `MaxPortrait.png`: original reference portraits
- `Assets/_Project/Art/Characters/`: original character source references

The active renderer is `TapAdventureArtWorld.cs`, with `TapAdventureCharacterRig.cs` and `TapAdventureHud.cs`. These are authored C# procedural art, not model files exported from Blender. The original greybox renderer is disabled if the art pass installs successfully; its tap colliders stay in place. If art creation fails, the code re-enables the fallback world.

The previous moving-character world has **no automatic startup**. The legacy domain, mini-game and content-tool scripts remain available for selective later reuse, but do **not** run as the current MVP. The legacy `GameSessionBootstrap` no longer creates a second save session automatically. This MVP intentionally uses one simple local `PlayerPrefs` save; a versioned save migration is future work.

## Still missing for release

- Production-quality 3D environment prefabs: current buildings, props and plants are composed from temporary geometric primitives.
- Rigged and animated Mia and Max in the world; approved source images are currently visible only as portraits.
- Professionally mastered music/voice assets and visual polish.
- Automated cross-platform Unity compilation, play-through tests on devices, store signing and compliance checks.
- More content beyond the small initial story and sorting mini-game.

Do not call this a release candidate until a real Play Mode run and Android/iOS device builds pass. For visual and character requirements, see `ASSET_REQUIREMENTS.md`, `VISUAL_REFERENCE_SPECIFICATION.md` and `GAME_DESIGN_DOCUMENT.md`.

Historical milestone documents were removed from the working tree during MVP cleanup; they remain recoverable through the Git history.
