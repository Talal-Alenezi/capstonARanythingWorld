using UnityEditor;
using UnityEngine;

namespace AnythingWorld.Editors
{
    [CustomEditor(typeof(AnythingObject))]
    [CanEditMultipleObjects]
    public class AnythingObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Refresh"))
            {
                foreach (var obj in Selection.gameObjects)
                {
                    obj.GetComponent<AnythingObject>().ExtractGuid();
                    obj.GetComponent<AnythingObject>().Load();
                }

            }
            if (GUILayout.Button("Extract GUID"))
            {
                foreach (var obj in Selection.gameObjects)
                {
                    obj.GetComponent<AnythingObject>().ExtractGuid();
                    EditorUtility.SetDirty(obj.GetComponent<AnythingObject>());
                }
            }
        }
    }
}