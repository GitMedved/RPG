# M34 — Canvas-based Mobile HUD Foundation

## Goal

Begin replacing the temporary IMGUI HUD with a responsive Unity uGUI Canvas while preserving the existing gameplay services, local save data and player interaction flow.

## Implemented

- Added the Unity UI package dependency `com.unity.ugui` 2.0.0.
- Added `MobileHudCanvas`, created at runtime beside `PrototypeHud`.
- Uses a Screen Space Overlay Canvas and CanvasScaler with a 1080×1920 reference resolution.
- Applies normalized `Screen.safeArea` anchors so the HUD avoids notches and device system bars.
- Adds a player status card (level, XP, coins and visit streak), daily gift action, Journal/Settings/World Map shortcuts, Backpack and contextual Interact dock.
- Adds a live interaction prompt and tutorial hint.
- Creates an EventSystem with StandaloneInputModule only when a scene EventSystem does not already exist.
- Keeps existing domain service calls and the mutually-exclusive panel state in `PrototypeHud`.
- The old IMGUI drawing of the top status card and action dock is bypassed when the Canvas HUD is present.

## Deliberate migration boundary

This is the first UI migration slice, not the final UI replacement:
- Detailed Backpack, Journal, World Map and Settings panel contents still use the existing IMGUI renderers.
- The Arcade UI remains on its separate responsive IMGUI implementation.
- The Canvas is assembled at runtime; no authored prefab, final icon sprites, localization tables or final typography assets have been introduced.
- The current visual treatment uses simple color panels and built-in Arial. It is a functional style foundation, not final Mia & Max art direction.

The next UI pass should migrate the four detail panels into scrollable Canvas panels, then replace temporary shapes/text with approved UI sprites and final typography.

## Unity acceptance checklist

1. Open the project and allow Package Manager to resolve `com.unity.ugui` 2.0.0.
2. Confirm there are no red compile errors.
3. Enter Play Mode and confirm the Canvas HUD appears once (no duplicate IMGUI top bar/action dock).
4. Test Daily Gift, Journal, Settings, World Map and Backpack buttons; existing panel actions must still work.
5. Walk near an interactable and verify the prompt changes and Interact button becomes enabled; tap it.
6. Verify the tutorial hint appears until the tutorial is completed.
7. Check portrait and landscape Game views and confirm all controls remain inside the safe area.
8. If the project uses the new Input System only, verify the EventSystem input module configuration; the current StandaloneInputModule expects the legacy Input Manager to remain enabled.

## Not validated here

No Unity Editor compilation, Play Mode, touch-device, Android or iOS build was run in this environment. Source-level review and brace-balance checks are not substitutes for Editor validation.
