# M57 — Compact responsive detail-panel controls

## Implemented
- The shared detail panel now calculates a dedicated bottom action zone from its actual available height.
- The scroll viewport is resized to the remaining space instead of retaining fixed 300 px bottom and 118 px top offsets.
- Settings rows, +/- controls and accessibility toggles scale their row height for shorter displays.
- The 11 Adventure Journal actions retain their two-column layout and dynamically compress row height and spacing.
- Portrait layouts preserve the original 38 px control rows; compact landscape layouts reduce them toward 20 px.
- The panel dimensions are capped by the actual safe-area size; the layout no longer forces a minimum panel width or height that could exceed a very short viewport.
- Added the Unity `.meta` asset for the new Editor audit script.
- The UI Migration Audit now checks for compact controls and dynamic viewport sizing.

## Validation required in Unity
1. Run Tools → Mia & Max → UI Migration Audit.
2. Open Settings and Adventure Journal at portrait and landscape aspect ratios.
3. Verify all setting +/- controls and both accessibility toggles are tappable.
4. Verify all 11 journal actions are visible and tappable.
5. Scroll the detail text in Backpack and World Map; ensure the scroll viewport remains usable in landscape.
6. Test safe-area insets and touch behavior on a physical device.

This is a source-level layout improvement. Unity compilation, runtime visual review and device testing remain pending.
