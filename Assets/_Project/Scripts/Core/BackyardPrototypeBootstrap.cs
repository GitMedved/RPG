using UnityEngine;
using UnityEngine.Rendering;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// Creates a playable, explicitly temporary backyard blockout when the project starts.
    /// No final character or environment art is fabricated here.
    /// </summary>
    public sealed class BackyardPrototypeBootstrap : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreatePrototype()
        {
            if (FindObjectOfType<BackyardPrototypeBootstrap>() != null) return;
            new GameObject("Backyard Prototype Bootstrap").AddComponent<BackyardPrototypeBootstrap>();
        }

        private void Start()
        {
            BuildBackyard();
        }

        private static void BuildBackyard()
        {
            Material grass = Mat(new Color(0.22f, 0.48f, 0.19f));
            Material beige = Mat(new Color(0.78f, 0.68f, 0.49f));
            Material roof = Mat(new Color(0.22f, 0.20f, 0.18f));
            Material white = Mat(new Color(0.91f, 0.89f, 0.82f));
            Material wood = Mat(new Color(0.48f, 0.29f, 0.14f));
            Material leaf = Mat(new Color(0.16f, 0.39f, 0.12f));
            Material orange = Mat(new Color(0.94f, 0.36f, 0.07f));
            Material blue = Mat(new Color(0.15f, 0.38f, 0.72f));

            Cube("Lawn_Blockout", new Vector3(0f, -0.25f, 0f), new Vector3(34f, 0.5f, 28f), grass);
            Cube("House_Main_Blockout", new Vector3(0f, 2.5f, 10f), new Vector3(12f, 5f, 5f), beige);
            Cube("House_Roof_Blockout", new Vector3(0f, 5.35f, 10f), new Vector3(13f, 0.55f, 5.8f), roof);
            Cube("Patio_Blockout", new Vector3(0f, 0.02f, 6.2f), new Vector3(9f, 0.12f, 2.7f), white);\n            BackyardDetailPrototype.Build();

            // White perimeter fence, leaving a gate opening at the far side.
            for (int x = -16; x <= 16; x += 2)
            {
                Cube("Fence_Back_Picket", new Vector3(x, 0.85f, 13.5f), new Vector3(0.13f, 1.7f, 0.15f), white);
                Cube("Fence_Front_Picket", new Vector3(x, 0.85f, -13.5f), new Vector3(0.13f, 1.7f, 0.15f), white);
            }
            Cube("Fence_Back_Rail_Upper", new Vector3(0f, 1.45f, 13.5f), new Vector3(34f, 0.12f, 0.18f), white);
            Cube("Fence_Back_Rail_Lower", new Vector3(0f, 0.55f, 13.5f), new Vector3(34f, 0.12f, 0.18f), white);
            for (int z = -12; z <= 12; z += 2)
            {
                Cube("Fence_Left_Picket", new Vector3(-16.5f, 0.85f, z), new Vector3(0.15f, 1.7f, 0.13f), white);
                Cube("Fence_Right_Picket", new Vector3(16.5f, 0.85f, z), new Vector3(0.15f, 1.7f, 0.13f), white);
            }

            // Central tree.
            GameObject treeTrunk = Cylinder("Main_Tree_Trunk", new Vector3(-7f, 2f, 2f), new Vector3(0.65f, 2f, 0.65f), wood);
            AddInteractable(treeTrunk, "Big backyard tree", "Look around");
            Sphere("Main_Tree_Canopy", new Vector3(-7f, 5f, 2f), new Vector3(4.2f, 4.1f, 4.2f), leaf);

            // Swing set on the left side.
            Cube("Swing_Top_Beam", new Vector3(-11f, 3.5f, -4f), new Vector3(5f, 0.18f, 0.18f), wood);
            for (int x = -13; x <= -9; x += 4)
            {
                GameObject swingLeg = Cube("Swing_Leg", new Vector3(x, 1.7f, -4f), new Vector3(0.16f, 3.4f, 0.16f), wood);
                if (x == -13) AddInteractable(swingLeg, "Garden swing", "Try the swing");
                Cube("Swing_Rope", new Vector3(x, 2.3f, -4f), new Vector3(0.05f, 2.2f, 0.05f), white);
                Cube("Swing_Seat", new Vector3(x, 1.15f, -4f), new Vector3(0.9f, 0.14f, 0.45f), blue);
            }

            // Sandbox and simple flower beds.
            GameObject sandbox = Cube("Sandbox_Base", new Vector3(10f, 0.22f, -7f), new Vector3(4.5f, 0.45f, 3.4f), wood);
            AddInteractable(sandbox, "Sandbox", "Play in the sand");
            Cube("Sandbox_Sand", new Vector3(10f, 0.47f, -7f), new Vector3(4.1f, 0.08f, 3f), Mat(new Color(0.78f, 0.66f, 0.43f)));
            for (int i = 0; i < 5; i++)
                Sphere("Garden_Shrub", new Vector3(-13f + i * 1.4f, 0.55f, 11.8f), Vector3.one * 1.1f, leaf);

            // Clearly labelled primitive character stand-ins.
            GameObject mia = CreateMia(blue, beige);
            GameObject max = CreateMax(orange, white);
            mia.AddComponent<PlayerInteraction>();

            GameObject joystick = new GameObject("Prototype Mobile Joystick");
            joystick.AddComponent<PrototypeMobileJoystick>();
            GameObject cameraObject = new GameObject("Prototype Follow Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
            camera.tag = "MainCamera";
            camera.fieldOfView = 55f;
            SmoothFollowCamera follow = cameraObject.AddComponent<SmoothFollowCamera>();
            follow.SetTarget(mia.transform);
            cameraObject.transform.position = mia.transform.position + new Vector3(0f, 7.5f, -8.5f);

            Light sun = new GameObject("Prototype Sun").AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1.2f;\n            sun.shadows = LightShadows.Soft;\n            RenderSettings.ambientMode = AmbientMode.Flat;\n            RenderSettings.ambientLight = new Color(0.68f, 0.72f, 0.78f);
            sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // A small red toy truck recalls the reference board's sandbox detail.
            Cube("Toy_Truck_Placeholder", new Vector3(7f, 0.35f, -4f), new Vector3(1.1f, 0.45f, 0.7f), Mat(new Color(0.85f, 0.12f, 0.08f)));
            Debug.Log("Mia & Max Backyard blockout created. All primitives are temporary placeholders.");
        }

        private static void AddInteractable(GameObject target, string title, string action)
        {
            BackyardInteractable interactable = target.AddComponent<BackyardInteractable>();
            interactable.Configure(title, action);
        }

        private static GameObject CreateMia(Material clothes, Material skin)
        {
            GameObject root = new GameObject("Mia_Stylized_Proxy");
            root.transform.position = new Vector3(0f, 0.1f, 0f);
            CharacterController cc = root.AddComponent<CharacterController>();
            cc.height = 1.8f; cc.radius = 0.38f; cc.center = new Vector3(0f, 0.9f, 0f);
            root.AddComponent<PrototypeThirdPersonController>();
            StylizedCharacterPrototype.BuildMia(root.transform);
            return root;
        }

        private static GameObject CreateMax(Material fur, Material cream)
        {
            GameObject root = new GameObject("Max_Stylized_Proxy");
            root.transform.position = new Vector3(-1.5f, 0.1f, -1.2f);
            StylizedCharacterPrototype.BuildMax(root.transform);
            root.AddComponent<MaxCompanionFollow>().SetTarget(GameObject.Find("Mia_Stylized_Proxy").transform);
            return root;
        }

        private static Material Mat(Color color)
        {
            Shader shader = GraphicsSettings.currentRenderPipeline != null
                ? Shader.Find("Universal Render Pipeline/Lit")
                : Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Standard");
            Material material = new Material(shader) { color = color };
            return material;
        }

        private static GameObject Cube(string name, Vector3 pos, Vector3 scale, Material material, Transform parent = null)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name; go.transform.position = pos; go.transform.localScale = scale;
            if (parent != null) go.transform.SetParent(parent, true);
            go.GetComponent<Renderer>().sharedMaterial = material;
            return go;
        }

        private static GameObject Sphere(string name, Vector3 pos, Vector3 scale, Material material, Transform parent = null)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = name; go.transform.position = pos; go.transform.localScale = scale;
            if (parent != null) go.transform.SetParent(parent, true);
            go.GetComponent<Renderer>().sharedMaterial = material;
            return go;
        }

        private static GameObject Cylinder(string name, Vector3 pos, Vector3 scale, Material material)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = name; go.transform.position = pos; go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = material;
            return go;
        }
    }
}
