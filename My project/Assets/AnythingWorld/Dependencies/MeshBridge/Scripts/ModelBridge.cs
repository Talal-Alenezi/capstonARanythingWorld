using System.Collections.Generic;
using UnityEngine;

using AnythingWorld.Utilities;


public class ModelBridge : MonoBehaviour
{
    [SerializeField]
    public List<MeshBridge> meshBridges;
    public void OnEnable()
    {
    }

    public void AddMeshBridgeUnits()
    {
        meshBridges = new List<MeshBridge>();
        var joints = GetComponentsInChildren<ConfigurableJoint>();
        for (var i = 0; i < joints.Length; i++)
        {
            if (!joints[i].gameObject.GetComponent<MeshBridge>())
            {
                meshBridges.Add(joints[i].gameObject.AddComponent<MeshBridge>());
            }
        }
    }

    public void RemoveMeshBridgeUnits()
    {
        if (meshBridges == null) return;
        foreach (var meshBridge in meshBridges)
        {
            AnythingSafeDestroy.SafeDestroyDelayed(meshBridge);
            AnythingSafeDestroy.ClearList<MeshBridge>(meshBridges);
        }
    }
}
