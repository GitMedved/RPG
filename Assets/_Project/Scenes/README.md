# Backyard — Active MVP Scene

`Backyard.unity` is intentionally a minimal serialized scene. The active game world is built at runtime by `MiaMax.Gameplay.TapAdventureWorldBootstrap` via `RuntimeInitializeOnLoadMethod`.

The gameplay contract is fixed-camera, tap-only; there is no joystick or movement controller in this scene.

To test:

1. Open `Backyard.unity` in Unity Editor 6000.3.0f1.
2. Run **Mia & Max → MVP → 1. Preflight Assets & Scene**.
3. Press Play, then run **Mia & Max → MVP → 2. Inspect Running World**.
4. Complete the tree → feather → birdhouse → flower bed → workbench → gate → golden stone path.

`BackyardPrototypeBootstrap` is a no-op compatibility component for old local Unity scenes; it does not initialize automatically. Do not reactivate its old free-roam blockout.
