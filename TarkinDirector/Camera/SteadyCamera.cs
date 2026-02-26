using UnityEngine;

namespace tarkin.Director
{
    [ExecuteAlways]
    public class SteadyCamera : MonoBehaviour
    {
        public Transform target;
        public float rotationSmoothSpeed = 5f;

        void Update()
        {
            transform.position = target.position;

            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, rotationSmoothSpeed * Time.deltaTime);
        }
    }
}
