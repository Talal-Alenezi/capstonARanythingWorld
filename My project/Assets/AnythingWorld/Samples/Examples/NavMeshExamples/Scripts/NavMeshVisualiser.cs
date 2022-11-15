using UnityEngine;
using UnityEngine.AI;

namespace AnythingWorld.Examples
{
    public class NavMeshVisualiser : MonoBehaviour
    {
        public bool showNavMesh = true;
        void Start()
        {
            if (!GetComponent<MeshFilter>())
            {
                gameObject.AddComponent<MeshFilter>();
            }
            if (!GetComponent<MeshRenderer>())
            {
                gameObject.AddComponent<MeshRenderer>();
            }
        }

        private void LateUpdate()
        {
            if (showNavMesh)
            {
                VisualiseNavMesh();
            }
        }
        private void VisualiseNavMesh()
        {
            var meshData = NavMesh.CalculateTriangulation();
            var nMesh = new Mesh();
            nMesh.vertices = meshData.vertices;
            nMesh.triangles = meshData.indices;
            GetComponent<MeshFilter>().mesh = nMesh;
        }
        public void ToggleNavMeshVis()
        {
            var rend = GetComponent<MeshRenderer>();

            rend.enabled = !rend.enabled;
        }
    }
}

