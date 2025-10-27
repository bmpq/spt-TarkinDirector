using System.Collections.Generic;
using UnityEngine;

namespace tarkin.BSP.Shared.ArmatureRetargeting
{
    public struct ResolvedBoneData
    {
        public bool IsMapped;
        public string TargetBoneName;
        public Vector3 RotationOffset;
        public Vector3 LocationOffset;
        public Vector3 LocalScale;
    }

    [CreateAssetMenu(fileName = "Map_", menuName = "ScriptableObjects/Armature Mapping")]
    public class ArmatureMapping : ScriptableObject
    {
        public Vector3 defaultOffsetRot = new Vector3(0, 0, 90);
        public Vector3 defaultOffsetLoc = Vector3.zero;
        public Vector3 defaultLocalScl = Vector3.one;

        public List<BoneMapping> boneMappings;

        private Dictionary<string, BoneMapping> _lookup;

        private void Initialize()
        {
            _lookup = new Dictionary<string, BoneMapping>();
            foreach (var mapping in boneMappings)
            {
                if (string.IsNullOrEmpty(mapping.boneSource)) continue;
                if (!_lookup.ContainsKey(mapping.boneSource))
                {
                    _lookup.Add(mapping.boneSource, mapping);
                }
                else
                {
                    Debug.LogWarning($"Duplicate source bone mapping found for '{mapping.boneSource}' in {this.name}. Only the first one will be used.", this);
                }
            }
        }

        public BoneMapping GetMapping(string sourceBoneName)
        {
#if UNITY_EDITOR

            Initialize();
#endif
            if (_lookup == null)
            {
                Initialize();
            }

            _lookup.TryGetValue(sourceBoneName, out BoneMapping mapping);
            return mapping;
        }

        public ResolvedBoneData GetResolvedBoneData(string sourceBoneName)
        {
            BoneMapping mapping = GetMapping(sourceBoneName);

            if (mapping == null)
            {
                return new ResolvedBoneData { IsMapped = false };
            }

            return new ResolvedBoneData
            {
                IsMapped = true,
                TargetBoneName = mapping.boneTarget,
                RotationOffset = mapping.overrideRotation ? mapping.offsetRot : this.defaultOffsetRot,
                LocationOffset = mapping.overrideLocation ? mapping.offsetLoc : this.defaultOffsetLoc,
                LocalScale = mapping.overrideScale ? mapping.absoluteScl : this.defaultLocalScl
            };
        }

        public static ResolvedBoneData GetPassThroughMapping(string sourceBoneName)
        {
            return new ResolvedBoneData
            {
                IsMapped = true,
                TargetBoneName = sourceBoneName,
                RotationOffset = Vector3.zero,
                LocationOffset = Vector3.zero,
                LocalScale = Vector3.one
            };
        }
    }
}