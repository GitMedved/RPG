# M33 — Responsive Arcade HUD

## Changes
- Wrapped the existing arcade IMGUI panel in a safe-area-aware GUI transform.
- Arcade panel scale now derives from the safe-area width and height, clamped between 0.72 and 1.2.
- The transform offsets the panel from device safe-area insets, improving notch/home-indicator handling.
- Existing mini-game rules, controls, reward logic and panel coordinates remain unchanged in logical UI space.

## Acceptance checks in Unity
1. Open the Arcade menu in portrait on a narrow phone-sized Game view.
2. Confirm the menu, daily reward count and all three game buttons remain visible and tappable.
3. Open Match-3 and confirm the full 8×8 board fits inside the panel.
4. Open Fishing and Max Runner; confirm sliders and buttons remain inside the safe area.
5. Switch to landscape and confirm controls remain visible and appropriately scaled.
6. Test a tablet/large Game view and confirm the scale does not exceed the intended maximum.
7. Confirm touch/mouse interaction still maps correctly after the GUI transform.

## Limitations
- This remains IMGUI prototype UI with fixed logical coordinates; it is not the final responsive Canvas UI.
- Small-screen touch target sizes and visual polish still need device validation.
- The HUD transform does not change gameplay camera or world scaling.

## Validation status
Source-level review only. Unity compile, Game view layout and physical-device touch tests remain pending.
