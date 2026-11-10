#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using MiaMax.Gameplay;

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

        [MenuItem("Tools/Mia & Max/Validate Character Runtime Prefabs")]
        public static void ValidateRuntimePrefabs()
        {
            int ready = 0;
            int pending = 0;
            var report = new System.Text.StringBuilder();
            report.AppendLine("MIA & MAX — CHARACTER RUNTIME PREFAB VALIDATION");
            ValidateRuntimePrefab("Mia", ref ready, ref pending, report);
            ValidateRuntimePrefab("Max", ref ready, ref pending, report);
            report.AppendLine();
            report.AppendLine("Ready: " + ready + " / 2");
            report.AppendLine("Pending: " + pending + " / 2");
            report.AppendLine("A valid prefab and Avatar do not mean that production animation clips or state transitions exist.");
            Debug.Log(report.ToString());
            EditorUtility.DisplayDialog("Character Runtime Validation",
                "Ready: " + ready + " / 2\nPending: " + pending + " / 2\n\nFull details are in the Console.",
                "OK");
        }

        private static void ValidateRuntimePrefab(string characterName, ref int ready, ref int pending,
            System.Text.StringBuilder report)
        {
            string path = "Assets/Resources/MiaMax/Characters/" + characterName + ".prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                pending++;
                report.AppendLine("[PENDING] " + characterName + " runtime prefab missing: " + path);
                return;
            }

            Animator animator = prefab.GetComponentInChildren<Animator>(true);
            if (animator == null)
            {
                pending++;
                report.AppendLine("[PENDING] " + characterName + " Animator component missing.");
                return;
            }

            bool avatarValid = animator.avatar != null && animator.avatar.isValid;
            bool rigTypeValid = avatarValid && (characterName == "Mia"
                ? animator.avatar.isHuman
                : !animator.avatar.isHuman);
            bool controllerAssigned = animator.runtimeAnimatorController != null;
            bool controllerHasProductionMotions = HasProductionMotions(animator.runtimeAnimatorController);
            bool rootMotionOff = !animator.applyRootMotion;
            bool contract = HasParameter(animator, "Speed", AnimatorControllerParameterType.Float) &&
                HasParameter(animator, "Grounded", AnimatorControllerParameterType.Bool) &&
                HasParameter(animator, "Interact", AnimatorControllerParameterType.Trigger) &&
                HasParameter(animator, "Celebrate", AnimatorControllerParameterType.Trigger) &&
                HasParameter(animator, "Garden", AnimatorControllerParameterType.Trigger) &&
                HasParameter(animator, "Reaction", AnimatorControllerParameterType.Trigger) &&
                HasParameter(animator, "Jump", AnimatorControllerParameterType.Trigger) &&
                HasParameter(animator, "Sleep", AnimatorControllerParameterType.Trigger) &&
                HasParameter(animator, "Play", AnimatorControllerParameterType.Trigger);
            bool hasGeometry = prefab.GetComponentInChildren<SkinnedMeshRenderer>(true) != null ||
                prefab.GetComponentInChildren<MeshRenderer>(true) != null;

            if (avatarValid && rigTypeValid && controllerAssigned && controllerHasProductionMotions && rootMotionOff && contract && hasGeometry)
            {
                ready++;
                report.AppendLine("[READY] " + characterName + " prefab, Avatar, Animator contract and root-motion setting.");
            }
            else
            {
                pending++;
                report.AppendLine("[PENDING] " + characterName +
                    " — Avatar valid: " + avatarValid +
                    "; Expected rig type: " + rigTypeValid +
                    "; Controller assigned: " + controllerAssigned +
                    "; Production states/motions complete: " + controllerHasProductionMotions +
                    "; Root Motion disabled: " + rootMotionOff +
                    "; Parameters complete: " + contract +
                    "; Render geometry present: " + hasGeometry);
            }

            if (animator.runtimeAnimatorController != null)
                report.AppendLine("         Controller: " + animator.runtimeAnimatorController.name);
            if (animator.avatar != null)
                report.AppendLine("         Avatar: " + animator.avatar.name +
                    " (Humanoid: " + animator.avatar.isHuman + "; expected " +
                    (characterName == "Mia" ? "Humanoid" : "Generic") + ")");
        }

        private static bool HasProductionMotions(RuntimeAnimatorController runtimeController)
        {
            AnimatorController controller = runtimeController as AnimatorController;
            if (controller == null || controller.layers == null || controller.layers.Length == 0)
                return false;

            string[] required =
            {
                "Idle", "Walk", "Run", "Interact", "Celebrate",
                "Garden", "Reaction", "Jump", "Sleep", "Play"
            };
            ChildAnimatorState[] states = controller.layers[0].stateMachine.states;
            foreach (string action in required)
            {
                bool found = false;
                foreach (ChildAnimatorState child in states)
                {
                    if (child.state != null && child.state.name == action && child.state.motion != null)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found) return false;
            }
            return true;
        }

        private static bool HasParameter(Animator animator, string parameterName,
            AnimatorControllerParameterType expectedType)
        {
            if (animator == null || animator.runtimeAnimatorController == null) return false;
            int id = Animator.StringToHash(parameterName);
            AnimatorControllerParameter[] parameters = animator.parameters;
            for (int i = 0; i < parameters.Length; i++)
                if (parameters[i].nameHash == id && parameters[i].type == expectedType) return true;
            return false;
        }

        [MenuItem("Tools/Mia & Max/Build Production Animator Controllers From Clips")]
        public static void BuildProductionAnimatorControllers()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Mia & Max Characters",
                    "Exit Play Mode before building Animator Controllers.", "OK");
                return;
            }

            EnsureFolder(ControllerFolder);
            bool miaBuilt = BuildProductionController("Mia", MiaFolder, ControllerFolder + "/Mia_Production.controller");
            bool maxBuilt = BuildProductionController("Max", MaxFolder, ControllerFolder + "/Max_Production.controller");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[Mia & Max] Production Animator Controllers: Mia " + (miaBuilt ? "created" : "pending") +
                "; Max " + (maxBuilt ? "created" : "pending") +
                ". Controllers are separate from the contract scaffolds and are not assigned to runtime prefabs automatically.");
            EditorUtility.DisplayDialog("Mia & Max Characters",
                "Mia controller: " + (miaBuilt ? "created" : "pending clips") +
                "\nMax controller: " + (maxBuilt ? "created" : "pending clips") +
                "\n\nThe command requires uniquely identifiable Idle, Walk, Run, Interact, Celebrate, Garden, Reaction, Jump, Sleep and Play clips for each character. Existing runtime prefabs are not modified.",
                "OK");
        }

        private static bool BuildProductionController(string characterName, string clipsFolder, string controllerPath)
        {
            string[] required =
            {
                "Idle", "Walk", "Run", "Interact", "Celebrate",
                "Garden", "Reaction", "Jump", "Sleep", "Play"
            };
            string[] guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { clipsFolder });
            var clips = new System.Collections.Generic.Dictionary<string, AnimationClip>();
            var used = new System.Collections.Generic.HashSet<int>();
            var missing = new System.Collections.Generic.List<string>();

            foreach (string action in required)
            {
                AnimationClip match = null;
                bool ambiguous = false;
                foreach (string guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    AnimationClip[] candidates = AssetDatabase.LoadAllAssetsAtPath(assetPath)
                        .OfType<AnimationClip>().ToArray();
                    foreach (AnimationClip candidate in candidates)
                    {
                        if (candidate == null || candidate.name.IndexOf(action, System.StringComparison.OrdinalIgnoreCase) < 0)
                            continue;
                        if (used.Contains(candidate.GetInstanceID())) continue;
                        if (match != null)
                        {
                            ambiguous = true;
                            break;
                        }
                        match = candidate;
                    }
                    if (ambiguous) break;
                }

                if (match == null || ambiguous) missing.Add(action);
                else
                {
                    clips.Add(action, match);
                    used.Add(match.GetInstanceID());
                }
            }

            if (missing.Count > 0)
            {
                Debug.LogWarning(characterName + " production Animator Controller not created. Missing or ambiguous clips: " +
                    string.Join(", ", missing) + ". Expected one uniquely named clip per action in " + clipsFolder);
                return false;
            }

            if (AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath) != null)
            {
                Debug.LogWarning(characterName + " production controller already exists and was preserved: " + controllerPath);
                return false;
            }

            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
            controller.AddParameter("Grounded", AnimatorControllerParameterType.Bool);
            controller.AddParameter("Interact", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Celebrate", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Garden", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Reaction", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Jump", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Sleep", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Play", AnimatorControllerParameterType.Trigger);

            AnimatorStateMachine machine = controller.layers[0].stateMachine;
            AnimatorState idle = machine.AddState("Idle");
            idle.motion = clips["Idle"];
            machine.defaultState = idle;
            AnimatorState walk = machine.AddState("Walk");
            walk.motion = clips["Walk"];
            AnimatorState run = machine.AddState("Run");
            run.motion = clips["Run"];

            AddFloatTransition(idle, walk, "Speed", AnimatorConditionMode.Greater, 0.1f);
            AddFloatTransition(walk, idle, "Speed", AnimatorConditionMode.Less, 0.1f);
            AddFloatTransition(walk, run, "Speed", AnimatorConditionMode.Greater, 2.5f);
            AddFloatTransition(run, walk, "Speed", AnimatorConditionMode.Less, 2.5f);

            string[] triggers = { "Interact", "Celebrate", "Garden", "Reaction", "Jump", "Sleep", "Play" };
            foreach (string trigger in triggers)
            {
                AnimatorState state = machine.AddState(trigger);
                state.motion = clips[trigger];
                AnimatorStateTransition enter = machine.AddAnyStateTransition(state);
                enter.AddCondition(AnimatorConditionMode.If, 0f, trigger);
                enter.hasExitTime = false;
                enter.duration = 0.08f;
                enter.canTransitionToSelf = false;

                AnimatorStateTransition exit = state.AddTransition(idle);
                exit.hasExitTime = true;
                exit.exitTime = 0.9f;
                exit.duration = 0.1f;
            }

            EditorUtility.SetDirty(controller);
            Debug.Log(characterName + " production Animator Controller created: " + controllerPath);
            return true;
        }

        private static void AddFloatTransition(AnimatorState from, AnimatorState to,
            string parameter, AnimatorConditionMode mode, float threshold)
        {
            AnimatorStateTransition transition = from.AddTransition(to);
            transition.hasExitTime = false;
            transition.duration = 0.12f;
            transition.AddCondition(mode, threshold, parameter);
        }

        [MenuItem("Tools/Mia & Max/Apply Character FBX Import Profiles")]
        public static void ApplyImportProfiles()
        {
            int miaCount = ApplyProfile(MiaFolder, "Mia", ModelImporterAnimationType.Human);
            int maxCount = ApplyProfile(MaxFolder, "Max", ModelImporterAnimationType.Generic);
            bool miaPrefab = BuildRuntimePrefab("Mia", MiaFolder);
            bool maxPrefab = BuildRuntimePrefab("Max", MaxFolder);
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

        private static bool BuildRuntimePrefab(string characterName, string modelFolder)
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
            // CharacterController / companion-follow own world movement; clips animate in place.
            animator.applyRootMotion = false;
            CharacterAnimationBridge bridge = wrapper.GetComponent<CharacterAnimationBridge>();
            if (bridge == null) bridge = wrapper.AddComponent<CharacterAnimationBridge>();
            bridge.Configure(animator);

            string productionControllerPath = ControllerFolder + "/" + characterName + "_Production.controller";
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(productionControllerPath);
            if (controller == null)
            {
                Debug.LogWarning(characterName + " runtime prefab was not created because its production Animator Controller is missing. " +
                    "Import the complete action clip set and run Build Production Animator Controllers From Clips first.");
                Object.DestroyImmediate(wrapper);
                return false;
            }
            animator.runtimeAnimatorController = controller;

            Avatar importedAvatar = null;
            Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(selectedModelPath);
            foreach (Object subAsset in subAssets)
            {
                Avatar candidate = subAsset as Avatar;
                if (candidate != null) { importedAvatar = candidate; break; }
            }
            if (animator.avatar == null && importedAvatar != null) animator.avatar = importedAvatar;

            string prefabPath = resourcesFolder + "/" + characterName + ".prefab";
            // Never silently replace a manually reviewed or customized runtime prefab.
            // Rebuild only after an explicit backup/removal by the developer.
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            {
                Debug.LogWarning(characterName + " runtime prefab already exists at " + prefabPath +
                    ". It was preserved. Back it up and remove it explicitly before rebuilding from FBX.");
                Object.DestroyImmediate(wrapper);
                return false;
            }

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
            controller.AddParameter("Garden", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Reaction", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Jump", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Sleep", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Play", AnimatorControllerParameterType.Trigger);
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
