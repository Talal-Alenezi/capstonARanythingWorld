using UnityEditor;
using UnityEngine;


namespace AnythingWorld.Examples
{
    [CustomEditor(typeof(ForestSpawner))]
    public class ForestSpawnerEditor : Editor
    {
        private ForestSpawner controller;
        private void OnEnable()
        {
            controller = (ForestSpawner)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Spawn Forest"))
            {
                controller.SpawnTrees();
            }
            if (GUILayout.Button("Clear Forest"))
            {
                controller.ClearExistingTrees();
            }
        }
    }
}

