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
                new Check { Label = "Mia Animator Controller", SearchFilter = "Mia t:AnimatorController", Folder = "Assets/_Project/Art/Characters/Runtime", RequiredForRelease = true },
                new Check { Label = "Max Animator Controller", SearchFilter = "Max t:AnimatorController", Folder = "Assets/_Project/Art/Characters/Runtime", RequiredForRelease = true },
                new Check { Label = "UI sprite assets", SearchFilter = "t:Sprite", Folder = "Assets/_Project/Art/UI", RequiredForRelease = true },
                new Check { Label = "Audio effects", SearchFilter = "t:AudioClip", Folder = "Assets/Resources/MiaMax/Audio/SFX", RequiredForRelease = true },
                new Check { Label = "Voice recordings", SearchFilter = "t:AudioClip", Folder = "Assets/_Project/Audio/Voice", RequiredForRelease = true },
                new Check { Label = "Music", SearchFilter = "t:AudioClip", Folder = "Assets/_Project/Audio/Music", RequiredForRelease = true }
            };

            int ready = 0;
            int pending = 0;
            var report = new System.Text.StringBuilder();
            report.AppendLine("MIA & MAX — PRODUCTION ASSET AUDIT");
            report.AppendLine("Missing production assets are reported as pending, not as prototype compile failures.");
            report.AppendLine();

            foreach (Check check in checks)
            {
                string[] assets = AssetDatabase.FindAssets(check.SearchFilter, new[] { check.Folder });
                bool found = assets != null && assets.Length > 0;
                if (found) ready++;
                else pending++;

                report.Append(found ? "[READY] " : "[PENDING] ");
                report.AppendLine(check.Label + " — " + check.Folder);
            }

            report.AppendLine();
            report.AppendLine("Ready: " + ready + " / " + checks.Count);
            report.AppendLine("Pending: " + pending + " / " + checks.Count);
            report.AppendLine("This audit checks asset presence only; rig quality, licences, import settings and device performance require manual review.");

            Debug.Log(report.ToString());
            EditorUtility.DisplayDialog("Mia & Max Asset Audit",
                "Asset audit complete. Ready: " + ready + " / " + checks.Count +
                "\nPending: " + pending + " / " + checks.Count +
                "\n\nFull report is in the Console.", "OK");
        }
    }
}
#endif
