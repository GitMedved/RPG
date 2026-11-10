#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MiaMax.EditorTools
{
    /// <summary>Creates a small original vector-style UI sprite set for the mobile HUD.</summary>
    public static class ProceduralUiArtGenerator
    {
        private const string Output = "Assets/Resources/MiaMax/UI";
        private const int Size = 256;

        [MenuItem("Tools/Mia & Max/UI/Generate Original UI Sprite Pack")]
        public static void GenerateAll()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Mia & Max UI", "Exit Play Mode before generating UI sprites.", "OK");
                return;
            }

            EnsureFolder("Assets/Resources");
            EnsureFolder("Assets/Resources/MiaMax");
            EnsureFolder(Output);

            int generated = 0;
            int errors = 0;
            var report = new System.Text.StringBuilder();
            report.AppendLine("MIA & MAX — ORIGINAL UI SPRITE GENERATION");

            TryCreate("rounded_panel", DrawRoundedPanel, 42f, ref generated, ref errors, report);
            TryCreate("rounded_button", DrawRoundedButton, 44f, ref generated, ref errors, report);
            TryCreate("badge", DrawBadge, 0f, ref generated, ref errors, report);
            TryCreate("icon_coin", DrawCoin, 0f, ref generated, ref errors, report);
            TryCreate("icon_seed", DrawSeed, 0f, ref generated, ref errors, report);
            TryCreate("icon_harvest", DrawHarvest, 0f, ref generated, ref errors, report);
            TryCreate("icon_star", DrawStar, 0f, ref generated, ref errors, report);
            TryCreate("icon_home", DrawHome, 0f, ref generated, ref errors, report);
            TryCreate("icon_journal", DrawJournal, 0f, ref generated, ref errors, report);
            TryCreate("icon_map", DrawMap, 0f, ref generated, ref errors, report);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            report.AppendLine();
            report.AppendLine("Generated: " + generated + " / 10");
            report.AppendLine("Errors: " + errors);
            report.AppendLine("Sprites are original editable source PNGs with single-sprite import settings.");
            report.AppendLine("They are a first cohesive UI art pass and still require visual/device review.");
            Debug.Log(report.ToString());
            EditorUtility.DisplayDialog("Mia & Max UI Sprite Pack",
                "Generated: " + generated + " / 10\nErrors: " + errors +
                "\n\nFull report is in Console.", "OK");
        }

        private static void TryCreate(string id, Action<Color[]> draw, float border,
            ref int generated, ref int errors, System.Text.StringBuilder report)
        {
            string path = Output + "/" + id + ".png";
            try
            {
                Color[] pixels = new Color[Size * Size];
                draw(pixels);
                Texture2D texture = new Texture2D(Size, Size, TextureFormat.RGBA32, false, true);
                texture.name = id;
                texture.SetPixels(pixels);
                texture.Apply(false, false);
                File.WriteAllBytes(path, texture.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(texture);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);

                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) throw new InvalidOperationException("Texture importer was not created.");
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.filterMode = FilterMode.Bilinear;
                importer.textureCompression = TextureImporterCompression.Compressed;
                importer.spritePixelsPerUnit = 100f;
                importer.spriteBorder = new Vector4(border, border, border, border);
                importer.SaveAndReimport();

                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (sprite == null) throw new InvalidOperationException("Unity did not import the PNG as a Sprite.");
                generated++;
                report.AppendLine("[CREATED] " + id + " — " + Size + "x" + Size + " sprite");
            }
            catch (Exception exception)
            {
                errors++;
                report.AppendLine("[ERROR] " + id + " — " + exception.Message);
            }
        }

        private static void DrawRoundedPanel(Color[] pixels)
        {
            RoundedRect(pixels, 20f, 236f, 20f, 236f, 25f, 4f, new Color(1f, 1f, 1f, 1f));
        }

        private static void DrawRoundedButton(Color[] pixels)
        {
            RoundedRect(pixels, 12f, 244f, 30f, 226f, 52f, 5f, Color.white);
            DrawLine(pixels, new Vector2(48f, 213f), new Vector2(208f, 213f), 2f,
                new Color(1f, 1f, 1f, 0.45f));
        }

        private static void DrawBadge(Color[] pixels)
        {
            DrawCircle(pixels, new Vector2(128f, 128f), 104f, 7f, Color.white);
            DrawCircle(pixels, new Vector2(128f, 128f), 86f, 0f, new Color(1f, 1f, 1f, 0.25f));
        }

        private static void DrawCoin(Color[] pixels)
        {
            DrawCircle(pixels, new Vector2(128f, 128f), 88f, 12f, Color.white);
            DrawCircle(pixels, new Vector2(128f, 128f), 50f, 7f, Color.white);
            DrawLine(pixels, new Vector2(128f, 78f), new Vector2(128f, 178f), 7f, Color.white);
        }

        private static void DrawSeed(Color[] pixels)
        {
            DrawEllipse(pixels, new Vector2(128f, 128f), 47f, 82f, -34f, Color.white);
            DrawLine(pixels, new Vector2(93f, 90f), new Vector2(164f, 173f), 7f, Color.white);
            DrawLine(pixels, new Vector2(130f, 132f), new Vector2(172f, 116f), 5f, Color.white);
        }

        private static void DrawHarvest(Color[] pixels)
        {
            DrawLine(pixels, new Vector2(92f, 54f), new Vector2(164f, 205f), 9f, Color.white);
            DrawEllipse(pixels, new Vector2(102f, 112f), 34f, 15f, -28f, Color.white);
            DrawEllipse(pixels, new Vector2(143f, 145f), 34f, 15f, 28f, Color.white);
            DrawEllipse(pixels, new Vector2(151f, 194f), 12f, 24f, 20f, Color.white);
            DrawEllipse(pixels, new Vector2(178f, 176f), 12f, 24f, 55f, Color.white);
            DrawEllipse(pixels, new Vector2(128f, 221f), 12f, 24f, 0f, Color.white);
        }

        private static void DrawStar(Color[] pixels)
        {
            Vector2[] points = new Vector2[10];
            for (int i = 0; i < 10; i++)
            {
                float angle = -Mathf.PI * 0.5f + i * Mathf.PI / 5f;
                float radius = i % 2 == 0 ? 104f : 46f;
                points[i] = new Vector2(128f + Mathf.Cos(angle) * radius, 128f + Mathf.Sin(angle) * radius);
            }
            FillPolygon(pixels, points, Color.white);
        }

        private static void DrawHome(Color[] pixels)
        {
            FillPolygon(pixels, new[]
            {
                new Vector2(30f, 126f), new Vector2(128f, 218f), new Vector2(226f, 126f),
                new Vector2(204f, 105f), new Vector2(128f, 177f), new Vector2(52f, 105f)
            }, Color.white);
            RoundedRect(pixels, 62f, 194f, 30f, 143f, 12f, 0f, Color.white);
            RoundedRect(pixels, 112f, 151f, 30f, 95f, 9f, 0f, new Color(0f, 0f, 0f, 0f));
            DrawLine(pixels, new Vector2(128f, 36f), new Vector2(128f, 95f), 3f, new Color(0f, 0f, 0f, 0f));
        }

        private static void DrawJournal(Color[] pixels)
        {
            RoundedRect(pixels, 45f, 211f, 28f, 228f, 16f, 0f, Color.white);
            DrawLine(pixels, new Vector2(91f, 196f), new Vector2(91f, 58f), 7f, new Color(0f, 0f, 0f, 0.32f));
            DrawLine(pixels, new Vector2(112f, 176f), new Vector2(184f, 176f), 6f, new Color(0f, 0f, 0f, 0.32f));
            DrawLine(pixels, new Vector2(112f, 143f), new Vector2(184f, 143f), 6f, new Color(0f, 0f, 0f, 0.32f));
            DrawLine(pixels, new Vector2(112f, 110f), new Vector2(184f, 110f), 6f, new Color(0f, 0f, 0f, 0.32f));
        }

        private static void DrawMap(Color[] pixels)
        {
            FillPolygon(pixels, new[]
            {
                new Vector2(24f, 70f), new Vector2(94f, 96f), new Vector2(164f, 66f),
                new Vector2(232f, 94f), new Vector2(232f, 204f), new Vector2(164f, 178f),
                new Vector2(94f, 208f), new Vector2(24f, 182f)
            }, Color.white);
            DrawLine(pixels, new Vector2(94f, 96f), new Vector2(94f, 208f), 5f, new Color(0f, 0f, 0f, 0.30f));
            DrawLine(pixels, new Vector2(164f, 66f), new Vector2(164f, 178f), 5f, new Color(0f, 0f, 0f, 0.30f));
            DrawCircle(pixels, new Vector2(128f, 134f), 16f, 0f, new Color(0f, 0f, 0f, 0.28f));
        }

        private static void RoundedRect(Color[] pixels, float left, float right, float bottom, float top,
            float radius, float border, Color fill)
        {
            for (int y = 0; y < Size; y++)
            for (int x = 0; x < Size; x++)
            {
                float qx = Mathf.Abs(x + 0.5f - (left + right) * 0.5f) - ((right - left) * 0.5f - radius);
                float qy = Mathf.Abs(y + 0.5f - (bottom + top) * 0.5f) - ((top - bottom) * 0.5f - radius);
                float distance = new Vector2(Mathf.Max(qx, 0f), Mathf.Max(qy, 0f)).magnitude +
                    Mathf.Min(Mathf.Max(qx, qy), 0f) - radius;
                float coverage = Mathf.Clamp01(0.5f - distance);
                if (coverage <= 0f) continue;
                Color color = fill;
                if (border > 0f && distance > -border)
                    color = Color.Lerp(new Color(0.83f, 0.84f, 0.80f, 1f), fill, Mathf.Clamp01((-distance) / border));
                Blend(pixels, x, y, color, coverage);
            }
        }

        private static void DrawCircle(Color[] pixels, Vector2 center, float radius, float stroke, Color color)
        {
            for (int y = 0; y < Size; y++)
            for (int x = 0; x < Size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                float coverage = stroke <= 0f
                    ? Mathf.Clamp01(radius + 0.5f - distance)
                    : Mathf.Clamp01(0.5f - Mathf.Abs(distance - (radius - stroke * 0.5f)) + stroke * 0.5f);
                if (coverage > 0f) Blend(pixels, x, y, color, coverage);
            }
        }

        private static void DrawEllipse(Color[] pixels, Vector2 center, float radiusX, float radiusY,
            float rotationDegrees, Color color)
        {
            float angle = rotationDegrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(angle), sin = Mathf.Sin(angle);
            for (int y = 0; y < Size; y++)
            for (int x = 0; x < Size; x++)
            {
                float dx = x + 0.5f - center.x, dy = y + 0.5f - center.y;
                float rx = dx * cos + dy * sin;
                float ry = -dx * sin + dy * cos;
                float d = Mathf.Sqrt((rx * rx) / (radiusX * radiusX) + (ry * ry) / (radiusY * radiusY));
                float coverage = Mathf.Clamp01((1f - d) * Mathf.Min(radiusX, radiusY));
                if (coverage > 0f) Blend(pixels, x, y, color, coverage);
            }
        }

        private static void DrawLine(Color[] pixels, Vector2 a, Vector2 b, float width, Color color)
        {
            Vector2 ab = b - a;
            float lengthSquared = ab.sqrMagnitude;
            for (int y = 0; y < Size; y++)
            for (int x = 0; x < Size; x++)
            {
                Vector2 p = new Vector2(x + 0.5f, y + 0.5f);
                float t = lengthSquared > 0f ? Mathf.Clamp01(Vector2.Dot(p - a, ab) / lengthSquared) : 0f;
                float distance = Vector2.Distance(p, a + ab * t);
                float coverage = Mathf.Clamp01(width * 0.5f + 0.5f - distance);
                if (coverage > 0f) Blend(pixels, x, y, color, coverage);
            }
        }

        private static void FillPolygon(Color[] pixels, Vector2[] points, Color color)
        {
            for (int y = 0; y < Size; y++)
            for (int x = 0; x < Size; x++)
            {
                Vector2 p = new Vector2(x + 0.5f, y + 0.5f);
                bool inside = false;
                for (int i = 0, j = points.Length - 1; i < points.Length; j = i++)
                {
                    bool crosses = ((points[i].y > p.y) != (points[j].y > p.y)) &&
                        (p.x < (points[j].x - points[i].x) * (p.y - points[i].y) /
                         (points[j].y - points[i].y + 0.0001f) + points[i].x);
                    if (crosses) inside = !inside;
                }
                if (inside) Blend(pixels, x, y, color, 1f);
            }
        }

        private static void Blend(Color[] pixels, int x, int y, Color color, float coverage)
        {
            int index = y * Size + x;
            Color source = color;
            source.a *= coverage;
            Color destination = pixels[index];
            float outAlpha = source.a + destination.a * (1f - source.a);
            if (outAlpha <= 0f) return;
            Color result = (source * source.a + destination * destination.a * (1f - source.a)) / outAlpha;
            result.a = outAlpha;
            pixels[index] = result;
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
