#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Prepares and audits the production backyard asset handoff.
    /// This tool never fabricates final art and never replaces runtime prototype objects.
    /// </summary>
    public static class BackyardEnvironmentAssetPipeline
    {
        private const string Root = "Assets/_Project/Art/Environment/Runtime";
        private const string Source = Root + "/Source";
        private const string Models = Root + "/Models";
        private const string Prefabs = Root + "/Prefabs";
        private const string Materials = Root + "/Materials";
        private const string Textures = Root + "/Textures";
        private const string Lod = Root + "/LOD";

        private static readonly string[] RequiredPrefabs =
        {
            "HouseExterior",
            "PatioDeck",
            "FenceStraight",
            "FenceCorner",
            "FenceGate",
            "MainTree",
            "SwingSet",
            "Sandbox",
            "ShrubCluster",
            "FlowerBed",
            "FlowerPot",
            "GardenPlot",
            "Workshop",
            "AdventureGate",
            "PathSegment"
        };

        [MenuItem("Tools/Mia & Max/Prepare Backyard Production Asset Folders")]
        public static void PrepareFolders()
        {
            EnsureFolder("Assets/_Project/Art");
            EnsureFolder("Assets/_Project/Art/Environment");
            EnsureFolder(Root);
            EnsureFolder(Source);
            EnsureFolder(Models);
            EnsureFolder(Prefabs);
            EnsureFolder(Materials);
            EnsureFolder(Textures);
            EnsureFolder(Lod);

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog(
                "Mia & Max — Backyard Asset Pipeline",
                "Production folders are ready.\n\n" +
                "Source models: " + Source + "\n" +
                "Imported models: " + Models + "\n" +
                "Runtime prefabs: " + Prefabs + "\n" +
                "Materials: " + Materials + "\n" +
                "Textures: " + Textures + "\n" +
                "LOD assets: " + Lod + "\n\n" +
                "No placeholder geometry was promoted to final art.",
                "OK");
        }

        [MenuItem("Tools/Mia & Max/Audit Backyard Production Assets")]
        public static void Audit()
        {
            var report = new StringBuilder();
            var prefabPaths = new HashSet<string>();
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { Prefabs });
            foreach (string guid in prefabGuids)
                prefabPaths.Add(AssetDatabase.GUIDToAssetPath(guid));

            int ready = 0;
            int pending = 0;
            report.AppendLine("MIA & MAX — BACKYARD PRODUCTION ASSET AUDIT");
            report.AppendLine("Expected runtime prefabs: " + RequiredPrefabs.Length);
            report.AppendLine();

            foreach (string requiredName in RequiredPrefabs)
            {
                string path = Prefabs + "/" + requiredName + ".prefab";
                if (!prefabPaths.Contains(path))
                {
                    pending++;
                    report.AppendLine("[PENDING] " + requiredName + " prefab missing: " + path);
                    continue;
                }

                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null)
                {
                    pending++;
                    report.AppendLine("[PENDING] " + requiredName + " could not be loaded.");
                    continue;
                }

                MeshFilter[] meshFilters = prefab.GetComponentsInChildren<MeshFilter>(true);
                SkinnedMeshRenderer[] skinned = prefab.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);
                bool hasGeometry = false;
                foreach (MeshFilter filter in meshFilters)
                    if (filter.sharedMesh != null) { hasGeometry = true; break; }
                if (!hasGeometry)
                    foreach (SkinnedMeshRenderer renderer in skinned)
                        if (renderer.sharedMesh != null) { hasGeometry = true; break; }

                bool hasMaterials = false;
                foreach (Renderer renderer in renderers)
                {
                    Material[] assigned = renderer.sharedMaterials;
                    if (assigned.Length == 0) continue;
                    foreach (Material material in assigned)
                    {
                        if (material != null) { hasMaterials = true; break; }
                    }
                    if (hasMaterials) break;
                }

                if (hasGeometry && hasMaterials)
                {
                    ready++;
                    report.AppendLine("[READY*] " + requiredName + " — mesh and at least one material assigned.");
                }
                else
                {
                    pending++;
                    report.AppendLine("[PENDING] " + requiredName +
                        " — mesh assigned: " + hasGeometry + "; material assigned: " + hasMaterials);
                }
            }

            report.AppendLine();
            report.AppendLine("Structurally ready: " + ready + " / " + RequiredPrefabs.Length);
            report.AppendLine("Pending: " + pending + " / " + RequiredPrefabs.Length);
            report.AppendLine("* READY means structural checks only. It does not certify reference likeness, topology, UVs, texture quality, licensing, mobile performance, collisions, or visual approval.");
            report.AppendLine("This audit is read-only. It does not alter scenes, prefabs, materials, or runtime bootstrap code.");

            Debug.Log(report.ToString());
            EditorUtility.DisplayDialog(
                "Mia & Max — Backyard Asset Audit",
                "Structurally ready: " + ready + " / " + RequiredPrefabs.Length +
                "\nPending: " + pending + " / " + RequiredPrefabs.Length +
                "\n\nFull report is in the Console.\nNo assets were modified.",
                "OK");
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path).Replace("\\", "/");
            string folder = Path.GetFileName(path);
            if (!AssetDatabase.IsValidFolder(parent))
                EnsureFolder(parent);
            if (!AssetDatabase.IsValidFolder(path))
                AssetDatabase.CreateFolder(parent, folder);
        }
    }
}
#endif
