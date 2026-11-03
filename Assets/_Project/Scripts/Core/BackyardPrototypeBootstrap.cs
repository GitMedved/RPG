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
            if (FindFirstObjectByType<BackyardPrototypeBootstrap>() != null) return;
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
            Cube("Patio_Blockout", new Vector3(0f, 0.02f, 6.2f), new Vector3(9f, 0.12f, 2.7f), white);

            // Three starter garden plots, placed in the open lawn beside the sandbox.
            for (int i = 0; i < 3; i++)
            {
                Vector3 plotPosition = new Vector3(5f + i * 2.1f, 0.12f, 1.5f);
                GameObject plot = Cube("Farm_Plot_" + (i + 1) + "_Placeholder", plotPosition,
                    new Vector3(1.7f, 0.22f, 1.45f), wood);
                BackyardInteractable plotInteractable = plot.AddComponent<BackyardInteractable>();
                plotInteractable.Configure("Garden plot", "Plant a seed");
                plot.AddComponent<FarmPlotPrototype>().Configure("plot_" + (i + 1), plotInteractable);
            }
            GameObject workshop = Cube("Workshop_Station_Placeholder", new Vector3(2.8f, 0.55f, 4.6f),
                new Vector3(1.8f, 1.1f, 1.1f), wood);
            BackyardInteractable workshopInteractable = workshop.AddComponent<BackyardInteractable>();
            workshopInteractable.Configure("Workshop", "Start a recipe");
            workshop.AddComponent<ProductionStationPrototype>().Configure(workshopInteractable);

            GameObject orderBoard = Cube("Order_Board_Placeholder", new Vector3(-3.2f, 1.05f, 4.6f),
                new Vector3(1.1f, 2.1f, 0.25f), wood);
            BackyardInteractable boardInteractable = orderBoard.AddComponent<BackyardInteractable>();
            boardInteractable.Configure("Order board", "Check orders");
            orderBoard.AddComponent<OrderBoardPrototype>().Configure(boardInteractable);

            GameObject questBoard = Cube("Adventure_Journal_Board_Placeholder", new Vector3(-5.2f, 1.05f, 4.6f),
                new Vector3(0.9f, 1.8f, 0.22f), wood);
            BackyardInteractable questInteractable = questBoard.AddComponent<BackyardInteractable>();
            questInteractable.Configure("Adventure journal", "Check story quests");
            questBoard.AddComponent<QuestBoardPrototype>().Configure(questInteractable);

            GameObject arcadeBoard = Cube("Mini_Game_Arcade_Board_Placeholder", new Vector3(0f, 1.1f, 4.6f),
                new Vector3(1.1f, 2.2f, 0.25f), blue);
            BackyardInteractable arcadeInteractable = arcadeBoard.AddComponent<BackyardInteractable>();
            arcadeInteractable.Configure("Mia & Max Arcade", "Play a mini-game");
            arcadeBoard.AddComponent<ArcadeBoardPrototype>().Configure(arcadeInteractable);

            GameObject collectionBoard = Cube("Collection_Book_Board_Placeholder", new Vector3(-7.5f, 1.05f, 7.1f),
                new Vector3(1.0f, 1.9f, 0.22f), wood);
            BackyardInteractable collectionInteractable = collectionBoard.AddComponent<BackyardInteractable>();
            collectionInteractable.Configure("Backyard collection", "Check keepsakes");
            collectionBoard.AddComponent<CollectionBookPrototype>().Configure(collectionInteractable);

            CreateKeepsake("Red Ball", "keepsake_red_ball", new Vector3(9f, 0.35f, -3f), Mat(new Color(0.88f, 0.12f, 0.12f)));
            CreateKeepsake("Blue Feather", "keepsake_blue_feather", new Vector3(-10f, 0.45f, 5f), Mat(new Color(0.18f, 0.48f, 0.92f)));
            CreateKeepsake("Old Button", "keepsake_old_button", new Vector3(4f, 0.28f, -8f), Mat(new Color(0.72f, 0.62f, 0.35f)));
            CreateKeepsake("Yellow Leaf", "keepsake_yellow_leaf", new Vector3(-2f, 0.35f, -9f), Mat(new Color(0.95f, 0.72f, 0.12f)));
            CreateKeepsake("Tiny Shell", "keepsake_tiny_shell", new Vector3(12f, 0.3f, 3f), Mat(new Color(0.9f, 0.78f, 0.68f)));

            BackyardDetailPrototype.Build();

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
            BuildFlowerGardenAnnex(mia.transform);

            GameObject joystick = new GameObject("Prototype Mobile Joystick");
            joystick.AddComponent<PrototypeMobileJoystick>();
            new GameObject("Prototype HUD").AddComponent<PrototypeHud>();
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
            sun.intensity = 1.2f;
            sun.shadows = LightShadows.Soft;
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.68f, 0.72f, 0.78f);
            sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // A small red toy truck recalls the reference board's sandbox detail.
            Cube("Toy_Truck_Placeholder", new Vector3(7f, 0.35f, -4f), new Vector3(1.1f, 0.45f, 0.7f), Mat(new Color(0.85f, 0.12f, 0.08f)));
            Debug.Log("Mia & Max Backyard blockout created. All primitives are temporary placeholders.");
        }


        private static void BuildFlowerGardenAnnex(Transform player)
        {
            Material path = Mat(new Color(0.78f, 0.69f, 0.48f));
            Material fence = Mat(new Color(0.91f, 0.89f, 0.82f));
            Material petalPink = Mat(new Color(0.96f, 0.42f, 0.63f));
            Material petalYellow = Mat(new Color(0.98f, 0.78f, 0.18f));
            Material petalPurple = Mat(new Color(0.58f, 0.39f, 0.84f));
            Material leaf = Mat(new Color(0.17f, 0.43f, 0.16f));
            Material wood = Mat(new Color(0.48f, 0.29f, 0.14f));

            GameObject entrance = Cube("Flower_Garden_Entrance_Gate", new Vector3(14.1f, 1.0f, 9.8f),
                new Vector3(0.25f, 2.0f, 2.3f), wood);
            BackyardInteractable entranceInteractable = entrance.AddComponent<BackyardInteractable>();
            entranceInteractable.Configure("Flower Garden Gate", "Enter the flower garden");
            FlowerGardenPortalPrototype entrancePortal = entrance.AddComponent<FlowerGardenPortalPrototype>();
            entrancePortal.Configure(player, new Vector3(10.5f, 0.15f, 9.5f), false, entranceInteractable);

            GameObject zoneRoot = new GameObject("Flower_Garden_Annex");
            zoneRoot.transform.position = Vector3.zero;
            FlowerGardenZonePrototype zone = zoneRoot.AddComponent<FlowerGardenZonePrototype>();
            GameObject content = new GameObject("Unlocked_Garden_Content");
            content.transform.SetParent(zoneRoot.transform, false);
            zone.Configure(content);

            Cube("Flower_Garden_Path", new Vector3(11.5f, 0.03f, 9.5f), new Vector3(5.0f, 0.08f, 1.4f), path, content.transform);
            for (int x = 9; x <= 14; x += 1)
            {
                Cube("Flower_Garden_Back_Fence", new Vector3(x, 0.7f, 12.2f), new Vector3(0.12f, 1.4f, 0.12f), fence, content.transform);
                Cube("Flower_Garden_Front_Fence", new Vector3(x, 0.7f, 7.0f), new Vector3(0.12f, 1.4f, 0.12f), fence, content.transform);
            }
            for (int z = 7; z <= 12; z += 1)
            {
                Cube("Flower_Garden_Left_Fence", new Vector3(8.6f, 0.7f, z), new Vector3(0.12f, 1.4f, 0.12f), fence, content.transform);
                Cube("Flower_Garden_Right_Fence", new Vector3(14.4f, 0.7f, z), new Vector3(0.12f, 1.4f, 0.12f), fence, content.transform);
            }

            CreateGardenResource(content.transform, player, "garden_flower_pink", "flower_pink", "Pink flower",
                new Vector3(9.5f, 0.45f, 10.8f), petalPink, leaf);
            CreateGardenResource(content.transform, player, "garden_flower_yellow", "flower_yellow", "Yellow flower",
                new Vector3(12.8f, 0.45f, 10.8f), petalYellow, leaf);
            CreateGardenResource(content.transform, player, "garden_flower_purple", "flower_purple", "Purple flower",
                new Vector3(12.8f, 0.45f, 8.0f), petalPurple, leaf);

            GameObject exit = Cube("Flower_Garden_Return_Gate", new Vector3(9.2f, 0.9f, 8.0f),
                new Vector3(0.2f, 1.8f, 1.2f), wood, content.transform);
            BackyardInteractable exitInteractable = exit.AddComponent<BackyardInteractable>();
            exitInteractable.Configure("Backyard Gate", "Return to the backyard");
            FlowerGardenPortalPrototype exitPortal = exit.AddComponent<FlowerGardenPortalPrototype>();
            exitPortal.Configure(player, new Vector3(13.0f, 0.15f, 10.8f), true, exitInteractable);
        }

        private static void CreateGardenResource(Transform parent, Transform player, string id, string itemId,
            string title, Vector3 position, Material flowerMaterial, Material leafMaterial)
        {
            GameObject root = new GameObject("Garden_Resource_" + id);
            root.transform.SetParent(parent, false);
            root.transform.position = position;
            Sphere("Flower_Head", position + Vector3.up * 0.25f, new Vector3(0.5f, 0.25f, 0.5f), flowerMaterial, root.transform);
            Cylinder("Flower_Stem", position + Vector3.down * 0.12f, new Vector3(0.08f, 0.35f, 0.08f), leafMaterial).transform.SetParent(root.transform, true);
            BackyardInteractable interactable = root.AddComponent<BackyardInteractable>();
            interactable.Configure(title, "Collect flower");
            root.AddComponent<FlowerGardenResourcePrototype>().Configure(id, itemId, title, interactable);
        }

        private static void CreateKeepsake(string title, string id, Vector3 position, Material material)
        {
            GameObject keepsake = Sphere("Keepsake_" + id, position, Vector3.one * 0.48f, material);
            BackyardInteractable interactable = keepsake.AddComponent<BackyardInteractable>();
            interactable.Configure(title, "Pick up keepsake");
            keepsake.AddComponent<BackyardCollectiblePrototype>().Configure(id, title, interactable);
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
