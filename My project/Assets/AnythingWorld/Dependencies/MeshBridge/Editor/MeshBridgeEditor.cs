using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshBridge))]
public class MeshBridgeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MeshBridge controller = (MeshBridge)target;

        base.OnInspectorGUI();
        if (GUILayout.Button("Refresh"))
        {
            controller.GetMeshes();
        }

        if (GUILayout.Button("Get Verts"))
        {
            controller.GetCoordinateVertices();
        }

        if (GUILayout.Button("Get matching"))
        {
            controller.GetMatchingPoints();
        }
    }
}
