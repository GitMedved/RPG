using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// First playable slice of the touch-led Mia & Max adventure.
    /// The world is static and camera-led: the player selects objects and chooses actions.
    /// No player locomotion, joystick or follow camera is created.
    /// </summary>
    public sealed class TapAdventureWorldBootstrap : MonoBehaviour
    {
        private const string SavePrefix = "MiaMax.TapAdventure.";
        private static TapAdventureWorldBootstrap instance;
        private Camera worldCamera;
        private GUIStyle titleStyle, bodyStyle, buttonStyle, mutedStyle, cardStyle;
        private Texture2D cardTexture, buttonTexture, buttonPressedTexture;
        private string selectedId = "";
        private string toast = "Welcome! Let's make a little birdhouse.";
        private int twigs, feathers, flowers, stars;
        private bool birdhouseBuilt, flowersWatered, forestUnlocked, treeGathered;
        private bool twigsGathered, featherGathered, miniGameClaimed, gardenstoneDiscovered;
        private DateTime flowersReadyUtc;
        private bool miniGameOpen;
        private int miniGameStep;
        private int miniGameScore;
        private string miniGameMessage = "Help Max sort the found treasures!";
        private Material grassMat, woodMat, plasterMat, stoneMat, leafMat, flowerMat, roofMat, waterMat;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (instance != null) return;
            GameObject root = new GameObject("MiaMax_TapAdventure_Runtime");
            instance = root.AddComponent<TapAdventureWorldBootstrap>();
        }

        public static void Build()
        {
            if (instance != null) return;
            GameObject root = new GameObject("MiaMax_TapAdventure_Runtime");
            instance = root.AddComponent<TapAdventureWorldBootstrap>();
        }

        private void Awake()
        {
            if (instance != null && instance != this) { Destroy(gameObject); return; }
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();
            BuildWorld();
        }

        private void Start()
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }

        private void LoadProgress()
        {
            twigs = PlayerPrefs.GetInt(SavePrefix + "twigs", 0);
            feathers = PlayerPrefs.GetInt(SavePrefix + "feathers", 0);
            flowers = PlayerPrefs.GetInt(SavePrefix + "flowers", 0);
            stars = PlayerPrefs.GetInt(SavePrefix + "stars", 0);
            birdhouseBuilt = PlayerPrefs.GetInt(SavePrefix + "birdhouse", 0) == 1;
            flowersWatered = PlayerPrefs.GetInt(SavePrefix + "watered", 0) == 1;
            forestUnlocked = PlayerPrefs.GetInt(SavePrefix + "forest", 0) == 1;
            treeGathered = PlayerPrefs.GetInt(SavePrefix + "treeGathered", 0) == 1;
            twigsGathered = PlayerPrefs.GetInt(SavePrefix + "twigsGathered", 0) == 1;
            featherGathered = PlayerPrefs.GetInt(SavePrefix + "featherGathered", 0) == 1;
            miniGameClaimed = PlayerPrefs.GetInt(SavePrefix + "miniGameClaimed", 0) == 1;
            gardenstoneDiscovered = PlayerPrefs.GetInt(SavePrefix + "gardenstoneDiscovered", 0) == 1;
            string savedTicks = PlayerPrefs.GetString(SavePrefix + "flowersReady", "0");
            if (long.TryParse(savedTicks, out long ticks) && ticks > 0 &&
                ticks <= DateTime.MaxValue.Ticks)
                flowersReadyUtc = new DateTime(ticks, DateTimeKind.Utc);
            else
                flowersReadyUtc = DateTime.MinValue;
        }

        private void SaveProgress()
        {
            PlayerPrefs.SetInt(SavePrefix + "twigs", twigs);
            PlayerPrefs.SetInt(SavePrefix + "feathers", feathers);
            PlayerPrefs.SetInt(SavePrefix + "flowers", flowers);
            PlayerPrefs.SetInt(SavePrefix + "stars", stars);
            PlayerPrefs.SetInt(SavePrefix + "birdhouse", birdhouseBuilt ? 1 : 0);
            PlayerPrefs.SetInt(SavePrefix + "watered", flowersWatered ? 1 : 0);
            PlayerPrefs.SetInt(SavePrefix + "forest", forestUnlocked ? 1 : 0);
            PlayerPrefs.SetInt(SavePrefix + "treeGathered", treeGathered ? 1 : 0);
            PlayerPrefs.SetInt(SavePrefix + "twigsGathered", twigsGathered ? 1 : 0);
            PlayerPrefs.SetInt(SavePrefix + "featherGathered", featherGathered ? 1 : 0);
            PlayerPrefs.SetInt(SavePrefix + "miniGameClaimed", miniGameClaimed ? 1 : 0);
            PlayerPrefs.SetInt(SavePrefix + "gardenstoneDiscovered", gardenstoneDiscovered ? 1 : 0);
            PlayerPrefs.SetString(SavePrefix + "flowersReady", flowersReadyUtc == DateTime.MinValue ? "0" : flowersReadyUtc.Ticks.ToString());
            PlayerPrefs.Save();
        }

        private readonly Vector2[] hotspotPositions =
        {
            new Vector2(0.08f, 0.35f), // tree trunk
            new Vector2(0.16f, 0.29f), // birdhouse
            new Vector2(0.50f, 0.37f), // gate in the fence
            new Vector2(0.84f, 0.53f), // workshop
            new Vector2(0.80f, 0.63f), // flowers
            new Vector2(0.17f, 0.65f), // pond edge above bottom panel
            new Vector2(0.18f, 0.49f), // twigs at tree roots
            new Vector2(0.48f, 0.60f), // feather discovery
            new Vector2(0.53f, 0.57f)  // Max mini-game
        };
        private readonly string[] hotspotIds = { "tree", "birdhouse", "gate", "workshop", "flowers", "pond", "twigs", "feather", "minigame" };

        private void BuildWorld()
        {
            CreateMaterials();
            BuildGroundAndPaths();
            BuildHouse();
            BuildTreeAndBirdhouse();
            BuildGarden();
            BuildWorkshop();
            BuildPondAndDecor();
            BuildAdventureGate();
            if (forestUnlocked) BuildSecretGarden();

            BuildCamera();
            BuildLighting();

            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.70f, 0.82f, 0.96f);
            RenderSettings.ambientEquatorColor = new Color(0.83f, 0.86f, 0.72f);
            RenderSettings.ambientGroundColor = new Color(0.38f, 0.32f, 0.24f);

            Debug.Log("[Mia & Max] Tap Adventure 3D greybox created: fixed angled camera, visible environment, raycast object selection. Production art is still required.");
        }

        private void CreateMaterials()
        {
            // These authored URP materials and their albedo/normal maps already exist
            // under Resources/MiaMax/Materials. Use them instead of the missing atlas.
            grassMat = LoadMaterialOrCreate("Grass_Lawn", "Meadow Grass", new Color(0.35f, 0.62f, 0.25f));
            woodMat = LoadMaterialOrCreate("Wood_Warm", "Honey Wood", new Color(0.48f, 0.29f, 0.14f));
            plasterMat = LoadMaterialOrCreate("Plaster_Cream", "Warm Cream Plaster", new Color(0.82f, 0.73f, 0.58f));
            stoneMat = LoadMaterialOrCreate("Stone_Patio", "Garden Stone", new Color(0.62f, 0.58f, 0.48f));
            leafMat = LoadMaterialOrCreate("Leaf_Green", "Leaf Green", new Color(0.25f, 0.55f, 0.20f));
            flowerMat = LoadMaterialOrCreate("Flower_Petal", "Petal Coral", new Color(1f, 0.54f, 0.48f));
            roofMat = LoadMaterialOrCreate("Roof_Shingle", "Terracotta Roof", new Color(0.48f, 0.22f, 0.16f));
            waterMat = LoadMaterialOrCreate("Water_Pond", "Pond Water", new Color(0.25f, 0.68f, 0.78f));
        }

        private Material LoadMaterialOrCreate(string resourceName, string displayName, Color fallback)
        {
            Material authored = Resources.Load<Material>("MiaMax/Materials/" + resourceName);
            if (authored != null) return authored;
            return MakeMaterial(displayName, null, Vector2.zero, fallback);
        }

        private Material MakeMaterial(string materialName, Texture2D atlas, Vector2 offset, Color tint)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            Material material = new Material(shader) { name = materialName, color = tint };
            if (atlas != null)
            {
                material.mainTexture = atlas;
                material.mainTextureScale = new Vector2(0.5f, 0.5f);
                material.mainTextureOffset = offset;
            }
            return material;
        }

        private void BuildGroundAndPaths()
        {
            Create("Meadow", PrimitiveType.Plane, new Vector3(0f, -0.12f, 0f), new Vector3(3.8f, 1f, 3.8f), grassMat, false);
            Create("Garden Path", PrimitiveType.Cube, new Vector3(0f, 0.015f, -1.8f), new Vector3(2.0f, 0.08f, 11f), stoneMat, false);
            Create("Patio", PrimitiveType.Cube, new Vector3(0f, 0.05f, 5.5f), new Vector3(8.4f, 0.18f, 3.0f), woodMat, false);
            // Rounded stepping stones create a readable path without floating labels.
            for (int i = 0; i < 7; i++)
            {
                GameObject stone = Create("Stepping Stone", PrimitiveType.Cylinder,
                    new Vector3((i % 2 == 0 ? -0.38f : 0.38f), 0.035f, -5.8f + i * 1.15f),
                    new Vector3(0.72f, 0.07f, 0.72f), stoneMat, false);
                stone.transform.rotation = Quaternion.Euler(0f, i * 19f, 0f);
            }
        }

        private void BuildHouse()
        {
            Create("Cozy House", PrimitiveType.Cube, new Vector3(0f, 2.0f, 8.0f), new Vector3(8.5f, 4.0f, 3.3f), plasterMat, false);
            Create("Roof", PrimitiveType.Cube, new Vector3(0f, 4.25f, 8.0f), new Vector3(9.0f, 0.45f, 3.9f), roofMat, false);
            Create("Front Door", PrimitiveType.Cube, new Vector3(0f, 1.05f, 6.28f), new Vector3(1.0f, 2.0f, 0.12f), woodMat, false);
            for (int x = -2; x <= 2; x += 4)
            {
                Create("Window Frame", PrimitiveType.Cube, new Vector3(x, 2.5f, 6.27f), new Vector3(1.3f, 1.25f, 0.12f), woodMat, false);
                Create("Window Glass", PrimitiveType.Cube, new Vector3(x, 2.5f, 6.18f), new Vector3(1.08f, 1.03f, 0.05f), waterMat, false);
            }
            GameObject porch = Create("Porch Steps", PrimitiveType.Cube, new Vector3(0f, 0.18f, 5.8f), new Vector3(2.3f, 0.32f, 0.65f), woodMat, false);
            AddTarget(porch, "house", "The Cozy House");
        }

        private void BuildTreeAndBirdhouse()
        {
            GameObject trunk = Create("Old Apple Tree", PrimitiveType.Cylinder, new Vector3(-5.2f, 2.0f, 0.7f),
                new Vector3(0.68f, 2.0f, 0.68f), woodMat, false);
            AddTarget(trunk, "tree", "Old Apple Tree");
            Create("Tree Canopy A", PrimitiveType.Sphere, new Vector3(-5.2f, 4.8f, 0.7f), new Vector3(3.8f, 3.2f, 3.6f), leafMat, false);
            Create("Tree Canopy B", PrimitiveType.Sphere, new Vector3(-6.5f, 4.1f, 1.1f), new Vector3(2.5f, 2.4f, 2.6f), leafMat, false);
            Create("Tree Canopy C", PrimitiveType.Sphere, new Vector3(-3.9f, 4.2f, 0.9f), new Vector3(2.5f, 2.6f, 2.5f), leafMat, false);
            GameObject birdhouse = Create("Birdhouse", PrimitiveType.Cube, new Vector3(-4.5f, 3.25f, 0.45f),
                new Vector3(0.8f, 0.72f, 0.62f), birdhouseBuilt ? woodMat : plasterMat, false);
            Create("Birdhouse Roof", PrimitiveType.Cube, new Vector3(-4.5f, 3.67f, 0.45f), new Vector3(1.0f, 0.16f, 0.78f), roofMat, false);
            GameObject birdTarget = Create("Birdhouse Interaction", PrimitiveType.Sphere, new Vector3(-4.5f, 3.25f, 0.45f),
                Vector3.one * 0.95f, MakeMaterial("Birdhouse Hit Area", null, Vector2.zero, new Color(1,1,1,0)), false);
            Renderer r = birdTarget.GetComponent<Renderer>(); if (r != null) r.enabled = false;
            AddTarget(birdTarget, "birdhouse", "Birdhouse");
            if (!birdhouseBuilt) { birdhouse.SetActive(false); GameObject roof = GameObject.Find("Birdhouse Roof"); if (roof != null) roof.SetActive(false); }
        }

        private void BuildGarden()
        {
            GameObject bed = Create("Flower Bed", PrimitiveType.Cube, new Vector3(4.2f, 0.22f, 0.3f),
                new Vector3(3.8f, 0.42f, 2.4f), woodMat, false);
            Create("Rich Soil", PrimitiveType.Cube, new Vector3(4.2f, 0.46f, 0.3f), new Vector3(3.45f, 0.08f, 2.05f),
                MakeMaterial("Soil", null, Vector2.zero, new Color(0.30f, 0.19f, 0.12f)), false);
            AddTarget(bed, "flowers", "Flower Bed");
            for (int i = 0; i < 5; i++)
            {
                float x = 3.0f + i * 0.58f;
                Create("Flower Stem", PrimitiveType.Cylinder, new Vector3(x, 0.86f, 0.2f + (i % 2) * 0.55f),
                    new Vector3(0.045f, 0.38f, 0.045f), leafMat, false);
                GameObject bud = Create("Flower Bloom", PrimitiveType.Sphere, new Vector3(x, 1.22f, 0.2f + (i % 2) * 0.55f),
                    Vector3.one * (flowersWatered && DateTime.UtcNow >= flowersReadyUtc ? 0.36f : 0.16f),
                    flowerMat, false);
                if (!flowersWatered || DateTime.UtcNow < flowersReadyUtc) bud.transform.localScale = Vector3.one * 0.12f;
            }
        }

        private void BuildWorkshop()
        {
            GameObject table = Create("Crafting Bench", PrimitiveType.Cube, new Vector3(5.0f, 0.72f, 4.2f),
                new Vector3(2.6f, 0.22f, 1.1f), woodMat, false);
            for (int x = 0; x < 2; x++)
                Create("Bench Leg", PrimitiveType.Cube, new Vector3(4.1f + x * 1.8f, 0.34f, 4.2f),
                    new Vector3(0.16f, 0.72f, 0.16f), woodMat, false);
            AddTarget(table, "workshop", "Crafting Bench");
            Create("Tool Rack", PrimitiveType.Cube, new Vector3(5.0f, 1.5f, 4.75f), new Vector3(1.6f, 0.9f, 0.12f), plasterMat, false);
        }

        private void BuildPondAndDecor()
        {
            GameObject pond = Create("Little Pond", PrimitiveType.Cylinder, new Vector3(-4.1f, 0.02f, -5.4f),
                new Vector3(3.3f, 0.12f, 2.4f), waterMat, false);
            pond.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            AddTarget(pond, "pond", "Little Pond");
            for (int i = 0; i < 8; i++)
            {
                float x = -8.0f + (i % 4) * 4.6f;
                float z = -8.0f + (i / 4) * 14f;
                Create("Garden Shrub", PrimitiveType.Sphere, new Vector3(x, 0.48f, z), new Vector3(1.2f, 0.9f, 1.1f), leafMat, false);
            }
            GameObject twigsTarget = Create("Fallen Twigs", PrimitiveType.Cylinder, new Vector3(2.0f, 0.16f, -3.8f),
                new Vector3(0.15f, 0.5f, 0.15f), woodMat, false);
            twigsTarget.transform.rotation = Quaternion.Euler(0f, 35f, 72f);
            AddTarget(twigsTarget, "twigs", "Fallen Twigs");
            GameObject featherTarget = Create("Blue Feather", PrimitiveType.Sphere, new Vector3(-1.8f, 0.22f, -6.8f),
                new Vector3(0.22f, 0.42f, 0.08f), MakeMaterial("Feather Blue", null, Vector2.zero, new Color(0.22f,0.55f,0.95f)), false);
            AddTarget(featherTarget, "feather", "Blue Feather");
            GameObject mini = Create("Max Treasure Basket", PrimitiveType.Cylinder, new Vector3(7.0f, 0.35f, -5.3f),
                new Vector3(1.0f, 0.45f, 1.0f), woodMat, false);
            AddTarget(mini, "minigame", "Max's Treasure Basket");
        }

        private void BuildAdventureGate()
        {
            GameObject gate = Create("Adventure Gate", PrimitiveType.Cube, new Vector3(8.0f, 1.15f, 0.2f),
                new Vector3(0.32f, 2.3f, 2.6f), forestUnlocked ? leafMat : woodMat, false);
            AddTarget(gate, "gate", "Secret Garden Gate");
            Create("Gate Post Left", PrimitiveType.Cylinder, new Vector3(7.0f, 1.2f, 0.2f), new Vector3(0.22f, 1.2f, 0.22f), woodMat, false);
            Create("Gate Post Right", PrimitiveType.Cylinder, new Vector3(9.0f, 1.2f, 0.2f), new Vector3(0.22f, 1.2f, 0.22f), woodMat, false);
        }


        private void BuildSecretGarden()
        {
            if (transform.Find("Secret Garden Clearing") != null) return;
            Material magicLeaf = MakeMaterial("Secret Garden Leaves", null, Vector2.zero, new Color(0.22f, 0.42f, 0.28f));
            Material glow = MakeMaterial("Secret Garden Glow", null, Vector2.zero, new Color(0.95f, 0.78f, 0.40f));
            Create("Secret Garden Clearing", PrimitiveType.Cylinder, new Vector3(12.5f, -0.02f, -1.0f),
                new Vector3(8.0f, 0.08f, 9.0f), grassMat, false);
            for (int i = 0; i < 7; i++)
            {
                float x = 9.2f + (i % 4) * 2.1f;
                float z = -4.2f + (i / 4) * 3.0f;
                Create("Secret Garden Tree Trunk", PrimitiveType.Cylinder, new Vector3(x, 1.4f, z),
                    new Vector3(0.32f, 1.4f, 0.32f), woodMat, false);
                Create("Secret Garden Canopy", PrimitiveType.Sphere, new Vector3(x, 3.3f, z),
                    new Vector3(2.1f, 2.2f, 2.0f), magicLeaf, false);
            }
            GameObject crystal = Create("Golden Garden Stone", PrimitiveType.Sphere, new Vector3(12.4f, 0.75f, -1.1f),
                Vector3.one * 0.8f, glow, false);
            AddTarget(crystal, "gardenstone", "Golden Garden Stone");
        }

        private void BuildCamera()
        {
            Camera existing = Camera.main;
            if (existing != null)
            {
                worldCamera = existing;
                worldCamera.clearFlags = CameraClearFlags.SolidColor;
                worldCamera.backgroundColor = new Color(0.68f, 0.82f, 0.94f);
            }
            else
            {
                GameObject cameraObject = new GameObject("Tap Adventure Fixed Camera");
                worldCamera = cameraObject.AddComponent<Camera>();
                if (FindFirstObjectByType<AudioListener>() == null)
                    cameraObject.AddComponent<AudioListener>();
                cameraObject.tag = "MainCamera";
            }

            // Portrait-friendly, fixed three-quarter view. The previous camera was
            // placed at ground level and looked horizontally, making the whole yard
            // appear edge-on and effectively invisible.
            worldCamera.orthographic = true;
            worldCamera.orthographicSize = 13.5f;
            worldCamera.transform.position = new Vector3(1.0f, 13.5f, -18f);
            worldCamera.transform.LookAt(new Vector3(1.0f, 1.0f, 0.5f));
            worldCamera.nearClipPlane = 0.1f;
            worldCamera.farClipPlane = 100f;
            worldCamera.allowHDR = true;
            worldCamera.allowMSAA = true;
        }

        private void BuildLighting()
        {
            Light existing = FindFirstObjectByType<Light>();
            if (existing != null) return;

            GameObject sunObject = new GameObject("Tap Adventure Warm Sun");
            Light sun = sunObject.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.color = new Color(1.0f, 0.88f, 0.70f);
            sun.intensity = 1.25f;
            sun.shadows = LightShadows.Soft;
            sunObject.transform.rotation = Quaternion.Euler(48f, -32f, 0f);
        }

        private GameObject Create(string objectName, PrimitiveType primitive, Vector3 position, Vector3 scale, Material material, bool collider)
        {
            GameObject go = GameObject.CreatePrimitive(primitive);
            go.name = objectName;
            go.transform.SetParent(transform, false);
            go.transform.position = position;
            go.transform.localScale = scale;
            Renderer renderer = go.GetComponent<Renderer>();
            if (renderer != null) renderer.sharedMaterial = material;
            Collider c = go.GetComponent<Collider>();
            if (!collider && c != null) c.enabled = false;
            return go;
        }

        private void AddTarget(GameObject go, string id, string displayName)
        {
            Collider c = go.GetComponent<Collider>();
            if (c == null) c = go.AddComponent<BoxCollider>();
            c.enabled = true;
            TapAdventureTarget target = go.AddComponent<TapAdventureTarget>();
            target.Id = id;
            target.DisplayName = displayName;
        }

        private void HandleWorldSelection()
        {
            if (miniGameOpen || worldCamera == null) return;

            Vector2 screen = Vector2.zero;
            bool pressed = false;
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                screen = Input.GetTouch(0).position;
                pressed = true;
            }
            else if (Input.GetMouseButtonDown(0))
            {
                screen = Input.mousePosition;
                pressed = true;
            }

            if (!pressed || Screen.width <= 0 || Screen.height <= 0) return;

            // Ignore the HUD and action card; world selection is based on real colliders.
            float normalizedY = screen.y / Screen.height;
            if (normalizedY < 0.22f || normalizedY > 0.80f) return;

            Ray ray = worldCamera.ScreenPointToRay(screen);
            if (!Physics.Raycast(ray, out RaycastHit hit, 150f, Physics.DefaultRaycastLayers,
                    QueryTriggerInteraction.Collide))
                return;

            TapAdventureTarget target = hit.collider.GetComponentInParent<TapAdventureTarget>();
            if (target != null)
                selectedId = target.Id;
        }

        private Texture2D MakeUiTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.hideFlags = HideFlags.HideAndDontSave;
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        private void EnsureStyles()
        {
            if (titleStyle != null) return;
            cardTexture = MakeUiTexture(new Color(1f, 0.965f, 0.88f, 0.94f));
            buttonTexture = MakeUiTexture(new Color(0.30f, 0.55f, 0.34f, 1f));
            buttonPressedTexture = MakeUiTexture(new Color(0.22f, 0.42f, 0.27f, 1f));
            cardStyle = new GUIStyle(GUI.skin.box);
            cardStyle.normal.background = cardTexture;
            cardStyle.border = new RectOffset(14, 14, 14, 14);
            cardStyle.padding = new RectOffset(12, 12, 8, 8);
            titleStyle = new GUIStyle(GUI.skin.label) { fontSize = Mathf.Max(20, Screen.width / 25), fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            titleStyle.normal.textColor = new Color(0.25f, 0.19f, 0.13f);
            bodyStyle = new GUIStyle(GUI.skin.label) { fontSize = Mathf.Max(14, Screen.width / 42), wordWrap = true };
            bodyStyle.normal.textColor = new Color(0.28f, 0.24f, 0.19f);
            mutedStyle = new GUIStyle(bodyStyle);
            mutedStyle.fontSize = Mathf.Max(12, Screen.width / 48);
            mutedStyle.normal.textColor = new Color(0.43f, 0.35f, 0.25f);
            buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = Mathf.Max(15, Screen.width / 38), fontStyle = FontStyle.Bold, wordWrap = true, alignment = TextAnchor.MiddleCenter };
            buttonStyle.normal.background = buttonTexture;
            buttonStyle.hover.background = buttonTexture;
            buttonStyle.active.background = buttonPressedTexture;
            buttonStyle.focused.background = buttonTexture;
            buttonStyle.normal.textColor = Color.white;
            buttonStyle.hover.textColor = Color.white;
            buttonStyle.active.textColor = Color.white;
            buttonStyle.padding = new RectOffset(12, 12, 8, 8);
        }

        private void OnGUI()
        {
            // The world is rendered by the fixed 3D camera. Never stretch the
            // optional concept-art texture over the live scene: it would hide all
            // 3D colliders and make tap selection feel disconnected from the world.
            EnsureStyles();
            float scale = Mathf.Clamp(Screen.width / 430f, 0.78f, 1.5f);
            float margin = 16f * scale;
            float topH = 92f * scale;
            GUI.Box(new Rect(margin, margin, Screen.width - margin * 2f, topH), GUIContent.none, cardStyle);
            GUI.Label(new Rect(margin + 16f, margin + 7f, Screen.width - margin * 2f - 32f, 34f * scale), "MIA & MAX  •  LITTLE MOMENTS, BIG ADVENTURES", titleStyle);
            GUI.Label(new Rect(margin + 16f, margin + 43f * scale, Screen.width - margin * 2f - 32f, 34f * scale),
                "✦ " + stars + "   •   Twigs " + twigs + "   •   Feathers " + feathers + "   •   Flowers " + flowers, bodyStyle);

            float questY = margin + topH + 10f * scale;
            GUI.Box(new Rect(margin, questY, Screen.width - margin * 2f, 76f * scale), GUIContent.none, cardStyle);
            GUI.Label(new Rect(margin + 14f, questY + 5f, Screen.width - margin * 2f - 28f, 30f * scale), "TODAY'S LITTLE ADVENTURE", mutedStyle);
            string quest = !birdhouseBuilt ? "Build a birdhouse: gather 3 twigs and 1 feather." :
                !flowersWatered ? "Water the flower bed and wait for the blooms." :
                DateTime.UtcNow < flowersReadyUtc ? "The flowers are growing. Come back a little later!" :
                !forestUnlocked ? "Craft a garden keepsake at the workbench to open the Secret Garden." :
                "The Secret Garden is open. What will you discover next?";
            GUI.Label(new Rect(margin + 14f, questY + 31f * scale, Screen.width - margin * 2f - 28f, 38f * scale), quest, bodyStyle);

            float panelH = selectedId == "" ? 72f : 226f * scale;
            Rect panel = new Rect(margin, Screen.height - panelH - margin, Screen.width - margin * 2f, panelH);
            GUI.Box(panel, GUIContent.none, cardStyle);
            if (selectedId == "")
            {
                GUI.Label(new Rect(panel.x + 14f, panel.y + 12f, panel.width - 28f, 48f), "Tap the tree, fallen twigs, flower bed, workshop or Max's basket to explore.", bodyStyle);
            }
            else DrawSelectedPanel(panel, scale);

            if (miniGameOpen) DrawMiniGame(scale);
            if (!string.IsNullOrEmpty(toast))
            {
                Rect toastRect = new Rect(margin, Screen.height - panelH - 62f * scale, Screen.width - margin * 2f, 44f * scale);
                GUI.Label(toastRect, toast, mutedStyle);
            }
        }

        private void DrawSelectedPanel(Rect panel, float scale)
        {
            string name = GetDisplayName(selectedId);
            GUI.Label(new Rect(panel.x + 14f, panel.y + 9f, panel.width - 28f, 34f * scale), name, titleStyle);
            string description = GetDescription(selectedId);
            GUI.Label(new Rect(panel.x + 14f, panel.y + 44f * scale, panel.width - 28f, 46f * scale), description, bodyStyle);
            Rect button = new Rect(panel.x + 14f, panel.y + 98f * scale, panel.width - 28f, 48f * scale);
            if (GUI.Button(button, GetActionLabel(selectedId), buttonStyle)) PerformAction(selectedId);
            if (GUI.Button(new Rect(panel.x + 14f, panel.y + 151f * scale, panel.width - 28f, 40f * scale), "Close", buttonStyle))
                selectedId = "";
        }

        private string GetDisplayName(string id)
        {
            switch (id)
            {
                case "tree": return "Old Apple Tree";
                case "twigs": return "Fallen Twigs";
                case "feather": return "A Blue Feather";
                case "birdhouse": return "Birdhouse";
                case "flowers": return "Flower Bed";
                case "workshop": return "Crafting Bench";
                case "pond": return "Little Pond";
                case "minigame": return "Max's Treasure Basket";
                case "gate": return "Secret Garden Gate";
                case "gardenstone": return "Golden Garden Stone";
                default: return "A little discovery";
            }
        }

        private string GetDescription(string id)
        {
            switch (id)
            {
                case "tree": return treeGathered ? "You have already gathered the loose twigs from this tree." : "A few loose twigs have fallen near the roots. They may be useful.";
                case "twigs": return "Some sturdy twigs have fallen nearby. They could be useful for a small building project.";
                case "feather": return "A bright blue feather. Perhaps a bird left it here.";
                case "birdhouse": return birdhouseBuilt ? "A new home! Perhaps its little visitors will arrive soon." : "A cozy birdhouse needs 3 twigs and 1 feather.";
                case "flowers": return flowersWatered && DateTime.UtcNow < flowersReadyUtc ? "The flowers are drinking up the water. They'll bloom soon." : "A cheerful flower bed. A little care will make it bloom.";
                case "workshop": return "Make useful things from the treasures you find around the yard.";
                case "pond": return "A quiet pond. Max loves watching the ripples and tiny fish.";
                case "minigame": return "Max found a basket of mixed-up treasures. Help sort them for a reward!";
                case "gate": return forestUnlocked ? "The Secret Garden is ready to explore!" : "Craft a garden keepsake at the workbench to unlock this gate.";
                case "gardenstone": return "A warm golden stone marks the beginning of a new chapter.";
                default: return "Take a closer look and see what you can do.";
            }
        }

        private string GetActionLabel(string id)
        {
            switch (id)
            {
                case "twigs": return "Collect twigs";
                case "feather": return "Collect feather";
                case "birdhouse": return birdhouseBuilt ? "Admire bird visitors" : "Build birdhouse (3 twigs + 1 feather)";
                case "flowers": return flowersWatered ? (DateTime.UtcNow >= flowersReadyUtc ? "Collect flowers" : "Check flowers") : "Water flowers";
                case "workshop": return "Make a little keepsake";
                case "minigame": return "Play treasure sorting";
                case "gate": return forestUnlocked ? "Enter Secret Garden" : "Check the gate";
                case "gardenstone": return "Touch the glowing stone";
                case "tree": return treeGathered ? "Look at the tree" : "Gather fallen twigs";
                case "pond": return "Watch the pond";
                default: return "Explore";
            }
        }

        private void PerformAction(string id)
        {
            switch (id)
            {
                case "twigs":
                    if (!twigsGathered)
                    {
                        twigs += 3; stars += 1; twigsGathered = true; toast = "You found a bundle of 3 useful twigs!";
                    }
                    else toast = "You already gathered these fallen twigs.";
                    break;
                case "feather":
                    if (!featherGathered) { feathers++; stars++; featherGathered = true; toast = "A beautiful feather for your collection!"; }
                    else toast = "You already collected this feather.";
                    break;
                case "birdhouse":
                    if (birdhouseBuilt) { toast = "A little bird has made this its home!"; }
                    else if (twigs >= 3 && feathers >= 1)
                    {
                        twigs -= 3; feathers -= 1; birdhouseBuilt = true; stars += 5;
                        Transform bird = transform.Find("Birdhouse");
                        if (bird != null) bird.gameObject.SetActive(true);
                        Transform roof = transform.Find("Birdhouse Roof");
                        if (roof != null) roof.gameObject.SetActive(true);
                        toast = "Wonderful! The birdhouse is ready. A new story begins.";
                    }
                    else toast = "You need 3 twigs and 1 feather. Look around the yard.";
                    break;
                case "flowers":
                    if (!flowersWatered)
                    {
                        flowersWatered = true; flowersReadyUtc = DateTime.UtcNow.AddMinutes(1);
                        UpdateFlowerVisuals();
                        toast = "Watered! The flowers will bloom in a little while.";
                    }
                    else if (DateTime.UtcNow >= flowersReadyUtc)
                    {
                        flowers += 3; stars += 4; flowersWatered = false; flowersReadyUtc = DateTime.MinValue;
                        toast = "Beautiful blooms! You collected 3 flowers.";
                        if (birdhouseBuilt) toast += " Visit the workbench to craft a keepsake.";
                        UpdateFlowerVisuals();
                    }
                    else toast = "The flowers are still growing. Come back soon.";
                    break;
                case "workshop":
                    if (!forestUnlocked && twigs >= 1 && flowers >= 1)
                    {
                        twigs--; flowers--; forestUnlocked = true; stars += 8;
                        BuildSecretGarden();
                        toast = "You crafted a garden keepsake! The Secret Garden Gate is open.";
                        toast += " A new garden clearing has appeared beyond the gate.";
                    }
                    else if (forestUnlocked) toast = "Your garden keepsake opened a new adventure!";
                    else toast = "Recipe: 1 twig + 1 flower. Gather twigs beneath the tree and grow flowers.";
                    break;
                case "minigame":
                    if (miniGameClaimed) toast = "Max has already sorted this basket. Come back for another adventure!";
                    else { miniGameOpen = true; miniGameStep = 0; miniGameScore = 0; miniGameMessage = "Tap the treasures in this order: feather, shell, leaf."; }
                    break;
                case "gate":
                    toast = forestUnlocked ? "Welcome to the Secret Garden! Explore the new discoveries." : "Craft a garden keepsake at the workbench to unlock this gate.";
                    if (forestUnlocked) toast += " The Secret Garden chapter is unlocked.";
                    break;
                case "gardenstone":
                    toast = gardenstoneDiscovered ? "The golden stone glows softly." : "The Secret Garden's story has only just begun!";
                    if (!gardenstoneDiscovered) { gardenstoneDiscovered = true; stars++; }
                    break;
                case "tree":
                    if (!treeGathered) { twigs += 2; treeGathered = true; stars++; toast = "You gathered 2 more twigs beneath the tree."; }
                    else toast = "The tree is peaceful in the afternoon light.";
                    break;
                case "pond": toast = "The water shimmers in the afternoon sun."; break;
            }
            SaveProgress();
        }

        private void UpdateFlowerVisuals()
        {
            bool readyToBloom = flowersWatered && DateTime.UtcNow >= flowersReadyUtc;
            float bloomScale = readyToBloom ? 0.36f : 0.12f;
            foreach (Transform child in transform)
                if (child.name == "Flower Bloom")
                    child.localScale = Vector3.one * bloomScale;
        }

        private void Update()
        {
            if (flowersWatered && DateTime.UtcNow >= flowersReadyUtc)
                UpdateFlowerVisuals();

            HandleWorldSelection();
        }

        private void DrawMiniGame(float scale)
        {
            Rect r = new Rect(20f, Screen.height * 0.27f, Screen.width - 40f, Screen.height * 0.42f);
            GUI.Box(r, GUIContent.none, cardStyle);
            GUI.Label(new Rect(r.x + 18f, r.y + 16f, r.width - 36f, 44f), "MAX'S TREASURE SORT", titleStyle);
            GUI.Label(new Rect(r.x + 18f, r.y + 65f, r.width - 36f, 62f), miniGameMessage, bodyStyle);
            string[] labels = { "🪶 Feather", "🐚 Shell", "🍃 Leaf" };
            for (int i = 0; i < 3; i++)
            {
                int index = i;
                if (GUI.Button(new Rect(r.x + 18f, r.y + 140f + i * 58f * scale, r.width - 36f, 50f * scale), labels[index], buttonStyle))
                {
                    int expected = miniGameStep;
                    if (index == expected)
                    {
                        miniGameStep++; miniGameScore++;
                        if (miniGameStep >= 3)
                        {
                            miniGameOpen = false;
                            miniGameClaimed = true;
                            stars += 5; feathers++;
                            toast = "Max did it! You earned 5 stars and a bonus feather.";
                            SaveProgress();
                        }
                        else miniGameMessage = "Great! Now find the next treasure.";
                    }
                    else miniGameMessage = "Not quite. Try the next item in the sequence.";
                }
            }
            if (GUI.Button(new Rect(r.x + 18f, r.yMax - 48f, r.width - 36f, 38f), "Leave mini-game", buttonStyle)) miniGameOpen = false;
        }
    }

    public sealed class TapAdventureTarget : MonoBehaviour
    {
        public string Id;
        public string DisplayName;
    }
}
