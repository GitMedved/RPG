#if UNITY_EDITOR
using System.Text;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MiaMax.EditorTools
{
    /// <summary>Read-only validation of the original generated gameplay audio pack.</summary>
    public static class AudioAssetAudit
    {
        private static readonly string[] Sfx =
        {
            "ui_tap", "plant", "harvest", "craft_start", "craft_ready", "reward",
            "collectible", "interact", "minigame_start", "minigame_win", "minigame_miss"
        };
        private static readonly string[] Ambience =
        {
            "backyard_wind", "backyard_birds", "woodland_wind", "woodland_birds"
        };
        private static readonly string[] Music =
        {
            "backyard_theme", "flower_garden_theme", "woodland_theme"
        };

        [MenuItem("Tools/Mia & Max/Audio/Audio Asset Audit (Read Only)")]
        public static void Run()
        {
            var report = new StringBuilder();
            int ready = 0;
            int missing = 0;
            int pendingImport = 0;
            int warnings = 0;
            report.AppendLine("MIA & MAX — AUDIO ASSET AUDIT");
            report.AppendLine("Read-only. Does not generate or modify audio assets.");
            report.AppendLine();

            AuditGroup("Sound effects", "Assets/Resources/MiaMax/Audio/SFX", Sfx,
                0.06f, 1.8f, ref ready, ref missing, ref pendingImport, ref warnings, report);
            AuditGroup("Ambience", "Assets/Resources/MiaMax/Audio/Ambience", Ambience,
                5f, 30f, ref ready, ref missing, ref pendingImport, ref warnings, report);
            AuditGroup("Music", "Assets/Resources/MiaMax/Audio/Music", Music,
                4f, 30f, ref ready, ref missing, ref pendingImport, ref warnings, report);

            report.AppendLine();
            report.AppendLine("Available: " + ready + " / " + (Sfx.Length + Ambience.Length + Music.Length));
            report.AppendLine("Pending import: " + pendingImport);
            report.AppendLine("Missing: " + missing);
            report.AppendLine("Warnings: " + warnings);
            report.AppendLine("Structural presence is not an artistic mix, loudness, loop-seam or device playback approval.");
            report.AppendLine("Voice recordings are checked separately by Tools > Mia & Max > Voice Asset Audit.");

            Debug.Log(report.ToString());
            EditorUtility.DisplayDialog("Mia & Max Audio Audit",
                "Available: " + ready + " / 18\nPending import: " + pendingImport + "\nMissing: " + missing +
                "\nWarnings: " + warnings + "\n\nFull report is in Console.", "OK");
        }

        private static void AuditGroup(string label, string folder, string[] ids,
            float minDuration, float maxDuration, ref int ready, ref int missing,
            ref int pendingImport, ref int warnings, StringBuilder report)
        {
            report.AppendLine("[" + label + "]");
            foreach (string id in ids)
            {
                string path = folder + "/" + id + ".wav";
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                if (clip == null)
                {
                    if (File.Exists(path))
                    {
                        pendingImport++;
                        warnings++;
                        report.AppendLine("[PENDING IMPORT] " + id + " — WAV exists on disk but Unity has not exposed an AudioClip yet: " + path);
                    }
                    else
                    {
                        missing++;
                        report.AppendLine("[MISSING] " + id + " — " + path);
                    }
                    continue;
                }

                ready++;
                bool durationValid = clip.length >= minDuration && clip.length <= maxDuration;
                bool rateValid = clip.frequency >= 22050;
                AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;
                AudioImporterSampleSettings sampleSettings = importer != null
                    ? importer.defaultSampleSettings : default(AudioImporterSampleSettings);
                bool importSettingsValid = importer != null && importer.forceToMono &&
                    sampleSettings.preloadAudioData && importer.loadInBackground;

                if (!durationValid || !rateValid || !importSettingsValid) warnings++;
                report.AppendLine((durationValid && rateValid && importSettingsValid ? "[READY] " : "[WARNING] ") +
                    id + " — " + clip.length.ToString("0.00") + " s, " + clip.frequency + " Hz, " +
                    clip.channels + " channel(s); duration " + durationValid +
                    "; sample rate " + rateValid + "; mobile import settings " + importSettingsValid);
            }
            report.AppendLine();
        }
    }
}
#endif
