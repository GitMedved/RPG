using UnityEngine;
using MiaMax.Infrastructure.Save;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// Animator adapter for production character prefabs. Safe to add before an Animator
    /// Controller is assigned: it simply remains visually inert until a valid rig is imported.
    /// Animator parameters: Speed (float), Grounded (bool), Interact (trigger), Celebrate (trigger).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CharacterAnimationBridge : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Transform movementReference;
        [SerializeField] private float speedDampTime = 0.12f;
        [SerializeField] private float runSpeed = 3.4f;

        private static readonly int SpeedId = Animator.StringToHash("Speed");
        private static readonly int GroundedId = Animator.StringToHash("Grounded");
        private static readonly int InteractId = Animator.StringToHash("Interact");
        private static readonly int CelebrateId = Animator.StringToHash("Celebrate");

        private Vector3 previousPosition;
        private float smoothedSpeed;

        private void Awake()
        {
            if (animator == null) animator = GetComponentInChildren<Animator>();
            if (characterController == null) characterController = GetComponent<CharacterController>();
            if (movementReference == null) movementReference = transform;
            previousPosition = transform.position;
        }

        private void Update()
        {
            Vector3 delta = transform.position - previousPosition;
            previousPosition = transform.position;
            float measured = delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
            smoothedSpeed = Mathf.Lerp(smoothedSpeed, Mathf.Clamp01(measured / runSpeed), 1f - Mathf.Exp(-Time.deltaTime / Mathf.Max(0.01f, speedDampTime)));
            if (animator == null || !animator.isActiveAndEnabled) return;

            animator.SetFloat(SpeedId, smoothedSpeed, speedDampTime, Time.deltaTime);
            if (characterController != null)
                animator.SetBool(GroundedId, characterController.isGrounded);
        }

        public void PlayInteract()
        {
            if (IsReducedMotionEnabled()) return;
            if (animator != null && animator.isActiveAndEnabled) animator.SetTrigger(InteractId);
        }

        public void PlayCelebrate()
        {
            if (IsReducedMotionEnabled()) return;
            if (animator != null && animator.isActiveAndEnabled) animator.SetTrigger(CelebrateId);
        }

        private static bool IsReducedMotionEnabled()
        {
            GameSession session = GameSessionBootstrap.Current;
            return session != null && session.Data != null && session.Data.Settings != null &&
                   session.Data.Settings.ReducedMotion;
        }
    }
}
