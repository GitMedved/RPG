#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Builds editable, UV-mapped stylized environment meshes for all backyard assets.
    /// Generated assets are kept separate from gameplay placeholders until reviewed.
    /// </summary>
    public static class BackyardProductionModelGenerator
    {
        private const string Root = "Assets/_Project/Art/Environment/Runtime";
        private const string PrefabFolder = Root + "/ProductionPrefabs";
        private const string MeshFolder = Root + "/ProductionMeshes";
        private const string MaterialFolder = Root + "/ProductionMaterials";
        private const string TextureFolder = Root + "/ProductionTextures";

        private static readonly string[] Names =
        {
            "HouseExterior", "PatioDeck", "FenceStraight", "FenceCorner", "FenceGate",
            "MainTree", "SwingSet", "Sandbox", "ShrubCluster", "FlowerBed",
            "FlowerPot", "GardenPlot", "Workshop", "AdventureGate", "PathSegment"
        };

        private static readonly Dictionary<string, Material> Materials = new Dictionary<string, Material>();
        private static int meshIndex;

        [MenuItem("Tools/Mia & Max/Backyard/Generate All 15 Production Models")]
        public static void GenerateAll()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Mia & Max", "Exit Play Mode before generating model assets.", "OK");
                return;
            }

            EnsureFolder(Root);
            // These dedicated output folders contain generated files only; rebuild them deterministically.
            ResetGeneratedFolder(PrefabFolder);
            ResetGeneratedFolder(MeshFolder);
            ResetGeneratedFolder(MaterialFolder);
            ResetGeneratedFolder(TextureFolder);
            Materials.Clear();
            meshIndex = 0;
            CreateMaterials();

            int created = 0;
            var errors = new List<string>();
            try
            {
                foreach (string assetName in Names)
                {
                    try
                    {
                        string path = PrefabFolder + "/" + assetName + ".prefab";
                        GameObject root = new GameObject(assetName + "_PRODUCTION");
                        try
                        {
                            BuildAsset(assetName, root.transform);
                            GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                            if (existing != null) PrefabUtility.SaveAsPrefabAsset(root, path);
                            else PrefabUtility.SaveAsPrefabAsset(root, path);
                            created++;
                        }
                        finally { UnityEngine.Object.DestroyImmediate(root); }
                    }
                    catch (Exception ex) { errors.Add(assetName + ": " + ex.Message); }
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            finally { Materials.Clear(); }

            string report = "[Mia & Max] Procedural production model generation\n" +
                "Generated: " + created + " / " + Names.Length + "\n" +
                "Errors: " + errors.Count + (errors.Count == 0 ? "\nNo generation errors." : "\n" + string.Join("\n", errors.ToArray())) +
                "\n\nAssets are editable procedural meshes with UV0 and generated texture materials. " +
                "They are not yet visually approved or connected to gameplay.";
            if (errors.Count == 0) Debug.Log(report); else Debug.LogError(report);
            EditorUtility.DisplayDialog("Mia & Max — Production Models",
                "Generated: " + created + " / " + Names.Length + "\nErrors: " + errors.Count +
                "\n\nProductionPrefabs is separate from gameplay placeholders. Review before integration; see Console.",
                "OK");
        }

        private static void CreateMaterials()
        {
            Materials["plaster"] = MakeMaterial("Plaster", new Color(0.88f, 0.79f, 0.64f), new Color(0.77f, 0.68f, 0.54f));
            Materials["roof"] = MakeMaterial("Roof", new Color(0.25f, 0.23f, 0.21f), new Color(0.36f, 0.32f, 0.28f));
            Materials["white"] = MakeMaterial("WarmWhite", new Color(0.94f, 0.91f, 0.83f), new Color(0.82f, 0.80f, 0.74f));
            Materials["wood"] = MakeMaterial("NaturalWood", new Color(0.55f, 0.35f, 0.20f), new Color(0.68f, 0.48f, 0.29f));
            Materials["darkwood"] = MakeMaterial("DarkWood", new Color(0.31f, 0.20f, 0.13f), new Color(0.42f, 0.28f, 0.18f));
            Materials["grass"] = MakeMaterial("LeafGreen", new Color(0.27f, 0.48f, 0.20f), new Color(0.38f, 0.57f, 0.26f));
            Materials["leaflight"] = MakeMaterial("LeafLight", new Color(0.43f, 0.60f, 0.29f), new Color(0.31f, 0.49f, 0.22f));
            Materials["soil"] = MakeMaterial("RichSoil", new Color(0.30f, 0.20f, 0.13f), new Color(0.39f, 0.27f, 0.17f));
            Materials["sand"] = MakeMaterial("WarmSand", new Color(0.83f, 0.72f, 0.51f), new Color(0.91f, 0.81f, 0.61f));
            Materials["stone"] = MakeMaterial("WarmStone", new Color(0.65f, 0.59f, 0.49f), new Color(0.76f, 0.69f, 0.57f));
            Materials["blue"] = MakeMaterial("WindowGlass", new Color(0.30f, 0.53f, 0.68f), new Color(0.48f, 0.69f, 0.79f));
            Materials["pink"] = MakeMaterial("FlowerPink", new Color(0.85f, 0.40f, 0.53f), new Color(0.96f, 0.59f, 0.64f));
            Materials["yellow"] = MakeMaterial("FlowerGold", new Color(0.96f, 0.72f, 0.22f), new Color(0.99f, 0.84f, 0.39f));
            Materials["metal"] = MakeMaterial("PaintedMetal", new Color(0.37f, 0.43f, 0.44f), new Color(0.53f, 0.57f, 0.55f));
        }

        private static Material MakeMaterial(string name, Color tint, Color pattern)
        {
            string texturePath = TextureFolder + "/" + name + "_Albedo.png";
            Texture2D texture = new Texture2D(128, 128, TextureFormat.RGBA32, false);
            texture.name = name + "_Albedo";
            for (int y = 0; y < 128; y++)
            for (int x = 0; x < 128; x++)
            {
                float grain = Mathf.PerlinNoise(x * 0.075f + name.Length, y * 0.075f) * 0.22f;
                float stripe = ((x / 8 + y / 11) % 2 == 0) ? 0.04f : -0.025f;
                texture.SetPixel(x, y, Color.Lerp(tint, pattern, Mathf.Clamp01(grain + stripe + 0.12f)));
            }
            texture.Apply();
            File.WriteAllBytes(texturePath, texture.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceSynchronousImport);
            Texture2D imported = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);

            string matPath = MaterialFolder + "/" + name + ".mat";
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            Material mat = new Material(shader);
            mat.name = name;
            mat.color = Color.white;
            mat.mainTexture = imported;
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", 0.18f);
            AssetDatabase.CreateAsset(mat, matPath);
            return mat;
        }

        private static void BuildAsset(string name, Transform root)
        {
            switch (name)
            {
                case "HouseExterior":
                    Box(root, "Lower_Siding", new Vector3(0, 1.7f, 0), new Vector3(10, 3.4f, 6), "plaster");
                    Box(root, "Upper_Siding", new Vector3(0, 4.35f, 0.15f), new Vector3(8.8f, 2.0f, 5.6f), "plaster");
                    Box(root, "Foundation", new Vector3(0, 0.18f, 0), new Vector3(10.25f, 0.36f, 6.2f), "stone");
                    GableRoof(root, 9.5f, 3.25f, 6.2f, 5.45f, "roof");
                    for (int x = -1; x <= 1; x += 2)
                    {
                        Window(root, new Vector3(x * 3.1f, 2.0f, -3.06f));
                        Window(root, new Vector3(x * 2.7f, 4.35f, -2.72f));
                    }
                    Box(root, "FrontDoor", new Vector3(0, 1.2f, -3.08f), new Vector3(1.15f, 2.35f, 0.14f), "wood");
                    Box(root, "DoorGlass", new Vector3(0, 1.8f, -3.17f), new Vector3(0.56f, 0.85f, 0.035f), "blue");
                    for (int y = 0; y < 8; y++) Box(root, "SidingTrim_" + y, new Vector3(0, 0.55f + y * 0.43f, -3.035f), new Vector3(9.95f, 0.035f, 0.035f), "white");
                    AddHeroLod(root, "house", "plaster");
                    break;
                case "PatioDeck":
                    Box(root, "DeckBase", new Vector3(0, 0.18f, 0), new Vector3(8, 0.35f, 4.5f), "wood");
                    for (int i = 0; i < 12; i++) Box(root, "DeckBoard_" + i, new Vector3(-3.65f + i * 0.66f, 0.37f, 0), new Vector3(0.62f, 0.045f, 4.4f), "darkwood");
                    Box(root, "TableTop", new Vector3(0, 1.15f, 0.2f), new Vector3(2.0f, 0.16f, 1.25f), "wood");
                    for (int x = -1; x <= 1; x += 2) for (int z = -1; z <= 1; z += 2)
                        Box(root, "TableLeg", new Vector3(x * 0.78f, 0.68f, 0.2f + z * 0.43f), new Vector3(0.13f, 0.9f, 0.13f), "darkwood");
                    for (int i = 0; i < 2; i++) { Box(root, "ChairSeat_" + i, new Vector3(-2.1f + i * 4.2f, 0.78f, 0), new Vector3(1.0f, 0.16f, 0.9f), "white"); Box(root, "ChairBack_" + i, new Vector3(-2.1f + i * 4.2f, 1.25f, 0.38f), new Vector3(1.0f, 0.85f, 0.12f), "wood"); }
                    Cylinder(root, "GrillBody", new Vector3(3.0f, 0.95f, -1.25f), new Vector3(0.85f, 0.42f, 0.55f), "metal", 20);
                    break;
                case "FenceStraight": FenceSection(root, false); break;
                case "FenceCorner": FenceSection(root, false); FenceSection(root, true); break;
                case "FenceGate":
                    FenceSection(root, false);
                    Box(root, "GateLeaf", new Vector3(0, 0.8f, -0.16f), new Vector3(1.5f, 1.55f, 0.12f), "wood");
                    Box(root, "GateRail", new Vector3(0, 1.15f, -0.24f), new Vector3(1.55f, 0.12f, 0.06f), "white");
                    break;
                case "MainTree":
                    Cylinder(root, "TaperedTrunk", new Vector3(0, 1.8f, 0), new Vector3(0.85f, 1.8f, 0.85f), "wood", 12);
                    Cylinder(root, "BranchLeft", new Vector3(-0.75f, 3.0f, 0), new Vector3(0.22f, 1.3f, 0.22f), "wood", 9, Quaternion.Euler(0, 0, 38));
                    Cylinder(root, "BranchRight", new Vector3(0.75f, 3.2f, 0.1f), new Vector3(0.2f, 1.2f, 0.2f), "wood", 9, Quaternion.Euler(0, 0, -42));
                    Ellipsoid(root, "CanopyCore", new Vector3(0, 5.0f, 0), new Vector3(3.1f, 2.8f, 3.0f), "grass");
                    Ellipsoid(root, "CanopyLeft", new Vector3(-1.35f, 4.6f, 0.1f), new Vector3(2.1f, 2.1f, 2.1f), "leaflight");
                    Ellipsoid(root, "CanopyRight", new Vector3(1.3f, 4.75f, -0.2f), new Vector3(2.0f, 2.25f, 2.0f), "grass");
                    Ellipsoid(root, "CanopyTop", new Vector3(0, 6.25f, 0), new Vector3(1.9f, 1.8f, 1.9f), "leaflight");
                    AddHeroLod(root, "tree", "grass");
                    break;
                case "SwingSet":
                    for (int z = -1; z <= 1; z += 2)
                    {
                        Box(root, "AFrameLegA_" + z, new Vector3(-1.45f, 1.7f, z * 0.72f), new Vector3(0.16f, 3.4f, 0.16f), "wood", Quaternion.Euler(0, 0, -22));
                        Box(root, "AFrameLegB_" + z, new Vector3(1.45f, 1.7f, z * 0.72f), new Vector3(0.16f, 3.4f, 0.16f), "wood", Quaternion.Euler(0, 0, 22));
                    }
                    Box(root, "TopBeam", new Vector3(0, 3.25f, 0), new Vector3(3.6f, 0.2f, 0.2f), "darkwood");
                    for (int i = 0; i < 2; i++) { Cylinder(root, "RopeA_" + i, new Vector3(-0.7f + i * 1.4f, 1.95f, 0), new Vector3(0.045f, 1.2f, 0.045f), "white", 8); Box(root, "Seat_" + i, new Vector3(-0.7f + i * 1.4f, 0.72f, 0), new Vector3(0.85f, 0.16f, 0.55f), "blue"); }
                    break;
                case "Sandbox":
                    Box(root, "Sand", new Vector3(0, 0.48f, 0), new Vector3(3.5f, 0.12f, 2.6f), "sand");
                    Box(root, "FrontRail", new Vector3(0, 0.35f, -1.38f), new Vector3(3.9f, 0.55f, 0.22f), "wood");
                    Box(root, "BackRail", new Vector3(0, 0.35f, 1.38f), new Vector3(3.9f, 0.55f, 0.22f), "wood");
                    Box(root, "LeftRail", new Vector3(-1.85f, 0.35f, 0), new Vector3(0.22f, 0.55f, 2.6f), "wood");
                    Box(root, "RightRail", new Vector3(1.85f, 0.35f, 0), new Vector3(0.22f, 0.55f, 2.6f), "wood");
                    break;
                case "ShrubCluster":
                    for (int i = 0; i < 7; i++) Ellipsoid(root, "Shrub_" + i, new Vector3((i - 3) * 0.55f, 0.55f + (i % 3) * 0.12f, (i % 2) * 0.35f), new Vector3(0.85f, 0.75f + (i % 2) * 0.12f, 0.82f), i % 2 == 0 ? "grass" : "leaflight");
                    break;
                case "FlowerBed":
                    Box(root, "SoilBed", new Vector3(0, 0.2f, 0), new Vector3(3.2f, 0.35f, 1.25f), "soil");
                    for (int i = 0; i < 9; i++) Flower(root, new Vector3(-1.35f + (i % 5) * 0.65f, 0.58f, (i / 5) * 0.42f), i % 2 == 0 ? "pink" : "yellow");
                    break;
                case "FlowerPot":
                    Cylinder(root, "PotBody", new Vector3(0, 0.38f, 0), new Vector3(0.65f, 0.42f, 0.65f), "pink", 16);
                    Cylinder(root, "Soil", new Vector3(0, 0.78f, 0), new Vector3(0.5f, 0.06f, 0.5f), "soil", 16);
                    Cylinder(root, "PlantStem", new Vector3(0, 1.15f, 0), new Vector3(0.06f, 0.42f, 0.06f), "grass", 8);
                    Flower(root, new Vector3(0, 1.55f, 0), "yellow");
                    break;
                case "GardenPlot":
                    Box(root, "RaisedBed", new Vector3(0, 0.25f, 0), new Vector3(1.9f, 0.45f, 1.55f), "wood");
                    Box(root, "SoilSurface", new Vector3(0, 0.5f, 0), new Vector3(1.72f, 0.08f, 1.37f), "soil");
                    for (int i = -1; i <= 1; i++) Box(root, "Furrow_" + i, new Vector3(i * 0.5f, 0.56f, 0), new Vector3(0.08f, 0.035f, 1.2f), "darkwood");
                    break;
                case "Workshop":
                    Box(root, "WorkbenchTop", new Vector3(0, 1.0f, 0), new Vector3(2.4f, 0.18f, 1.1f), "wood");
                    for (int x = -1; x <= 1; x += 2) for (int z = -1; z <= 1; z += 2) Box(root, "Leg_" + x + "_" + z, new Vector3(x * 1.0f, 0.5f, z * 0.4f), new Vector3(0.14f, 0.95f, 0.14f), "darkwood");
                    Box(root, "ToolBoard", new Vector3(0, 1.85f, 0.48f), new Vector3(2.1f, 1.35f, 0.12f), "wood");
                    for (int i = 0; i < 3; i++) Box(root, "Tool_" + i, new Vector3(-0.65f + i * 0.65f, 1.9f, 0.39f), new Vector3(0.12f, 0.65f, 0.06f), "metal", Quaternion.Euler(0, 0, (i - 1) * 15));
                    break;
                case "AdventureGate":
                    Box(root, "LeftPost", new Vector3(-1.25f, 1.45f, 0), new Vector3(0.28f, 2.9f, 0.32f), "wood");
                    Box(root, "RightPost", new Vector3(1.25f, 1.45f, 0), new Vector3(0.28f, 2.9f, 0.32f), "wood");
                    Box(root, "Header", new Vector3(0, 2.85f, 0), new Vector3(3.0f, 0.35f, 0.42f), "darkwood");
                    Box(root, "GatePanel", new Vector3(0, 0.78f, 0), new Vector3(2.05f, 1.5f, 0.14f), "wood");
                    for (int i = -2; i <= 2; i++) Box(root, "GateSlat_" + i, new Vector3(i * 0.35f, 0.8f, -0.1f), new Vector3(0.09f, 1.35f, 0.08f), "white");
                    break;
                case "PathSegment":
                    Box(root, "PathBase", new Vector3(0, 0.07f, 0), new Vector3(2.5f, 0.14f, 3.5f), "stone");
                    for (int i = 0; i < 18; i++)
                    {
                        float x = -1.0f + (i % 3) * 0.95f;
                        float z = -1.35f + (i / 3) * 0.45f;
                        Ellipsoid(root, "StonePaver_" + i, new Vector3(x, 0.17f, z), new Vector3(0.38f, 0.07f, 0.18f), "sand");
                    }
                    break;
            }
        }

        private static void AddHeroLod(Transform root, string type, string material)
        {
            GameObject low = new GameObject("LOD1_Simplified");
            low.transform.SetParent(root, false);
            Renderer lowRenderer;
            if (type == "house") lowRenderer = Box(low.transform, "Low_HouseMass", new Vector3(0, 2.7f, 0), new Vector3(10, 5.4f, 6), "plaster");
            else
            {
                Cylinder(low.transform, "Low_Trunk", new Vector3(0, 1.8f, 0), new Vector3(0.8f, 1.8f, 0.8f), "wood", 8);
                lowRenderer = Ellipsoid(low.transform, "Low_Canopy", new Vector3(0, 5.0f, 0), new Vector3(3.2f, 2.8f, 3.2f), material);
            }
            foreach (Transform child in root)
                if (child.gameObject != low) child.gameObject.SetActive(true);
            low.SetActive(true);
            var highRenderers = new List<Renderer>();
            foreach (Transform child in root)
                if (child.gameObject != low) highRenderers.AddRange(child.GetComponentsInChildren<Renderer>());
            LODGroup group = root.gameObject.AddComponent<LODGroup>();
            group.SetLODs(new[] { new LOD(0.25f, highRenderers.ToArray()), new LOD(0.08f, low.GetComponentsInChildren<Renderer>()) });
            group.RecalculateBounds();
        }

        private static void Window(Transform root, Vector3 p)
        {
            Box(root, "WindowFrame", p, new Vector3(1.35f, 1.05f, 0.12f), "white");
            Box(root, "WindowGlass", p + new Vector3(0, 0, -0.075f), new Vector3(1.08f, 0.8f, 0.035f), "blue");
            Box(root, "WindowMullionV", p, new Vector3(0.055f, 0.82f, 0.04f), "white");
            Box(root, "WindowMullionH", p, new Vector3(1.08f, 0.055f, 0.04f), "white");
        }

        private static void FenceSection(Transform root, bool corner)
        {
            for (int i = 0; i < 9; i++)
            {
                Vector3 pos = new Vector3(-1.6f + i * 0.4f, 0.82f, 0);
                if (corner) pos = new Vector3(0, 0.82f, -1.6f + i * 0.4f);
                Box(root, "Picket_" + i, pos, new Vector3(0.14f, 1.65f, 0.16f), "white");
                Box(root, "PicketTip_" + i, pos + Vector3.up * 0.88f, new Vector3(0.16f, 0.18f, 0.18f), "white");
            }
            if (!corner)
            {
                Box(root, "TopRail", new Vector3(0, 1.32f, 0), new Vector3(3.5f, 0.13f, 0.18f), "white");
                Box(root, "BottomRail", new Vector3(0, 0.42f, 0), new Vector3(3.5f, 0.13f, 0.18f), "white");
            }
        }

        private static void Flower(Transform root, Vector3 p, string petalMaterial)
        {
            Cylinder(root, "Stem", p + Vector3.down * 0.18f, new Vector3(0.045f, 0.25f, 0.045f), "grass", 7);
            Ellipsoid(root, "FlowerCenter", p + Vector3.up * 0.12f, Vector3.one * 0.16f, "yellow");
            for (int i = 0; i < 6; i++)
            {
                float a = i * Mathf.PI / 3f;
                Vector3 offset = new Vector3(Mathf.Cos(a) * 0.2f, 0.12f, Mathf.Sin(a) * 0.2f);
                Ellipsoid(root, "Petal_" + i, p + offset, new Vector3(0.2f, 0.09f, 0.16f), petalMaterial);
            }
        }

        private static Renderer Box(Transform parent, string name, Vector3 position, Vector3 size, string mat, Quaternion rotation = default(Quaternion))
        {
            Vector3 h = size * 0.5f;
            Vector3[] v =
            {
                new Vector3(-h.x,-h.y,-h.z), new Vector3(h.x,-h.y,-h.z), new Vector3(h.x,h.y,-h.z), new Vector3(-h.x,h.y,-h.z),
                new Vector3(h.x,-h.y,h.z), new Vector3(-h.x,-h.y,h.z), new Vector3(-h.x,h.y,h.z), new Vector3(h.x,h.y,h.z),
                new Vector3(-h.x,-h.y,h.z), new Vector3(-h.x,-h.y,-h.z), new Vector3(-h.x,h.y,-h.z), new Vector3(-h.x,h.y,h.z),
                new Vector3(h.x,-h.y,-h.z), new Vector3(h.x,-h.y,h.z), new Vector3(h.x,h.y,h.z), new Vector3(h.x,h.y,-h.z),
                new Vector3(-h.x,h.y,-h.z), new Vector3(h.x,h.y,-h.z), new Vector3(h.x,h.y,h.z), new Vector3(-h.x,h.y,h.z),
                new Vector3(-h.x,-h.y,h.z), new Vector3(h.x,-h.y,h.z), new Vector3(h.x,-h.y,-h.z), new Vector3(-h.x,-h.y,-h.z)
            };
            int[] t = new int[36];
            for (int f = 0; f < 6; f++) { int o = f * 4, k = f * 6; t[k]=o; t[k+1]=o+1; t[k+2]=o+2; t[k+3]=o; t[k+4]=o+2; t[k+5]=o+3; }
            Vector2[] uv = new Vector2[24];
            for (int f = 0; f < 6; f++) { uv[f*4]=new Vector2(0,0); uv[f*4+1]=new Vector2(1,0); uv[f*4+2]=new Vector2(1,1); uv[f*4+3]=new Vector2(0,1); }
            return CreateMeshObject(parent, name, position, rotation, v, t, uv, mat);
        }

        private static Renderer Cylinder(Transform parent, string name, Vector3 position, Vector3 scale, string mat, int sides, Quaternion rotation = default(Quaternion))
        {
            var v = new List<Vector3>(); var uv = new List<Vector2>(); var tri = new List<int>();
            for (int y = 0; y < 2; y++) for (int i = 0; i <= sides; i++)
            {
                float a = i * Mathf.PI * 2f / sides;
                v.Add(new Vector3(Mathf.Cos(a) * 0.5f, y - 0.5f, Mathf.Sin(a) * 0.5f));
                uv.Add(new Vector2((float)i / sides, y));
            }
            for (int i = 0; i < sides; i++) { int a=i, b=i+1, c=sides+2+i, d=sides+1+i; tri.AddRange(new[]{a,b,c,a,c,d}); }
            return CreateMeshObject(parent, name, position, rotation, v.ToArray(), tri.ToArray(), uv.ToArray(), mat, scale);
        }

        private static Renderer Ellipsoid(Transform parent, string name, Vector3 position, Vector3 scale, string mat)
        {
            const int rings = 10, segments = 16;
            var v = new List<Vector3>(); var uv = new List<Vector2>(); var tri = new List<int>();
            for (int y = 0; y <= rings; y++)
            {
                float phi = Mathf.PI * y / rings;
                for (int x = 0; x <= segments; x++)
                {
                    float theta = 2f * Mathf.PI * x / segments;
                    v.Add(new Vector3(Mathf.Sin(phi)*Mathf.Cos(theta)*0.5f, Mathf.Cos(phi)*0.5f, Mathf.Sin(phi)*Mathf.Sin(theta)*0.5f));
                    uv.Add(new Vector2((float)x/segments, 1f-(float)y/rings));
                }
            }
            for (int y = 0; y < rings; y++) for (int x = 0; x < segments; x++)
            {
                int a=y*(segments+1)+x, b=a+segments+1;
                tri.AddRange(new[]{a,b,a+1,a+1,b,b+1});
            }
            return CreateMeshObject(parent, name, position, Quaternion.identity, v.ToArray(), tri.ToArray(), uv.ToArray(), mat, scale);
        }

        private static void GableRoof(Transform root, float width, float depth, float eaveY, float ridgeY, string material)
        {
            float half = width * 0.5f;
            Vector3[] left = { new Vector3(-half,eaveY,-depth/2), new Vector3(0,ridgeY,-depth/2), new Vector3(0,ridgeY,depth/2), new Vector3(-half,eaveY,depth/2) };
            Vector3[] right = { new Vector3(0,ridgeY,-depth/2), new Vector3(half,eaveY,-depth/2), new Vector3(half,eaveY,depth/2), new Vector3(0,ridgeY,depth/2) };
            Quad(root, "RoofSlopeLeft", left, material);
            Quad(root, "RoofSlopeRight", right, material);
        }

        private static Renderer Quad(Transform root, string name, Vector3[] vertices, string mat)
        {
            return CreateMeshObject(root, name, Vector3.zero, Quaternion.identity, vertices,
                new[]{0,1,2,0,2,3}, new[]{Vector2.zero, Vector2.right, Vector2.one, Vector2.up}, mat);
        }

        private static Renderer CreateMeshObject(Transform parent, string name, Vector3 position, Quaternion rotation,
            Vector3[] vertices, int[] triangles, Vector2[] uv, string material, Vector3 scale = default(Vector3))
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localRotation = rotation == default(Quaternion) ? Quaternion.identity : rotation;
            if (scale == default(Vector3)) scale = Vector3.one;
            go.transform.localScale = scale;

            Mesh mesh = new Mesh();
            mesh.name = name + "_Mesh";
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            string meshPath = MeshFolder + "/" + (++meshIndex).ToString("D4") + "_" + Safe(name) + ".asset";
            AssetDatabase.CreateAsset(mesh, meshPath);

            MeshFilter filter = go.AddComponent<MeshFilter>();
            filter.sharedMesh = mesh;
            MeshRenderer renderer = go.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = Materials[material];
            return renderer;
        }

        private static string Safe(string value)
        {
            foreach (char c in Path.GetInvalidFileNameChars()) value = value.Replace(c, '_');
            return value.Replace(' ', '_');
        }

        private static void ResetGeneratedFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) AssetDatabase.DeleteAsset(path);
            EnsureFolder(path);
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path).Replace('\\', '/');
            string folder = Path.GetFileName(path);
            if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, folder);
        }
    }
}
#endif
