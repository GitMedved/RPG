#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using MiaMax.Gameplay;
using UnityEditor;
using UnityEngine;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Creates clearly marked primitive blockout prefab assets for the backyard.
    /// These are structural placeholders, not approved production art.
    /// </summary>
    public static class BackyardPlaceholderPrefabGenerator
    {
        private const string PrefabFolder = "Assets/_Project/Art/Environment/Runtime/Prefabs";
        private const string MaterialFolder = "Assets/_Project/Art/Environment/Runtime/Materials";

        private sealed class Palette
        {
            public Material Cream, Roof, White, Wood, DarkWood, Grass, Leaf, Soil, Sand, Stone, Blue, Pink, Yellow;
        }

        [MenuItem("Tools/Mia & Max/Backyard/Create Placeholder Prefab Set (15)")]
        public static void CreatePlaceholderSet()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Mia & Max", "Exit Play Mode before generating prefab assets.", "OK");
                return;
            }

            EnsureFolder("Assets/_Project/Art/Environment/Runtime");
            EnsureFolder(PrefabFolder);
            EnsureFolder(MaterialFolder);
            Palette palette = CreatePalette();

            string[] names =
            {
                "HouseExterior", "PatioDeck", "FenceStraight", "FenceCorner", "FenceGate",
                "MainTree", "SwingSet", "Sandbox", "ShrubCluster", "FlowerBed",
                "FlowerPot", "GardenPlot", "Workshop", "AdventureGate", "PathSegment"
            };

            int created = 0;
            int preserved = 0;
            var errors = new List<string>();

            for (int i = 0; i < names.Length; i++)
            {
                string path = PrefabFolder + "/" + names[i] + ".prefab";
                if (File.Exists(path))
                {
                    preserved++;
                    continue;
                }

                GameObject root = new GameObject(names[i] + "_PLACEHOLDER");
                try
                {
                    root.AddComponent<BackyardPlaceholderAssetMarker>().replacementNotes =
                        "Generated primitive blockout for " + names[i] +
                        ". Replace with approved production mesh, UVs, textures and reviewed materials. " +
                        "Gameplay logic must remain on separate stable gameplay roots.";
                    Build(names[i], root.transform, palette);
                    bool success;
                    PrefabUtility.SaveAsPrefabAsset(root, path, out success);
                    if (success) created++;
                    else errors.Add("Unity did not confirm prefab save: " + path);
                }
                catch (Exception exception)
                {
                    errors.Add(path + ": " + exception.Message);
                }
                finally
                {
                    UnityEngine.Object.DestroyImmediate(root);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            string report = "[Mia & Max] Backyard placeholder prefab generation\n" +
                "Created: " + created + " / 15\n" +
                "Existing assets preserved: " + preserved + "\n" +
                "Errors: " + errors.Count + "\n" +
                (errors.Count == 0 ? "No generation errors." : string.Join("\n", errors.ToArray())) +
                "\n\nIMPORTANT: all generated assets are primitive placeholders, not production-ready art.";
            if (errors.Count > 0) Debug.LogError(report);
            else Debug.LogWarning(report);

            EditorUtility.DisplayDialog("Mia & Max — Placeholder Prefabs",
                "Created: " + created + "\nPreserved existing: " + preserved +
                "\nErrors: " + errors.Count +
                "\n\nThese are explicitly marked temporary placeholders. See Console for details.",
                "OK");
        }

        private static void Build(string name, Transform root, Palette p)
        {
            switch (name)
            {
                case "HouseExterior":
                    Part(PrimitiveType.Cube, "Lower_Wall", root, new Vector3(0, 1.5f, 0), new Vector3(8, 3, 5), p.Cream);
                    Part(PrimitiveType.Cube, "Upper_Wall", root, new Vector3(0, 4.2f, 0), new Vector3(7.4f, 2.4f, 4.7f), p.Cream);
                    Part(PrimitiveType.Cube, "Roof", root, new Vector3(0, 5.65f, 0), new Vector3(8.8f, 0.35f, 5.8f), p.Roof);
                    Part(PrimitiveType.Cube, "Front_Door", root, new Vector3(0, 1.05f, -2.54f), new Vector3(0.95f, 2.1f, 0.12f), p.Wood);
                    for (int x = -2; x <= 2; x += 4)
                    {
                        Part(PrimitiveType.Cube, "Ground_Floor_Window", root, new Vector3(x, 1.9f, -2.58f), new Vector3(1.15f, 0.9f, 0.1f), p.Blue);
                        Part(PrimitiveType.Cube, "Upper_Floor_Window", root, new Vector3(x, 4.25f, -2.4f), new Vector3(1.0f, 0.85f, 0.1f), p.Blue);
                    }
                    break;
                case "PatioDeck":
                    Part(PrimitiveType.Cube, "Deck", root, new Vector3(0, 0.08f, 0), new Vector3(6, 0.16f, 4), p.Wood);
                    for (int i = -2; i <= 2; i++)
                        Part(PrimitiveType.Cube, "Deck_Plank", root, new Vector3(i * 1.15f, 0.17f, 0), new Vector3(0.035f, 0.025f, 3.9f), p.DarkWood);
                    Part(PrimitiveType.Cube, "Table_Top", root, new Vector3(0, 0.95f, 0), new Vector3(1.8f, 0.14f, 1.1f), p.Cream);
                    for (int x = -1; x <= 1; x += 2)
                        for (int z = -1; z <= 1; z += 2)
                            Part(PrimitiveType.Cylinder, "Table_Leg", root, new Vector3(x * 0.65f, 0.5f, z * 0.35f), new Vector3(0.09f, 0.45f, 0.09f), p.DarkWood);
                    break;
                case "FenceStraight":
                    FencePanel(root, p, false);
                    break;
                case "FenceCorner":
                    FencePanel(root, p, false);
                    FencePanel(root, p, true);
                    break;
                case "FenceGate":
                    FencePanel(root, p, false);
                    Part(PrimitiveType.Cube, "Gate_Door", root, new Vector3(0, 0.65f, -0.15f), new Vector3(1.3f, 1.3f, 0.12f), p.Wood);
                    break;
                case "MainTree":
                    Part(PrimitiveType.Cylinder, "Trunk", root, new Vector3(0, 1.7f, 0), new Vector3(0.65f, 1.7f, 0.65f), p.Wood);
                    Part(PrimitiveType.Sphere, "Canopy_Main", root, new Vector3(0, 4.3f, 0), new Vector3(3.7f, 3.5f, 3.7f), p.Leaf);
                    Part(PrimitiveType.Sphere, "Canopy_Left", root, new Vector3(-1.25f, 3.8f, 0.2f), new Vector3(2.3f, 2.4f, 2.4f), p.Leaf);
                    Part(PrimitiveType.Sphere, "Canopy_Right", root, new Vector3(1.25f, 3.9f, -0.2f), new Vector3(2.3f, 2.5f, 2.3f), p.Leaf);
                    break;
                case "SwingSet":
                    for (int x = -1; x <= 1; x += 2)
                    {
                        Part(PrimitiveType.Cube, "A_Frame_Leg", root, new Vector3(x * 1.2f, 1.65f, 0), new Vector3(0.13f, 3.3f, 0.13f), p.Wood).transform.localRotation = Quaternion.Euler(0, 0, x * -18);
                        Part(PrimitiveType.Cube, "Swing_Rope", root, new Vector3(x * 0.55f, 1.75f, 0), new Vector3(0.045f, 2.2f, 0.045f), p.White);
                        Part(PrimitiveType.Cube, "Swing_Seat", root, new Vector3(x * 0.55f, 0.62f, 0), new Vector3(0.85f, 0.13f, 0.48f), p.Blue);
                    }
                    Part(PrimitiveType.Cube, "Top_Beam", root, new Vector3(0, 3.2f, 0), new Vector3(3.4f, 0.16f, 0.16f), p.Wood);
                    break;
                case "Sandbox":
                    Part(PrimitiveType.Cube, "Wooden_Bed", root, new Vector3(0, 0.28f, 0), new Vector3(3.8f, 0.55f, 2.8f), p.Wood);
                    Part(PrimitiveType.Cube, "Sand_Surface", root, new Vector3(0, 0.58f, 0), new Vector3(3.45f, 0.08f, 2.45f), p.Sand);
                    break;
                case "ShrubCluster":
                    for (int i = 0; i < 5; i++)
                        Part(PrimitiveType.Sphere, "Shrub_" + i, root, new Vector3((i - 2) * 0.75f, 0.55f + (i % 2) * 0.12f, (i % 2) * 0.3f), Vector3.one * 1.1f, p.Leaf);
                    break;
                case "FlowerBed":
                    Part(PrimitiveType.Cube, "Soil_Bed", root, new Vector3(0, 0.18f, 0), new Vector3(3.2f, 0.35f, 1.3f), p.Wood);
                    for (int i = 0; i < 5; i++) Flower(root, p, new Vector3((i - 2) * 0.55f, 0.65f, 0));
                    break;
                case "FlowerPot":
                    Part(PrimitiveType.Cylinder, "Pot", root, new Vector3(0, 0.3f, 0), new Vector3(0.55f, 0.3f, 0.55f), p.Pink);
                    Part(PrimitiveType.Cylinder, "Stem", root, new Vector3(0, 0.95f, 0), new Vector3(0.06f, 0.45f, 0.06f), p.Leaf);
                    Flower(root, p, new Vector3(0, 1.3f, 0));
                    break;
                case "GardenPlot":
                    Part(PrimitiveType.Cube, "Raised_Soil_Plot", root, new Vector3(0, 0.16f, 0), new Vector3(1.8f, 0.3f, 1.5f), p.Soil);
                    for (int i = -1; i <= 1; i++)
                        Part(PrimitiveType.Cube, "Soil_Ridge", root, new Vector3(i * 0.48f, 0.34f, 0), new Vector3(0.12f, 0.08f, 1.25f), p.DarkWood);
                    break;
                case "Workshop":
                    Part(PrimitiveType.Cube, "Workbench", root, new Vector3(0, 0.85f, 0), new Vector3(2.2f, 0.18f, 1.0f), p.Wood);
                    for (int x = -1; x <= 1; x += 2)
                        for (int z = -1; z <= 1; z += 2)
                            Part(PrimitiveType.Cube, "Workbench_Leg", root, new Vector3(x * 0.9f, 0.4f, z * 0.35f), new Vector3(0.12f, 0.8f, 0.12f), p.DarkWood);
                    Part(PrimitiveType.Cube, "Tool_Board", root, new Vector3(0, 1.65f, 0.45f), new Vector3(1.7f, 1.1f, 0.12f), p.Cream);
                    break;
                case "AdventureGate":
                    Part(PrimitiveType.Cube, "Left_Post", root, new Vector3(-1.2f, 1.4f, 0), new Vector3(0.22f, 2.8f, 0.25f), p.Wood);
                    Part(PrimitiveType.Cube, "Right_Post", root, new Vector3(1.2f, 1.4f, 0), new Vector3(0.22f, 2.8f, 0.25f), p.Wood);
                    Part(PrimitiveType.Cube, "Header", root, new Vector3(0, 2.65f, 0), new Vector3(2.7f, 0.28f, 0.35f), p.DarkWood);
                    Part(PrimitiveType.Cube, "Gate_Panel", root, new Vector3(0, 0.7f, 0), new Vector3(1.9f, 1.4f, 0.12f), p.Wood);
                    break;
                case "PathSegment":
                    Part(PrimitiveType.Cube, "Gravel_Path", root, new Vector3(0, 0.04f, 0), new Vector3(2.4f, 0.08f, 3.5f), p.Stone);
                    break;
            }
        }

        private static void FencePanel(Transform root, Palette p, bool rotate)
        {
            for (int i = 0; i < 7; i++)
            {
                GameObject picket = Part(PrimitiveType.Cube, "Picket", root,
                    new Vector3(-1.5f + i * 0.5f, 0.8f, 0), new Vector3(0.12f, 1.6f, 0.13f), p.White);
                if (rotate) picket.transform.localRotation = Quaternion.Euler(0, 90, 0);
            }
            Part(PrimitiveType.Cube, "Upper_Rail", root, new Vector3(0, 1.3f, 0), new Vector3(3.2f, 0.12f, 0.16f), p.White);
            Part(PrimitiveType.Cube, "Lower_Rail", root, new Vector3(0, 0.45f, 0), new Vector3(3.2f, 0.12f, 0.16f), p.White);
        }

        private static void Flower(Transform root, Palette p, Vector3 position)
        {
            Part(PrimitiveType.Cylinder, "Flower_Stem", root, position + Vector3.down * 0.2f, new Vector3(0.045f, 0.25f, 0.045f), p.Leaf);
            Part(PrimitiveType.Sphere, "Flower_Center", root, position + Vector3.up * 0.12f, Vector3.one * 0.2f, p.Yellow);
            for (int i = 0; i < 5; i++)
            {
                float angle = i * 72f * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(Mathf.Cos(angle), 0.12f, Mathf.Sin(angle)) * 0.23f;
                Part(PrimitiveType.Sphere, "Petal", root, position + offset, new Vector3(0.2f, 0.12f, 0.2f), p.Pink);
            }
        }

        private static GameObject Part(PrimitiveType type, string name, Transform parent,
            Vector3 localPosition, Vector3 localScale, Material material)
        {
            GameObject part = GameObject.CreatePrimitive(type);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localScale = localScale;
            Collider collider = part.GetComponent<Collider>();
            if (collider != null) UnityEngine.Object.DestroyImmediate(collider);
            Renderer renderer = part.GetComponent<Renderer>();
            if (renderer != null) renderer.sharedMaterial = material;
            return part;
        }

        private static Palette CreatePalette()
        {
            return new Palette
            {
                Cream = GetMaterial("Placeholder_Cream", new Color(0.78f, 0.69f, 0.54f)),
                Roof = GetMaterial("Placeholder_Roof", new Color(0.22f, 0.2f, 0.18f)),
                White = GetMaterial("Placeholder_White", new Color(0.91f, 0.89f, 0.82f)),
                Wood = GetMaterial("Placeholder_Wood", new Color(0.48f, 0.29f, 0.14f)),
                DarkWood = GetMaterial("Placeholder_DarkWood", new Color(0.27f, 0.16f, 0.08f)),
                Grass = GetMaterial("Placeholder_Grass", new Color(0.22f, 0.48f, 0.19f)),
                Leaf = GetMaterial("Placeholder_Leaf", new Color(0.16f, 0.39f, 0.12f)),
                Soil = GetMaterial("Placeholder_Soil", new Color(0.32f, 0.22f, 0.13f)),
                Sand = GetMaterial("Placeholder_Sand", new Color(0.78f, 0.66f, 0.43f)),
                Stone = GetMaterial("Placeholder_Stone", new Color(0.68f, 0.6f, 0.48f)),
                Blue = GetMaterial("Placeholder_Blue", new Color(0.18f, 0.4f, 0.72f)),
                Pink = GetMaterial("Placeholder_Pink", new Color(0.86f, 0.38f, 0.56f)),
                Yellow = GetMaterial("Placeholder_Yellow", new Color(0.96f, 0.76f, 0.16f))
            };
        }

        private static Material GetMaterial(string name, Color color)
        {
            string path = MaterialFolder + "/" + name + ".mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material != null) return material;
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            material = new Material(shader) { name = name, color = color };
            AssetDatabase.CreateAsset(material, path);
            return material;
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