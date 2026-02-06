using UnityEngine;

[ExecuteAlways]
public class HierarchyMirror : MonoBehaviour
{
    [SerializeField] private Transform sourceRoot;
    [SerializeField] private bool editModeOnly;

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
        target.localPosition = source.localPosition;
        target.localRotation = source.localRotation;
        target.localScale = source.localScale;

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