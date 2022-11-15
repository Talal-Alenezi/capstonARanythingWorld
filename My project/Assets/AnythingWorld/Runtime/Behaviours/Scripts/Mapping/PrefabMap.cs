using UnityEngine;

public static class PrefabMap
{
    public static string THING_PREFAB_PATH = "Prefabs/Things/";
    public static string THING_PREFAB_TAG = "AWThing";


    public static string PrefabName(string thingName, string behaviourName)
    {
        return thingName + "__" + behaviourName;
    }
    public static GameObject ThingPrefab(string thingName, string behaviourName)
    {
        // try to find [thingName]_[behaviourName]
        var prefabName = PrefabName(thingName, behaviourName);
        var prefabLoc = THING_PREFAB_PATH + prefabName;
        var groupPref = Resources.Load(prefabLoc) as GameObject;
        // try just with thingName if [thingName]_[behaviourName] fails
        if (groupPref == null)
        {
            prefabName = thingName;
            prefabLoc = THING_PREFAB_PATH + prefabName;
            groupPref = Resources.Load(prefabLoc) as GameObject;
        }
        // otherwise we can't find prefab, throw error
        if (groupPref == null)
        {
            Debug.LogError("NO PREFAB FOUND OF TYPE : " + prefabName);
            return null;
        }
        //Debug.Log("Creating prefab clone : " + prefabName);
        groupPref.name = prefabName + "_AWClone";
        groupPref.tag = THING_PREFAB_TAG;
        return groupPref;
    }

}
