#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Builds a simple, in-place Animator state machine from explicitly named imported clips.
    /// Missing clips are reported; this tool never fabricates animation assets.
    /// </summary>
    public static class CharacterAnimatorStateMachineBuilder
    {
        private const string RuntimeRoot = "Assets/_Project/Art/Characters/Runtime";
        private const string ControllerRoot = RuntimeRoot + "/Controllers";

        [MenuItem("Tools/Mia & Max/Build Character Animator State Machines")]
        public static void BuildBoth()
        {
            BuildCharacter("Mia", RuntimeRoot + "/Mia");
            BuildCharacter("Max", RuntimeRoot + "/Max");
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Mia & Max Animator Setup",
                "Animator state machine scaffolds were rebuilt. Missing clips are listed in the Console. " +
                "A state without a motion is a placeholder, not a finished animation.", "OK");
        }

        private static void BuildCharacter(string character, string clipFolder)
        {
            string controllerPath = ControllerRoot + "/" + character + "_AnimatorContract.controller";
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
            if (controller == null)
            {
                Debug.LogWarning("[M48] " + character + ": controller not found. Run Prepare Character Asset Pipeline first.");
                return;
            }

            EnsureParameter(controller, "Speed", AnimatorControllerParameterType.Float);
            EnsureParameter(controller, "Grounded", AnimatorControllerParameterType.Bool);
            EnsureParameter(controller, "Interact", AnimatorControllerParameterType.Trigger);
            EnsureParameter(controller, "Celebrate", AnimatorControllerParameterType.Trigger);
            EnsureParameter(controller, "Garden", AnimatorControllerParameterType.Trigger);
            EnsureParameter(controller, "Reaction", AnimatorControllerParameterType.Trigger);
            EnsureParameter(controller, "Jump", AnimatorControllerParameterType.Trigger);
            EnsureParameter(controller, "Sleep", AnimatorControllerParameterType.Trigger);
            EnsureParameter(controller, "Play", AnimatorControllerParameterType.Trigger);

            Dictionary<string, AnimationClip> clips = FindClips(clipFolder);
            AnimationClip idleClip = FindClip(clips, "idle");
            AnimationClip walkClip = FindClip(clips, "walk");
            AnimationClip runClip = FindClip(clips, "run");
            AnimationClip interactClip = FindClip(clips, "interact");
            AnimationClip celebrateClip = FindClip(clips, "celebrate");
            AnimationClip gardenClip = FindClip(clips, "garden");
            AnimationClip reactionClip = FindClip(clips, "reaction");
            AnimationClip jumpClip = FindClip(clips, "jump");
            AnimationClip sleepClip = FindClip(clips, "sleep");
            AnimationClip playClip = FindClip(clips, "play");

            AnimatorStateMachine machine = controller.layers[0].stateMachine;
            AnimatorState[] oldStates = new AnimatorState[machine.states.Length];
            for (int i = 0; i < machine.states.Length; i++) oldStates[i] = machine.states[i].state;
            for (int i = 0; i < oldStates.Length; i++) machine.RemoveState(oldStates[i]);
            AnimatorStateTransition[] oldAny = machine.anyStateTransitions;
            for (int i = 0; i < oldAny.Length; i++) machine.RemoveAnyStateTransition(oldAny[i]);

            AnimatorState idle = AddState(machine, "Idle", idleClip, new Vector3(260f, 80f, 0f));
            AnimatorState walk = walkClip != null ? AddState(machine, "Walk", walkClip, new Vector3(480f, 80f, 0f)) : null;
            AnimatorState run = runClip != null ? AddState(machine, "Run", runClip, new Vector3(700f, 80f, 0f)) : null;
            AnimatorState interact = interactClip != null ? AddState(machine, "Interact", interactClip, new Vector3(480f, 240f, 0f)) : null;
            AnimatorState celebrate = celebrateClip != null ? AddState(machine, "Celebrate", celebrateClip, new Vector3(700f, 240f, 0f)) : null;
            AnimatorState garden = gardenClip != null ? AddState(machine, "Garden", gardenClip, new Vector3(920f, 240f, 0f)) : null;
            AnimatorState reaction = reactionClip != null ? AddState(machine, "Reaction", reactionClip, new Vector3(1140f, 240f, 0f)) : null;
            AnimatorState jump = jumpClip != null ? AddState(machine, "Jump", jumpClip, new Vector3(920f, 400f, 0f)) : null;
            AnimatorState sleep = sleepClip != null ? AddState(machine, "Sleep", sleepClip, new Vector3(1140f, 400f, 0f)) : null;
            AnimatorState playful = playClip != null ? AddState(machine, "Play", playClip, new Vector3(1360f, 400f, 0f)) : null;
            machine.defaultState = idle;

            if (walk != null)
            {
                AddCondition(idle.AddTransition(walk), AnimatorConditionMode.Greater, 0.08f, "Speed");
                AddCondition(walk.AddTransition(idle), AnimatorConditionMode.Less, 0.08f, "Speed");
            }
            if (run != null && walk != null)
            {
                AddCondition(walk.AddTransition(run), AnimatorConditionMode.Greater, 0.65f, "Speed");
                AddCondition(run.AddTransition(walk), AnimatorConditionMode.Less, 0.65f, "Speed");
            }
            else if (run != null)
            {
                AddCondition(idle.AddTransition(run), AnimatorConditionMode.Greater, 0.65f, "Speed");
                AddCondition(run.AddTransition(idle), AnimatorConditionMode.Less, 0.65f, "Speed");
            }

            AddActionTransitions(machine, interact, idle, "Interact");
            AddActionTransitions(machine, celebrate, idle, "Celebrate");
            AddActionTransitions(machine, garden, idle, "Garden");
            AddActionTransitions(machine, reaction, idle, "Reaction");
            AddActionTransitions(machine, jump, idle, "Jump");
            AddActionTransitions(machine, sleep, idle, "Sleep");
            AddActionTransitions(machine, playful, idle, "Play");

            EditorUtility.SetDirty(controller);
            Debug.Log("[M48] " + character + " Animator built. Clips found — Idle: " + Has(idleClip) +
                ", Walk: " + Has(walkClip) + ", Run: " + Has(runClip) +
                ", Interact: " + Has(interactClip) + ", Celebrate: " + Has(celebrateClip) +
                ", Garden: " + Has(gardenClip) + ", Reaction: " + Has(reactionClip) +
                ", Jump: " + Has(jumpClip) + ", Sleep: " + Has(sleepClip) + ", Play: " + Has(playClip) +
                ". Missing clips remain unbuilt; Idle uses an empty placeholder if absent.");
        }

        private static AnimatorState AddState(AnimatorStateMachine machine, string name,
            AnimationClip clip, Vector3 position)
        {
            AnimatorState state = machine.AddState(name, position);
            state.motion = clip;
            return state;
        }

        private static void AddActionTransitions(AnimatorStateMachine machine, AnimatorState action,
            AnimatorState returnState, string trigger)
        {
            if (action == null) return;
            AnimatorStateTransition enter = machine.AddAnyStateTransition(action);
            enter.hasExitTime = false;
            enter.duration = 0.12f;
            enter.canTransitionToSelf = false;
            enter.AddCondition(AnimatorConditionMode.If, 0f, trigger);

            AnimatorStateTransition exit = action.AddTransition(returnState);
            exit.hasExitTime = true;
            exit.exitTime = 0.9f;
            exit.duration = 0.15f;
        }

        private static void AddCondition(AnimatorStateTransition transition,
            AnimatorConditionMode mode, float threshold, string parameter)
        {
            transition.hasExitTime = false;
            transition.duration = 0.12f;
            transition.AddCondition(mode, threshold, parameter);
        }

        private static void EnsureParameter(AnimatorController controller, string name,
            AnimatorControllerParameterType type)
        {
            foreach (AnimatorControllerParameter parameter in controller.parameters)
                if (parameter.name == name && parameter.type == type) return;
            controller.AddParameter(name, type);
        }

        private static Dictionary<string, AnimationClip> FindClips(string folder)
        {
            var clips = new Dictionary<string, AnimationClip>();
            if (!AssetDatabase.IsValidFolder(folder)) return clips;
            string[] guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { folder });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (Object asset in assets)
                {
                    AnimationClip clip = asset as AnimationClip;
                    if (clip != null && !clip.name.StartsWith("__preview__", System.StringComparison.OrdinalIgnoreCase))
                        clips[clip.name.ToLowerInvariant()] = clip;
                }
            }
            return clips;
        }

        private static AnimationClip FindClip(Dictionary<string, AnimationClip> clips, string keyword)
        {
            foreach (KeyValuePair<string, AnimationClip> pair in clips)
                if (pair.Key == keyword) return pair.Value;
            foreach (KeyValuePair<string, AnimationClip> pair in clips)
                if (pair.Key.Contains(keyword)) return pair.Value;
            return null;
        }

        private static string Has(AnimationClip clip) { return clip != null ? clip.name : "missing"; }
    }
}
#endif
