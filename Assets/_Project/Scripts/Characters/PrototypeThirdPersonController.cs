using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// Temporary blockout controller. Replace primitive Mia with the approved rigged model later.
    /// Uses legacy Input Manager axes so the prototype has no external package dependency.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class PrototypeThirdPersonController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 4.2f;
        [SerializeField] private float turnSpeed = 12f;
        [SerializeField] private Transform cameraTransform;

        private CharacterController controller;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (cameraTransform == null && Camera.main != null)
                cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            Vector2 input = ReadMovement();
            Vector3 forward = cameraTransform != null ? cameraTransform.forward : Vector3.forward;
            Vector3 right = cameraTransform != null ? cameraTransform.right : Vector3.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 direction = (forward * input.y + right * input.x);
            if (direction.sqrMagnitude > 1f) direction.Normalize();

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion target = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, turnSpeed * Time.deltaTime);
            }

            if (controller.isGrounded && controller.velocity.y < 0f)
                controller.Move(Vector3.down * 2f * Time.deltaTime);

            controller.Move((direction * moveSpeed + Vector3.up * controller.velocity.y) * Time.deltaTime);
        }

        private static Vector2 ReadMovement()
        {
            Vector2 keyboard = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (keyboard.sqrMagnitude > 0.01f) return Vector2.ClampMagnitude(keyboard, 1f);

            if (Input.touchCount == 0) return Vector2.zero;
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) return Vector2.zero;

            Vector2 delta = touch.position - touch.rawPosition;
            return Vector2.ClampMagnitude(delta / 120f, 1f);
        }
    }
}
