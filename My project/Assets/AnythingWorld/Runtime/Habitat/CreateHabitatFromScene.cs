#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using AnythingWorld;
public class CreateHabitatFromScene : MonoBehaviour
{
    [MenuItem("GameObject/Anything World/Create Habitat Prefab")]
    static void CreateHabitatPrefab(MenuCommand command)
    {
        var topOfHierarchy = command.context as GameObject;
        var newHierarchy = new GameObject(topOfHierarchy.name);
        TryFindAwObjInChildren(topOfHierarchy, newHierarchy);
        SaveHierarchyAsPrefab(newHierarchy);
    }
    static void TryFindAwObjInChildren(GameObject originalParent, GameObject newParent)
    {
        //Find all children of the source gameobject
        foreach (var childObject in originalParent.GetComponentsInChildren<Transform>())
        {
            if (childObject.TryGetComponent<AWObj>(out var aWObj))
            {
                Debug.Log(aWObj.name);
                AddAnythingObjectToClonedHierarchy(newParent, aWObj.transform);
            }
        }
    }

    static void AddAnythingObjectToClonedHierarchy(GameObject parent, Transform objectTransform)
    {
        var anythingObject = new GameObject(objectTransform.gameObject.name);
        anythingObject.transform.parent = parent.transform;
        anythingObject.transform.position = objectTransform.position;
        anythingObject.transform.rotation = objectTransform.rotation;
        anythingObject.transform.localScale = objectTransform.localScale;
        anythingObject.AddComponent<AnythingObject>().createInEditor=false;
        anythingObject.GetComponent<AnythingObject>().SetGuid(objectTransform.GetComponent<AWObj>().ModelName);
    }
    static void SaveHierarchyAsPrefab(GameObject hierarchyTop)
    {
        PrefabUtility.SaveAsPrefabAsset(hierarchyTop, "Assets/AnythingWorld/Resources/Prefabs/Environments/"+hierarchyTop.name);
    }
}
#endif
