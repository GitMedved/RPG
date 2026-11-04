# Compile fixes — Unity built-in modules and save model types

## Changes
- Added the built-in Unity package `com.unity.modules.animation` to `Packages/manifest.json`, which supplies `UnityEngine.Animator`.
- Added an explicit `MiaMax.Infrastructure.Save` import to `GameSettingsService.cs`.
- Moved `PlayerSettingsData` and `AchievementProgressData` into their own public serializable source files in the save namespace, with Unity meta files. `PlayerSaveData` continues to reference these types.
- No save field names or JSON field layout were changed by the type split.

## Editor verification
After pulling the feature branch, allow Unity Package Manager to resolve the manifest and recompile. Then confirm that the three reported CS1069/CS0246 errors are gone. Runtime interaction log messages such as “Mia interacts with Garden plot” are normal debug output, not compile errors.
