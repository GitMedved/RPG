using UnityEngine;
using MiaMax.Domain;
using MiaMax.Infrastructure.Save;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// Safe adapter between character movement roots and imported production Animators.
    /// The gameplay controller owns movement; imported clips must not move the root.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CharacterAnimationBridge : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private float speedDampTime = 0.12f;
        [SerializeField] private float runSpeed = 3.4f;

        private static readonly int SpeedId = Animator.StringToHash("Speed");
        private static readonly int GroundedId = Animator.StringToHash("Grounded");
        private static readonly int InteractId = Animator.StringToHash("Interact");
        private static readonly int CelebrateId = Animator.StringToHash("Celebrate");
        private static readonly int GardenId = Animator.StringToHash("Garden");
        private static readonly int ReactionId = Animator.StringToHash("Reaction");
        private static readonly int JumpId = Animator.StringToHash("Jump");
        private static readonly int SleepId = Animator.StringToHash("Sleep");
        private static readonly int PlayId = Animator.StringToHash("Play");

        private bool hasSpeed;
        private bool hasGrounded;
        private bool hasInteract;
        private bool hasCelebrate;
        private bool hasGarden;
        private bool hasReaction;
        private bool hasJump;
        private bool hasSleep;
        private bool hasPlay;
        private Vector3 previousPosition;
        private float smoothedSpeed;
        private RuntimeAnimatorController boundController;

        /// <summary>Allows an editor-built runtime prefab to bind its imported Animator explicitly.</summary>
        public void Configure(Animator target, CharacterController movementController = null)
        {
            animator = target;
            characterController = movementController;
            BindAnimator();
            previousPosition = transform.position;
        }

        private void Awake()
        {
            if (characterController == null) characterController = GetComponentInParent<CharacterController>();
            BindAnimator();
            previousPosition = transform.position;
        }

        private void BindAnimator()
        {
            if (animator == null) animator = GetComponentInChildren<Animator>(true);
            if (animator == null) return;

            // Root motion would fight the CharacterController and Max's companion-follow logic.
            animator.applyRootMotion = false;
            hasSpeed = HasParameter(animator, SpeedId, AnimatorControllerParameterType.Float);
            hasGrounded = HasParameter(animator, GroundedId, AnimatorControllerParameterType.Bool);
            hasInteract = HasParameter(animator, InteractId, AnimatorControllerParameterType.Trigger);
            hasCelebrate = HasParameter(animator, CelebrateId, AnimatorControllerParameterType.Trigger);
            hasGarden = HasParameter(animator, GardenId, AnimatorControllerParameterType.Trigger);
            hasReaction = HasParameter(animator, ReactionId, AnimatorControllerParameterType.Trigger);
            hasJump = HasParameter(animator, JumpId, AnimatorControllerParameterType.Trigger);
            hasSleep = HasParameter(animator, SleepId, AnimatorControllerParameterType.Trigger);
            hasPlay = HasParameter(animator, PlayId, AnimatorControllerParameterType.Trigger);
            boundController = animator.runtimeAnimatorController;
        }

        private static bool HasParameter(Animator target, int id, AnimatorControllerParameterType type)
        {
            if (target == null || target.runtimeAnimatorController == null) return false;
            AnimatorControllerParameter[] parameters = target.parameters;
            for (int i = 0; i < parameters.Length; i++)
                if (parameters[i].nameHash == id && parameters[i].type == type) return true;
            return false;
        }

        private void Update()
        {
            Vector3 delta = transform.position - previousPosition;
            previousPosition = transform.position;
            float measured = delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
            float damping = Mathf.Max(0.01f, speedDampTime);
            smoothedSpeed = Mathf.Lerp(smoothedSpeed, Mathf.Clamp01(measured / Mathf.Max(0.1f, runSpeed)),
                1f - Mathf.Exp(-Time.deltaTime / damping));

            if (animator == null)
            {
                // Supports an Animator added to a model after the bridge was instantiated.
                BindAnimator();
                if (animator == null) return;
            }
            else if (animator.runtimeAnimatorController != boundController)
            {
                // Re-cache parameter hashes when a controller is replaced at runtime.
                BindAnimator();
            }

            if (!animator.isActiveAndEnabled || animator.runtimeAnimatorController == null) return;

            if (hasSpeed) animator.SetFloat(SpeedId, smoothedSpeed, damping, Time.deltaTime);
            if (hasGrounded)
                // Max is moved by companion-follow rather than a CharacterController.
                animator.SetBool(GroundedId, characterController == null || characterController.isGrounded);
        }

        public void PlayInteract()
        {
            if (IsReducedMotionEnabled()) return;
            if (animator != null && animator.isActiveAndEnabled && hasInteract)
                animator.SetTrigger(InteractId);
        }

        public void PlayCelebrate()
        {
            if (IsReducedMotionEnabled()) return;
            if (animator != null && animator.isActiveAndEnabled && hasCelebrate)
                animator.SetTrigger(CelebrateId);
        }

        public void PlayGarden() { PlayAction(GardenId, hasGarden); }
        public void PlayReaction() { PlayAction(ReactionId, hasReaction); }
        public void PlayJump() { PlayAction(JumpId, hasJump); }
        public void PlaySleep() { PlayAction(SleepId, hasSleep); }
        public void PlayPlayfulAction() { PlayAction(PlayId, hasPlay); }

        private void PlayAction(int triggerId, bool supported)
        {
            if (IsReducedMotionEnabled()) return;
            if (animator != null && animator.isActiveAndEnabled && supported)
                animator.SetTrigger(triggerId);
        }

        private static bool IsReducedMotionEnabled()
        {
            GameSession session = GameSessionBootstrap.Current;
            return session != null && session.Data != null && session.Data.Settings != null &&
                   session.Data.Settings.ReducedMotion;
        }
    }
}