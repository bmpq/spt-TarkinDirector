using UnityEngine;

namespace tarkin.BSP.Shared
{
    [ExecuteAlways]
    [RequireComponent(typeof(Rigidbody))]
    internal class PhysicalOnDemand : MonoBehaviour
    {
        [SerializeField] private Transform virtualParent;

        private Rigidbody _rb;
        private Rigidbody rb { get { if (_rb == null) _rb = GetComponent<Rigidbody>(); return _rb; } }

        private Vector3 _lastPosition;
        private Quaternion _lastRotation;

        private Vector3 _trackedVelocity;
        private Vector3 _trackedAngularVelocity;

        void OnEnable()
        {
            ResetToKinematic();
        }

        private void OnValidate()
        {
            ResetToKinematic();
        }

        public void Trigger()
        {
            if (!rb.isKinematic)
                return;

            rb.isKinematic = false;

            rb.velocity = _trackedVelocity;
            rb.angularVelocity = _trackedAngularVelocity;
        }

        public void ResetToKinematic()
        {
            rb.isKinematic = true;

            if (virtualParent != null)
            {
                transform.position = virtualParent.position;
                transform.rotation = virtualParent.rotation;
                _lastPosition = virtualParent.position;
                _lastRotation = virtualParent.rotation;
            }
            _trackedVelocity = Vector3.zero;
            _trackedAngularVelocity = Vector3.zero;
        }

        void LateUpdate()
        {
            if (!rb.isKinematic || virtualParent == null)
                return;

            float deltaTime = Time.deltaTime;
            if (deltaTime > 0)
            {
                _trackedVelocity = (virtualParent.position - _lastPosition) / deltaTime;

                Quaternion deltaRotation = virtualParent.rotation * Quaternion.Inverse(_lastRotation);
                deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);

                angle = Mathf.DeltaAngle(0, angle);

                Vector3 angularVelocity = axis * (angle * Mathf.Deg2Rad) / deltaTime;
                _trackedAngularVelocity = angularVelocity;
            }

            transform.position = virtualParent.position;
            transform.rotation = virtualParent.rotation;
            transform.localScale = virtualParent.lossyScale;

            _lastPosition = virtualParent.position;
            _lastRotation = virtualParent.rotation;
        }
    }
}