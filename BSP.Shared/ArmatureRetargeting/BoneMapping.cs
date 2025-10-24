using UnityEngine;

namespace tarkin.BSP.Shared.ArmatureRetargeting
{
    [CreateAssetMenu(fileName = "", menuName = "ScriptableObjects/Bone Mapping")]
    public class BoneMapping : ScriptableObject
    {
        public string boneSource;
        public string boneTarget;

        public bool overrideRotation;
        public Vector3 offsetRot;

        public bool overrideLocation;
        public Vector3 offsetLoc;

        public bool overrideScale;
        public Vector3 absoluteScl = Vector3.one;
    }
}