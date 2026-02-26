using System.IO;
using UnityEditor;
using tarkin.Director.ArmatureRetargeting;

namespace tarkin.Director.EditorTools
{
    [InitializeOnLoad]
    public static class ArmatureMappingFilenameSync
    {
        static ArmatureMappingFilenameSync()
        {
            EditorApplication.projectChanged += UpdateFilenames;
        }

        static void UpdateFilenames()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(BoneMapping)}");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<BoneMapping>(path);
                string name = Path.GetFileNameWithoutExtension(path);

                if (name.Contains("=="))
                {
                    string[] mapping = name.Split(new[] { "==" }, System.StringSplitOptions.None);
                    asset.boneSource = mapping[0];
                    asset.boneTarget = mapping[1];
                    EditorUtility.SetDirty(asset);
                }
            }
        }
    }
}
