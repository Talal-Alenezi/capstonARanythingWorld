using AnythingWorld.Animation;
using AnythingWorld.Habitat;
using AnythingWorld.Utilities;
using System;
using UnityEngine;

namespace AnythingWorld
{
    public class AnythingBase : MonoBehaviour
    {
        #region Fields
        private static readonly string setupPrefabDir = "Prefabs/SetupPrefabs/AnythingSetup";
        public static AnythingSettings Settings
        {
            get
            {
                if (AnythingSettings.Instance == null)
                {
                    AnythingSettings.CreateInstance<AnythingSettings>();
                }
                return AnythingSettings.Instance;
            }
        }
        #endregion

        #region Protected Methods

        public static void ResetEverything()
        {
           

            var sceneAWObjs = FindObjectsOfType<AWObj>();
            foreach (var awObj in sceneAWObjs)
            {
                awObj.flaggedDestroyed = true;
                AnythingSafeDestroy.SafeDestroyDelayed(awObj.gameObject);
            }
            var sceneAWHabitats = FindObjectsOfType<AWHabitat>();
            foreach (var awHab in sceneAWHabitats)
            {
                AnythingSafeDestroy.SafeDestroyDelayed(awHab.gameObject);
            }
            // remove groups too
            var sceneFlocks = FindObjectsOfType<FlockManager>();
            foreach (var flockObj in sceneFlocks)
            {
                AnythingSafeDestroy.SafeDestroyDelayed(flockObj.gameObject);
            }
            AnythingCreator.Instance.LayoutGrid.InitNewGrid(10, 10);
        }
        #endregion


        public delegate void OpenApiGenWindowDelegate();
        public static OpenApiGenWindowDelegate openAPIWindowDelegate;

        public static bool CheckAwSetup()
        {
            if (FindObjectOfType<AnythingSetup>())
            {
                return true;
            }
            else
            {

                try
                {
                    var setup = Instantiate(Resources.Load(setupPrefabDir)) as GameObject;
                    setup.name = "AnythingSetup";
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error instantiating setup prefab from {setupPrefabDir}: {e.Message}");
                    return false;
                }

            }


        }
    }
}

