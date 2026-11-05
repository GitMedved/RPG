using UnityEngine;
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

        private bool hasSpeed;
        private bool hasGrounded;
        private bool hasInteract;
        private bool hasCelebrate;
        private Vector3 previousPosition;
        private float smoothedSpeed;

        private void Awake()
        {
            if (characterController == null) characterController = GetComponent<CharacterController>();
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

            if (animator == null || !animator.isActiveAndEnabled) return;
            if (animator.runtimeAnimatorController == null)
            {
                // Supports a controller assigned after the character prefab was instantiated.
                BindAnimator();
                if (animator == null || animator.runtimeAnimatorController == null) return;
            }

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

        private static bool IsReducedMotionEnabled()
        {
            GameSession session = GameSessionBootstrap.Current;
            return session != null && session.Data != null && session.Data.Settings != null &&
                   session.Data.Settings.ReducedMotion;
        }
    }
}