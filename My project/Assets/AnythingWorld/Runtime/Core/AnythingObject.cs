using AnythingWorld.Utilities;
using System;
using UnityEngine;

namespace AnythingWorld
{
    [Serializable]
    public class AnythingObject : MonoBehaviour
    {
        #region Fields

        public string objectName, objectGuid;

        [SerializeField]
        public bool createInEditor = false,
            hasBehaviour = false,
            hasCollider = false,
            addMeshCollider = false,
            randomYRotation = false,
            globallyPosition = false;

        public BatchLoadAssets batchedLoader = null;
        public AWObj attachedAwObj = null;

        #endregion Fields

        public void DestroyRequestedModels()
        {
            var awObjArray = GetComponentsInChildren<AWObj>();
            foreach (var awObj in awObjArray)
            {
                awObj.flaggedDestroyed = true;
                awObj.StopRequestPipeline();
                AnythingSafeDestroy.SafeDestroyDelayed(awObj.gameObject);
            }
        }

        public void Load()
        {
            //Debug.Log(($"Loading {GetGuid()} in AnythingObject "));
            if (objectGuid == null) ExtractGuid();
            DestroyRequestedModels();
            var createObjectTerm = objectName;
            if (objectGuid != "") createObjectTerm = objectName + "#" + objectGuid;
            attachedAwObj = AnythingCreator.Instance.MakeObject(createObjectTerm, Vector3.zero, Quaternion.identity, Vector3.one, transform, hasBehaviour, hasCollider, globallyPosition);
            if (addMeshCollider) attachedAwObj.onObjectMadeSuccessfullyInstanced = AddMeshCollider;

            if (randomYRotation)
            {
                transform.eulerAngles = new Vector3(transform.rotation.x, UnityEngine.Random.Range(0, 360), transform.rotation.z);
            }
            transform.name = createObjectTerm;
        }

        public void AddMeshCollider(AWObj instance)
        {
            //Debug.Log("Add mesh collider called", this.gameObject);
            instance.onObjectMadeSuccessfullyInstanced -= AddMeshCollider;
            try
            {
                GetComponentInChildren<MeshRenderer>().gameObject.AddComponent<MeshCollider>();
            }
            catch
            {
                Debug.LogWarning("No mesh found to apply mesh collider.");
            }
        }

        public void SetGuid(string name)
        {
            var split = name.Split('#');
            if (split.Length != 2) return;
            objectName = split[0];
            objectGuid = split[1];
        }

        public string GetGuid()
        {
            return objectName + "#" + objectGuid;
        }

        public void ExtractGuid()
        {
            SetGuid(gameObject.name);
        }
    }
}