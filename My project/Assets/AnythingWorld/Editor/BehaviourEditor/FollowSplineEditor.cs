using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom editor for for the FollowSpline behaviour script.
/// </summary>
[CustomEditor(typeof(FollowSpline))]
public class FollowSplineEditor : Editor
{
    #region Fields
    FollowSpline controller;
    string[] nameList;
    PathConfig[] configList;
    int _configListIndex = 0;
    private float spacePx = 10;
    #endregion

    #region Unity Callbacks
    private void OnEnable()
    {
        controller = (FollowSpline)target;
        PopulateConfigList();
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Refresh Path"))
        {
            controller.InitializeAnimator();
        }
        if (GUILayout.Button("Remove Spline Path"))
        {
            controller.RemoveSplinePath();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(spacePx);
        if (nameList != null)
        {
            _configListIndex = EditorGUILayout.Popup(_configListIndex, nameList);
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Load Path"))
        {
            controller.pathCreator.PathConfig = configList[_configListIndex];
            controller.pathCreator.LoadPath();
        }
        if (GUILayout.Button("Refresh Path List"))
        {
            PopulateConfigList();
        }
        GUILayout.EndHorizontal();



    }
    #endregion

    #region Internal Methods
    public void PopulateConfigList()
    {
        string[] assetNames = AssetDatabase.FindAssets("", new[] { "Assets/AnythingWorld/Resources/PathConfig" });

        List<string> _pathNames = new List<string>();
        List<PathConfig> _configList = new List<PathConfig>();

        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var character = AssetDatabase.LoadAssetAtPath<PathConfig>(SOpath);
            _pathNames.Add(character.name);
            _configList.Add(character);
        }
        nameList = _pathNames.ToArray();
        configList = _configList.ToArray();
    }
    #endregion

}
