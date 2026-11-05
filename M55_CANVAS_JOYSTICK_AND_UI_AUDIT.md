# M55 — Canvas joystick and UI migration audit

## Implemented
- Replaced the mobile joystick's IMGUI drawing with a dedicated Screen Space Overlay Canvas.
- Kept touch input ownership, dead zone, normalized movement vector and safe-area hit region unchanged.
- The Canvas visuals use non-raycastable Images, so they do not intercept HUD button input.
- The joystick Canvas sorts below the main mobile HUD and scales with the existing 1080×1920 reference.
- The joystick recalculates its anchor and dimensions when resolution, orientation or safe area changes.
- Added an Editor audit at Tools → Mia & Max → UI Migration Audit to list remaining runtime OnGUI entry points.

## Remaining prototype UI
- PrototypeHud retains its IMGUI implementation only as a fallback when MobileHudCanvas is absent.
- The main scene HUD uses MobileHudCanvas; the joystick now also uses Canvas.
- Final art, typography, iconography, animation and device-specific layout tuning remain production tasks.

## Validation required in Unity
1. Pull the feature branch and allow scripts to compile.
2. Run Tools → Mia & Max → UI Migration Audit.
3. Enter Play Mode and verify joystick response, HUD button clicks and panel scrolling.
4. Test portrait and landscape orientations, notched safe areas and at least one physical touch device.

No Unity compile, Play Mode or physical-device validation is claimed by this source change.