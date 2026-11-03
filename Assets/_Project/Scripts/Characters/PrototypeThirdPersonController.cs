using UnityEngine;
using MiaMax.Domain.Progression;
using MiaMax.Infrastructure.Save;

namespace MiaMax.Gameplay
{
    /// <summary>Greybox movement controller. Keyboard in Editor; virtual joystick on touch devices.</summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class PrototypeThirdPersonController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 4.2f;
        [SerializeField] private float turnSpeed = 12f;
        [SerializeField] private float gravity = -22f;
        [SerializeField] private Transform cameraTransform;

        private CharacterController controller;
        private float verticalVelocity;
        private TutorialService tutorial;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (cameraTransform == null && Camera.main != null)
                cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            Vector2 input = ReadMovement();
            if (tutorial == null && GameSessionBootstrap.Current != null)
                tutorial = new TutorialService(GameSessionBootstrap.Current);
            if (input.sqrMagnitude > 0.01f && tutorial != null)
                tutorial.MarkMovementSeen();
            Vector3 forward = cameraTransform != null ? cameraTransform.forward : Vector3.forward;
            Vector3 right = cameraTransform != null ? cameraTransform.right : Vector3.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 direction = forward * input.y + right * input.x;
            if (direction.sqrMagnitude > 1f) direction.Normalize();

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion target = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, turnSpeed * Time.deltaTime);
            }

            if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
            else verticalVelocity += gravity * Time.deltaTime;

            Vector3 velocity = direction * moveSpeed + Vector3.up * verticalVelocity;
            controller.Move(velocity * Time.deltaTime);
        }

        private static Vector2 ReadMovement()
        {
            Vector2 keyboard = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (keyboard.sqrMagnitude > 0.01f) return Vector2.ClampMagnitude(keyboard, 1f);
            return PrototypeMobileJoystick.Instance != null ? PrototypeMobileJoystick.Instance.Value : Vector2.zero;
        }
    }
}
