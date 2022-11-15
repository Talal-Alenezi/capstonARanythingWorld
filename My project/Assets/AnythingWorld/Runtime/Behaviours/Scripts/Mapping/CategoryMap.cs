using System.Collections.Generic;
using UnityEngine;

public static class CategoryMap
{
    // TODO: put in Google Sheets, another way rather than massive dictionaries
    // TODO: remove any duplication with prefab & behaviour maps
    public static Dictionary<string, List<string>> CategoryDict;

    public static void SetUpMaps()
    {
        CategoryDict = new Dictionary<string, List<string>>();
        CategoryDict["quadruped"] = new List<string>();
        CategoryDict["quadruped_ungulate"] = new List<string>();
        CategoryDict["quadruped_fat"] = new List<string>(); 
        CategoryDict["quadruped_fat_small_generic"] = new List<string>();
        CategoryDict["quadruped_fat_shortleg_generic"] = new List<string>();
        CategoryDict["biped"] = new List<string>();
        CategoryDict["winged_flyer"] = new List<string>();
        CategoryDict["winged_standing"] = new List<string>();
        CategoryDict["winged_standing_small"] = new List<string>();
        CategoryDict["uniform"] = new List<string>();
        CategoryDict["hopper"] = new List<string>();
        CategoryDict["multileg_crawler"] = new List<string>();
        CategoryDict["multileg_crawler_eight"] = new List<string>();
        CategoryDict["multileg_crawler_big"] = new List<string>();
        CategoryDict["multileg_flyer"] = new List<string>();
        CategoryDict["vehicle_four_wheel"] = new List<string>();
        CategoryDict["vehicle_three_wheel"] = new List<string>();
        CategoryDict["vehicle_two_wheel"] = new List<string>();
        CategoryDict["vehicle_one_wheel"] = new List<string>();
        CategoryDict["vehicle_load"] = new List<string>();
        CategoryDict["vehicle_other"] = new List<string>();
        CategoryDict["vehicle_flyer"] = new List<string>();
        CategoryDict["vehicle_propeller"] = new List<string>();
        CategoryDict["vehicle_uniform"] = new List<string>();
        CategoryDict["vehicle_other"] = new List<string>();
        CategoryDict["scenery"] = new List<string>();
    }

    public static string GetCategory(string thingName)
    {
        if (CategoryDict == null)
            SetUpMaps();

        var category = "unknown";

        foreach (var kvp in CategoryDict)
        {
            foreach (var categoryStr in kvp.Value)
            {

                if (categoryStr == thingName)
                {
                    category = kvp.Key;
                    //  Debug.LogWarning("found category for : " + categoryStr +  " -> " + kvp.Key);
                    break;
                }
            }
        }
        return category;
    }

    public static void SetCategory(string categoryName, string thingName)
    {
        if (CategoryDict == null)
            SetUpMaps();

        // Debug.Log(">>>>> " + categoryName + " // " + thingName);

        if (CategoryDict.ContainsKey(categoryName))
        {
            CategoryDict[categoryName].Add(thingName);
        }
        else
        {
            Debug.LogError("No category found for : " + categoryName);
        }
    }
}