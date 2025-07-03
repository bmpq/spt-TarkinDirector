using UnityEngine;

namespace tarkin.BSP.Shared
{
    [DefaultExecutionOrder(10)]
    internal class HierarchyChild : MonoBehaviour
    {
        public string path;

        void Start()
        {
            GameObject parent = GameObject.Find(path);
            if (parent != null)
                transform.SetParent(parent.transform, true);
        }
    }
}
