# Missing script and Mobile HUD diagnostics

## What changed

- `MobileHudCanvas` now marks itself ready only after all required Canvas references have been created.
- Its periodic `Update` refresh is disabled if construction was incomplete, preventing a setup failure from producing a stream of secondary HUD null-reference errors.
- Added the read-only editor command `Tools → Mia & Max → Audit Missing Script References`.
- The audit scans all currently loaded scenes and project prefabs, reports each affected GameObject hierarchy and the number of missing MonoBehaviour references, and does not delete anything.

## Local verification

1. Pull `codex/mia-max-foundation`.
2. Open the project and wait for script compilation/import to finish.
3. Open the same Backyard scene that produced the errors.
4. Run `Tools → Mia & Max → Audit Missing Script References`.
5. Copy the complete audit output from Console.
6. Enter Play Mode and check whether the HUD appears and whether the old `MobileHudCanvas.Refresh` exception is gone.

## Interpreting results

- If the audit lists missing scripts in the Backyard scene, those are serialized components in the local scene. The report gives the exact hierarchy path. Do not remove them blindly; identify whether each component is obsolete or whether its original script should be restored.
- If the audit reports zero missing references but Unity still prints `The referenced script (Unknown) is missing`, the warning may originate from a scene/prefab not currently loaded, a package asset, or an object created by another editor/runtime process. Open and audit the relevant scene/prefab.
- If the HUD reports that Canvas construction was incomplete, inspect the first exception immediately before that message. The later HUD refresh errors should no longer repeat.
- The audit does not inspect objects created only at runtime and does not certify Play Mode, build, or device behavior.
