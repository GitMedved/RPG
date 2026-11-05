# M56 — Responsive Canvas detail panels

## Implemented
- The shared detail sheet now clamps its width and height to the active safe-area viewport.
- Portrait layouts retain the existing 620×1040 design size when space permits.
- Landscape and shorter viewports shrink the panel rather than extending beyond the visible safe area.
- Existing panel contents, buttons, scroll behavior and gameplay callbacks are unchanged.
- Extended the UI Migration Audit to check for the responsive panel sizing path.

## Validation required in Unity
1. Run Tools → Mia & Max → UI Migration Audit.
2. Open Backpack, World Map, Settings and Adventure Journal in portrait and landscape.
3. Check a small phone-sized Game view and a tablet-sized Game view.
4. Verify close button, scroll viewport and bottom action controls remain reachable inside notched safe areas.

The change is source-level only; Unity compilation and visual/device validation remain pending.
