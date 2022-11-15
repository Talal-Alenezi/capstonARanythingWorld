using System;
using AnythingWorld.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif
using UnityEngine;
using UnityEngine.Networking;

namespace AnythingWorld.Habitat
{
    public class AnythingHabitat : MonoBehaviour
    {
        #region Fields
        private AWHabitat habitatController;
        private Dictionary<string, List<GameObject>> _madeThings;
        [SerializeField]
        private static AnythingHabitat instance;
        public static AnythingHabitat Instance
        {
            get
            {
                if (instance == null)
                {
                    if (FindObjectOfType<AnythingHabitat>())
                    {
                        instance = FindObjectOfType<AnythingHabitat>();
                        return instance;
                    }
                    else
                    {
                        var anythingHabitatGameObject = new GameObject("Anything Habitat");
                        anythingHabitatGameObject.transform.position = new Vector3(0, 0, 0);
                        var anythingCreator = anythingHabitatGameObject.AddComponent<AnythingHabitat>();
                        instance = anythingCreator;
                    }

                }
                return instance;
            }
        }
        #endregion

#if UNITY_EDITOR
        private EditorCoroutine makeHabitatEditorRoutine;
#endif
        private Coroutine makeHabitatRoutine;

#if UNITY_EDITOR
        private List<EditorCoroutine> populateHabitatEditorCoroutines = new List<EditorCoroutine>();
#endif
        private List<Coroutine> populateHabitatCoroutines = new List<Coroutine>();

        public void MakeHabitat(string habitatName, bool createCreatures = false)
        {
            if (Application.isEditor && !Application.isPlaying)
            {
#if UNITY_EDITOR
                if (makeHabitatEditorRoutine != null)
                    EditorCoroutineUtility.StopCoroutine(makeHabitatEditorRoutine);

                makeHabitatEditorRoutine = EditorCoroutineUtility.StartCoroutine(RequestHabitatObjects(habitatName, createCreatures), this);
#endif
            }
            else
            {

                if (makeHabitatRoutine != null)
                    StopCoroutine(makeHabitatRoutine);
                makeHabitatRoutine = StartCoroutine(RequestHabitatObjects(habitatName, createCreatures));
            }

        }

        private AWHabitat InstantiateHabitatPrefab(string habitatName)
        {
            if (!EnvironmentMap.TryGetEnvironmentPrefab(habitatName, out var environmentObject)) return null;

            var environmentPrefab = Instantiate(environmentObject, environmentObject.transform.localPosition, environmentObject.transform.rotation) as GameObject;
            environmentPrefab.transform.parent = transform;
            //Add tagging script to habitat instantiation 
            var habitatTag = environmentPrefab.AddComponent<AWHabitat>();
                
            //If batch loader, request
            CallLoadModels(environmentPrefab);
            var lightGameObject = GameObject.FindGameObjectWithTag("AWSun");
            var environmentLighting = EnvironmentMap.EnvironmentLighting(habitatName);
            SetSkybox(environmentLighting);
            SetLightParameters(lightGameObject, environmentLighting);
            SetFogSettings(environmentLighting);
            return habitatTag;
        }

        private static void CallLoadModels(GameObject environmentPrefab)
        {
            if (environmentPrefab.GetComponentInChildren<BatchLoadAssets>())
            {
                //Debug.Log("Found batch load asset");
                environmentPrefab.GetComponentInChildren<BatchLoadAssets>().Load();
            }
            else
            {
                var anythingObjects = environmentPrefab.GetComponentsInChildren<AnythingObject>();
                foreach (var anythingObject in anythingObjects)
                {
                    anythingObject.Load();
                }
            }
        }

        private static void SetLightParameters(GameObject lightGameObject, EnvironmentLights environLights)
        {
            if (lightGameObject != null)
            {
                var awLight = lightGameObject.GetComponent<Light>();
                awLight.color = environLights.LightColor;
                awLight.intensity = environLights.LightIntensity;
            }
        }

        private static void SetSkybox(EnvironmentLights environLights)
        {
            var skyBoxMaterial = Resources.Load("Materials/" + environLights.SkyBoxMaterial) as Material;
            RenderSettings.skybox = skyBoxMaterial;
        }

        private static void SetFogSettings(EnvironmentLights environLights)
        {
            RenderSettings.fogColor = environLights.FogColor;
            RenderSettings.fogMode = environLights.FogType;
            switch (environLights.FogType)
            {
                case FogMode.ExponentialSquared:
                    RenderSettings.fogDensity = environLights.FogDensity;
                    break;
                case FogMode.Linear:
                    RenderSettings.fogStartDistance = environLights.FogStart;
                    RenderSettings.fogEndDistance = environLights.FogEnd;
                    break;
                case FogMode.Exponential:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerator RequestHabitatObjects(string habitatName, bool populateScene, GameObject parent = null)
        {
            var sceneAWHabitats = FindObjectsOfType<AWHabitat>();
            foreach (var awHab in sceneAWHabitats)
            {
#if UNITY_EDITOR
                EditorUtility.SetDirty(awHab.gameObject);
#endif
                AnythingSafeDestroy.SafeDestroyImmediate(awHab.gameObject);
            }
            habitatController = InstantiateHabitatPrefab(habitatName);
            if (populateScene)
            {
                PopulateScene(habitatName);
            }

            yield return null;
        }

        private void PopulateScene(string habitatName)
        {
            if (Application.isEditor && !Application.isPlaying)
            {
#if UNITY_EDITOR
                if (populateHabitatEditorCoroutines.Count > 0)
                {
                    populateHabitatEditorCoroutines.DoAction(EditorCoroutineUtility.StopCoroutine);
                    populateHabitatCoroutines = new List<Coroutine>();
                }
                    

                var routine = EditorCoroutineUtility.StartCoroutine(RequestScenePopulation(habitatName), this);
                populateHabitatEditorCoroutines.AddItem(routine);
#endif
            }
            else
            {
                if (populateHabitatCoroutines.Count > 0)
                {
                    populateHabitatCoroutines.DoAction(StopCoroutine);
                    populateHabitatCoroutines = new List<Coroutine>();
                }

                var routine = StartCoroutine((RequestScenePopulation(habitatName)));
                populateHabitatCoroutines.AddItem(routine);
            }
        }

        private IEnumerator RequestScenePopulation(string habitatName)
        {
            var www = UnityWebRequest.Get(GenerateApiCall(habitatName));
            yield return www.SendWebRequest();
            if (CheckWebRequest.IsError(www))
            {
                yield break;
            }

            var habitatObjectsArray = JSONHelper.GetJSONArray<AWThing>(www.downloadHandler.text);

            if (Application.isEditor && !Application.isPlaying)
            {
#if UNITY_EDITOR
                EditorCoroutineUtility.StartCoroutine(MakeRandomHabitatCreatures(20, habitatObjectsArray), this);
#endif
            }
            else
            {
                StartCoroutine(MakeRandomHabitatCreatures(20, habitatObjectsArray));
            }
        }

        private static string GenerateApiCall(string habitatName)
        {
            var apiCall = $"{AnythingApiConfig.ApiUrlStem}/anything?{habitatName}=true&key={AnythingSettings.Instance.apiKey}&app={AnythingSettings.Instance.appName}";
            if (AnythingSettings.DebugEnabled) Debug.Log(apiCall);
            return apiCall;
        }

        private IEnumerator MakeRandomHabitatCreatures(int desiredObjectCount, AWThing[] objectArray, GameObject parent = null)
        {

            var habitatObjectsCreated = new List<string>();
            for (var i = 0; i < desiredObjectCount; i++)
            {
                var randomIndex = UnityEngine.Random.Range(0, objectArray.Length);
                if (objectArray[randomIndex].behaviour != "static")
                {
                    CreateRandomObject(objectArray, randomIndex, habitatObjectsCreated);
                    i++;
                }
                yield return null;
            }
        }

        private void CreateRandomObject(AWThing[] objectArray, int randomIndex, List<string> habitatObjectsCreated)
        {
            var selectedObject = objectArray[randomIndex].name;
            try
            {
                var awObj = AnythingCreator.Instance.MakeObject(objectArray[randomIndex].name);
                awObj.onObjectMadeSuccessfullyInstanced += AddRandomMovementToObject;
            }
            catch
            {
                Debug.Log($"Problem requesting MakeObject for {objectArray[randomIndex].name}");
            }

            if (!habitatObjectsCreated.Contains(selectedObject))
            {
                habitatObjectsCreated.Add(selectedObject);
            }
        }

        public void AddRandomMovementToObject(AWObj awObj)
        {
            if (awObj == null) return;
            if (habitatController == null) return;

            if (!awObj.GetComponentInChildren<RandomMovement>()) return;
            var randomMovement = awObj.AddBehaviour<RandomMovement>(true);

            FitBehaviourToWalkablePlane(randomMovement);
        }

        private void FitBehaviourToWalkablePlane(RandomMovement randomMovement)
        {
            if (!habitatController.GetComponentInChildren<WalkablePlane>()) return;
            var bounds = habitatController.GetComponentInChildren<WalkablePlane>().GetComponent<MeshCollider>().bounds;
            randomMovement.targetSpawnRadius = (bounds.size.x / 2);
            randomMovement.AnchorPos = bounds.center;
        }
    }

}
