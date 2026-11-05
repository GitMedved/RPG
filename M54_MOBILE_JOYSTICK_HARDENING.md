# M54 — Mobile joystick safe-area and lifecycle hardening

## Changes
- The touch target and visual joystick now use the device safe area instead of a fixed bottom-left pixel origin.
- Joystick radius scales conservatively with screen width and is clamped to a readable range.
- Movement normalization uses the effective on-screen radius so sensitivity stays consistent across resolutions.
- Input resets on disable, app focus loss and pause, preventing a stuck movement vector after interruptions.
- GUI colour is restored to its previous value after drawing.

## Validation required
1. Open the generated backyard scene and verify the joystick is visible at the lower-left inside the safe area.
2. Test touch movement in four directions and diagonals on at least a narrow phone and a notched phone.
3. Start a second touch on the HUD while holding the joystick; confirm the joystick retains only its original finger.
4. Background the app while moving, resume, and confirm Mia stops until a new touch begins.
5. Verify the HUD buttons remain reachable and do not overlap the joystick.

This is still a lightweight prototype visual using IMGUI and raw touch input, not the final skinned Canvas control. Unity compilation, touch hardware validation and device profiling remain pending.
