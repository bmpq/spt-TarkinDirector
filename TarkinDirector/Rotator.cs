using UnityEngine;

namespace tarkin.Director
{
    internal class Rotator : MonoBehaviour
    {
        public Vector3 axis;
        public float speed;

        void Start()
        {

        }

        void Update()
        {
            transform.Rotate(axis, speed * Time.deltaTime);
        }
    }
}
