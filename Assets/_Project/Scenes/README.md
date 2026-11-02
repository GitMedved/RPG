# Backyard Prototype Scene

The initial prototype generates a deterministic greybox backyard at runtime from `BackyardPrototypeBootstrap`. This avoids committing a hand-authored Unity YAML scene before the exact Editor patch and rendering pipeline have been verified.

To run:
1. Open the repository root with Unity 6.3 (6000.3.x) or update `ProjectSettings/ProjectVersion.txt` to your installed compatible Unity 6 LTS patch.
2. Allow Unity to resolve the empty package manifest and generate its local project metadata.
3. Create a new empty scene and save it as `Assets/_Project/Scenes/Backyard.unity`.
4. Press Play. The runtime bootstrap creates the backyard, Mia/Max primitive stand-ins and follow camera.

Controls: WASD/arrow keys in the Editor. On a touch device, drag from the initial touch position to move. Touch UX will be replaced by a proper virtual joystick in the next iteration.

All generated primitives are placeholders, not final character or environment art.
