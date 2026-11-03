# M11 — Android and iOS build readiness

This checklist prepares the Unity project for real platform builds. It does not claim that either platform has been built or tested.

## Shared project checks
- Confirm the exact Unity 6000.3 LTS Editor patch installed in Unity Hub.
- Open the project and resolve every package in Package Manager.
- Create and assign the URP Pipeline Asset and Renderer Data in the Editor; see `Assets/_Project/URP_SETUP.md`.
- Confirm the Backyard scene is in Build Settings and the scene opens from a clean project import.
- Resolve all compiler errors and investigate every recurring runtime warning.
- Test safe areas, touch targets, pause/resume, application focus loss and local save recovery.
- Confirm orientation choice and camera framing on target devices before locking Player Settings.
- Add an app icon, splash screen, package/bundle identifiers and version numbers only after the product identity is final.
- Keep secrets, signing certificates and provisioning profiles out of Git.

## Android
1. Install Android Build Support, Android SDK & NDK Tools and OpenJDK through Unity Hub for the selected Editor.
2. Set a unique application identifier and version.
3. Configure minimum/target API levels to current store requirements at release time.
4. Start with ARM64; add other architectures only when required.
5. Test Vulkan with OpenGLES3 fallback on physical devices.
6. Build an APK for local smoke tests, then an Android App Bundle for Play Console.
7. Verify install, first launch, background/resume, save persistence, touch input, safe areas and thermal/performance behaviour.

## iOS
1. Install iOS Build Support through Unity Hub.
2. Use a Mac with a supported Xcode version.
3. Set a unique bundle identifier, signing team and provisioning profile in Xcode.
4. Build the Xcode project from Unity and resolve signing settings in Xcode.
5. Test on a physical iPhone/iPad; Simulator alone is insufficient for performance and touch validation.
6. Verify install, first launch, background/resume, save persistence, safe areas and Metal rendering.

## Release gates
- No build is called ready until a clean build succeeds.
- No device support is claimed until the app has been installed and smoke-tested on real hardware.
- Store submission requires privacy, child-safety, age-rating, accessibility and purchase-policy review.
- Ads, analytics, accounts, cloud sync and real-money billing are not part of this prototype package.
