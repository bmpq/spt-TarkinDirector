using UnityEditor;
using System;

namespace tarkin.Director.EditorTools
{
    [CustomEditor(typeof(EffectEmitter))]
    public class EffectEmitterEditor : UnityEditor.Editor
    {
        private readonly string[] _effectNames = new string[]
        {
            "Generic Soft",
            "BodyArmor",
            "Concrete",
            "Wood",
            "Metal",
            "MetalNoDecal",
            "Helmet",
            "HelmetRicochet",
            "Sparks",
            "Plastic",
            "Glass",
            "Gaslamp",
            "Lamp",
            "Flashbang",
            "Body",
            "Soil",
            "Water",
            "Swamp",
            "Grenade",
            "Grenade_indoor",
            "Grenade_test",
            "Grenade_new",
            "Artillery_mine",
            "big_explosion",
            "landmine",
            "big_round_impact",
            "big_round_impact_explosive",
            "big_smoky_explosion",
            "smallgrenade_expl",
            "spg_explosion",
            "DefaultNoDecal",
            "Grenade_new2",
            "Fire_extinguished",
            "Gas_explosion"
        };

        private SerializedProperty nameProp;
        private SerializedProperty magnitudeProp;
        private SerializedProperty volumeProp;
        private SerializedProperty drawDecalProp;
        private SerializedProperty intervalProp;

        private void OnEnable()
        {
            nameProp = serializedObject.FindProperty("Name");
            magnitudeProp = serializedObject.FindProperty("Magnitude");
            volumeProp = serializedObject.FindProperty("Volume");
            drawDecalProp = serializedObject.FindProperty("DrawDecal");
            intervalProp = serializedObject.FindProperty("Interval");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            string currentName = nameProp.stringValue;
            int currentIndex = Array.IndexOf(_effectNames, currentName);

            if (currentIndex < 0)
            {
                currentIndex = 0;
            }

            int newIndex = EditorGUILayout.Popup("Name", currentIndex, _effectNames);

            if (newIndex != currentIndex)
            {
                nameProp.stringValue = _effectNames[newIndex];
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(magnitudeProp);
            EditorGUILayout.PropertyField(volumeProp);
            EditorGUILayout.PropertyField(drawDecalProp);
            EditorGUILayout.PropertyField(intervalProp);

            serializedObject.ApplyModifiedProperties();
        }
    }
}