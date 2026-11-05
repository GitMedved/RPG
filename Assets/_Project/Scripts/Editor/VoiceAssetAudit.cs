#if UNITY_EDITOR
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Read-only presence audit for the stable M49 voice take IDs.
    /// Run via Tools/Mia & Max/Voice Asset Audit.
    /// </summary>
    public static class VoiceAssetAudit
    {
        private static readonly string[] VoiceIds =
        {
            "mia_greeting_01",
            "mia_curiosity_01",
            "mia_discovery_01",
            "mia_plant_01",
            "mia_harvest_01",
            "mia_interact_01",
            "mia_quest_01",
            "mia_success_01",
            "mia_encourage_01",
            "mia_goodbye_01",
            "max_meow_happy_01",
            "max_meow_curious_01",
            "max_meow_surprised_01",
            "max_purr_01",
            "max_effort_01",
            "max_triumph_01"
        };

        private const string Root = "Assets/Resources/MiaMax/Audio/Voice/";

        [MenuItem("Tools/Mia & Max/Voice Asset Audit")]
        public static void Run()
        {
            int available = 0;
            int missing = 0;
            var report = new StringBuilder();
            report.AppendLine("MIA & MAX — VOICE ASSET AUDIT");
            report.AppendLine("Expected runtime folder: " + Root);
            report.AppendLine();

            foreach (string id in VoiceIds)
            {
                string path = Root + id + ".wav";
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                if (clip != null)
                {
                    available++;
                    report.AppendLine("[AVAILABLE] " + id + " (" + clip.frequency + " Hz, " +
                        clip.channels + " channel(s), " + clip.samples + " samples)");
                }
                else
                {
                    missing++;
                    report.AppendLine("[MISSING] " + id + " — " + path);
                }
            }

            report.AppendLine();
            report.AppendLine("Available: " + available + " / " + VoiceIds.Length);
            report.AppendLine("Missing: " + missing + " / " + VoiceIds.Length);
            report.AppendLine("Presence only: audition quality, rights/licence, pronunciation and import settings still require review.");
            report.AppendLine("This command is read-only and does not create placeholder clips.");

            Debug.Log(report.ToString());
            EditorUtility.DisplayDialog("Mia & Max Voice Audit",
                "Voice audit complete. Available: " + available + " / " + VoiceIds.Length +
                "\nMissing: " + missing + " / " + VoiceIds.Length +
                "\n\nFull report is in the Console.", "OK");
        }
    }
}
#endif
