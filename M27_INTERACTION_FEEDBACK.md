# M27 — Interaction Feedback Toasts

## Goal
Make world interactions feel acknowledged without requiring the Adventure Journal to be open.

## Changes
- `BackyardInteractable` publishes a lightweight static interaction-feedback event after an interaction is triggered.
- `PrototypeHud` subscribes while enabled and displays a compact, centered toast for 2.6 seconds using unscaled time.
- The toast is independent of the journal, backpack and settings panels and does not alter gameplay state or save data.
- Existing interaction events, interaction audio cue and gameplay callbacks remain in place.

## Acceptance checks in Unity
1. Enter Play Mode and approach an interactable object.
2. Confirm the contextual Interact button appears as before.
3. Interact using the HUD button and confirm a short message appears near the top center.
4. Repeat using keyboard E/Space and confirm the same feedback appears.
5. Confirm the toast disappears after roughly 2.6 seconds, including while the game is paused (unscaled time).
6. Open/close the Journal and Backpack and confirm the toast still renders without blocking the action dock.

## Limitations
- The toast is a prototype IMGUI presentation, not the final branded notification component.
- It reports the interactable's configured name and action label; it does not claim that every interaction produced a reward.
- No save schema change, analytics, network dependency or new asset requirement.

## Validation status
Source changes and structural checks only. Unity Editor compilation, Play Mode and mobile aspect-ratio/device checks must be performed locally.
