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
                "Mia FBX files belong in: " + MiaFolder + "\n" +
                "Max FBX files belong in: " + MaxFolder + "\n\n" +
                "Next run: Tools > Mia & Max > Apply Character FBX Import Profiles.",
                "OK");
        }

        [MenuItem("Tools/Mia & Max/Apply Character FBX Import Profiles")]
        public static void ApplyImportProfiles()
        {
            int miaCount = ApplyProfile(MiaFolder, ModelImporterAnimationType.Human);
            int maxCount = ApplyProfile(MaxFolder, ModelImporterAnimationType.Generic);
            bool miaPrefab = BuildRuntimePrefab("Mia", MiaFolder, ControllerFolder + "/Mia_AnimatorContract.controller");
            bool maxPrefab = BuildRuntimePrefab("Max", MaxFolder, ControllerFolder + "/Max_AnimatorContract.controller");
            AssetDatabase.SaveAssets();

            Debug.Log("Mia & Max FBX import profiles applied. Mia Humanoid models: " + miaCount +
                      "; Max Generic/quadruped models: " + maxCount +
                      ". Review each imported Avatar and deformation in the Model Importer.");
            EditorUtility.DisplayDialog("Character FBX Profiles",
                "Mia Humanoid FBX files processed: " + miaCount +
                "\nMax Generic FBX files processed: " + maxCount +
                "\n\nNo FBX files are required for the tool to run. Import and manually review production models when available.",
                "OK");
        }

        private static int ApplyProfile(string folder, ModelImporterAnimationType animationType)
        {
            string[] guids = AssetDatabase.FindAssets("t:Model", new[] { folder });
            int processed = 0;
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (!assetPath.ToLowerInvariant().EndsWith(".fbx")) continue;

                ModelImporter importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
                if (importer == null) continue;

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
                processed++;
            }
            return processed;
        }

        private static bool BuildRuntimePrefab(string characterName, string modelFolder, string controllerPath)
        {
            string[] guids = AssetDatabase.FindAssets("t:Model", new[] { modelFolder });
            var modelPaths = new List<string>();
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (assetPath.ToLowerInvariant().EndsWith(".fbx")) modelPaths.Add(assetPath);
            }
            modelPaths.Sort();
            if (modelPaths.Count == 0) return false;

            string characterKey = characterName.ToLowerInvariant();
            string selectedModelPath = null;
            foreach (string candidatePath in modelPaths)
            {
                string fileName = Path.GetFileNameWithoutExtension(candidatePath).ToLowerInvariant();
                if (fileName == characterKey || fileName == characterKey + "_model" ||
                    fileName == characterKey + "_character")
                {
                    selectedModelPath = candidatePath;
                    break;
                }
            }
            if (selectedModelPath == null && modelPaths.Count == 1)
                selectedModelPath = modelPaths[0];
            if (selectedModelPath == null)
            {
                Debug.LogWarning(characterName + " prefab was not generated: multiple FBX files exist. " +
                    "Name the base model " + characterName + ".fbx (or " + characterName + "_Model.fbx) " +
                    "to distinguish it from animation FBX files.");
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
