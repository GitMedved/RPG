#if UNITY_EDITOR
using System.Collections.Generic;
using MiaMax.Gameplay;
using UnityEditor;
using UnityEngine;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Read-only structural audit for the generated backyard placeholder prefabs.
    /// This does not certify visual quality or production readiness.
    /// </summary>
    public static class BackyardPlaceholderPrefabAudit
    {
        private const string PrefabFolder = "Assets/_Project/Art/Environment/Runtime/Prefabs";

        private static readonly string[] ExpectedPrefabs =
        {
            "HouseExterior", "PatioDeck", "FenceStraight", "FenceCorner", "FenceGate",
            "MainTree", "SwingSet", "Sandbox", "ShrubCluster", "FlowerBed",
            "FlowerPot", "GardenPlot", "Workshop", "AdventureGate", "PathSegment"
        };

        [MenuItem("Tools/Mia & Max/Backyard/Audit Placeholder Prefabs (Read Only)")]
        public static void RunAudit()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Mia & Max", "Exit Play Mode before auditing prefab assets.", "OK");
                return;
            }

            int found = 0;
            int withMarker = 0;
            int withMeshes = 0;
            int withMaterials = 0;
            int approvedFlags = 0;
            int missing = 0;
            var issues = new List<string>();

            foreach (string prefabName in ExpectedPrefabs)
            {
                string path = PrefabFolder + "/" + prefabName + ".prefab";
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null)
                {
                    missing++;
                    issues.Add("[MISSING] " + path);
                    continue;
                }

                found++;
                GameObject instance = PrefabUtility.LoadPrefabContents(path);
                try
                {
                    BackyardPlaceholderAssetMarker marker =
                        instance.GetComponent<BackyardPlaceholderAssetMarker>();
                    if (marker != null)
                    {
                        withMarker++;
                        if (marker.productionApproved) approvedFlags++;
                    }
                    else
                    {
                        issues.Add("[NO PLACEHOLDER MARKER] " + prefabName);
                    }

                    MeshFilter[] meshes = instance.GetComponentsInChildren<MeshFilter>(true);
                    int validMeshes = 0;
                    foreach (MeshFilter mesh in meshes)
                        if (mesh.sharedMesh != null) validMeshes++;

                    if (validMeshes > 0) withMeshes++;
                    else issues.Add("[NO MESH] " + prefabName);

                    Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
                    int validMaterials = 0;
                    foreach (Renderer renderer in renderers)
                    {
                        if (renderer.sharedMaterials == null || renderer.sharedMaterials.Length == 0)
                            continue;

                        bool allAssigned = true;
                        foreach (Material material in renderer.sharedMaterials)
                            if (material == null) allAssigned = false;

                        if (allAssigned) validMaterials++;
                    }

                    if (renderers.Length > 0 && validMaterials == renderers.Length) withMaterials++;
                    else issues.Add("[MATERIAL GAP] " + prefabName + " (" +
                        validMaterials + "/" + renderers.Length + " renderers assigned)");
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(instance);
                }
            }

            string report =
                "[Mia & Max] Backyard placeholder prefab structural audit — READ ONLY\n" +
                "Expected prefabs: " + ExpectedPrefabs.Length + "\n" +
                "Found: " + found + "/" + ExpectedPrefabs.Length + "\n" +
                "Placeholder markers: " + withMarker + "/" + found + "\n" +
                "Prefabs with valid meshes: " + withMeshes + "/" + found + "\n" +
                "Prefabs with all renderer materials assigned: " + withMaterials + "/" + found + "\n" +
                "Incorrect productionApproved=true flags: " + approvedFlags + "\n" +
                "Missing prefabs: " + missing + "\n" +
                (issues.Count == 0 ? "Structural checks passed." : string.Join("\n", issues.ToArray())) +
                "\n\nIMPORTANT: structural checks do not assess silhouette, proportions, topology, UVs, textures, " +
                "lighting, reference likeness, optimization, or production-art approval. These assets remain placeholders.";

            if (issues.Count > 0 || approvedFlags > 0) Debug.LogWarning(report);
            else Debug.Log(report);

            EditorUtility.DisplayDialog("Mia & Max — Placeholder Prefab Audit",
                "Found: " + found + "/" + ExpectedPrefabs.Length +
                "\nMarkers: " + withMarker +
                "\nMesh-ready: " + withMeshes +
                "\nMaterial-ready: " + withMaterials +
                "\nIssues: " + issues.Count +
                "\n\nSee Console for the full read-only report. Placeholder assets are not production-ready.",
                "OK");
        }
    }
}
#endif
