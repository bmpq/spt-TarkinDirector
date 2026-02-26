using UnityEngine;

namespace tarkin.Director
{
    internal class FreeLookCamera : MonoBehaviour
    {
        [SerializeField] private float mouseSensitivity = 10f;

        private Vector3 currentRotation = Vector3.zero;

        void Update()
        {
            if (Input.GetMouseButton(2))
            {
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

                currentRotation.x -= mouseY;
                currentRotation.y += mouseX;

                transform.rotation = Quaternion.Euler(currentRotation);
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                transform.localRotation = Quaternion.identity;
            }
        }
    }
}
