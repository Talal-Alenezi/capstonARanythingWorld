using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class MeshBridge : MonoBehaviour
{
    public Vector3[] vertices;
    public int[] triangles;
    public int[] triCount;
    public List<Vector3> vertiCoords;
    public List<Vector3> vertiCoords2;
    public Vector3[,] vertPair;
    public MeshFilter thisMesh;
    public MeshFilter connectedMesh;
    public List<Vector3> thisVerts;
    public List<Vector3> connectedVerts;
    public List<Vector2> matchingVerts;
    private void OnEnable()
    {
        GetMeshes();
    }
    public void GetMeshes()
    {
        GetThisMesh();
        GetConnectedMesh();
    }
    private void GetThisMesh()
    {
        thisMesh = gameObject.GetComponentInChildren<MeshFilter>();
    }
    private void GetConnectedMesh()
    {
        if (TryGetComponent<ConfigurableJoint>(out var configJoint))
        {
            connectedMesh = configJoint.connectedBody.gameObject.GetComponentInChildren<MeshFilter>();
        }

        if (connectedMesh != null) return;

        var joints = new List<ConfigurableJoint>();
        joints = GetComponents<ConfigurableJoint>().ToList<ConfigurableJoint>();

        foreach (var joint in joints)
        {
            if (joint.connectedBody.GetComponentInChildren<MeshFilter>())
            {
                connectedMesh = joint.connectedBody.GetComponentInChildren<MeshFilter>();
            }
        }
    }
    public void GetCoordinateVertices()
    {
        thisVerts = new List<Vector3>();
        connectedVerts = new List<Vector3>();

        var localToWorld = thisMesh.transform.localToWorldMatrix;
        for (var i = 0; i < thisMesh.sharedMesh.vertices.Length; ++i)
        {
            var tempCoord = localToWorld.MultiplyPoint3x4(thisMesh.sharedMesh.vertices[i]);
            thisVerts.Add(tempCoord);
        }


        localToWorld = connectedMesh.transform.localToWorldMatrix;
        for (var i = 0; i < connectedMesh.sharedMesh.vertices.Length; ++i)
        {
            var tempCoord = localToWorld.MultiplyPoint3x4(connectedMesh.sharedMesh.vertices[i]);
            connectedVerts.Add(tempCoord);
        }



    }
    public void GetMatchingPoints()
    {
        matchingVerts = new List<Vector2>();
        for (var i = 0; i < thisVerts.Count; i++)
        {
            for (var i2 = 0; i2 < connectedVerts.Count; i2++)
            {
                if (thisVerts[i] == connectedVerts[i2])
                {
                    matchingVerts.Add(new Vector2(i, i2));
                }
            }
        }
    }
    public void GetEdgeVerts()
    {
        thisMesh = GetComponentInChildren<MeshFilter>();
        vertices = thisMesh.sharedMesh.vertices;
        triangles = thisMesh.sharedMesh.triangles;
        Debug.Log("Test");
        vertiCoords = new List<Vector3>();
        var localToWorld = thisMesh.transform.localToWorldMatrix;
        for (var i = 0; i < thisMesh.sharedMesh.vertices.Length; ++i)
        {
            var tempCoord = localToWorld.MultiplyPoint3x4(thisMesh.sharedMesh.vertices[i]);
            vertiCoords.Add(tempCoord);
        }
        triCount = new int[vertices.Length];

        for (var i = 0; i < connectedMesh.sharedMesh.vertices.Length; i++)
        {
            var tempCoord = localToWorld.MultiplyPoint3x4(connectedMesh.sharedMesh.vertices[i]);
            vertiCoords2.Add(tempCoord);
        }
    }
    private void OnDrawGizmosSelected()
    {
        foreach (var vert in matchingVerts)
        {
            Gizmos.DrawSphere(thisVerts[(int)vert.x], 0.05f);
        }
    }

    public void LateUpdate()
    {

    }
}
