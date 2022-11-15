using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(ModelBridge))]
public class ModelBridgeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ModelBridge controller = (ModelBridge)target;

        if (GUILayout.Button("Refresh"))
        {
            controller.AddMeshBridgeUnits();
        }
        if (GUILayout.Button("Remove"))
        {
            controller.RemoveMeshBridgeUnits();
        }

    }
}
