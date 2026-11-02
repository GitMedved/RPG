using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>Simple placeholder companion follow behaviour for Max.</summary>
    public sealed class MaxCompanionFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 followOffset = new Vector3(-1.3f, 0f, -1.2f);
        [SerializeField] private float followSpeed = 3.2f;
        [SerializeField] private float stopDistance = 1.15f;

        public void SetTarget(Transform value) => target = value;

        private void Update()
        {
            if (target == null) return;
            Vector3 destination = target.position + target.TransformDirection(followOffset);
            destination.y = transform.position.y;
            Vector3 delta = destination - transform.position;
            if (delta.magnitude <= stopDistance) return;

            transform.position += delta.normalized * followSpeed * Time.deltaTime;
            Vector3 look = target.position - transform.position;
            look.y = 0f;
            if (look.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look), 6f * Time.deltaTime);
        }
    }
}
