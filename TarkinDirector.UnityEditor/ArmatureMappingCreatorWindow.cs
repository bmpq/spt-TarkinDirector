using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using tarkin.BSP.Shared.ArmatureRetargeting;
using JetBrains.Annotations;

public class BoneMapperWindow : EditorWindow
{
    private Transform _sourceRoot;
    private Transform _targetRoot;

    [System.Serializable]
    private class BonePair
    {
        public Transform source;
        public Transform target;
        public int hierarchyDepth;
    }

    [SerializeField]
    private List<BonePair> _bonePairs = new List<BonePair>();

    private Vector2 _scrollPosition;

    private string _savePath = "Assets/_Projects/BSP/BSP.Shared/ArmatureRetargeting/Mappings/NewMapping/";

    private struct BoneAssignment
    {
        public BonePair pair;
        public Transform targetBone;
    }

    [MenuItem("Tools/Armature Bone Mapper")]
    public static void ShowWindow()
    {
        GetWindow<BoneMapperWindow>("Bone Mapper");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Armature Bone Mapper", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        _sourceRoot = (Transform)EditorGUILayout.ObjectField("Source Root", _sourceRoot, typeof(Transform), true);
        if (EditorGUI.EndChangeCheck())
        {
            PopulateBoneList();
        }

        _targetRoot = (Transform)EditorGUILayout.ObjectField("Target Root (for suggestions)", _targetRoot, typeof(Transform), true);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Bone Mappings", EditorStyles.boldLabel);

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));

        if (_bonePairs.Count == 0)
        {
            EditorGUILayout.LabelField("Assign a Source Root to see bone list.");
        }
        else
        {
            var rowStyle = new GUIStyle(EditorStyles.toolbarButton);
            int originalIndent = EditorGUI.indentLevel;

            for (int i = 0; i < _bonePairs.Count; i++)
            {
                var pair = _bonePairs[i];
                EditorGUI.indentLevel = originalIndent + pair.hierarchyDepth;

                EditorGUILayout.BeginHorizontal(rowStyle);

                Color prevColor = GUI.color;
                if (pair.target == null)
                    GUI.color = Color.yellow;

                // Source Bone (read-only)
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField(pair.source, typeof(Transform), true, GUILayout.MinWidth(100));
                EditorGUI.EndDisabledGroup();

                int targetBoneDepthIndent = 0;
                if (pair.target != null && pair.target != _targetRoot)
                {
                    targetBoneDepthIndent++;
                    Transform parent = pair.target.parent;
                    while (parent != _targetRoot && parent != null)
                    {
                        targetBoneDepthIndent++;
                        parent = parent.parent;
                    }
                }

                EditorGUI.indentLevel = originalIndent + targetBoneDepthIndent;

                pair.target = (Transform)EditorGUILayout.ObjectField(pair.target, typeof(Transform), true, GUILayout.MinWidth(100));

                GUI.color = prevColor;

                // Suggestion Button
                Transform suggestionSource = null;
                bool canShowButton = false;

                if (pair.hierarchyDepth == 0)
                {
                    if (_targetRoot != null)
                    {
                        suggestionSource = _targetRoot;
                        canShowButton = true;
                    }
                }
                else
                {
                    BonePair parentPair = FindParentPair(i);
                    if (parentPair != null && parentPair.target != null)
                    {
                        suggestionSource = parentPair.target;
                        canShowButton = true;
                    }
                }

                if (canShowButton)
                {
                    if (GUILayout.Button(
                        new GUIContent("", EditorGUIUtility.IconContent("d_icon dropdown@2x").image), EditorStyles.iconButton, GUILayout.Width(22)))
                    {
                        ShowSuggestionsMenu(pair, suggestionSource);
                    }
                }
                else
                {
                    GUILayout.Space(26);
                }

                EditorGUI.BeginDisabledGroup(pair.target == null);
                if (GUILayout.Button(
                    new GUIContent("", EditorGUIUtility.IconContent("d_winbtn_win_close").image), EditorStyles.iconButton, 
                    GUILayout.Width(22)))
                {
                    Undo.RecordObject(this, "Clear Bone Target");
                    pair.target = null;
                    GUI.FocusControl(null);
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel = originalIndent;
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Output Settings", EditorStyles.boldLabel);
        _savePath = EditorGUILayout.TextField("Save Path", _savePath);

        if (GUILayout.Button("Create BoneMapping Assets"))
        {
            CreateMappingAssets();
        }
    }

    private BonePair FindParentPair(int childIndex)
    {
        if (childIndex <= 0 || childIndex >= _bonePairs.Count)
            return null;

        int childDepth = _bonePairs[childIndex].hierarchyDepth;
        for (int i = childIndex - 1; i >= 0; i--)
        {
            if (_bonePairs[i].hierarchyDepth == childDepth - 1)
            {
                return _bonePairs[i];
            }
        }
        return null;
    }

    private void ShowSuggestionsMenu(BonePair currentPair, Transform suggestionSource)
    {
        if (suggestionSource == null) return;

        GenericMenu menu = new GenericMenu();

        var immediateChildren = new List<Transform>();
        foreach (Transform child in suggestionSource)
        {
            immediateChildren.Add(child);
        }

        Transform bestGuess = immediateChildren.FirstOrDefault(t => t.name == currentPair.source.name);
        if (bestGuess != null)
        {
            menu.AddItem(new GUIContent($"Best Guess: {bestGuess.name}"), false, AssignTarget, new BoneAssignment { pair = currentPair, targetBone = bestGuess });
            menu.AddSeparator("");
        }

        menu.AddItem(new GUIContent("None"), currentPair.target == null, AssignTarget, new BoneAssignment { pair = currentPair, targetBone = null });
        menu.AddSeparator("");

        foreach (var targetBone in immediateChildren)
        {
            menu.AddItem(new GUIContent(targetBone.name),
                         currentPair.target == targetBone,
                         AssignTarget,
                         new BoneAssignment { pair = currentPair, targetBone = targetBone });
        }

        if (menu.GetItemCount() <= 2)
        {
            menu.AddDisabledItem(new GUIContent("No children to suggest"));
        }

        menu.ShowAsContext();
    }

    private void AssignTarget(object userData)
    {
        var assignment = (BoneAssignment)userData;
        Undo.RecordObject(this, "Assign Bone Target");
        assignment.pair.target = assignment.targetBone;
        Repaint();
    }

    private void PopulateBoneList()
    {
        Undo.RecordObject(this, "Populate Bone List");
        _bonePairs.Clear();
        if (_sourceRoot == null)
        {
            Repaint();
            return;
        }
        AddChildrenRecursively(_sourceRoot, 0);
    }

    private void AddChildrenRecursively(Transform parent, int depth)
    {
        BonePair newPair = new BonePair
        {
            source = parent,
            target = null,
            hierarchyDepth = depth
        };

        if (newPair.source == _sourceRoot)
            newPair.target = _targetRoot;

        _bonePairs.Add(newPair);

        foreach (Transform child in parent)
        {
            AddChildrenRecursively(child, depth + 1);
        }
    }

    private void CreateMappingAssets()
    {
        if (_bonePairs.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "No source bones to map. Please assign a Source Root.", "OK");
            return;
        }
        if (!Directory.Exists(_savePath))
        {
            Directory.CreateDirectory(_savePath);
        }
        int createdCount = 0;
        AssetDatabase.StartAssetEditing();
        try
        {
            foreach (var pair in _bonePairs)
            {
                if (pair.source != null && pair.target != null)
                {
                    BoneMapping mapping = ScriptableObject.CreateInstance<BoneMapping>();
                    mapping.boneSource = pair.source.name;
                    mapping.boneTarget = pair.target.name;

                    string fileName = $"BM_{SanitizeFileName(pair.source.name)}.asset";
                    string assetPath = Path.Combine(_savePath, fileName);
                    assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

                    AssetDatabase.CreateAsset(mapping, assetPath);
                    createdCount++;
                }
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Success", $"Successfully created {createdCount} BoneMapping assets in '{_savePath}'.", "OK");
        Object folder = AssetDatabase.LoadAssetAtPath(_savePath, typeof(Object));
        if (folder != null)
        {
            EditorGUIUtility.PingObject(folder);
        }
    }

    private string SanitizeFileName(string name)
    {
        string invalidChars = new string(Path.GetInvalidFileNameChars());
        foreach (char c in invalidChars)
        {
            name = name.Replace(c.ToString(), "");
        }
        return name;
    }
}