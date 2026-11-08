#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Applies the chosen public-facing studio label and stable application identifiers.
    /// AleksDevelop is a project/studio brand label, not a claim of legal-entity status.
    /// </summary>
    public static class ReleaseIdentitySetup
    {
        private const string StudioName = "AleksDevelop";
        private const string ProductName = "Mia & Max Adventures";
        private const string ApplicationId = "com.aleksdevelop.miamax";

        [MenuItem("Tools/Mia & Max/Apply Release Identity")]
        public static void Apply()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Apply Mia & Max release identity",
                "Set Company Name to " + StudioName +
                "\nProduct Name to " + ProductName +
                "\nAndroid and iOS application ID to " + ApplicationId +
                "\n\nThis is a technical project identity only. It does not register a company or determine the legal seller shown in app stores.",
                "Apply", "Cancel");

            if (!confirmed)
                return;

            PlayerSettings.companyName = StudioName;
            PlayerSettings.productName = ProductName;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, ApplicationId);
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, ApplicationId);

            AssetDatabase.SaveAssets();
            Debug.Log("[Mia & Max] Release identity applied: " + StudioName +
                " / " + ProductName + " / " + ApplicationId +
                ". Store seller/legal publisher details must match the real account holder.");
            EditorUtility.DisplayDialog("Mia & Max", "Release identity settings applied. Run Release Readiness Audit to verify.", "OK");
        }
    }
}
#endif
