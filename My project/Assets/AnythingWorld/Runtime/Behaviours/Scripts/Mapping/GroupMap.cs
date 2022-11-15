/*
*
*
* TODO: make 
*
*
*/



using UnityEngine;

public static class GroupMap
{

    public static string GROUP_PREFAB_PATH = "Prefabs/Groups/";
    public enum FlockType
    {
        flying,
        swimming
    }
    private static bool _hasGroup;
    public static bool HasGroup
    {
        get
        {
            return _hasGroup;
        }
        set
        {
            _hasGroup = value;
        }
    }

    ///Does the behaviour description for this object put it into the flocking groups
    public static bool IsObjectFlocking(string behaviourName, string categoryName)
    {
        //Debug.Log($"Is object flocking: {behaviourName}:{categoryName}");
        if (behaviourName != null)
        {
            // TODO: develop system for groups
            if (behaviourName.IndexOf("swim") != -1 || (behaviourName == "fly" && categoryName.IndexOf("vehicle") == -1))
            {
                //Debug.Log("True");
                return true;
            }
        }
        //Debug.Log("False");
        return false;
    }

    
    public static GameObject LoadFlockPrefab(string behaviourName)
    {
        var groupPrefabName = "";

        if (behaviourName == "fly")
        {
            groupPrefabName = "Bird_Flock";
        }
        else if (behaviourName.IndexOf("swim") != -1)
        {
            groupPrefabName = "Fish_Shoal";
        }

        var groupPrefab = Resources.Load(GROUP_PREFAB_PATH + groupPrefabName) as GameObject;
        groupPrefab.name = groupPrefabName;
        return groupPrefab;
    }
    public static FlockType GetFlockTypeFromBehaviour(string behaviourName)
    {
        if (behaviourName == "fly")
        {
            return FlockType.flying;
        }
        else
        {
            return FlockType.swimming;
        }
    }

}
