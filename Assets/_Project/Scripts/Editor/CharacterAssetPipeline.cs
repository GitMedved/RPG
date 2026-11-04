#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Creates predictable character asset folders, applies character-specific FBX rig profiles,
    /// and creates Animator Controller contract scaffolds. It does not create character art or clips.
    /// </summary>
    public static class CharacterAssetPipeline
    {
        private const string RuntimeRoot = "Assets/_Project/Art/Characters/Runtime";
        private const string MiaFolder = RuntimeRoot + "/Mia";
        private const string MaxFolder = RuntimeRoot + "/Max";
        private const string ControllerFolder = RuntimeRoot + "/Controllers";

        [MenuItem("Tools/Mia & Max/Prepare Character Asset Pipeline")]
        public static void PreparePipeline()
        {
            EnsureFolder("Assets/_Project/Art/Characters");
            EnsureFolder(RuntimeRoot);
            EnsureFolder(MiaFolder);
            EnsureFolder(MaxFolder);
            EnsureFolder(ControllerFolder);

            string miaController = ControllerFolder + "/Mia_AnimatorContract.controller";
            string maxController = ControllerFolder + "/Max_AnimatorContract.controller";
            CreateControllerContract(miaController);
            CreateControllerContract(maxController);

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Mia & Max Character Pipeline",
                "Character folders and Animator contract assets are ready.\n\n" +
                "Mia base model and clips belong in: " + MiaFolder + "\n" +
                "Max base model and clips belong in: " + MaxFolder + "\n\n" +
                "Next run: Tools > Mia & Max > Apply Character FBX Import Profiles.",
                "OK");
        }

        [MenuItem("Tools/Mia & Max/Apply Character FBX Import Profiles")]
        public static void ApplyImportProfiles()
        {
            int miaCount = ApplyProfile(MiaFolder, "Mia", ModelImporterAnimationType.Human);
            int maxCount = ApplyProfile(MaxFolder, "Max", ModelImporterAnimationType.Generic);
            bool miaPrefab = BuildRuntimePrefab("Mia", MiaFolder, ControllerFolder + "/Mia_AnimatorContract.controller");
            bool maxPrefab = BuildRuntimePrefab("Max", MaxFolder, ControllerFolder + "/Max_AnimatorContract.controller");
            AssetDatabase.SaveAssets();

            Debug.Log("Mia & Max FBX import profiles applied. Mia Humanoid models: " + miaCount +
                      "; Max Generic/quadruped models: " + maxCount +
                      ". Review each imported Avatar and deformation in the Model Importer.");
            EditorUtility.DisplayDialog("Character FBX Profiles",
                "Mia base model processed: " + miaCount +
                "\nMax base model processed: " + maxCount +
                "\n\nNo FBX files are required for the tool to run. Import and manually review production models when available.",
                "OK");
        }

        private static int ApplyProfile(string folder, string characterName, ModelImporterAnimationType animationType)
        {
            string modelPath = FindBaseModelPath(folder, characterName);
            if (string.IsNullOrEmpty(modelPath))
            {
                Debug.LogWarning(characterName + " base FBX was not found or was ambiguous in " + folder +
                    ". Add one base file named " + characterName + ".fbx or " + characterName + "_Model.fbx.");
                return 0;
            }

            ModelImporter importer = AssetImporter.GetAtPath(modelPath) as ModelImporter;
            if (importer == null) return 0;

            bool changed = false;
            if (importer.animationType != animationType)
            {
                importer.animationType = animationType;
                changed = true;
            }
            if (importer.avatarSetup != ModelImporterAvatarSetup.CreateFromThisModel)
            {
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                changed = true;
            }
            if (!importer.importAnimation)
            {
                importer.importAnimation = true;
                changed = true;
            }
            if (importer.importCameras)
            {
                importer.importCameras = false;
                changed = true;
            }
            if (importer.importLights)
            {
                importer.importLights = false;
                changed = true;
            }
            if (changed) importer.SaveAndReimport();
            return 1;
        }

        private static string FindBaseModelPath(string folder, string characterName)
        {
            string[] guids = AssetDatabase.FindAssets("t:Model", new[] { folder });
            var modelPaths = new List<string>();
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (assetPath.ToLowerInvariant().EndsWith(".fbx")) modelPaths.Add(assetPath);
            }
            modelPaths.Sort();
            if (modelPaths.Count == 0) return null;

            string characterKey = characterName.ToLowerInvariant();
            foreach (string candidatePath in modelPaths)
            {
                string fileName = Path.GetFileNameWithoutExtension(candidatePath).ToLowerInvariant();
                if (fileName == characterKey || fileName == characterKey + "_model" ||
                    fileName == characterKey + "_character")
                    return candidatePath;
            }
            return modelPaths.Count == 1 ? modelPaths[0] : null;
        }

        private static bool BuildRuntimePrefab(string characterName, string modelFolder, string controllerPath)
        {
            string selectedModelPath = FindBaseModelPath(modelFolder, characterName);
            if (string.IsNullOrEmpty(selectedModelPath))
            {
                Debug.LogWarning(characterName + " runtime prefab was not generated because a unique base FBX was not found.");
                return false;
            }

            GameObject modelAsset = AssetDatabase.LoadAssetAtPath<GameObject>(selectedModelPath);
            if (modelAsset == null) return false;

            string resourcesFolder = "Assets/Resources/MiaMax/Characters";
            EnsureFolder("Assets/Resources");
            EnsureFolder("Assets/Resources/MiaMax");
            EnsureFolder(resourcesFolder);

            GameObject wrapper = new GameObject(characterName + "_Runtime");
            GameObject modelInstance = Object.Instantiate(modelAsset);
            modelInstance.name = characterName + "_Model";
            modelInstance.transform.SetParent(wrapper.transform, false);

            Animator animator = modelInstance.GetComponentInChildren<Animator>();
            if (animator == null) animator = modelInstance.AddComponent<Animator>();

            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
            if (controller != null) animator.runtimeAnimatorController = controller;

            Avatar importedAvatar = null;
            Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(selectedModelPath);
            foreach (Object subAsset in subAssets)
            {
                Avatar candidate = subAsset as Avatar;
                if (candidate != null) { importedAvatar = candidate; break; }
            }
            if (animator.avatar == null && importedAvatar != null) animator.avatar = importedAvatar;

            string prefabPath = resourcesFolder + "/" + characterName + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(wrapper, prefabPath);
            Object.DestroyImmediate(wrapper);
            return AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null;
        }

        private static void CreateControllerContract(string assetPath)
        {
            if (AssetDatabase.LoadAssetAtPath<AnimatorController>(assetPath) != null) return;

            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(assetPath);
            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
            controller.AddParameter("Grounded", AnimatorControllerParameterType.Bool);
            controller.AddParameter("Interact", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Celebrate", AnimatorControllerParameterType.Trigger);
            EditorUtility.SetDirty(controller);
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            int separator = path.LastIndexOf('/');
            if (separator <= 0) return;
            string parent = path.Substring(0, separator);
            string name = path.Substring(separator + 1);
            EnsureFolder(parent);
            if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder(parent, name);
        }
    }
}
#endif
