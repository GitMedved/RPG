# M21 — Player settings & accessibility foundation

## Added
- Added persistent player preferences to the existing local save: master, effects, music and voice volume; UI scale; vibration preference; reduced-motion preference.
- Added load-time sanitization and safe defaults for existing saves. The save schema remains v1 because the new settings object is additive and optional.
- Added a compact Settings entry in the prototype HUD. Volume and UI scale use explicit +/- controls to avoid writing the save every frame while dragging sliders.
- Added independent audio mix control to `GameAudioService` and apply saved master volume through Unity's `AudioListener`.
- Added UI scaling range of 85–115% and a vibration helper guarded by the saved preference and mobile platform check.

## Current limits
- This is still the IMGUI prototype panel, not final Canvas UI.
- Vibration preference and reduced-motion preference are stored; they are not yet wired to a full haptics event map or a camera/animation system. The current prototype has no camera-shake system to reduce.
- Language remains English-first; localization switching is not exposed until translated strings exist.
- Audio remains silent until licensed/approved clips are imported and assigned.

## Unity acceptance checklist
1. Open the feature branch in the project's pinned Unity Editor and resolve all imports.
2. Confirm no compiler errors; open Settings from the HUD.
3. Change each volume channel and confirm master/effects/music/voice behave independently after clips are assigned.
4. Change UI scale and confirm the HUD responds without clipping on portrait and landscape aspect ratios.
5. Toggle vibration and reduced motion, close/reopen the app, and confirm values persist.
6. Confirm older saves load with default settings and no progress/inventory loss.

No Unity compile, Play Mode or physical-device test has been run remotely.
