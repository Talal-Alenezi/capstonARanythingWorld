using System;
using UnityEngine;

namespace AnythingWorld.Utilities
{
    [Serializable]
    public class CenterMeshPivot : MonoBehaviour
    {
        private const bool SHOW_DEBUG_CENTER_SPHERE = false;
        private GameObject pivotSphere;
        [SerializeField]
        public GameObject newPivotObject;
        [SerializeField]
        private float wheelRadius = 0;
        public PivotPosition pivotPosition = PivotPosition.CenterPivot;
        public enum PivotPosition
        {
            CenterPivot,
            TopPivot,
            LowerPivot
        };
        public void CreateNewMeshPivot()
        {
            var loadedObject = transform.GetChild(0).gameObject;
            var meshObject = loadedObject.GetComponentInChildren<MeshFilter>().gameObject;
            newPivotObject = new GameObject("MeshPivot").gameObject;
            newPivotObject.transform.parent = loadedObject.transform;
            newPivotObject.transform.localPosition = Vector3.zero;
            var wheelMesh = meshObject.GetComponent<MeshFilter>();
            wheelRadius = wheelMesh.sharedMesh.bounds.extents.y * loadedObject.transform.localScale.y;
            var wheelRenderer = meshObject.GetComponent<MeshRenderer>();
            var meshCenterPositionLocal = wheelMesh.sharedMesh.bounds.center;
            var meshCenterPositionWorld = meshObject.transform.TransformPoint(meshCenterPositionLocal);
            newPivotObject.transform.position = meshCenterPositionWorld;
            var newWheel = Instantiate(meshObject);
            newWheel.transform.parent = loadedObject.transform;
            newWheel.transform.SetPositionAndRotation(meshObject.transform.position, meshObject.transform.rotation);
            newWheel.transform.localScale = meshObject.transform.localScale;
            newWheel.GetComponent<MeshRenderer>().enabled = true;
            newWheel.transform.parent = newPivotObject.transform;
            wheelRenderer.enabled = false;
            AnythingSafeDestroy.SafeDestroyImmediate(wheelRenderer.gameObject);
        }
        private void CenterPivotOnMesh()
        {
            CreateNewMeshPivot();
        }
        public Transform PivotTransform
        {
            get
            {
                if (newPivotObject == null) CreateNewMeshPivot();
                return newPivotObject.transform;
            }
        }

        public Vector3 PivotCenter
        {
            get
            {
                if (newPivotObject == null)
                {
                    CenterPivotOnMesh();
                }
                return newPivotObject.transform.position;
            }
        }
        public float Radius
        {
            get
            {
                if (newPivotObject==null)
                {
                    CenterPivotOnMesh();
                }

                return wheelRadius;
            }
        }
    }
}

