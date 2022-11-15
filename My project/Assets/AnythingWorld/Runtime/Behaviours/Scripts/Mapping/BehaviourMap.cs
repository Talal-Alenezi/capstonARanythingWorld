using System.Collections.Generic;

public static class BehaviourMap
{
    public static Dictionary<string, List<string>> BehaviourDict;

    public static void SetUpMaps()
    {
        BehaviourDict = new Dictionary<string, List<string>>();

        BehaviourDict["swim"] = new List<string>();
        BehaviourDict["swim2"] = new List<string>();
        BehaviourDict["swim3"] = new List<string>();
        BehaviourDict["fly"] = new List<string>();
        BehaviourDict["walk"] = new List<string>();
        BehaviourDict["walk2"] = new List<string>();
        BehaviourDict["crawl"] = new List<string>();
        BehaviourDict["waddle"] = new List<string>();
        BehaviourDict["bob"] = new List<string>();
        BehaviourDict["float"] = new List<string>();
        BehaviourDict["drive"] = new List<string>();
        BehaviourDict["hop"] = new List<string>();
        BehaviourDict["wriggle"] = new List<string>();
        BehaviourDict["slither"] = new List<string>();
        BehaviourDict["slithervertical"] = new List<string>();
        BehaviourDict["squat"] = new List<string>();
        BehaviourDict["ride"] = new List<string>();
    }

    public static string GetBehaviour(string thingName)
    {
        if (BehaviourDict == null)
            SetUpMaps();

        var category = "unknown";

        foreach (var kvp in BehaviourDict)
        {
            foreach (var behaviourStr in kvp.Value)
            {
                if (behaviourStr == thingName)
                {
                    category = kvp.Key;
                    // Debug.LogWarning("found behaviour for : " + behaviourStr +  " -> " + kvp.Key);
                    break;
                }
            }
        }
        return category;
    }

    public static void SetBehaviour(string behaviourName, string thingName)
    {
        //Debug.Log("SetBehaviour -> " + behaviourName);

        if (BehaviourDict == null)
            SetUpMaps();

        if (BehaviourDict.ContainsKey(behaviourName))
        {
            BehaviourDict[behaviourName].Add(thingName);
        }
    }
}