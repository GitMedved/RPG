#if UNITY_EDITOR
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Read-only inventory for downloaded third-party environment source assets.
    /// It reports imported Unity models, mesh triangles, model materials and textures.
    /// It never changes assets or marks them production-approved.
    /// </summary>
    public static class FreeEnvironmentAssetInventory
    {
        private const string ModelRoot = "Assets/_Project/Art/Environment/Runtime/Models";

        [MenuItem("Tools/Mia & Max/Assets/Run Free Environment Asset Inventory")]
        public static void Run()
        {
            AssetDatabase.Refresh();
            var report = new StringBuilder();
            string[] modelGuids = AssetDatabase.FindAssets("t:Model", new[] { ModelRoot });
            string[] materialGuids = AssetDatabase.FindAssets("t:Material", new[] { ModelRoot });
            string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { ModelRoot });

            int importedModels = 0;
            int meshes = 0;
            long triangles = 0;

            report.AppendLine("MIA & MAX — FREE ENVIRONMENT SOURCE ASSET INVENTORY");
            report.AppendLine("Root: " + ModelRoot);
            report.AppendLine();

            foreach (string guid in modelGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(path);
                int localMeshes = 0;
                long localTriangles = 0;
                int localMaterials = 0;

                foreach (Object asset in subAssets)
                {
                    Mesh mesh = asset as Mesh;
                    if (mesh != null)
                    {
                        localMeshes++;
                        localTriangles += mesh.triangles.Length / 3;
                    }

                    if (asset is Material)
                        localMaterials++;
                }

                importedModels++;
                meshes += localMeshes;
                triangles += localTriangles;
                report.AppendLine("[MODEL] " + path +
                    " | meshes: " + localMeshes +
                    " | triangles: " + localTriangles +
                    " | embedded materials: " + localMaterials);
            }

            report.AppendLine();
            report.AppendLine("Imported model files: " + importedModels);
            report.AppendLine("Meshes: " + meshes);
            report.AppendLine("Approx. triangles: " + triangles);
            report.AppendLine("Standalone materials: " + materialGuids.Length);
            report.AppendLine("Textures: " + textureGuids.Length);
            report.AppendLine();
            report.AppendLine("This is an import inventory only. It does not certify correct scale, visual style, UVs, texture quality, collisions, LODs, licensing, mobile performance, or production readiness.");
            report.AppendLine("If model count is zero for a downloaded GLB pack, verify Unity glTFast package resolution and inspect the Unity Console.");

            Debug.Log(report.ToString());
            EditorUtility.DisplayDialog(
                "Mia & Max — Free Asset Inventory",
                "Models imported: " + importedModels +
                "\nMeshes: " + meshes +
                "\nApprox. triangles: " + triangles +
                "\nMaterials: " + materialGuids.Length +
                "\nTextures: " + textureGuids.Length +
                "\n\nFull report is in Console.",
                "OK");
        }
    }
}
#endif
