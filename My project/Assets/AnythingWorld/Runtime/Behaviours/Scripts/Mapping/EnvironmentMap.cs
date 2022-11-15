using System.Collections.Generic;
using UnityEngine;

public static class EnvironmentMap
{
    public static string ENVIRONMENT_PATH = "Prefabs/Environments/";
    public static string ENVIRONMENT_TAG = "AWEnvironment";
    public static string DEFAULT_ENVIRONMENT = "testing";
    public static string BLUE_SKYBOX = "AWSkyBoxBlue";
    public static string GREEN_SKYBOX = "AWSkyBoxGreen";
    public static string DESERT_SKYBOX = "AWSkyBoxDesert";
    public static string SPACE_SKYBOX = "AWSkyBoxSpace";
    public static string UNDERWATER_SKYBOX = "AWSkyBoxUnderwater";
    public static string JUNGLE_SKYBOX = "AWSkyBoxJungle";
    public static string CITY_SKYBOX = "AWSkyBoxCity";

    public static Dictionary<string, EnvironmentLights> LightingSettings;

    public static void SetupEnvironments()
    {
        LightingSettings = new Dictionary<string, EnvironmentLights>();
        // TODO: retrieve from API, hardcoded for now
        // TODO: add thumbnails to editor for habitats
        LightingSettings.Add("testing", new EnvironmentLights(GREEN_SKYBOX, new Color(0.78f, 0.78f, 0.78f), 1f, new Color(0.25f, 0.99f, 0.5f), 0.003f, 60f, 600f, FogMode.Linear));
        LightingSettings.Add("cave", new EnvironmentLights(BLUE_SKYBOX, new Color(0.8f, 0.8f, 0.8f), 1f, new Color(0.75f, 0.95f, 0.98f), 0.003f, 10f, 40f));
        LightingSettings.Add("underwater", new EnvironmentLights(UNDERWATER_SKYBOX, new Color(0.79f, 0.78f, 0.78f), 0.4f, new Color(0.07f, 0.17f, 0.34f), 0.02f, 4f, 140f, FogMode.Linear));
        LightingSettings.Add("forest", new EnvironmentLights(BLUE_SKYBOX, new Color(0.8f, 0.8f, 0.8f), 1f, new Color(0.8f, 0.8f, 0.8f), 0.01f, 10f, 40f));
        LightingSettings.Add("desert", new EnvironmentLights(DESERT_SKYBOX, new Color(0.7f, 0.79f, 0.23f), 1f, new Color(0.96f, 0.9f, 0.44f), 0.0015f, 0f, 0f));
        LightingSettings.Add("farm", new EnvironmentLights(BLUE_SKYBOX, new Color(0.8f, 0.8f, 0.8f), 1f, new Color(0.75f, 0.95f, 0.98f), 0.003f, 10f, 40f));
        LightingSettings.Add("beach", new EnvironmentLights(BLUE_SKYBOX, new Color(0.8f, 0.8f, 0.8f), 1f, new Color(0.75f, 0.95f, 0.98f), 0.003f, 10f, 40f));
        LightingSettings.Add("city", new EnvironmentLights(CITY_SKYBOX, new Color(0.8f, 0.8f, 0.8f), 1f, new Color(0.42f, 0.52f, 0.65f), 0.007f, 0f, 0f));
        LightingSettings.Add("pet", new EnvironmentLights(BLUE_SKYBOX, new Color(0.8f, 0.8f, 0.8f), 1f, new Color(0.75f, 0.95f, 0.98f), 0.003f, 10f, 40f));
        LightingSettings.Add("icescape", new EnvironmentLights(BLUE_SKYBOX, new Color(0.8f, 0.8f, 0.6f), 2f, new Color(0.75f, 0.95f, 0.98f), 0.003f, 10f, 40f));
        LightingSettings.Add("jungle", new EnvironmentLights(JUNGLE_SKYBOX, new Color(0.73f, 0.97f, 0.38f), 1f, new Color(0.23f, 0.65f, 0.5f), 0f, 0f, 280f, FogMode.Linear));
        LightingSettings.Add("lake", new EnvironmentLights(BLUE_SKYBOX, new Color(0.8f, 0.8f, 0.8f), 1f, new Color(0.75f, 0.95f, 0.98f), 0.003f, 10f, 40f));
        LightingSettings.Add("pond", new EnvironmentLights(BLUE_SKYBOX, new Color(0.8f, 0.8f, 0.8f), 1f, new Color(0.75f, 0.95f, 0.98f), 0.003f, 10f, 40f));
        LightingSettings.Add("river", new EnvironmentLights(BLUE_SKYBOX, new Color(0.8f, 0.8f, 0.8f), 1f, new Color(0.75f, 0.95f, 0.98f), 0.003f, 10f, 40f));
        LightingSettings.Add("swamp", new EnvironmentLights(BLUE_SKYBOX, new Color(0.8f, 0.8f, 0.8f), 1f, new Color(0.75f, 0.95f, 0.98f), 0.003f, 10f, 40f));
        LightingSettings.Add("grass", new EnvironmentLights(BLUE_SKYBOX, new Color(0.8f, 0.8f, 0.8f), 1f, new Color(0.75f, 0.95f, 0.98f), 0.003f, 10f, 40f));
        LightingSettings.Add("rural", new EnvironmentLights(BLUE_SKYBOX, new Color(0.8f, 0.8f, 0.8f), 1f, new Color(0.75f, 0.95f, 0.98f), 0.003f, 10f, 40f));
        LightingSettings.Add("urban", new EnvironmentLights(BLUE_SKYBOX, new Color(0.8f, 0.8f, 0.8f), 1f, new Color(0.75f, 0.95f, 0.98f), 0.003f, 10f, 40f));
        LightingSettings.Add("garden", new EnvironmentLights(BLUE_SKYBOX, new Color(0.8f, 0.8f, 0.8f), 1f, new Color(0.75f, 0.95f, 0.98f), 0.003f, 10f, 40f));
        LightingSettings.Add("space", new EnvironmentLights(SPACE_SKYBOX, new Color(1f, 0.96f, 0.84f), 0.3f, new Color(0.16f, 0.16f, 0.16f), 0.01f, 0f, 0f));
        LightingSettings.Add("savannah", new EnvironmentLights(BLUE_SKYBOX, new Color(0.8f, 0.8f, 0.8f), 1f, new Color(0.75f, 0.95f, 0.98f), 0.003f, 10f, 40f));
        LightingSettings.Add("mountain", new EnvironmentLights(BLUE_SKYBOX, new Color(0.65f, 0.75f, 0.78f), 1f, new Color(0.48f, 0.66f, 0.76f), 0.003f, 10f, 40f));
        LightingSettings.Add("magical", new EnvironmentLights(BLUE_SKYBOX, new Color(0.8f, 0.8f, 0.8f), 1f, new Color(0.75f, 0.95f, 0.98f), 0.003f, 10f, 40f));
        LightingSettings.Add("grassland", new EnvironmentLights(BLUE_SKYBOX, new Color(0.8f, 0.8f, 0.8f), 1f, new Color(0.75f, 0.95f, 0.98f), 0.003f, 10f, 40f));
        // LightingSettings.Add("sea",new EnvironmentLights(new Color(0.8f,0.8f,0.8f),0.4f,new Color(0.35f,0.35f,0.65f),0.02f)); // TODO: add back in sea with surface
    }

    public static EnvironmentLights EnvironmentLighting(string environmentName)
    {
        if (LightingSettings == null)
            SetupEnvironments();

        return LightingSettings[environmentName];
    }

    public static bool TryGetEnvironmentPrefab(string environmentName, out GameObject environmentPrefab)
    {
        var prefabLoc = ENVIRONMENT_PATH + environmentName;
        environmentPrefab = Resources.Load(prefabLoc) as GameObject;
     
       
        if (environmentPrefab == null)
        {
            environmentName = DEFAULT_ENVIRONMENT;
            prefabLoc = ENVIRONMENT_PATH + environmentName;
            environmentPrefab = Resources.Load(prefabLoc) as GameObject;
        }
        // otherwise we can't find prefab, throw error
        if (environmentPrefab == null)
        {
            Debug.LogError("NO PREFAB FOUND OF TYPE : " + environmentName);
            environmentPrefab = null;
            return false;
        }
        environmentPrefab.name = environmentName + "_AWEnvironment";
        environmentPrefab.tag = ENVIRONMENT_TAG;
        return true;
    }

}
