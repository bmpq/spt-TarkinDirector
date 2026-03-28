using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace tarkin.Director.ArmatureRetargeting
{
    [ExecuteAlways]
    public class ArmatureRetarget : MonoBehaviour
    {
        [SerializeField] private Transform sourceArmatureRoot;
        [SerializeField] private Transform destinationArmatureRoot;
        [SerializeField] private ArmatureMapping mapping;

        [Space(10)]
        public Vector3 GlobalRotationOffset;
        public Vector3 GlobalLocationOffset;

        private void Update()
        {
            if (destinationArmatureRoot == null)
                return;

            var allDestBones = destinationArmatureRoot.GetComponentsInChildren<Transform>();

            foreach (Transform sourceBone in sourceArmatureRoot.GetComponentsInChildren<Transform>())
            {
                ResolvedBoneData boneData;
                if (mapping == null)
                    boneData = ArmatureMapping.GetPassThroughMapping(sourceBone.name);
                else
                    boneData = mapping.GetResolvedBoneData(sourceBone.name);
                if (!boneData.IsMapped)
                    continue;

                Transform destBone = allDestBones.FirstOrDefault(d => d.name == boneData.TargetBoneName);

                if (destBone == null)
                    continue;

                destBone.transform.position = sourceBone.TransformPoint(boneData.LocationOffset + GlobalLocationOffset);
                destBone.rotation = sourceBone.rotation * Quaternion.Euler(boneData.RotationOffset + GlobalRotationOffset);
                Vector3 scl = sourceBone.localScale;
                scl.Scale(boneData.LocalScale);
                destBone.localScale = scl;
            }
        }

        public static void SetWorldScale(Transform t, Vector3 worldScale)
        {
            if (t.parent == null)
            {
                t.localScale = worldScale;
            }
            else
            {
                Vector3 parentScale = t.parent.lossyScale;
                t.localScale = new Vector3(
                    worldScale.x / parentScale.x,
                    worldScale.y / parentScale.y,
                    worldScale.z / parentScale.z
                );
            }
        }
    }

}