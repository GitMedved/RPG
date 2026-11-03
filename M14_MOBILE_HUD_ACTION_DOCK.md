# M14 — Mobile HUD & Action Dock

## Goal

Replace the scattered prototype controls with one compact, safe-area-aware mobile HUD while retaining the existing IMGUI prototype approach.

## Included

- Keeps level, XP and coins in the upper-left status card.
- Keeps Adventure Journal quick access in the upper-right safe area.
- Moves Backpack access into a compact bottom action dock.
- Adds a contextual Interact button to the bottom dock.
- The Interact button is enabled only when Mia is near an interactable object.
- Shows the nearby object's name and action above the dock.
- Reuses the existing PlayerInteraction and BackyardInteractable flow; keyboard E/Space interaction remains available.
- Moves the interaction prompt out of the separate PlayerInteraction IMGUI overlay so the HUD owns the presentation.
- Keeps Inventory and Journal mutually exclusive.
- Respects Screen.safeArea and scales the main controls for different screen sizes.

## Unity acceptance checks

1. Compile the project and confirm no new Console errors.
2. In Play Mode, verify the status card remains in the upper-left and Adventure Journal remains in the upper-right.
3. Verify the bottom dock is visible and does not cover the joystick or tutorial hint.
4. Walk near the farm plot, workshop, order board, keepsakes and flower resources. The nearby prompt should update and Interact should activate the nearest object.
5. Walk away from interactables. Interact should become disabled.
6. Open Backpack and Journal in turn; confirm they close one another.
7. Confirm E/Space interaction still works in the Editor.
8. Check a narrow portrait Game view and a landscape/resized Game view; confirm controls remain inside the safe area and remain tappable.

## Limitations

- This remains a functional IMGUI prototype, not the final designed Unity Canvas UI.
- Emoji/symbol glyph appearance depends on the platform font and must be replaced with authored icon sprites in the visual production pass.
- Touch target sizes and layout need real-device verification.
- No Unity compile, Play Mode, Android/iOS build or device test is claimed until performed in the Editor/on hardware.
