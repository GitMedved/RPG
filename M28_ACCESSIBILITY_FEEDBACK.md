# M28 — Accessibility Feedback Integration

## Changes
- The existing vibration preference now affects world interaction feedback: the HUD calls the settings service when an interaction toast is received; vibration remains restricted to mobile platforms and only runs when enabled.
- The existing reduced-motion preference now suppresses the Animator `Interact` and `Celebrate` one-shot triggers in `CharacterAnimationBridge`.
- Default settings preserve the previous behaviour. No save schema changes were made.

## Acceptance checks in Unity
1. On a mobile device, enable vibration and interact with an object; confirm one short haptic response.
2. Disable vibration and repeat; confirm no haptic response.
3. In the Editor, confirm interaction feedback still appears even though Editor vibration is not expected.
4. Assign a test Animator Controller with Interact/Celebrate triggers, turn Reduced Motion off and confirm triggers fire.
5. Turn Reduced Motion on and confirm those two one-shot triggers are suppressed.
6. Confirm normal locomotion parameter updates continue in both modes.

## Scope note
Reduced Motion currently suppresses only the bridge's one-shot interaction/celebration triggers. It does not yet modify locomotion animation, camera motion, mini-game effects or UI transitions. Haptic patterns are not customizable; the platform's basic vibration API is used.

## Validation status
Source-level review only. Unity compilation, Animator behaviour and physical-device vibration must be verified locally.
