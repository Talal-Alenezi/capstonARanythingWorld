using System.Collections;
using UnityEngine;

namespace AnythingWorld.Animation
{
    public class AddCollider : MonoBehaviour
    {
        private AWObj controllingAwObj;
        public bool isTriggerCollider = false;
        public float colliderScale = 1f;
        public bool addCapsuleCollider = false;

        private void Start()
        {
            controllingAwObj = GetComponentInParent<AWObj>();
            if (controllingAwObj == null) 
                Debug.LogWarning($"Could not find AwObj in parent for {gameObject.name}");
            

            StartCoroutine(WaitForAWObjCompletion());
        }

        private IEnumerator WaitForAWObjCompletion()
        {
            if (controllingAwObj != null)
            {
                while (!controllingAwObj.ObjMade)
                    yield return new WaitForEndOfFrame();
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }

            CreateCollider();
        }

        private void CreateCollider()
        {
            if (TryGetComponent<CapsuleCollider>(out var _)) return;
            // Collect bounds of all the meshes that construct the object
            var meshCollection = new Bounds();
            var childrenMeshes = transform.GetComponentsInChildren<MeshFilter>();

            for (var i = 0; i < childrenMeshes.Length; i++)
            {
                // Find the parent of the mesh to obtain the scale
                var parent = ((MeshFilter)childrenMeshes.GetValue(i)).GetComponentInParent<Transform>().parent;

                // Calculate scaled mesh size and center
                var scaledSize = Vector3.Scale(((MeshFilter)childrenMeshes.GetValue(i)).sharedMesh.bounds.size, parent.localScale);
                var scaledCenter = Vector3.Scale(((MeshFilter)childrenMeshes.GetValue(i)).sharedMesh.bounds.center, parent.localScale);

                // Get scaled bounds of the mesh
                var scaledBounds = ((MeshFilter)childrenMeshes.GetValue(i)).sharedMesh.bounds;
                scaledBounds.size = scaledSize;
                scaledBounds.center = scaledCenter + parent.localPosition;

                meshCollection.Encapsulate(scaledBounds);
            }
            if (addCapsuleCollider)
            {
                var modelCollider = transform.gameObject.AddComponent<CapsuleCollider>();
                modelCollider.radius = (meshCollection.size * colliderScale).x;
                modelCollider.height = (meshCollection.size * colliderScale).y;
                modelCollider.center = meshCollection.center;
                modelCollider.isTrigger = isTriggerCollider;
            }
            else
            {
                // Create collider for the transform
                var modelCollider = transform.gameObject.AddComponent<BoxCollider>();
                modelCollider.size = meshCollection.size * colliderScale;
                modelCollider.center = meshCollection.center;
                modelCollider.isTrigger = isTriggerCollider;
            }
        }
    }
}