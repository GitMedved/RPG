# M38 — Tutorial Flow Repair

## Changes

- Connected successful planting to tutorial step 2/6.
- Connected successful harvesting to tutorial step 3/6.
- Failed planting/harvesting attempts do not advance the guide.
- Tutorial hints now include the current step number (for example, STEP 3/6).
- Added a deterministic Editor smoke check that plays through the six tutorial stages: movement, planting, harvesting, crafting, order delivery and an arcade reward claim.
- The Editor smoke command now reports six grouped checks instead of five.

## Validation

The tutorial smoke scenario uses an isolated in-memory player session and deterministic timestamps. It does not access the user's actual save file or require a scene to be open.

Run **Tools → Mia & Max → Run Core Smoke Checks** in Unity Editor. Expected result: **6/6 checks passed**.

Unity compilation, Play Mode, touch controls, UI layout and Android/iOS device behavior still require local validation. No Unity Editor execution is claimed by this repository update.
