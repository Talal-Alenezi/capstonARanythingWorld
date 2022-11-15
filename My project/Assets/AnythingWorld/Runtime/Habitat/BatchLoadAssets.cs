using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AnythingWorld.Utilities;
namespace AnythingWorld
{
    [ExecuteAlways]
    public class BatchLoadAssets : MonoBehaviour
    {
        public List<AnythingObject> objectList;
        public List<string> uniqueModelGuidList;
        public List<AWObj> batchedAwObjList;
        public bool loadInEditor = true;
        private GameObject batchedObjectHolder;
        public bool objectsCreated = false;
        public List<AWObj> requestList;

        public void Load()
        {
            ResetBatchLoader();
            GatherAssetList();
            RequestBatchObjects();
        }
        public void ResetBatchLoader()
        {

            AnythingSafeDestroy.SafeDestroyDelayed(gameObject.GetComponentsInChildren<AWObj>());
            AnythingSafeDestroy.SafeDestroyDelayed(gameObject.GetComponentsInChildren<AnythingWorld.Habitat.PreloadedAssetsContainer>());
            RemoveExistingInstantiatedObjects();
            batchedAwObjList = new List<AWObj>();
            uniqueModelGuidList = new List<string>();
            requestList = new List<AWObj>();
            objectsCreated = false;
        }
        public void RemoveExistingInstantiatedObjects()
        {
            foreach (var ao in objectList)
            {
                ao?.DestroyRequestedModels();
            }
        }
        public void GatherAssetList()
        {
            objectList = transform.GetComponentsInChildren<AnythingObject>().ToList();
            uniqueModelGuidList = new List<string>();
            //for each object to be requested in children
            foreach (var requester in objectList.Where(requester => !uniqueModelGuidList.Contains(requester.GetGuid())))
            {
                uniqueModelGuidList.Add(requester.GetGuid());
            }
        }
        public void RequestBatchObjects()
        {
            batchedObjectHolder = new GameObject("PreloadedAssets");
            batchedObjectHolder.transform.position = Vector3.one * -3;
            batchedObjectHolder.AddComponent<AnythingWorld.Habitat.PreloadedAssetsContainer>();
            batchedAwObjList = new List<AWObj>();
            batchedObjectHolder.transform.parent = this.transform;
            foreach (var objReference in uniqueModelGuidList.Select(guid => AnythingCreator.Instance.MakeObject(guid, Vector3.zero, Quaternion.identity, Vector3.one * 0.00001f, batchedObjectHolder.transform)))
            {
                objReference.onObjectMadeSuccessfullyInstanced += CreateObjectsOnBatchedObjectCompletion;
                batchedAwObjList.Add(objReference);
            }
        }

        public void CreateObjectsOnBatchedObjectCompletion(AWObj instance)
        {
            //Debug.Log($"{instance.objName} made, instantiating clones.");
            instance.onObjectMadeSuccessfullyInstanced -= CreateObjectsOnBatchedObjectCompletion;
            foreach (var obj in objectList.Where(obj => obj.GetGuid() == instance.name))
            {
                obj.Load();
                objectsCreated = true;
            }
        }
    }
}
