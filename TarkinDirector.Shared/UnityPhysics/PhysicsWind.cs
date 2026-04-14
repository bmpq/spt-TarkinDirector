using UnityEngine;

namespace tarkin.Director.Shared
{
    public class PhysicsWind : MonoBehaviour
    {
        public float windForce = 100f;
        public float radius = 2f;
        public float falloffDistance = 20f;
        public LayerMask layerMask = ~0;
        public int maxHits = 64;

        private Collider[] _hitColliders;

        private void Awake()
        {
            _hitColliders = new Collider[maxHits];
        }

        private void FixedUpdate()
        {
            ApplyWindForce();
        }

        private void ApplyWindForce()
        {
            Vector3 startPoint = transform.position;
            Vector3 endPoint = transform.position + transform.forward * falloffDistance;

            int hitCount = Physics.OverlapCapsuleNonAlloc(startPoint, endPoint, radius, _hitColliders, layerMask);

            for (int i = 0; i < hitCount; i++)
            {
                Rigidbody rb = _hitColliders[i].attachedRigidbody;

                if (rb != null && !rb.isKinematic)
                {
                    Vector3 offset = rb.position - transform.position;
                    float distanceAlongForward = Vector3.Dot(offset, transform.forward);

                    distanceAlongForward = Mathf.Clamp(distanceAlongForward, 0f, falloffDistance);

                    float falloffMultiplier = 1f - (distanceAlongForward / falloffDistance);

                    Vector3 appliedForce = transform.forward * windForce * falloffMultiplier;

                    rb.AddForce(appliedForce, ForceMode.Force);
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.5f);

            Vector3 start = transform.position;
            Vector3 end = start + transform.forward * falloffDistance;

            Gizmos.DrawWireSphere(start, radius);
            Gizmos.DrawWireSphere(end, radius);

            Vector3 up = transform.up * radius;
            Vector3 right = transform.right * radius;

            Gizmos.DrawLine(start + up, end + up);
            Gizmos.DrawLine(start - up, end - up);
            Gizmos.DrawLine(start + right, end + right);
            Gizmos.DrawLine(start - right, end - right);

            Gizmos.color = new Color(0.2f, 0.8f, 1f, 1f);
            Gizmos.DrawLine(start, end);
        }
#endif
    }
}