#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Reports remaining runtime IMGUI entry points and checks that the mobile HUD
    /// and joystick use the uGUI Canvas path. This is a source audit, not a Play Mode test.
    /// </summary>
    public static class UiMigrationAudit
    {
        private const string ScriptsRoot = "Assets/_Project/Scripts";

        [MenuItem("Tools/Mia & Max/UI Migration Audit")]
        public static void Run()
        {
            List<string> imguiFiles = new List<string>();
            string[] files = Directory.Exists(ScriptsRoot)
                ? Directory.GetFiles(ScriptsRoot, "*.cs", SearchOption.AllDirectories)
                : new string[0];

            for (int i = 0; i < files.Length; i++)
            {
                string source = File.ReadAllText(files[i]);
                if (source.Contains("void OnGUI(") || source.Contains("void OnGUI ()"))
                    imguiFiles.Add(files[i].Replace('\\', '/'));
            }

            string joystickPath = ScriptsRoot + "/UI/PrototypeMobileJoystick.cs";
            string hudPath = ScriptsRoot + "/Gameplay/MobileHudCanvas.cs";
            bool joystickCanvas = File.Exists(joystickPath) &&
                File.ReadAllText(joystickPath).Contains("ScreenSpaceOverlay") &&
                !File.ReadAllText(joystickPath).Contains("void OnGUI(");
            bool hudCanvas = File.Exists(hudPath) &&
                File.ReadAllText(hudPath).Contains("ScreenSpaceOverlay");
            bool legacyFallback = imguiFiles.Contains(ScriptsRoot + "/Gameplay/PrototypeHud.cs");

            Debug.Log("[Mia & Max] UI migration audit\n" +
                "Mobile HUD Canvas: " + (hudCanvas ? "FOUND" : "MISSING") + "\n" +
                "Joystick Canvas: " + (joystickCanvas ? "FOUND" : "MISSING") + "\n" +
                "PrototypeHud IMGUI fallback: " + (legacyFallback ? "PRESENT (intentional fallback)" : "NOT FOUND") + "\n" +
                "Remaining OnGUI files: " + imguiFiles.Count + "\n" +
                string.Join("\n", imguiFiles.ToArray()) + "\n\n" +
                "The audit does not validate visual layout, touch response or device behavior.");

            if (!joystickCanvas || !hudCanvas)
                Debug.LogError("[Mia & Max] UI migration audit found a missing Canvas implementation.");
            else if (imguiFiles.Count > 1)
                Debug.LogWarning("[Mia & Max] Additional IMGUI entry points remain. Review the file list above.");
        }
    }
}
#endif