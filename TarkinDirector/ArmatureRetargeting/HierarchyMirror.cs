using UnityEngine;

namespace tarkin.Director.ArmatureRetargeting
{
    [ExecuteAlways]
    public class HierarchyMirror : MonoBehaviour
    {
        [SerializeField] private Transform sourceRoot;
        [SerializeField] private bool editModeOnly;
        [SerializeField] private bool skipRoot;

        void Update()
        {
            if (editModeOnly && Application.isPlaying)
                return;

            if (sourceRoot == null || sourceRoot == transform)
                return;

            MirrorTransforms(sourceRoot, transform);
        }

        private void MirrorTransforms(Transform source, Transform target)
        {
            if (!(skipRoot && target == transform))
            {
                target.localPosition = source.localPosition;
                target.localRotation = source.localRotation;
                target.localScale = source.localScale;
            }

            foreach (Transform sourceChild in source)
            {
                Transform targetChild = target.Find(sourceChild.name);

                if (targetChild != null)
                {
                    MirrorTransforms(sourceChild, targetChild);
                }
            }
        }
    }
}