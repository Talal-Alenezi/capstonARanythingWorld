using System.Collections.Generic;
using UnityEngine;

using AnythingWorld.Utilities;


namespace AnythingWorld.Examples
{
    public class ForestSpawner : MonoBehaviour
    {
        public Terrain terrainObject = null;
        public float treeHeightLimit = 0.5f;
        public GameObject treeObject = null;
        public float radius = 10;
        public Vector2 regionSize = Vector2.one;
        public int rejectionSamples = 30;
        public float displayRadius = 1;
        public float treeHeightOffset = 1;
        public bool treeRandomRotation = false;
        public bool treeRandomScale = false;
        public float minScale = 1f;
        public float maxScale = 2f;
        public bool ShowDebugGizmos = false;

        List<Vector2> points;
        [SerializeField]
        private List<GameObject> treeList;

        private void Start()
        {

        }
        public void SpawnTrees()
        {
            points = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples);
            ClearExistingTrees();
            foreach (var point in points)
            {
                var point3D = new Vector3(point.x, 0, point.y);
                point3D = point3D + transform.position;
                var scaledTreeHeightOffset = treeHeightOffset;
                if (terrainObject != null)
                {
                    var terrainHeight = terrainObject.SampleHeight(point3D);
                    if (terrainHeight < treeHeightLimit)
                    {
                        var newTree = Object.Instantiate(treeObject, transform);
                        treeList.Add(newTree);

                        if (treeRandomRotation == true)
                        {
                            var angle = Random.value * Mathf.PI * 2;
                            newTree.transform.Rotate(new Vector3(0, 1, 0), (angle * Mathf.Rad2Deg));
                        }
                        if (treeRandomScale == true)
                        {
                            var newScale = Random.Range(minScale, maxScale);
                            newTree.transform.localScale = newTree.transform.localScale * newScale;
                            scaledTreeHeightOffset *= newScale;
                        }


                        newTree.transform.position = new Vector3(point3D.x, terrainHeight + scaledTreeHeightOffset, point3D.z);
                    }
                }
            }
        }
        public void ClearExistingTrees()
        {
            if (treeList != null && treeList.Count > 0)
            {

                foreach (var tree in treeList)
                {
                    AnythingSafeDestroy.SafeDestroyDelayed(tree);
                }
            }
            treeList = new List<GameObject>();
        }
        void OnDrawGizmos()
        {
            if (ShowDebugGizmos == false) return;
            Gizmos.DrawWireCube(regionSize / 2, regionSize);
            if (points != null)
            {
                foreach (var point in points)
                {


                    var point3D = new Vector3(point.x, 0, point.y);
                    point3D = point3D + transform.position;
                    Gizmos.DrawSphere(point3D, displayRadius);
                }
            }
        }
    }
}

