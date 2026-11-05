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
    /// Generates deterministic, tileable, hand-painted-style procedural texture sets and matching
    /// mobile-friendly materials. All output is local Unity PNG/Material assets; no external packages.
    /// </summary>
    public static class WorldTextureLibraryBuilder
    {
        private const string Root = "Assets/_Project/Art/Textures/Generated";
        private const string AlbedoFolder = Root + "/Albedo";
        private const string NormalFolder = Root + "/Normal";
        private const string HeightFolder = Root + "/Height";
        private const string MaterialFolder = "Assets/Resources/MiaMax/Materials";
        private const int DefaultSize = 512;

        private sealed class Surface
        {
            public string Name;
            public Color Base;
            public Color Accent;
            public float Scale;
            public float Relief;
            public float Smoothness;
            public float Metallic;
            public Surface(string name, Color baseColor, Color accent, float scale, float relief, float smoothness, float metallic)
            {
                Name = name; Base = baseColor; Accent = accent; Scale = scale; Relief = relief;
                Smoothness = smoothness; Metallic = metallic;
            }
        }

        private static readonly Surface[] Surfaces =
        {
            new Surface("Grass_Lawn", C(91,151,66), C(132,177,79), 8f, .10f, .12f, 0f),
            new Surface("Soil_Garden", C(105,69,43), C(151,105,66), 13f, .24f, .08f, 0f),
            new Surface("Wood_Warm", C(151,101,57), C(194,145,87), 5f, .18f, .22f, 0f),
            new Surface("Bark_Tree", C(105,68,43), C(157,108,70), 4f, .34f, .12f, 0f),
            new Surface("Plaster_Cream", C(220,205,169), C(239,226,196), 3f, .035f, .18f, 0f),
            new Surface("Roof_Shingle", C(76,72,68), C(111,103,93), 11f, .16f, .20f, 0f),
            new Surface("Stone_Patio", C(166,157,137), C(199,189,166), 6f, .12f, .24f, 0f),
            new Surface("Sand_Play", C(211,183,126), C(237,213,161), 14f, .08f, .12f, 0f),
            new Surface("Leaf_Green", C(70,132,55), C(116,169,72), 7f, .12f, .16f, 0f),
            new Surface("Fabric_Canvas", C(207,198,171), C(232,222,195), 22f, .045f, .28f, 0f),
            new Surface("Ceramic_Glaze", C(177,197,190), C(221,224,205), 4f, .025f, .62f, 0f),
            new Surface("Metal_Painted", C(62,73,75), C(111,125,124), 18f, .055f, .48f, .28f),
            new Surface("Paving_Brick", C(171,119,87), C(205,157,117), 5f, .12f, .18f, 0f),
            new Surface("Water_Pond", C(78,151,166), C(145,203,201), 9f, .035f, .78f, 0f),
            new Surface("Flower_Petal", C(232,143,157), C(255,206,183), 6f, .035f, .34f, 0f),
            new Surface("Vegetable_Green", C(89,151,66), C(164,190,88), 8f, .10f, .20f, 0f),
            new Surface("Vegetable_Red", C(188,66,48), C(235,133,71), 8f, .07f, .28f, 0f),
            new Surface("Vegetable_Orange", C(225,132,43), C(250,190,77), 8f, .07f, .28f, 0f),
            new Surface("Vegetable_Purple", C(116,76,132), C(177,126,169), 8f, .08f, .26f, 0f),
            new Surface("Path_Gravel", C(151,135,111), C(194,177,147), 17f, .22f, .10f, 0f),
            new Surface("Fence_WhitePaint", C(226,224,208), C(250,247,230), 6f, .045f, .28f, 0f),
            new Surface("Leather_Brown", C(119,75,47), C(160,111,72), 10f, .09f, .32f, 0f),
            new Surface("Fur_Ginger", C(202,112,49), C(244,169,83), 19f, .12f, .20f, 0f),
            new Surface("Fur_Cream", C(225,211,178), C(248,239,211), 19f, .08f, .22f, 0f),
            new Surface("Skin_Warm", C(225,174,139), C(248,210,177), 12f, .035f, .24f, 0f),
            new Surface("Hair_Blonde", C(206,157,69), C(248,215,119), 7f, .13f, .28f, 0f),
            new Surface("Denim_Blue", C(54,91,143), C(91,133,184), 18f, .12f, .18f, 0f),
            new Surface("Cotton_White", C(224,222,210), C(250,247,235), 23f, .045f, .20f, 0f),
            new Surface("Eye_Green", C(57,139,92), C(143,194,112), 5f, .015f, .72f, 0f),
            new Surface("Plastic_Toy", C(218,82,59), C(250,151,79), 5f, .025f, .48f, 0f),
            new Surface("Paper_Illustration", C(225,211,174), C(247,235,207), 9f, .055f, .08f, 0f),
            new Surface("Interior_Wallpaper", C(205,190,162), C(232,218,189), 4f, .025f, .18f, 0f),
            new Surface("Floor_Wood", C(137,91,53), C(183,133,82), 4f, .13f, .24f, 0f),
            new Surface("Floor_Tile", C(190,184,165), C(224,217,196), 6f, .07f, .30f, 0f),
            new Surface("Wicker_Rattan", C(167,124,75), C(213,172,112), 12f, .18f, .22f, 0f),
            new Surface("Rubber_Soft", C(57,61,62), C(91,96,93), 16f, .055f, .12f, 0f),
            new Surface("Stone_Mossy", C(108,121,91), C(157,163,119), 7f, .23f, .10f, 0f),
            new Surface("Mushroom_Cap", C(179,82,61), C(231,158,109), 5f, .08f, .26f, 0f),
            new Surface("Berry_Skin", C(132,43,72), C(194,77,106), 7f, .045f, .38f, 0f),
            new Surface("Pond_Reeds", C(95,128,63), C(153,171,88), 9f, .12f, .16f, 0f)
        };

        [MenuItem("Tools/Mia & Max/Generate Complete Texture Library")]
        public static void Generate()
        {
            EnsureFolder("Assets/_Project/Art");
            EnsureFolder("Assets/_Project/Art/Textures");
            EnsureFolder(Root);
            EnsureFolder(AlbedoFolder);
            EnsureFolder(NormalFolder);
            EnsureFolder(HeightFolder);
            EnsureFolder("Assets/Resources");
            EnsureFolder("Assets/Resources/MiaMax");
            EnsureFolder(MaterialFolder);

            int created = 0;
            foreach (Surface surface in Surfaces)
            {
                Texture2D height = BuildHeight(surface, DefaultSize);
                Texture2D albedo = BuildAlbedo(surface, height);
                Texture2D normal = BuildNormal(height, surface.Relief);
                WriteTexture(albedo, AlbedoFolder + "/" + surface.Name + "_Albedo.png", false);
                WriteTexture(normal, NormalFolder + "/" + surface.Name + "_Normal.png", true);
                WriteTexture(height, HeightFolder + "/" + surface.Name + "_Height.png", true);
                CreateMaterial(surface);
                UnityEngine.Object.DestroyImmediate(albedo);
                UnityEngine.Object.DestroyImmediate(normal);
                UnityEngine.Object.DestroyImmediate(height);
                created++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Mia & Max texture library generated: " + created + " surfaces, " +
                (created * 3) + " PNG maps and " + created + " materials. Review the generated assets before production use.");
            EditorUtility.DisplayDialog("Mia & Max Texture Library",
                "Generated " + created + " stylized surfaces.\n\n" +
                "Albedo, normal and height PNGs: " + (created * 3) + "\n" +
                "Matching materials: " + created + "\n\n" +
                "Output: " + Root + "\n" +
                "Character maps are generic surface starters only; final Mia/Max maps must be authored for the production models' UV layouts.",
                "OK");
        }

        [MenuItem("Tools/Mia & Max/Audit Generated Texture Library")]
        public static void Audit()
        {
            int complete = 0;
            var report = new System.Text.StringBuilder();
            report.AppendLine("MIA & MAX — GENERATED TEXTURE LIBRARY AUDIT");
            foreach (Surface s in Surfaces)
            {
                bool a = File.Exists(AlbedoFolder + "/" + s.Name + "_Albedo.png");
                bool n = File.Exists(NormalFolder + "/" + s.Name + "_Normal.png");
                bool h = File.Exists(HeightFolder + "/" + s.Name + "_Height.png");
                bool m = AssetDatabase.LoadAssetAtPath<Material>(MaterialFolder + "/" + s.Name + ".mat") != null;
                bool ok = a && n && h && m;
                if (ok) complete++;
                report.AppendLine((ok ? "[READY] " : "[PENDING] ") + s.Name +
                    " — Albedo " + a + ", Normal " + n + ", Height " + h + ", Material " + m);
            }
            report.AppendLine("Complete surfaces: " + complete + " / " + Surfaces.Length);
            report.AppendLine("Character UV-specific maps and imported model assignments require production meshes.");
            Debug.Log(report.ToString());
            EditorUtility.DisplayDialog("Texture Library Audit",
                "Complete surfaces: " + complete + " / " + Surfaces.Length +
                "\n\nFull report is in the Console.", "OK");
        }

        private static Texture2D BuildHeight(Surface s, int size)
        {
            var tex = NewTexture(size);
            var pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float u = (float)x / size, v = (float)y / size;
                float n = TileNoise(u, v, s.Scale, 137 + StableHash(s.Name));
                float fine = TileNoise(u, v, s.Scale * 3f, 911 + StableHash(s.Name));
                float pattern = Pattern(s.Name, u, v);
                float h = Mathf.Clamp01(.48f + (n - .5f) * .62f + (fine - .5f) * .18f + pattern);
                pixels[y * size + x] = new Color(h, h, h, 1f);
            }
            tex.SetPixels(pixels); tex.Apply(false, false); return tex;
        }

        private static Texture2D BuildAlbedo(Surface s, Texture2D height)
        {
            int size = height.width;
            var tex = NewTexture(size);
            var src = height.GetPixels();
            var dst = new Color[src.Length];
            for (int i = 0; i < src.Length; i++)
            {
                float h = src[i].r;
                float shade = .78f + h * .42f;
                float variation = Mathf.Lerp(.91f, 1.10f, h);
                Color c = Color.Lerp(s.Base, s.Accent, Mathf.Clamp01((h - .28f) * 1.5f));
                c *= shade * variation;
                dst[i] = new Color(Mathf.Clamp01(c.r), Mathf.Clamp01(c.g), Mathf.Clamp01(c.b), 1f);
            }
            tex.SetPixels(dst); tex.Apply(false, false); return tex;
        }

        private static Texture2D BuildNormal(Texture2D height, float strength)
        {
            int size = height.width;
            var tex = NewTexture(size);
            var src = height.GetPixels();
            var dst = new Color[src.Length];
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                int xm = (x + size - 1) % size, xp = (x + 1) % size;
                int ym = (y + size - 1) % size, yp = (y + 1) % size;
                float dx = (src[y * size + xp].r - src[y * size + xm].r) * strength * 3.5f;
                float dy = (src[yp * size + x].r - src[ym * size + x].r) * strength * 3.5f;
                Vector3 n = new Vector3(-dx, -dy, 1f).normalized;
                dst[y * size + x] = new Color(n.x * .5f + .5f, n.y * .5f + .5f, n.z * .5f + .5f, 1f);
            }
            tex.SetPixels(dst); tex.Apply(false, false); return tex;
        }

        private static float Pattern(string name, float u, float v)
        {
            if (name.Contains("Wood") || name.Contains("Bark"))
            {
                float grain = Mathf.Sin((v * 22f + Mathf.Sin(u * 9f) * .12f) * Mathf.PI * 2f);
                return grain * .055f;
            }
            if (name.Contains("Roof") || name.Contains("Paving") || name.Contains("Stone"))
            {
                float line = Mathf.Abs(Mathf.Sin(v * 18f * Mathf.PI));
                return (line < .10f ? -.16f : 0f);
            }
            if (name.Contains("Fabric"))
                return (Mathf.Sin(u * 160f * Mathf.PI) * Mathf.Sin(v * 160f * Mathf.PI)) * .025f;
            if (name.Contains("Grass") || name.Contains("Leaf") || name.Contains("Vegetable") || name.Contains("Fur"))
                return Mathf.Sin((u * 31f + v * 13f) * Mathf.PI * 2f) * .025f;
            if (name.Contains("Metal"))
                return Mathf.Sin(v * 190f * Mathf.PI) * .018f;
            return 0f;
        }

        // Periodic value noise: integer lattice coordinates wrap, so opposite texture edges match.
        private static float TileNoise(float u, float v, float frequency, int seed)
        {
            float x = u * frequency, y = v * frequency;
            int x0 = Mathf.FloorToInt(x), y0 = Mathf.FloorToInt(y);
            float tx = Smooth(x - x0), ty = Smooth(y - y0);
            float a = Hash01(x0, y0, frequency, seed);
            float b = Hash01(x0 + 1, y0, frequency, seed);
            float c = Hash01(x0, y0 + 1, frequency, seed);
            float d = Hash01(x0 + 1, y0 + 1, frequency, seed);
            return Mathf.Lerp(Mathf.Lerp(a, b, tx), Mathf.Lerp(c, d, tx), ty);
        }

        private static float Hash01(int x, int y, float frequency, int seed)
        {
            int period = Mathf.Max(1, Mathf.RoundToInt(frequency));
            x = Mod(x, period); y = Mod(y, period);
            unchecked
            {
                int h = x * 374761393 + y * 668265263 + seed * 1442695041;
                h = (h ^ (h >> 13)) * 1274126177;
                h ^= h >> 16;
                return (h & 0x7fffffff) / 2147483647f;
            }
        }

        private static int StableHash(string value)
        {
            unchecked
            {
                int hash = 23;
                for (int i = 0; i < value.Length; i++) hash = hash * 31 + value[i];
                return hash;
            }
        }

        private static int Mod(int x, int m) { int r = x % m; return r < 0 ? r + m : r; }
        private static float Smooth(float t) { return t * t * (3f - 2f * t); }

        private static void CreateMaterial(Surface s)
        {
            string path = MaterialFolder + "/" + s.Name + ".mat";
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null)
            {
                Debug.LogError("Mia & Max texture generator: no supported Lit shader is available. Enable URP/Lit or the Standard shader and retry.");
                return;
            }
            if (mat == null) mat = new Material(shader);
            else if (mat.shader != shader) mat.shader = shader;

            Texture2D albedo = AssetDatabase.LoadAssetAtPath<Texture2D>(AlbedoFolder + "/" + s.Name + "_Albedo.png");
            Texture2D normal = AssetDatabase.LoadAssetAtPath<Texture2D>(NormalFolder + "/" + s.Name + "_Normal.png");
            if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", albedo);
            if (mat.HasProperty("_MainTex")) mat.SetTexture("_MainTex", albedo);
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", Color.white);
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", Color.white);
            if (mat.HasProperty("_BumpMap"))
            {
                mat.SetTexture("_BumpMap", normal);
                if (normal != null) mat.EnableKeyword("_NORMALMAP");
            }
            if (mat.HasProperty("_BumpScale")) mat.SetFloat("_BumpScale", .65f);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", s.Smoothness);
            if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", s.Metallic);
            mat.name = s.Name;
            if (AssetDatabase.LoadAssetAtPath<Material>(path) == null)
                AssetDatabase.CreateAsset(mat, path);
            else EditorUtility.SetDirty(mat);
        }

        private static void WriteTexture(Texture2D texture, string path, bool linear)
        {
            File.WriteAllBytes(path, texture.EncodeToPNG());
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return;
            importer.textureType = path.Contains("_Normal") ? TextureImporterType.NormalMap : TextureImporterType.Default;
            importer.sRGBTexture = !linear && !path.Contains("_Normal");
            importer.mipmapEnabled = true;
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.filterMode = FilterMode.Bilinear;
            importer.maxTextureSize = DefaultSize;
            importer.textureCompression = TextureImporterCompression.Compressed;
            importer.SaveAndReimport();
        }

        private static Texture2D NewTexture(int size)
        {
            return new Texture2D(size, size, TextureFormat.RGBA32, false, true)
            { wrapMode = TextureWrapMode.Repeat, filterMode = FilterMode.Bilinear, name = "GeneratedTexture" };
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path).Replace("\\", "/");
            string name = Path.GetFileName(path);
            if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }

        private static Color C(int r, int g, int b) { return new Color(r / 255f, g / 255f, b / 255f, 1f); }
    }
}
#endif
