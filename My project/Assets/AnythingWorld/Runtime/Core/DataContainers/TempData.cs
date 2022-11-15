using System.Collections.Generic;
using UnityEngine;

namespace AnythingWorld.DataContainers
{
    public class TempData
    {
        public List<GameObject> objLoadedObjs = new List<GameObject>();
        public List<Renderer> objRendererList = new List<Renderer>();
        public List<MeshFilter> objMeshList = new List<MeshFilter>();
        public List<Rigidbody> prefabRigidbodies;
        public Collider[] prefabColliders;
        public GameObject boundsShow;
        public string modelPartsJson;
    }
}