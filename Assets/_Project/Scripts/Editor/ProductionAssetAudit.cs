#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Reports production asset readiness without failing the prototype while assets are pending.
    /// Run via Tools/Mia & Max/Production Asset Audit.
    /// </summary>
    public static class ProductionAssetAudit
    {
        private sealed class Check
        {
            public string Label;
            public string SearchFilter;
            public string Folder;
            public bool RequiredForRelease;
            public int MinimumCount = 1;
        }

        [MenuItem("Tools/Mia & Max/Production Asset Audit")]
        public static void Run()
        {
            var checks = new List<Check>
            {
                new Check { Label = "Mia reference image", SearchFilter = "Mia t:Texture2D", Folder = "Assets/_Project/Art/Characters", RequiredForRelease = true },
                new Check { Label = "Max reference image", SearchFilter = "Max t:Texture2D", Folder = "Assets/_Project/Art/Characters", RequiredForRelease = true },
                new Check { Label = "Backyard reference image", SearchFilter = "backyard_collage t:Texture2D", Folder = "Assets/_Project/Art/Environment", RequiredForRelease = true },
                new Check { Label = "Mia production model (FBX)", SearchFilter = "Mia t:Model", Folder = "Assets/_Project/Art/Characters/Runtime", RequiredForRelease = true },
                new Check { Label = "Max production model (FBX)", SearchFilter = "Max t:Model", Folder = "Assets/_Project/Art/Characters/Runtime", RequiredForRelease = true },
                new Check { Label = "Mia runtime prefab", SearchFilter = "Mia t:Prefab", Folder = "Assets/Resources/MiaMax/Characters", RequiredForRelease = true },
                new Check { Label = "Max runtime prefab", SearchFilter = "Max t:Prefab", Folder = "Assets/Resources/MiaMax/Characters", RequiredForRelease = true },
                new Check { Label = "Mia animation clips", SearchFilter = "t:AnimationClip", Folder = "Assets/_Project/Art/Characters/Runtime/Mia", RequiredForRelease = true, MinimumCount = 10 },
                new Check { Label = "Max animation clips", SearchFilter = "t:AnimationClip", Folder = "Assets/_Project/Art/Characters/Runtime/Max", RequiredForRelease = true, MinimumCount = 10 },
                new Check { Label = "Mia production Animator Controller", SearchFilter = "Mia_Production t:AnimatorController", Folder = "Assets/_Project/Art/Characters/Runtime/Controllers", RequiredForRelease = true },
                new Check { Label = "Max production Animator Controller", SearchFilter = "Max_Production t:AnimatorController", Folder = "Assets/_Project/Art/Characters/Runtime/Controllers", RequiredForRelease = true },
                new Check { Label = "Generated world materials", SearchFilter = "t:Material", Folder = "Assets/Resources/MiaMax/Materials", RequiredForRelease = true },
                new Check { Label = "Generated albedo textures", SearchFilter = "t:Texture2D", Folder = "Assets/_Project/Art/Textures/Generated/Albedo", RequiredForRelease = true },
                new Check { Label = "Generated normal maps", SearchFilter = "t:Texture2D", Folder = "Assets/_Project/Art/Textures/Generated/Normal", RequiredForRelease = true },
                new Check { Label = "Generated height maps", SearchFilter = "t:Texture2D", Folder = "Assets/_Project/Art/Textures/Generated/Height", RequiredForRelease = true },
                new Check { Label = "UI sprite assets", SearchFilter = "t:Sprite", Folder = "Assets/Resources/MiaMax/UI", RequiredForRelease = true, MinimumCount = 10 },
                new Check { Label = "Audio effects", SearchFilter = "t:AudioClip", Folder = "Assets/Resources/MiaMax/Audio/SFX", RequiredForRelease = true, MinimumCount = 11 },
                new Check { Label = "Voice recordings", SearchFilter = "t:AudioClip", Folder = "Assets/Resources/MiaMax/Audio/Voice", RequiredForRelease = true, MinimumCount = 16 },
                new Check { Label = "Music", SearchFilter = "t:AudioClip", Folder = "Assets/Resources/MiaMax/Audio/Music", RequiredForRelease = true, MinimumCount = 3 },
                new Check { Label = "Ambient loops", SearchFilter = "t:AudioClip", Folder = "Assets/Resources/MiaMax/Audio/Ambience", RequiredForRelease = true, MinimumCount = 4 }
            };

            int ready = 0;
            int pending = 0;
            var report = new System.Text.StringBuilder();
            report.AppendLine("MIA & MAX — PRODUCTION ASSET AUDIT");
            report.AppendLine("Missing production assets are reported as pending, not as prototype compile failures.");
            report.AppendLine();

            foreach (Check check in checks)
            {
                bool folderExists = AssetDatabase.IsValidFolder(check.Folder);
                int assetCount = folderExists ? CountAssets(check.SearchFilter, check.Folder) : 0;
                bool found = folderExists && assetCount >= check.MinimumCount;
                if (found) ready++;
                else pending++;

                report.Append(found ? "[READY] " : "[PENDING] ");
                report.AppendLine(check.Label + " — " + assetCount + " / " + check.MinimumCount + " required; " + check.Folder +
                    (folderExists ? string.Empty : " (folder not created yet)"));
            }

            report.AppendLine();
            report.AppendLine("Ready: " + ready + " / " + checks.Count);
            report.AppendLine("Pending: " + pending + " / " + checks.Count);
            report.AppendLine("Production Animator Controllers are required; parameter-only contract scaffolds do not satisfy this check.");
            report.AppendLine("This audit checks asset presence only; rig quality, clip content, licences, import settings and device performance require manual review.");

            Debug.Log(report.ToString());
            EditorUtility.DisplayDialog("Mia & Max Asset Audit",
                "Asset audit complete. Ready: " + ready + " / " + checks.Count +
                "\nPending: " + pending + " / " + checks.Count +
                "\n\nFull report is in the Console.", "OK");
        }

        private static int CountAssets(string filter, string folder)
        {
            string[] guids = AssetDatabase.FindAssets(filter, new[] { folder });
            if (guids == null || guids.Length == 0) return 0;
            if (filter != "t:AnimationClip") return guids.Length;

            var uniqueClips = new HashSet<string>();
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                foreach (UnityEngine.Object asset in assets)
                {
                    AnimationClip clip = asset as AnimationClip;
                    if (clip != null) uniqueClips.Add(assetPath + "|" + clip.name);
                }
            }
            return uniqueClips.Count;
        }
    }
}
#endif
