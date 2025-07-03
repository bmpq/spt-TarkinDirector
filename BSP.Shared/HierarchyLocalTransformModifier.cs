using UnityEngine;

namespace tarkin.BSP.Shared
{
    internal class HierarchyLocalTransformModifier : MonoBehaviour
    {
        public string path;

        private Transform target;
        private Vector3 initPos;
        private Quaternion initRot;
        private Vector3 initScale;

        void OnEnable()
        {
            target = GameObject.Find(path)?.transform;
            if (target != null)
            {
                initPos = target.localPosition;
                initRot = target.localRotation;
                initScale = target.localScale;

                target.localPosition = transform.localPosition;
                target.localRotation = transform.localRotation;
                target.localScale = transform.localScale;
            }
        }

        void OnDisable()
        {
            if (target != null)
            {
                target.localPosition = initPos;
                target.localRotation = initRot;
                target.localScale = initScale;
            }
        }
    }
}
