using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AnythingWorld.Editors
{
    [CustomEditor(typeof(BatchLoadAssets))]
    public class BatchLoaderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            BatchLoadAssets batchLoaderScript = (BatchLoadAssets)target;
            if (GUILayout.Button("Refresh & Load"))
            {
                batchLoaderScript.Load();
            }
            if (GUILayout.Button("Clear"))
            {
                batchLoaderScript.ResetBatchLoader();
            }
        }
    }
}
