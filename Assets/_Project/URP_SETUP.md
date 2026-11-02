# URP and mobile rendering setup

## Compatibility baseline
- Unity Editor baseline in ProjectSettings: 6000.3.0f1.
- Universal Render Pipeline package: 17.3.0. Unity's Graphics repository changelog lists 17.3.0 as compatible with Unity 6000.3.0b1. Confirm Package Manager resolves the package cleanly in the exact installed 6000.3 LTS patch before committing any generated pipeline assets.

## Install
1. Open the project in Unity 6000.3.x.
2. Open Window → Package Management → Package Manager.
3. Add/resolve Universal RP 17.3.0 (manifest pins this package).
4. In Assets → Create → Rendering → URP Asset (with Universal Renderer), create a pipeline asset and renderer data.
5. Assign the pipeline asset in Project Settings → Graphics → Default Render Pipeline and Project Settings → Quality for every quality tier.
6. Convert/replace prototype materials with URP/Lit and confirm the scene is not pink.
7. Keep MSAA at 2x or 4x for mobile after profiling; start with render scale 1.0, HDR off, opaque texture off, depth texture off unless a feature needs them.
8. Use one main directional light; keep realtime shadow distance and cascades conservative. Prefer baked lighting for static scenery.

## Mobile validation
- Test Android Vulkan and OpenGLES3 fallback on at least one physical device.
- Test iOS Metal on a physical device.
- Profile CPU, GPU, memory, thermal behaviour and frame pacing. Do not claim platform support based only on Editor Play Mode.

The repository does not include generated URP asset YAML yet because it must be authored by the exact Unity Editor/package version. This avoids fragile hand-written ScriptableObject serialization.
