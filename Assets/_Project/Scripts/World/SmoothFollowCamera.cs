using UnityEngine;

namespace MiaMax.Gameplay
{
    public sealed class SmoothFollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 7.5f, -8.5f);
        [SerializeField] private float smoothTime = 0.18f;
        [SerializeField] private float lookHeight = 1.2f;
        private Vector3 velocity;

        public void SetTarget(Transform value) => target = value;

        private void LateUpdate()
        {
            if (target == null) return;
            Vector3 desired = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
            transform.LookAt(target.position + Vector3.up * lookHeight);
        }
    }
}
