
using UnityEngine;

namespace tarkin.Director
{
    [ExecuteAlways]
    public class LookAtCamera : MonoBehaviour
    {
        public Transform target;

        [Range(0.1f, 50f)]
        public float smoothness = 5f;

        void LateUpdate()
        {
            if (target == null) return;

            Vector3 direction = target.position - transform.position;

            if (direction == Vector3.zero) return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothness * Time.deltaTime);
        }
    }
}
