#if UNITY_EDITOR

using Unity.EditorCoroutines.Editor;
using UnityEditor;

#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Diagnostics;
using System.Linq;
using UnityEngine.Networking;
using AnythingWorld.Utilities;
using AnythingWorld.Animation;
using AnythingWorld.DataContainers;
using Debug = UnityEngine.Debug;

namespace AnythingWorld
{
    [ExecuteAlways, Serializable]
    public class AnythingCreator : AnythingBase
    {
        #region Scene Ledger

        [SerializeField, HideInInspector]
        private SceneLedger sceneLedger = null;

        public SceneLedger SceneLedger
        {
            get { return sceneLedger ??= new SceneLedger(); }
        }

        #endregion Scene Ledger

        //Once search has been completed this will be returned.
        public delegate void SearchCompleteDelegate(SearchResult[] thumbnailResults, AWThing[] jsonResults, bool failed = false);

        private SearchCompleteDelegate searchDelegate;

        private Coroutine categorySearchCoroutine = null;

        [SerializeField]
        public static List<GameObject> objectsInScene;

        public List<string> ObjectNamesBeingMade { get; private set; } = new List<string>();

#if UNITY_EDITOR
        private EditorCoroutine makeObjectEditorRoutine;
#endif
        private Coroutine makeObjectRoutine;

        //Controller for swimming creatures in scene
        [SerializeField]
        private FlockManager swimFlockManager;

        //Controller for flying creatures in scene
        [SerializeField]
        private FlockManager flyingFlockManager;

        public bool MakeObjectRoutineRunning { get; private set; } = false;

        public Vector3 layoutGridOffset = new Vector3(-45, 0, -45);

        #region AnythingCreator Instance

        [SerializeField]
        private static AnythingCreator instance;

        public static AnythingCreator Instance
        {
            get
            {
                if (instance == null && CheckAwSetup())
                {
                    instance = GameObject.FindObjectOfType<AnythingCreator>();
                    if (instance != null)
                    {
                        instance.Init();
                        return instance;
                    }
                    else
                    {
                        var anythingCreatorGo = new GameObject();
                        anythingCreatorGo.transform.parent = null;
                        anythingCreatorGo.name = "Anything Creator";
                        var anythingCreator = anythingCreatorGo.AddComponent<AnythingCreator>();
                        instance = anythingCreator;
                        instance.Init();
                    }
                }
                return instance;
            }
        }

        #endregion AnythingCreator Instance

        #region Grid Instance

        [SerializeField]
        private GridUtility layoutGrid = null;

        public GridUtility LayoutGrid
        {
            get
            {
                if (layoutGrid == null)
                {
                    layoutGrid = new GridUtility();
                    layoutGrid.InitNewGrid(10, 10, layoutGridOffset);
#if UNITY_EDITOR
                    EditorApplication.playModeStateChanged -= layoutGrid.SerializeChanges;
                    EditorApplication.playModeStateChanged += layoutGrid.SerializeChanges;
#endif
                }
                layoutGrid.Offset = layoutGridOffset;
                return layoutGrid;
            }
            set
            {
                layoutGrid = value;
            }
        }

        public Queue AnythingQueue { get; private set; } = new Queue();

        #endregion Grid Instance

        public void Start()
        {
#if UNITY_EDITOR
            //This must be done or the delegate subscription will not persist through playmode and the seriaizatin will break.
            if (layoutGrid == null) return;
            EditorApplication.playModeStateChanged -= layoutGrid.SerializeChanges;
            EditorApplication.playModeStateChanged += layoutGrid.SerializeChanges;
#endif
        }

        public void Init()
        {
            //AnythingQueue = new Queue();
            //ObjectNamesBeingMade = new List<string>();
            AnythingWorld.Utilities.MaterialCacheUtil.CacheAndGetMaterials();
        }

        #region Adjust and Clones

        public void AdjustGameObjects(string objName, int quantity)
        {
            var thingGroup = SearchSceneLedger(objName);
            thingGroup?.Adjust(quantity);
        }

        public void AdjustGameObjects(ThingGroup thingGroup, int quantity)
        {
            thingGroup?.Adjust(quantity);
        }

        #endregion Adjust and Clones

        #region MakeObject

        public List<AWObj> MakeManyObjects(string objName, int quantity, bool hasBehaviour = true)
        {
            if (quantity < 0) return null;
            var madeObjects = new List<AWObj>();
            for (var i = 0; i < quantity; i++)
            {
                var goRef = new GameObject();
                var awRef = goRef.AddComponent<AWObj>();
                AddToQueue(objName.ToLower(), true, hasBehaviour, true, false, Vector3.zero, Quaternion.identity, Vector3.one, null, goRef);
                madeObjects.Add(awRef);
            }
            return madeObjects;
        }

        /// <summary>
        /// Adds an object of name <c>objName</c> to the AnythingCreator make queue.
        /// <example> For example:
        /// <code>
        /// AWObj catAWObj = AnythingCreator.Instance.MakeObject(cat);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="objectName">Name of object to be requested from server.</param>
        /// <param name="hasBehaviour">Will object receive default behaviour and animations.</param>
        /// <param name="hasCollider">Will object receive collider.</param>
        /// <returns>Reference to <c>AWObj</c> on the new object.</returns>
        public AWObj MakeObject(string objectName, bool hasBehaviour = true, bool hasCollider = true)
        {
            // Game object is created ahead of asynchronous make process so that we can pass a reference to the AWObj back to the user at maketime.
            var goRef = new GameObject();
            // AWObj cannot be instantiated
            var awRef = goRef.AddComponent<AWObj>();
            // Returns reference to caller.
            AddToQueue(objectName.ToLower(), true, hasBehaviour, hasCollider, true, transform.localPosition, transform.localRotation, transform.localScale, null, goRef);
            return awRef;
        }

        public AWObj MakeObject(string objectName, bool placeOnGrid, bool hasBehaviour = true, bool hasCollider = true)
        {
            // Game object is created ahead of asynchronous make process so that we can pass a reference to the AWObj back to the user at maketime.
            var goRef = new GameObject();
            // AWObj cannot be instantiated
            var awRef = goRef.AddComponent<AWObj>();
            // Returns reference to caller.
            AddToQueue(objectName.ToLower(), placeOnGrid, hasBehaviour, hasCollider, true, transform.localPosition, transform.localRotation, transform.localScale, null, goRef);
            return awRef;
        }

        /// <summary>
        /// Adds an object of name <c>objName</c> to the AnythingCreator make queue.
        /// <para>
        /// Takes a parent transform that the request object will be parented to.
        /// </para>
        /// </summary>
        /// <param name="objectName">Name of object to be requested from server.</param>
        /// <param name="parentTransform">Transform to parent object to.</param>
        /// <param name="hasBehaviour">Will object receive default behaviour and animations.</param>
        /// <param name="hasCollider">Will object receive collider.</param>
        /// <param name="positionGlobally">Will object transform be applied globally or locally (Default: locally)</param>
        /// <returns>Reference to <c>AWObj</c> on the new object.</returns>
        public AWObj MakeObject(string objectName, Transform parentTransform, bool hasBehaviour = true, bool hasCollider = true, bool positionGlobally = false)
        {
            var goRef = new GameObject();
            var awRef = goRef.AddComponent<AWObj>();
            AddToQueue(objectName.ToLower(), false, hasBehaviour, hasCollider, positionGlobally, goRef.transform.localPosition, goRef.transform.localRotation, goRef.transform.localScale, parentTransform, goRef);
            return awRef;
        }

        /// <summary>
        /// Adds an object of name <c>objName</c> to the AnythingCreator make queue.
        /// <para>
        /// Allows object to be parented to an object, and a given transform applied locally or globally.
        /// </para>
        /// </summary>
        /// <param name="objectName">Name of object to be requested from server.</param>
        /// <param name="objectTransform">Transform to be applied to object after creation.</param>
        /// <param name="parentTransform">Transform that new object will be parented to.</param>
        /// <param name="hasBehaviour">Will object receive default behaviour and animations.</param>
        /// <param name="hasCollider">Will object receive collider.</param>
        /// <param name="positionGlobally">Will object transform be applied globally or locally (Default: locally)</param>
        /// <returns>Reference to <c>AWObj</c> on the new object.</returns>
        public AWObj MakeObject(string objectName, Transform objectTransform, Transform parentTransform, bool hasBehaviour = true, bool hasCollider = true, bool positionGlobally = false)
        {
            var goRef = new GameObject();
            var awRef = goRef.AddComponent<AWObj>();
            AddToQueue(objectName.ToLower(), false, hasBehaviour, hasCollider, positionGlobally, objectTransform.localPosition, objectTransform.localRotation, objectTransform.localScale, parentTransform, goRef);
            return awRef;
        }

        /// <summary>
        /// Adds an object of name <c>objName</c> to the AnythingCreator make queue.
        /// </summary>
        /// <param name="objectName">Name of object to be requested from server.</param>
        /// <param name="objectPosition"><c>Vector3</c> position to be applied to new object.</param>
        /// <param name="hasBehaviour">Will object receive default behaviour and animations.</param>
        /// <param name="hasCollider">Will object receive collider.</param>
        /// <returns>Reference to <c>AWObj</c> on the new object.</returns>
        public AWObj MakeObject(string objectName, Vector3 objectPosition, bool hasBehaviour = true, bool hasCollider = true)
        {
            var goRef = new GameObject();
            var awRef = goRef.AddComponent<AWObj>();
            AddToQueue(objectName.ToLower(), false, hasBehaviour, hasCollider, false, objectPosition, Quaternion.identity, Vector3.one, null, goRef);
            return awRef;
        }

        /// <summary>
        /// Adds an object of name <c>objName</c> to the AnythingCreator make queue.
        /// </summary>
        /// <param name="objectName">Name of object to be requested from server.</param>
        /// <param name="objectPosition"><c>Vector3</c> position to be applied to new object.</param>
        /// <param name="objectRotation"><c>Quaternion</c> rotation to be applied to new object.</param>
        /// <param name="hasBehaviour">Will object receive default behaviour and animations.</param>
        /// <param name="hasCollider">Will object receive collider.</param>
        /// <returns>Reference to <c>AWObj</c> on the new object.</returns>
        public AWObj MakeObject(string objectName, Vector3 objectPosition, Quaternion objectRotation, bool hasBehaviour = true, bool hasCollider = true)
        {
            var goRef = new GameObject();
            var awRef = goRef.AddComponent<AWObj>();
            AddToQueue(objectName.ToLower(), false, hasBehaviour, hasCollider, false, objectPosition, objectRotation, Vector3.one, null, goRef);
            return awRef;
        }

        /// <summary>
        /// Adds an object of name <c>objName</c> to the AnythingCreator make queue.
        /// </summary>
        /// <param name="objectName">Name of object to be requested from server.</param>
        /// <param name="objectPosition"><c>Vector3</c> position to be applied to new object.</param>
        /// <param name="objectRotation"><c>Quaternion</c> rotation to be applied to new object.</param>
        /// <param name="objectScale"><c>Vector3</c> scale to be applied to new object.</param>
        /// <param name="hasBehaviour">Will object receive default behaviour and animations.</param>
        /// <param name="hasCollider">Will object receive collider.</param>
        /// <returns></returns>
        public AWObj MakeObject(string objectName, Vector3 objectPosition, Quaternion objectRotation, Vector3 objectScale, bool hasBehaviour = true, bool hasCollider = true)
        {
            var goRef = new GameObject();
            var awRef = goRef.AddComponent<AWObj>();
            AddToQueue(objectName.ToLower(), false, hasBehaviour, hasCollider, false, objectPosition, objectRotation, objectScale, null, goRef);
            return awRef;
        }

        /// <summary>
        /// Adds an object of name <c>objName</c> to the AnythingCreator make queue.
        /// </summary>
        /// <param name="objectName">Name of object to be requested from server.</param>
        /// <param name="objectPosition"><c>Vector3</c> position to be applied to new object.</param>
        /// <param name="objectRotation"><c>Quaternion</c> rotation to be applied to new object.</param>
        /// <param name="objectScale"><c>Vector3</c> scale to be applied to new object.</param>
        /// <param name="parentTransform">Transform that new object will be parented to.</param>
        /// <param name="hasBehaviour">Will object receive default behaviour and animations.</param>
        /// <param name="hasCollider">Will object receive collider.</param>
        /// <param name="positionGlobally">Will object transform be applied globally or locally (Default: locally)</param>
        /// <returns></returns>
        public AWObj MakeObject(string objectName, Vector3 objectPosition, Quaternion objectRotation, Vector3 objectScale, Transform parentTransform, bool hasBehaviour = true, bool hasCollider = true, bool positionGlobally = false)
        {
            var goRef = new GameObject();
            var awRef = goRef.AddComponent<AWObj>();
            goRef.transform.parent = parentTransform;
            AddToQueue(objectName.ToLower(), false, hasBehaviour, hasCollider, positionGlobally, objectPosition, objectRotation, objectScale, parentTransform, goRef);
            return awRef;
        }

        #endregion MakeObject

        #region Search

        public void RequestCategorySearchResults(string searchTerm, SearchCompleteDelegate searchCompleteDelegate)
        {
            try
            {
                categorySearchCoroutine = StartCoroutine(CategorySearch(searchTerm, searchCompleteDelegate));
            }
            catch (Exception e)
            {
                var emptyList = new List<SearchResult>();
                searchDelegate += searchCompleteDelegate;
                searchDelegate(null, null);
                searchDelegate -= searchCompleteDelegate;

                Debug.LogError("Exception thrown while attempting category search");
                Debug.LogError(e);
            }
        }

        public void CancelCategorySearchCoroutine()
        {
            if (categorySearchCoroutine != null)
            {
                StopCoroutine(categorySearchCoroutine);
            }
        }

        /// <summary>
        /// IEnumerator <c>CategorySearch</c> requests a list of search results and generates a list of <see cref="SearchResult"/> objects.
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="delegateFunc">Function to be called once search results have loaded.</param>
        /// <returns></returns>
        public IEnumerator CategorySearch(string searchTerm, SearchCompleteDelegate delegateFunc)
        {
            if (searchTerm == "")
            {
                var emptyList = new List<SearchResult>();
                searchDelegate += delegateFunc;
                searchDelegate(emptyList.ToArray(), null);
                searchDelegate -= delegateFunc;
                yield break;
            }

            searchDelegate += delegateFunc;

            #region Get Result JSON Array

            var apiKey = AnythingSettings.Instance.apiKey;
            var appName = AnythingSettings.Instance.appName;

            if (string.IsNullOrEmpty(appName))
            {
                appName = AnythingSettings.Instance.appName = "My App";
            }
            if (string.IsNullOrEmpty(apiKey))
            {
                Debug.LogError("Please enter an API Key in AnythingSettings!");
                yield break;
            }

            // Trim our request string
            char[] charsToTrim = { '*', ' ', '\'', ',', '.' };
            searchTerm = searchTerm.Trim(charsToTrim);
            // lowercase our request string
            searchTerm = searchTerm.ToLower();
            // Make API call string
            var apiCall = AnythingApiConfig.ApiUrlStem + "/anything?key=" + AnythingSettings.ApiKey + "&search=" + searchTerm;
            if (AnythingSettings.Instance.showDebugMessages) Debug.Log($"Requesting search results for \"{searchTerm}\". \n URL: \"{apiCall}\"");

            var stopwatch = new System.Diagnostics.Stopwatch();
            var www = UnityWebRequest.Get(apiCall);
            yield return www.SendWebRequest();

            if (CheckWebRequest.IsError(www))
            {
                var errorResponseString = www.downloadHandler.text;
                ErrorResponse errorResponse = null;
                try
                {
                    errorResponse = JsonUtility.FromJson<ErrorResponse>(errorResponseString);
                }
                catch
                {
                    searchDelegate += delegateFunc;
                    searchDelegate(null, null);
                    searchDelegate -= delegateFunc;
                    Debug.LogWarning($"Unexpected error returned from server: \n {errorResponseString}");
                    yield break;
                }

                if (errorResponse.code == "Model not found")
                {
                    searchDelegate += delegateFunc;
                    searchDelegate(Array.Empty<SearchResult>(), null);
                    searchDelegate -= delegateFunc;
                    if (AnythingSettings.DebugEnabled) Debug.LogWarning($"{errorResponse.code}: {errorResponse.message}");
                    yield break;
                }
                else
                {
                    searchDelegate += delegateFunc;
                    searchDelegate(Array.Empty<SearchResult>(), null);
                    searchDelegate -= delegateFunc;
                    Debug.LogWarning($"{errorResponse.code}: {errorResponse.message}");
                    yield break;
                }
            }
            //Convert response to json format
            var result = www.downloadHandler.text;
            result = FixJson(result);
            if (AnythingSettings.Instance.showDebugMessages) Debug.Log($"Response returned for for \"{searchTerm}\". \n {result.Substring(0, 500)}...");
            Regex.Replace(www.downloadHandler.text, @"[[\]]", "");
            var searchResultList = new List<SearchResult>();

            AWThing[] resultsArray;
            try
            {
                resultsArray = JSONHelper.FromJson<AWThing>(result);
            }
            catch (Exception e)
            {
                Debug.LogError("Problem generating array: " + e.Message);
                yield break;
            }

            #endregion Get Result JSON Array

            var searchResultArray = new SearchResult[resultsArray.Length];
            for (var i = 0; i < searchResultArray.Length; i++)
            {
                try
                {
                    searchResultArray[i] = new SearchResult(resultsArray[i]);
                }
                catch
                {
                    Debug.Log($"Error setting value at index {i}");
                }
            }
            yield return GetThumbnailTextures(searchResultArray);

            //Turn JSON into AWThing data format.
            searchDelegate(searchResultArray, resultsArray);

            //Unsubscribe search delegate
            searchDelegate -= delegateFunc;
        }

        #endregion Search

        private IEnumerator GetThumbnailTextures(SearchResult[] resultsList)
        {
            var requests = new List<UnityWebRequestAsyncOperation>(resultsList.Length);
            // Start all requests
            foreach (var result in resultsList)
            {
                var url = result.data.thumbnails.aw_thumbnail_transparent ?? result.data.model.other.aw_thumbnail;
                var www = UnityWebRequestTexture.GetTexture(url);
                // starts the request but doesn't wait for it for now
                requests.Add(www.SendWebRequest());
            }

            // Now wait for all requests parallel
            yield return new WaitUntil(() => AllRequestsDone(requests));

            // Now evaluate all results
            HandleAllRequestsWhenFinished(requests, resultsList);

            foreach (var request in requests)
            {
                try
                {
                    request.webRequest.Dispose();
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Problem disposing of async web request");
                    Debug.LogException(e);
                }
            }
        }

        private bool AllRequestsDone(IEnumerable<UnityWebRequestAsyncOperation> requests)
        {
            return requests.All(r => r.isDone);
        }

        private void HandleAllRequestsWhenFinished(IReadOnlyList<UnityWebRequestAsyncOperation> requests, SearchResult[] searchResult)
        {
            for (var i = 0; i < requests.Count; i++)
            {
                var www = requests[i].webRequest;
                if (CheckWebRequest.IsSuccess(www))
                {
                    // Else if successful
                    var myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    searchResult[i].Thumbnail = myTexture;
                }
                else
                {
                    // If failed
                    searchResult[i].ResultHasThumbnail = false;
                }
            }
        }

        private string FixJson(string value)
        {
            value = "{\"Items\":" + value + "}";
            return value;
        }

        #region Creation

        /// <summary>
        /// Creates <see cref="AnythingQueueObject"/> from parameters and adds it to the <c>anythingQueue</c>.
        /// </summary>
        private void AddToQueue(string objName, bool autoLayout, bool hasBehaviour, bool hasCollider, bool positionGlobally, Vector3 objectPos, Quaternion objectRot, Vector3 objectScale, Transform parentTrans, GameObject objRef)
        {
            if (AnythingQueue == null)
            {
                Init();
            }

            var aQObject = new AnythingQueueObject(objName, autoLayout, hasBehaviour, hasCollider, positionGlobally, objectPos, objectRot, objectScale, parentTrans, objRef);
            AnythingQueue.Enqueue(aQObject);
            if (MakeObjectRoutineRunning) return;
            //If MakeObjectProcess is not currently running make it
            if (Application.isEditor && !Application.isPlaying)
            {
#if UNITY_EDITOR
                if (makeObjectEditorRoutine != null)
                    EditorCoroutineUtility.StopCoroutine(makeObjectEditorRoutine);
                makeObjectEditorRoutine = EditorCoroutineUtility.StartCoroutine(MakeObjectFromQueueLoop(), this);
#endif
            }
            else
            {
                if (makeObjectRoutine != null)
                    StopCoroutine(makeObjectRoutine);
                makeObjectRoutine = StartCoroutine(MakeObjectFromQueueLoop());
            }
        }

        private const float MakeObjectTimeout = 40000; //30s
        private const float IndividualObjectTimeout = 10000; //10s
        private IEnumerator MakeObjectFromQueueLoop()
        {
            MakeObjectRoutineRunning = true;
            var sw = new Stopwatch();
            sw.Start();
            //While objects are in make queue
            while (AnythingQueue.Count > 0)
            {
                if (sw.ElapsedMilliseconds > MakeObjectTimeout)
                {
                    MakeObjectRoutineRunning = false;
                    throw new TimeoutException($"Timed out making object queue.");
                }
                if (!(AnythingQueue.Dequeue() is AnythingQueueObject dequeuedObject)) continue;
                if (sw.ElapsedMilliseconds - dequeuedObject.startWaitTime > IndividualObjectTimeout)
                {
                    Debug.LogWarning($"Object '{dequeuedObject.objName}' waited too long to clone ({IndividualObjectTimeout/1000}s), removing from queue.");
                }else if (ObjectNamesBeingMade.Contains(dequeuedObject.objName))
                {
                    if (dequeuedObject.startWaitTime == 0) dequeuedObject.startWaitTime = sw.ElapsedMilliseconds;
                    AnythingQueue.Enqueue((dequeuedObject));
                    yield return WaitForEndOfFrame();
                    continue;
                }

                if (AnythingSettings.Instance.showDebugMessages) Debug.Log($"Dequeued \"{dequeuedObject?.objName}\" creation request. Attempting make process.");
                var cloneableObject = FindCloneableObject(FindObjectsOfType(typeof(AWObj),true) as AWObj[], dequeuedObject);
                var cloneable = cloneableObject != null;


                if (cloneable)
                {
                    if (AnythingSettings.DebugEnabled) Debug.Log($"Clowning object {dequeuedObject.objName} after waiting {(sw.ElapsedMilliseconds - dequeuedObject.startWaitTime) / 1000}s");
                    
                    if (AnythingSettings.Instance.showDebugMessages) Debug.Log($"{dequeuedObject?.objName} is cloned from {cloneableObject.name} ", cloneableObject);
                    StartCoroutineEditorSafe(CloneObject(dequeuedObject, cloneableObject));
                }
                else
                {

                    ObjectNamesBeingMade.Add(dequeuedObject?.objName);
                    if(AnythingSettings.Instance.showDebugMessages) Debug.Log($"Making new object {dequeuedObject.objName} after waiting {(sw.ElapsedMilliseconds - dequeuedObject.startWaitTime)/1000}s");
                    StartCoroutineEditorSafe(MakeNewObject(dequeuedObject));
                }
            }


            MakeObjectRoutineRunning = false;
            yield return WaitForEndOfFrame();
        }

        private void StartCoroutineEditorSafe(IEnumerator func)
        {
            if (Application.isEditor && !Application.isPlaying)
            {
#if UNITY_EDITOR
                EditorCoroutineUtility.StartCoroutine(func, this);
#endif
            }
            else
            {
                StartCoroutine(func);
            }
        }

        private static IEnumerator WaitForEndOfFrame()
        {
            if (Application.isEditor && !Application.isPlaying)
            {
#if UNITY_EDITOR

                yield return new EditorWaitForSeconds(0.01f);
#endif
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }

        private static GameObject FindCloneableObject(AWObj[] cloneableObjects, AnythingQueueObject dequeuedObjectParams)
        {
            if (cloneableObjects == null) return null;
            Array.Reverse(cloneableObjects);
            foreach (var objectController in cloneableObjects)
            {
                if (objectController == null) continue;
                if (objectController.flaggedDestroyed) continue;
                if (objectController.ModelName != dequeuedObjectParams.objName) continue;

                if (!objectController.GetComponentInChildren<MeshFilter>()) continue;
                if (objectController.GetComponentInChildren<MeshFilter>().sharedMesh != null) 
                    return objectController.gameObject;
            }
            return null;
        }

        private IEnumerator MakeNewObject(AnythingQueueObject queuedObjectParameters)
        {
            var thingGameObject = AssignTargetGameObject(queuedObjectParameters);
            var thingAwObj = thingGameObject.GetComponent<AWObj>();
            thingGameObject.name = queuedObjectParameters.objName;
            thingAwObj.MakeAwObj(queuedObjectParameters.objName, queuedObjectParameters.hasBehaviour, queuedObjectParameters.hasCollider);

            const float timeout = 40f;
#if UNITY_EDITOR
            var startupTime = (float)EditorApplication.timeSinceStartup;
#else
            var startupTime = Time.realtimeSinceStartup;
#endif

            while (thingAwObj.CanBeMade)
            {
                var timeHanging = TimeHanging(startupTime);
                if (timeHanging > timeout)
                {
                    CancelCreation(thingAwObj, thingGameObject, queuedObjectParameters);
                    yield break;
                }
                yield return WaitForEndOfFrame();
            }
            if (HasError(thingAwObj))
            {
                DisplayError(thingAwObj);
                DestroyFailedObject(thingAwObj, thingGameObject);
                yield return WaitForEndOfFrame();
                ObjectNamesBeingMade.Remove(queuedObjectParameters.objName);
                yield break;
            }
            thingAwObj.awThingTransform.TryGetComponent<Rigidbody>(out var rb);
            DisableRigidbody(rb);
            SetRendererActive(thingAwObj,false);

            if (!TrySetPosition(queuedObjectParameters, thingAwObj, thingGameObject)) yield break;
            thingAwObj.ObjPositioned = true;
            yield return WaitForEndOfFrame();
            SetRendererActive(thingAwObj, true);
            EnableRigidbody(thingAwObj, rb);

            yield return WaitForEndOfFrame();

            //Debug.Log("object removed from making list");
            ObjectNamesBeingMade.Remove(queuedObjectParameters.objName);
            //Debug.Log("Finished new object");
        }

    

        private IEnumerator CloneObject(AnythingQueueObject objectParameters, GameObject objectBeingCloned)
        {
            if (objectParameters.gameObjectReference != null)
                AnythingSafeDestroy.SafeDestroyDelayed(objectParameters.gameObjectReference);

            AWObj awObjToClone;

            if (objectBeingCloned != null)
            {
                awObjToClone = objectBeingCloned.GetComponent<AWObj>();
            }
            else
            {
                yield return MakeNewObject(objectParameters);
                yield break;
            }

            if (awObjToClone != null && !awObjToClone.flaggedDestroyed)
            {
                if (AnythingSettings.Instance.showDebugMessages) Debug.Log($"Found AWObj to clone for {objectParameters.objName}");
            }
            else
            {
                yield return MakeNewObject(objectParameters);
                yield break;
            }

            //Instantiate clone of object
            var targetGameObject = Instantiate(objectBeingCloned);

            AWObj targetController = null;
            if (targetGameObject.TryGetComponent<AWObj>(out var clonedController))
            {
                //Assign reference to cloned controller back to passed in AWObj
                targetController = clonedController;
                targetController.ObjectData = clonedController.ObjectData;
            }
            targetController.awThingTransform.TryGetComponent<Rigidbody>(out var rb);



            //Parent to parent transform
            if (objectParameters.parentTransform != null) targetGameObject.transform.parent = objectParameters.parentTransform;

            //Scale new object
            targetGameObject.transform.localScale = objectParameters.objectScale;

            //SetRendererActive(targetController, false);

            if (!TrySetPosition(objectParameters, targetController, targetGameObject)) yield break;

            //SetRendererActive(targetController, false);


            if (HasError(targetController))
            {
                DisplayError(targetController);
                DestroyFailedObject(targetController, targetGameObject);
                yield break;
            }

            if (targetController != null)
            {
                targetController.ObjMade = true;
            }

            targetGameObject.name = awObjToClone.name;
            targetController.ObjPositioned = true;


            //SetRendererActive(targetController, true);


            yield return WaitForEndOfFrame();
            yield return null;
        }


        private static void SetRendererActive(AWObj thingAwObj, bool isEnabled)
        {
            foreach (var renderer in thingAwObj.GetComponentsInChildren<MeshRenderer>())
            {
                renderer.enabled = isEnabled;
            }
        }

        private static void DisableRigidbody(Rigidbody rb)
        {
            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
                rb.Sleep();
            }
        }

        private static void EnableRigidbody(AWObj thingAwObj, Rigidbody rb)
        {
            if (rb != null)
            {
                TurnOnGravitySelectively(thingAwObj, rb);
                rb.isKinematic = false;
                rb.WakeUp();
            }
        }

        private static void TurnOnGravitySelectively(AWObj thingAwObj, Rigidbody rb)
        {
            if (thingAwObj.ObjectData.category=="vehicle_uniform")
            {
                if (thingAwObj.ObjectData.behaviour == "drive")
                {
                    rb.useGravity = false;
                    return;
                }
            }
            if (thingAwObj.ObjectData.behaviour == "float")
            {
                rb.useGravity = false;
                return;
            }
            if (thingAwObj.ObjectData.behaviour == "static")
            {
                rb.useGravity = false;
                return;
            }
            rb.useGravity = true;
        }
        private void RemoveBehaviour(GameObject targetGameObject, AWObj targetController)
        {
            var behaviourScript = targetGameObject.GetComponentInChildren<Behaviours.AWBehaviour>();
            if (behaviourScript != null)
            {
                targetController.RemoveExistingBehaviours();
            }
        }
        private void ResetBehaviour(AWObj targetController, GameObject targetGameObject, Rigidbody rb, bool hasBehaviour)
        {
            Debug.Log("resetting behaviour");
            ResetObjectPartsRotationAndPosition(targetGameObject, rb);
            if (hasBehaviour)
            {

                Debug.Log("adding default behaviour");
                targetController.ObjectData.addDefaultBehaviour = true;
                targetController.AddDefaultBehaviourToObject();
            }
           
        }
        private static void DisplayError(AWObj thingAwObj)
        {
            if (thingAwObj.objError != null)
            {
                Debug.LogWarning($"Cancelled make process early for \"{thingAwObj.ModelName}\" due to error. \n {thingAwObj.objError.code}: {thingAwObj.objError.message}");

            }
            else
            {
                Debug.LogWarning($"Error: Unspecified error making { thingAwObj.ModelName}: Make object process canceled early.");
            }
        }

        private static float TimeHanging(float startupTime)
        {
#if UNITY_EDITOR
            return (float)EditorApplication.timeSinceStartup - startupTime;
#else
                    return Time.realtimeSinceStartup - startupTime;
#endif
        }



        /// <summary>
        /// Tries to set position and returns false if failed.
        /// </summary>
        /// <param name="objectParameters"></param>
        /// <param name="thingAwObj"></param>
        /// <param name="thingGameObject"></param>
        /// <returns></returns>
        private bool TrySetPosition(AnythingQueueObject objectParameters, AWObj thingAwObj, GameObject thingGameObject)
        {
            if (objectParameters == null) return false;
            if (thingGameObject == null) return false;
            try
            {
                var category = thingAwObj.ObjectData.category;
                var behaviour = thingAwObj.ObjectData.behaviour;
                if (GroupMap.IsObjectFlocking(behaviour, category))
                {
                    AssignObjectToFlock(thingAwObj, objectParameters);
                }
                else
                {
                    if (objectParameters.autoLayout)
                    {
                        objectParameters.objectPos = GetGridPosition(thingAwObj, thingAwObj.ObjectScale);
                    }
                    if (objectParameters.parentTransform != null)
                    {
                        thingGameObject.transform.parent = objectParameters.parentTransform;
                    }
                    thingAwObj.awThingTransform.localPosition = Vector3.zero;
                    PositionAndScaleObject(thingGameObject, objectParameters);
                }
            }
            catch(Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            return true;
        }

        private GameObject AssignTargetGameObject(AnythingQueueObject anythingQueueObject)
        {
            if (anythingQueueObject.gameObjectReference != null)
            {
                var thingGameObject = anythingQueueObject.gameObjectReference;
                return thingGameObject;
            }
            else
            {
                var thingGameObject = new GameObject("Loading Object");
                thingGameObject.AddComponent<AWObj>();
                return thingGameObject;
            }
        }

        private bool HasError(AWObj awObj)
        {
            if (awObj == null) return true;
            if (awObj.objHasError) return true;
            //if (awObj.objError != null) return true;
            return false;
        }

        private static void DestroyFailedObject(AWObj awObj, GameObject o)
        {
            try
            {
                AnythingSafeDestroy.SafeDestroyDelayed(o);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Error destroying failed object {awObj.ModelName}: {e.Message}");
                Debug.LogException(e);
            }
        }

        private void CancelCreation(AWObj thingAwObj, GameObject thingGameObject, AnythingQueueObject objectParameters)
        {
            if (thingGameObject != null)
            {
                Debug.LogError($"Timeout waiting for AWObj {thingGameObject.name} finish making in AWC!");
                thingAwObj.StopAllCoroutines();
                AnythingSafeDestroy.SafeDestroyImmediate(thingGameObject);
                thingAwObj.objHasError = true;
            }
            ObjectNamesBeingMade.Remove(objectParameters.objName);
        }

        private static void ResetObjectPartsRotationAndPosition(GameObject targetGameObject, Rigidbody rb)
        {
            foreach (var colliders in targetGameObject.GetComponentsInChildren<Collider>())
            {
                colliders.enabled = false;
            }
            ResetBodyTransform(targetGameObject);

            foreach (var joint in targetGameObject.GetComponentsInChildren<Joint>())
            {
                bool rbEnabledOriginally = false;
                if (joint.TryGetComponent<Collider>(out var jointCollider)) { jointCollider.enabled = false; }
                if (joint.TryGetComponent<Rigidbody>(out var jointRb))
                {
                    rbEnabledOriginally = rb.isKinematic;
                    rb.isKinematic = false;
                }

                joint.transform.localPosition = Vector3.zero;
                joint.transform.localRotation = Quaternion.identity;

                ResetJointChildTransformsRotations(joint);
                if (jointRb) rb.isKinematic = rbEnabledOriginally;
            }

            foreach (var colliders in targetGameObject.GetComponentsInChildren<Collider>())
            {
                colliders.enabled = true;
            }
        }

        private static void ResetJointChildTransformsRotations(Joint joint)
        {
            foreach (var loadedObjectData in joint.gameObject.GetComponentsInChildren<LoadedObjectData>())
            {
                loadedObjectData.transform.localRotation = loadedObjectData.zeroedRotation;
                loadedObjectData.transform.localPosition = loadedObjectData.zeroedPosition;
            }
        }

        private static void ResetBodyTransform(GameObject targetGameObject)
        {
            foreach (var rigidb in targetGameObject.GetComponentsInChildren<Rigidbody>())
            {
                if (rigidb.name == "body")
                {
                    rigidb.transform.localPosition = Vector3.zero;
                    rigidb.transform.localRotation = Quaternion.identity;

                    //ResetChildTransformRb(rigidb);
                    break;
                }
            }
        }

        private static void ResetChildTransformRb(Rigidbody rigidb)
        {
            foreach (var bodyChildTransform in rigidb.GetComponentsInChildren<Transform>())
            {
                bodyChildTransform.localPosition = Vector3.zero;
                bodyChildTransform.localRotation = Quaternion.identity;
            }
        }

        private void PositionAndScaleObject(GameObject gO, AnythingQueueObject queueObject)
        {
            if (queueObject.positionGlobally == true)
            {
                gO.transform.SetPositionAndRotation(queueObject.objectPos, queueObject.objectRot);
            }
            else
            {
                gO.transform.parent = queueObject.parentTransform;
                gO.transform.localPosition = queueObject.objectPos;
                gO.transform.localRotation = queueObject.objectRot;
            }
            gO.transform.localScale = queueObject.objectScale;
        }

        /// <summary>
        /// Search scene ledger for object of matching name.
        /// </summary>
        /// <param name="_name">String to find matching object name to.</param>
        /// <returns>Matching <see cref="ThingGroup"/> object holding instances of the specific game object.</returns>
        private ThingGroup SearchSceneLedger(string _name)
        {
            foreach (var group in SceneLedger.ThingGroups.ToArray())
            {
                if (group.objectName == _name)
                {
                    return group;
                }
            }

            return SceneLedger.NewThingGroup(_name);
        }

        private void AssignObjectToFlock(AWObj objectController, AnythingQueueObject objectParameters)
        {
            var flockType = GroupMap.GetFlockTypeFromBehaviour(BehaviourMap.GetBehaviour(objectParameters.objName.ToLower()));

            if (flockType == GroupMap.FlockType.flying)
            {
                AddObjectToFlyingFlock(objectController);
            }
            else if (flockType == GroupMap.FlockType.swimming)
            {
                AddObjectToSwimmingFlock(objectController);
            }
        }

        private void AddObjectToFlyingFlock(AWObj objectController)
        {
            //If no flying flock, make one
            if (flyingFlockManager == null)
            {
                flyingFlockManager = new GameObject().AddComponent<FlockManager>();
                flyingFlockManager.name = "Flying Flock";
                flyingFlockManager.transform.Translate(0, flyingFlockManager.flockZoneSize, 0);
            }

            var flockMember = objectController.gameObject.AddComponent<FlockMember>();
            flockMember.flockMemberType = FlockMember.MemberType.bird;
            flyingFlockManager.AddMember(flockMember);
        }

        private void AddObjectToSwimmingFlock(AWObj objectController)
        {
            //If no swim flock, make one
            if (swimFlockManager == null)
            {
                swimFlockManager = new GameObject().AddComponent<FlockManager>();
                swimFlockManager.name = "Swim Flock";
                swimFlockManager.transform.Translate(0, swimFlockManager.flockZoneSize, 0);
            }
            var flockMember = objectController.gameObject.AddComponent<FlockMember>();
            flockMember.flockMemberType = FlockMember.MemberType.fish;
            swimFlockManager.AddMember(flockMember);
        }

        #endregion Creation

        #region Grid Layout

        protected Vector3 GetGridPosition(AWObj gridObj, float gridScale)
        {
            var gridPos = LayoutGrid.TryGetNextAvailablePosition(gridScale);


            if (!gridObj.requestedObject.preserveOriginalPosition)
            {
                var yOffset = gridObj.BoundsYOffset;
                gridPos.y = yOffset;
            }

            return gridPos;
        }

        public void ResetAutoLayout()
        {
            layoutGrid = null;
            //It through each group in the scene ledger and place each item in group on the grid.
            foreach (var group in SceneLedger.ThingGroups.ToArray())
            {
                foreach (var awCreature in group.awInstances.Where(awCreature => awCreature != null))
                {
                    awCreature.PrefabTransform.transform.position = GetGridPosition(awCreature, awCreature.ObjectScale);
                }
            }
        }

        #endregion Grid Layout

        public string GetDisplayName(string rawObjectName)
        {
            if (rawObjectName != null)
            {
                var displayName = Regex.Replace(rawObjectName, @"\d", "");
                var displayArray = displayName.ToCharArray();
                displayArray[0] = char.ToUpper(displayArray[0]);
                return new string(displayArray);
            }
            else
            {
                return null;
            }
        }
    }
}