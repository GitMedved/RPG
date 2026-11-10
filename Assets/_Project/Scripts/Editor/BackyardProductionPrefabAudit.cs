#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using MiaMax.Gameplay;
using UnityEditor;
using UnityEngine;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Read-only gate for imported backyard production prefabs.
    /// Placeholder markers always block production readiness.
    /// </summary>
    public static class BackyardProductionPrefabAudit
    {
        private const string PrefabFolder = "Assets/_Project/Art/Environment/Runtime/ProductionPrefabs";

        private static readonly string[] RequiredPrefabs =
        {
            "HouseExterior", "PatioDeck", "FenceStraight", "FenceCorner", "FenceGate",
            "MainTree", "SwingSet", "Sandbox", "ShrubCluster", "FlowerBed",
            "FlowerPot", "GardenPlot", "Workshop", "AdventureGate", "PathSegment"
        };

        [MenuItem("Tools/Mia & Max/Backyard/Audit Production Prefabs (Read Only)")]
        public static void Run()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Mia & Max", "Exit Play Mode before auditing prefab assets.", "OK");
                return;
            }

            var report = new StringBuilder();
            int ready = 0;
            int blocked = 0;
            int missing = 0;
            int totalTriangles = 0;
            int totalTextures = 0;

            report.AppendLine("MIA & MAX — BACKYARD PRODUCTION PREFAB AUDIT");
            report.AppendLine("Read-only. No scenes, prefabs or assets are modified.");
            report.AppendLine();

            foreach (string prefabName in RequiredPrefabs)
            {
                string assetPath = PrefabFolder + "/" + prefabName + ".prefab";
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (prefab == null)
                {
                    missing++;
                    blocked++;
                    report.AppendLine("[BLOCKED] " + prefabName + " — prefab missing.");
                    continue;
                }

                GameObject contents = PrefabUtility.LoadPrefabContents(assetPath);
                try
                {
                    var issues = new List<string>();
                    BackyardPlaceholderAssetMarker[] placeholders =
                        contents.GetComponentsInChildren<BackyardPlaceholderAssetMarker>(true);
                    if (placeholders.Length > 0)
                        issues.Add("temporary placeholder marker present (" + placeholders.Length + ")");

                    MeshFilter[] meshFilters = contents.GetComponentsInChildren<MeshFilter>(true);
                    SkinnedMeshRenderer[] skinned = contents.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                    Renderer[] renderers = contents.GetComponentsInChildren<Renderer>(true);
                    Collider[] colliders = contents.GetComponentsInChildren<Collider>(true);

                    int validMeshes = 0;
                    int uvReadyMeshes = 0;
                    int triangles = 0;
                    foreach (MeshFilter filter in meshFilters)
                    {
                        if (filter.sharedMesh == null) continue;
                        validMeshes++;
                        triangles += CountTriangles(filter.sharedMesh);
                        if (HasUsableUv0(filter.sharedMesh)) uvReadyMeshes++;
                    }
                    foreach (SkinnedMeshRenderer skinnedRenderer in skinned)
                    {
                        if (skinnedRenderer.sharedMesh == null) continue;
                        validMeshes++;
                        triangles += CountTriangles(skinnedRenderer.sharedMesh);
                        if (HasUsableUv0(skinnedRenderer.sharedMesh)) uvReadyMeshes++;
                    }

                    if (validMeshes == 0)
                        issues.Add("no valid mesh assigned");
                    else if (uvReadyMeshes != validMeshes)
                        issues.Add("UV0 missing or incomplete on " +
                            (validMeshes - uvReadyMeshes) + " mesh(es)");

                    int materialSlots = 0;
                    int missingMaterials = 0;
                    foreach (Renderer renderer in renderers)
                    {
                        Material[] materials = renderer.sharedMaterials;
                        if (materials == null || materials.Length == 0)
                        {
                            missingMaterials++;
                            continue;
                        }
                        foreach (Material material in materials)
                        {
                            materialSlots++;
                            if (material == null) missingMaterials++;
                        }
                    }

                    if (renderers.Length == 0)
                        issues.Add("no renderers found");
                    else if (renderers.Length > 8)
                        issues.Add("renderer budget exceeded: " + renderers.Length + " (target <= 8 per prefab)");
                    if (missingMaterials > 0)
                        issues.Add("missing material slots: " + missingMaterials);
                    if (colliders.Length > 0)
                        issues.Add("visual prefab contains " + colliders.Length +
                            " collider(s); keep gameplay collision on stable gameplay roots");

                    string[] dependencies = AssetDatabase.GetDependencies(assetPath, true);
                    int textureCount = 0;
                    foreach (string dependency in dependencies)
                        if (dependency.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase) ||
                            dependency.EndsWith(".tga", System.StringComparison.OrdinalIgnoreCase) ||
                            dependency.EndsWith(".jpg", System.StringComparison.OrdinalIgnoreCase) ||
                            dependency.EndsWith(".jpeg", System.StringComparison.OrdinalIgnoreCase))
                            textureCount++;

                    LODGroup[] lodGroups = contents.GetComponentsInChildren<LODGroup>(true);
                    if (textureCount == 0)
                        issues.Add("no referenced texture files; production textures are required");
                    if ((prefabName == "HouseExterior" || prefabName == "MainTree") &&
                        lodGroups.Length == 0)
                        issues.Add("LODGroup missing on mobile hero asset");
                    totalTriangles += triangles;
                    totalTextures += textureCount;

                    if (issues.Count == 0)
                    {
                        ready++;
                        report.AppendLine("[STRUCTURALLY CLEAN] " + prefabName +
                            " — meshes: " + validMeshes +
                            ", renderers: " + renderers.Length +
                            ", material slots: " + materialSlots +
                            ", UV-ready meshes: " + uvReadyMeshes + " / " + validMeshes +
                            ", triangles: " + triangles +
                            ", textures: " + textureCount +
                            ", LOD groups: " + lodGroups.Length);
                    }
                    else
                    {
                        blocked++;
                        report.AppendLine("[BLOCKED] " + prefabName + " — " +
                            string.Join("; ", issues.ToArray()) +
                            " | meshes: " + validMeshes +
                            ", UV-ready meshes: " + uvReadyMeshes + " / " + validMeshes +
                            ", triangles: " + triangles +
                            ", textures: " + textureCount +
                            ", LOD groups: " + lodGroups.Length);
                    }
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(contents);
                }
            }

            report.AppendLine();
            report.AppendLine("Structurally clean: " + ready + " / " + RequiredPrefabs.Length);
            report.AppendLine("Blocked: " + blocked + " / " + RequiredPrefabs.Length);
            report.AppendLine("Missing prefabs: " + missing);
            report.AppendLine("Total triangles across prefab assets (instances not counted): " + totalTriangles);
            report.AppendLine("Total referenced texture files (dependencies may be shared): " + totalTextures);
            report.AppendLine();
            report.AppendLine("IMPORTANT: STRUCTURALLY CLEAN is not artistic or release approval.");
            report.AppendLine("This audit cannot judge likeness, silhouette quality, UV quality, texture appearance,");
            report.AppendLine("licensing, device performance, or owner approval. Review those manually before integration.");

            if (blocked == 0) Debug.Log(report.ToString());
            else Debug.LogWarning(report.ToString());

            EditorUtility.DisplayDialog("Mia & Max — Production Prefab Audit",
                "Structurally clean: " + ready + " / " + RequiredPrefabs.Length +
                "\nBlocked: " + blocked +
                "\nMissing: " + missing +
                "\n\nSee Console for per-asset details. This audit does not approve visual quality.",
                "OK");
        }

        private static bool HasUsableUv0(Mesh mesh)
        {
            if (mesh == null || mesh.vertexCount == 0) return false;
            Vector2[] uv = mesh.uv;
            return uv != null && uv.Length == mesh.vertexCount;
        }

        private static int CountTriangles(Mesh mesh)
        {
            if (mesh == null) return 0;
            int count = 0;
            for (int subMesh = 0; subMesh < mesh.subMeshCount; subMesh++)
            {
                if (mesh.GetTopology(subMesh) != MeshTopology.Triangles) continue;
                count += (int)mesh.GetIndexCount(subMesh) / 3;
            }
            return count;
        }
    }
}
#endif
