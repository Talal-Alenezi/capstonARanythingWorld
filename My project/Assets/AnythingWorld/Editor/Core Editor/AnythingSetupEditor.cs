using UnityEditor;
using UnityEngine;

namespace AnythingWorld.Editors
{
    [CustomEditor(typeof(AnythingSetup))]
    public class AnythingSetupEditor : Editor
    {
        private Vector2 scrollPosition;
        public override void OnInspectorGUI()
        {

            base.OnInspectorGUI();
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            if (GUILayout.Button("Generate attributions from scene objects"))
            {
                AnythingSetup.GenerateAttributionsFromScene();
            }

            GUILayout.TextArea(AnythingSetup.ModelAttributionStringBlock);
            GUILayout.EndScrollView();
        }
    }
}

