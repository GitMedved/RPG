# M59 — Mobile UI contrast and readable button labels

## Changes

- Added WCAG-style relative-luminance contrast calculation to the shared mobile UI theme.
- Button labels now select the higher-contrast option between the warm cream and dark ink text colors based on their actual button background.
- Darkened the leaf-green action color so cream labels remain readable.
- Extended the UI Migration Audit to check the 4.5:1 contrast threshold for the core forest, leaf and honey button colors.

## Validation boundary

The contrast calculation is deterministic source logic; the audit must still be run in the local Unity Editor. This does not replace a full accessibility review, dynamic text scaling review, color-blindness review or physical-device testing.
