using UnityEngine;
using UnityEditor;
using tarkin.Director.ArmatureRetargeting;

namespace tarkin.Director.EditorTools
{
    [CustomEditor(typeof(ArmatureMapping))]
    public class ArmatureMappingEditor : UnityEditor.Editor
    {
        private SerializedProperty boneMappingsProp;

        private void OnEnable()
        {
            boneMappingsProp = serializedObject.FindProperty("boneMappings");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_Script", "boneMappings");
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(boneMappingsProp);

            if (boneMappingsProp.isExpanded)
            {
                EditorGUI.indentLevel++;

                for (int i = 0; i < boneMappingsProp.arraySize; i++)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    SerializedProperty elementProp = boneMappingsProp.GetArrayElementAtIndex(i);

                    if (elementProp.objectReferenceValue != null)
                    {
                        SerializedObject nestedObject = new SerializedObject(elementProp.objectReferenceValue);
                        nestedObject.Update();

                        var boneTargetProp = nestedObject.FindProperty("boneTarget");
                        var overrideRotProp = nestedObject.FindProperty("overrideRotation");
                        var offsetRotProp = nestedObject.FindProperty("offsetRot");
                        var overrideLocProp = nestedObject.FindProperty("overrideLocation");
                        var offsetLocProp = nestedObject.FindProperty("offsetLoc");
                        var overrideSclProp = nestedObject.FindProperty("overrideScale");
                        var absoluteSclProp = nestedObject.FindProperty("absoluteScl");

                        EditorGUILayout.BeginHorizontal();
                        
                        using (new EditorGUI.DisabledScope(true))
                        {
                            EditorGUILayout.ObjectField(elementProp.objectReferenceValue, typeof(BoneMapping), false);
                        }
                        GUILayout.Box(new GUIContent($"", EditorGUIUtility.IconContent("d_Profiler.NextFrame").image), EditorStyles.miniLabel);

                        EditorGUILayout.PropertyField(boneTargetProp, GUIContent.none);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(overrideRotProp);
                        if (overrideRotProp.boolValue)
                        {
                            EditorGUILayout.PropertyField(offsetRotProp, GUIContent.none);
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(overrideLocProp);
                        if (overrideLocProp.boolValue)
                        {
                            EditorGUILayout.PropertyField(offsetLocProp, GUIContent.none);
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(overrideSclProp);
                        if (overrideSclProp.boolValue)
                        {
                            EditorGUILayout.PropertyField(absoluteSclProp, GUIContent.none);
                        }
                        EditorGUILayout.EndHorizontal();

                        nestedObject.ApplyModifiedProperties();
                    }

                    EditorGUILayout.EndVertical();
                }

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}