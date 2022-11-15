using AnythingWorld.Animation;
using AnythingWorld.Behaviours;
using AnythingWorld.DataContainers;
using AnythingWorld.Utilities;
using Dummiesman;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif
using UnityEngine;
using UnityEngine.Networking;

namespace AnythingWorld
{
    struct ShaderPropertyEditing<T>
    {
        public string property;
        public T variable;
    }

    [Serializable, ExecuteAlways]
    public class AWObj : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        public bool flaggedDestroyed = false;

        [SerializeField, HideInInspector]
        public delegate void OnBehaviourRefreshDelegate();

        [SerializeField, HideInInspector]
        public OnBehaviourRefreshDelegate onBehaviourRefreshMethod;

        [SerializeField, HideInInspector]
        public delegate void OnObjectMadeSuccessfully();

        [SerializeField, HideInInspector]
        public OnObjectMadeSuccessfully onObjectMadeSuccessfullyDelegate;

        [SerializeField, HideInInspector]
        public delegate void OnObjectMadeSuccessfullyInstanced(AWObj instance);

        [SerializeField, HideInInspector]
        public OnObjectMadeSuccessfullyInstanced onObjectMadeSuccessfullyInstanced;

        [SerializeField, HideInInspector]
        public delegate void OnObjectFailed(AWObj instance);
        [SerializeField, HideInInspector]
        public OnObjectFailed onObjectFailedDelegate;

        private string apiOverride = "";

        public AWThing requestedObject;

        [SerializeField, HideInInspector]
        public AWBehaviourController behaviourController;

        [SerializeField, HideInInspector]
        public ErrorResponse objError;

        [SerializeField, HideInInspector]
        public bool objHasError = false;

        [SerializeField, HideInInspector]
        private bool objMade = false;

        [SerializeField, HideInInspector]
        private bool objPositioned = false;

        [SerializeField, HideInInspector]
        private float boundsYOffset;

        [SerializeField, HideInInspector]
        public float BoundsYOffset => boundsYOffset;

        public string CreatorAttribution => ObjectData.attribution;

        public float ObjectScale => ObjectData.inputScale;

        [SerializeField, HideInInspector]
        public GameObject behaviourObject;

        [SerializeField, HideInInspector]
        public GameObject PrefabTransform
        {
            get
            {
                if (ObjectData.instantiatedPrefab == null)
                {
                    if (!objMade) return null;
                    var addedCollider = GetComponentInChildren<AddCollider>();
                    ObjectData.instantiatedPrefab = addedCollider != null ? GetComponentInChildren<AddCollider>().gameObject : transform.gameObject;
                    return ObjectData.instantiatedPrefab;
                }
                else
                {
                    return ObjectData.instantiatedPrefab;
                }
            }
        }

        [HideInInspector]
        public bool ObjMade
        {
            get => objMade;
            set => objMade = value;
        }

        public bool ObjPositioned
        {
            get
            {
                return objPositioned;
            }
            set
            {
                objPositioned = value;
            }
        }
        public Transform awThingTransform
        {
            get
            {
                return gameObject.FindComponentInChildWithTag<Transform>("AWThing");
            }
        }
        public bool CanBeMade => !this.ObjMade && !this.objHasError;

#if UNITY_EDITOR
        [HideInInspector]
        private EditorCoroutine requestObjectEditorRoutine;
        private List<EditorCoroutine> allEditorCoroutines = new List<EditorCoroutine>();
#endif
        private Coroutine requestObjectRoutine;
        [SerializeField]
        public ObjectData ObjectData = new ObjectData();

        private TempData TempData { get; set; } = new TempData();
        public string ModelName => ObjectData.returnedGuid ?? ObjectData.requestedGuid;

        public void MakeAwObj(string objName, bool hasBehaviour, bool hasCollider)
        {
            ObjectData.hasBehaviourController = hasBehaviour;
            ObjectData.hasColliderGenerator = hasCollider;
            MakeAwObj(objName);
        }

        private void ResetAwObj()
        {
            ObjectData = new ObjectData();
            TempData = new TempData();
            objMade = false;
            objPositioned = false;
            objHasError = false;
            requestedObject = new AWThing();
            if (gameObject.FindComponentInChildWithTag<Transform>("AWThing"))
            {
                AnythingSafeDestroy.SafeDestroyImmediate(gameObject.FindComponentInChildWithTag<Transform>("AWThing").gameObject);
            }
        }
        public void MakeAwObj(string objName)
        {
            ResetAwObj();

            ObjectData.requestedGuid = objName.ToLower();

            if (Application.isEditor && !Application.isPlaying)
            {
#if UNITY_EDITOR

                allEditorCoroutines = new List<EditorCoroutine>();
                if (requestObjectEditorRoutine != null)
                {
                    EditorCoroutineUtility.StopCoroutine(requestObjectEditorRoutine);
                }
                requestObjectEditorRoutine = EditorCoroutineUtility.StartCoroutine(RequestObject(ObjectData.requestedGuid), this);
                allEditorCoroutines.Add(requestObjectEditorRoutine);
#endif
            }
            else
            {
                if (requestObjectRoutine != null) StopCoroutine(requestObjectRoutine);
                requestObjectRoutine = StartCoroutine(RequestObject(ObjectData.requestedGuid));
            }
        }

        public void ActivateRBs(bool shouldActivate)
        {
            if (TempData.prefabRigidbodies == null)
                GatherRbs();

            foreach (var rb in TempData.prefabRigidbodies)
            {
                if ((shouldActivate && ObjectData.hasBehaviourController) || !shouldActivate)
                {
                    if (rb.isKinematic)
                    {
                        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                    }

                    rb.isKinematic = false;
                    //rb.isKinematic = !shouldActivate;
                }
                rb.velocity = rb.angularVelocity = Vector3.zero;
            }

            //Debug.Log($"setting colliders to: {shouldActivate}");
            if ((ObjectData.hasColliderGenerator) || (!ObjectData.hasColliderGenerator && !shouldActivate))
            {
                foreach (var cl in TempData.prefabColliders)
                {
                    cl.enabled = shouldActivate;
                }
            }
            if (shouldActivate && !ObjectData.hasBehaviourController)
                return;

            foreach (var awB in gameObject.GetComponentsInChildren<AWBehaviour>())
            {
                awB.enabled = shouldActivate;
            };
        }

        public string GetObjCatBehaviour()
        {
            if (ObjectData.hasBehaviourController)
            {
                return ObjectData.behaviourCategoryName;
            }
            else
            {
                return ObjectData.category;
            }
        }

        #region Request Object Pipeline

        /// <summary>
        /// Requests object from the server and if successful hands off to object loading processes.
        /// </summary>
        /// <param name="objName"></param>
        /// <returns></returns>
        private IEnumerator RequestObject(string objName)
        {
            yield return new WaitForEndOfFrame();
            ObjectData = new ObjectData();
            ObjectData.requestedGuid = objName;
            SetDefaultAppName();
            if (RenderPipelineNotFound())
            {
                ThrowMakerError("Render pipeline not found.");
                yield break;
            }
            if (ApiKeyMissing())
            {
                ThrowMakerError("API key missing");
                yield break;
            }

            if (SearchNameEmpty(objName))
            {
                ThrowMakerError("Search name empty");
                yield break;
            }



            var apiCall = BuildApiCall(ref objName);

            if (AnythingSettings.Instance.showDebugMessages) Debug.Log($"Making request to server: {apiCall}");

            var www = UnityWebRequest.Get(apiCall);
            www.timeout = 20;
            yield return www.SendWebRequest();

            if (CheckWebRequest.IsError(www))
            {
                ThrowMakerError(ErrorUtility.HandleErrorResponseAWDebug(www.downloadHandler.text));
                yield break;
            }
            var result = www.downloadHandler.text;

            if (AnythingSettings.Instance.showDebugMessages) Debug.Log($"JSON response for \"{objName}\" returned. \n {result}");

            if (ResultEmpty(result))
            {
                ThrowMakerError($"Empty result returned for {objName} by server.");
                yield break;
            }

            if (!JsonExtracted(www, out requestedObject))
            {
                ThrowMakerError($"Could not extract json for {objName}");
                yield break;
            }



            PopulateObjectData(objName, requestedObject);

            if (Application.isEditor && !Application.isPlaying)
            {
#if UNITY_EDITOR
                var loadMaterialRoutine = EditorCoroutineUtility.StartCoroutine(LoadMaterial(), this);
                allEditorCoroutines.Add(loadMaterialRoutine);
#endif
            }
            else
            {
                StartCoroutine(LoadMaterial());
            }
        }

        private void PopulateObjectData(string objName, AWThing requestData)
        {
            if (AnythingSettings.DebugEnabled) Debug.Log("Populating object data");
            ObjectData.returnedGuid = requestData.name;
            ObjectData.attribution = objName + " by " + requestData.author;
            if (requestedObject.behaviour == "static")
            {
                ObjectData.hasBehaviourController = false;
            }

            if (ObjectData.hasBehaviourController)
            {
                behaviourController = gameObject.AddComponent<AWBehaviourController>();
                behaviourController.awObj = this;
            }

            CategoryMap.SetCategory(requestData.type, ObjectData.returnedGuid);
            ObjectData.category = CategoryMap.GetCategory(ObjectData.returnedGuid);
            BehaviourMap.SetBehaviour(requestData.behaviour, ObjectData.returnedGuid);

            ObjectData.behaviourCategoryName = PrefabMap.PrefabName(ObjectData.category, requestData.behaviour);
            ObjectData.behaviour = requestData.behaviour;
            ObjectData.inputDimensions = new Vector3(requestData.scale.width, requestData.scale.height, requestData.scale.length);
            TempData.modelPartsJson = JsonUtility.ToJson(requestData.model.parts);

            switch (ObjectData.category)
            {
                case "multileg_flyer":
                    var awMultilegFlyerParts = JsonUtility.FromJson<AWMultilegFlyerParts>(TempData.modelPartsJson);
                    BuildObjParts(awMultilegFlyerParts);
                    break;

                case "multileg_crawler":
                    var awMultilegCrawlerParts = JsonUtility.FromJson<AWMultilegCrawlerParts>(TempData.modelPartsJson);
                    BuildObjParts(awMultilegCrawlerParts);
                    break;

                case "multileg_crawler_big":
                    var awMultilegCrawlerBigParts = JsonUtility.FromJson<AWMultilegCrawlerParts>(TempData.modelPartsJson);
                    BuildObjParts(awMultilegCrawlerBigParts);
                    break;

                case "multileg_crawler_eight":
                    var awMultilegCrawlerEightParts = JsonUtility.FromJson<AWMultilegCrawlerEightParts>(TempData.modelPartsJson);
                    BuildObjParts(awMultilegCrawlerEightParts);
                    break;

                case "winged_standing":
                    var awWingedStandingParts = JsonUtility.FromJson<AWWingedStandingParts>(TempData.modelPartsJson);
                    BuildObjParts(awWingedStandingParts);
                    break;

                case "winged_standing_small":
                    var awWingedStandingSmallParts = JsonUtility.FromJson<AWWingedStandingParts>(TempData.modelPartsJson);
                    BuildObjParts(awWingedStandingSmallParts);
                    break;

                case "winged_flyer":
                    var awWingedFlyerParts = JsonUtility.FromJson<AWWingedFlyerParts>(TempData.modelPartsJson);
                    BuildObjParts(awWingedFlyerParts);
                    break;

                case "quadruped_standard":
                case "quadruped":
                    var awQuadParts = JsonUtility.FromJson<AWQuadParts>(TempData.modelPartsJson);
                    BuildObjParts(awQuadParts);
                    break;

                case "quadruped_ungulate":
                    var awQuadUngulateParts = JsonUtility.FromJson<AWQuadParts>(TempData.modelPartsJson);
                    BuildObjParts(awQuadUngulateParts);
                    break;

                case "quadruped_fat":
                    var awQuadFatParts = JsonUtility.FromJson<AWQuadFatParts>(TempData.modelPartsJson);
                    BuildObjParts(awQuadFatParts);
                    break;

                case "quadruped_fat_small_generic":
                    var awQuadFatSmallParts = JsonUtility.FromJson<AWQuadFatParts>(TempData.modelPartsJson);
                    BuildObjParts(awQuadFatSmallParts);
                    break;

                case "quadruped_fat_shortleg_generic":
                    var awQuadFatShortLegParts = JsonUtility.FromJson<AWQuadFatParts>(TempData.modelPartsJson);
                    BuildObjParts(awQuadFatShortLegParts);
                    break;

                case "hopper":
                    var awHopperParts = JsonUtility.FromJson<AWHopperParts>(TempData.modelPartsJson);
                    BuildObjParts(awHopperParts);
                    break;

                case "vehicle_four_wheel":
                    var awVehicleFourParts = JsonUtility.FromJson<AWVehicleFourWheelParts>(TempData.modelPartsJson);
                    BuildObjParts(awVehicleFourParts);
                    break;

                case "vehicle_three_wheel":
                    var awVehicleThreeParts = JsonUtility.FromJson<AWVehicleThreeWheelParts>(TempData.modelPartsJson);
                    BuildObjParts(awVehicleThreeParts);
                    break;

                case "vehicle_two_wheel":
                    var awVehicleTwoParts = JsonUtility.FromJson<AWVehicleTwoWheelParts>(TempData.modelPartsJson);
                    BuildObjParts(awVehicleTwoParts);
                    break;

                case "vehicle_one_wheel":
                    var awVehicleOneParts = JsonUtility.FromJson<AWVehicleOneWheelParts>(TempData.modelPartsJson);
                    BuildObjParts(awVehicleOneParts);
                    break;

                case "vehicle_load":
                    var awVehicleLoadParts = JsonUtility.FromJson<AWVehicleLoadParts>(TempData.modelPartsJson);
                    BuildObjParts(awVehicleLoadParts);
                    break;

                case "vehicle_flyer":
                    var awVehicleFlyerParts = JsonUtility.FromJson<AWVehicleFlyerParts>(TempData.modelPartsJson);
                    BuildObjParts(awVehicleFlyerParts);
                    break;

                case "vehicle_propeller":
                    var awVehiclePropellerParts = JsonUtility.FromJson<AWVehiclePropellerParts>(TempData.modelPartsJson);
                    BuildObjParts(awVehiclePropellerParts);
                    break;

                case "biped":
                    var awBipedParts = JsonUtility.FromJson<AWBipedParts>(TempData.modelPartsJson);
                    BuildObjParts(awBipedParts);
                    break;

                case "uniform":
                case "vehicle_other":
                case "vehicle_uniform":
                    var awUniformParts = JsonUtility.FromJson<AWUniformParts>(TempData.modelPartsJson);
                    BuildObjParts(awUniformParts);
                    break;

                case "floater":
                    var awFloaterParts = JsonUtility.FromJson<AWFloaterParts>(TempData.modelPartsJson);
                    BuildObjParts(awFloaterParts);
                    break;

                case "scenery":
                    var aWSceneryParts = JsonUtility.FromJson<AWSceneryParts>(TempData.modelPartsJson);
                    BuildObjParts(aWSceneryParts);
                    break;

                default:
                    Debug.LogError("NO CATEGORY FOUND FOR OBJECT -> " + ObjectData.category + " -> ABORTING!");
                    break;
            }
        }

        private bool JsonExtracted(UnityWebRequest www, out AWThing jsonResult)
        {
            var trimmedResult = www.downloadHandler.text;
            trimmedResult = trimmedResult.TrimStart('[');
            trimmedResult = trimmedResult.TrimEnd(']');
            var objectJsonString = trimmedResult;

            try
            {
                jsonResult = JsonUtility.FromJson<AWThing>(objectJsonString);
            }
            catch (Exception e)
            {
                Debug.LogError($"{ObjectData.requestedGuid} JSON does not match AWThing container" + "\n" + e);
                //Debug.LogError($"Error loading response for {objName} into AWThing.");
                ThrowMakerError($"{ObjectData.requestedGuid} JSON does not match AWThing container" + "\n" + e);
                jsonResult = null;
                return false;
            }
            return true;
        }

        private bool ResultEmpty(string result)
        {
            if (result != "[]") return false;
            Debug.LogError($"Error with model request \"{ObjectData.requestedGuid}\" \n Access customer support at our discord: \" https://discord.gg/knJgyn936s \"");
            ThrowMakerError("Error with fetching results");
            return true;
        }

        private string BuildApiCall(ref string searchTerm)
        {
            searchTerm = TrimObjName(searchTerm);

            var baseUrl = AnythingApiConfig.ApiUrlStem + "/anything?key=" + AnythingSettings.ApiKey;
            if (apiOverride != "")
            {
                baseUrl = apiOverride + "/anything?key=" + AnythingSettings.ApiKey;
            }

            var encodedObjName = BuildEncodedObjName(searchTerm);
            var encodedAppName = Uri.EscapeUriString(AnythingSettings.AppName);

            if (encodedAppName != "")
            {
                baseUrl = baseUrl + "&app=" + encodedAppName;
            }

            var apiCall = baseUrl + "&name=" + encodedObjName;
            return apiCall;
        }

        private static string TrimObjName(string objName)
        {
            char[] charsToTrim = { '*', ' ', '\'', ',', '.' };
            objName = objName.Trim(charsToTrim);
            return objName;
        }

        private static string BuildEncodedObjName(string objName)
        {
            var encodedObjName = System.Uri.EscapeUriString(objName);
            if (encodedObjName.Contains("#"))
            {
                encodedObjName = encodedObjName.Replace("#", "%23");
            }
            if (encodedObjName.Contains(" "))
            {
                encodedObjName = encodedObjName.Replace(" ", "%20");
            }

            return encodedObjName;
        }

        private bool SearchNameEmpty(string objName)
        {
            if (objName == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ApiKeyMissing()
        {
            if (string.IsNullOrEmpty(AnythingSettings.Instance.apiKey))
            {
                return true;
            }

            return false;
        }

        private static void SetDefaultAppName()
        {
            if (string.IsNullOrEmpty(AnythingSettings.Instance.appName))
            {
                AnythingSettings.Instance.appName = "My App";
            }
        }

        private bool RenderPipelineNotFound()
        {
            if (UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline)
            {
                return false;
            }
            else
            {
                Debug.LogWarning("Warning: Standard RP detected, HDRP or URP must be installed to use Anything World.");
                return true;
            }

        }

        /// <summary>
        /// Load material from JSON request.
        /// </summary>
        /// <returns>Coroutine</returns>
        private IEnumerator LoadMaterial()
        {
            if (AnythingSettings.DebugEnabled) Debug.Log("Load material");
            var matUrl = requestedObject.model.other.material;
            var www = UnityWebRequest.Get(matUrl);
            yield return www.SendWebRequest();
            if (CheckWebRequest.IsError(www))
            {
                Debug.LogWarning($"Error requesting material from material URL. \n URL: \"{matUrl}\"");
                ThrowMakerError($"Error loading material", $"Could not load material for {ObjectData.requestedGuid}");
                yield break;
            }
            ObjectData.mtlFile = www.downloadHandler.text;

            if (Application.isEditor && !Application.isPlaying)
            {
#if UNITY_EDITOR
                EditorCoroutineUtility.StartCoroutine(LoadTexture(), this);
#endif
            }
            else
            {
                StartCoroutine(LoadTexture());
            }
        }

        /// <summary>
        /// Loads texture from JSON request.
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadTexture()
        {
            var textureAddresses = requestedObject.model.other.texture;
            if (textureAddresses != null && textureAddresses.Length != 0)
            {
                foreach (var textureAddress in textureAddresses)
                {
                    var webRequest = UnityWebRequestTexture.GetTexture(textureAddress);
                    webRequest.timeout = 40;
                    yield return webRequest.SendWebRequest();
                    if (CheckWebRequest.IsError(webRequest))
                    {
                        ThrowMakerError("Timeout while requesting texture");
                        yield break;
                    }

                    var textureName = System.Uri.UnescapeDataString(GetTextureName(textureAddress));
                    try
                    {
                        if (AnythingSettings.Instance.showDebugMessages) Debug.Log("Loaded texture file called " + textureName);

                        if (CheckWebRequest.IsError(webRequest))
                        {
                            PrintAwDebugWarning($"Error loading texture on {ObjectData.requestedGuid}", $"Could not load texture file {textureName} on {ObjectData.requestedGuid}", textureAddress);
                            ObjectData.objTextures.Add(textureName, null);
                            continue;
                        }
                        else
                        {
                            Texture tex = DownloadHandlerTexture.GetContent(webRequest);
                            if (AnythingSettings.Instance.showDebugMessages) Debug.Log("Loading texture: " + textureName);
                            ObjectData.objTextures.Add(textureName, tex);
                        }
                    }
                    catch (MissingReferenceException e)
                    {
                        Debug.Log("Missing reference exception thrown while loading textures: " + e);
                        ThrowMakerError("Texture Loading Error", "Missing reference exception thrown while loading textures: " + e);
                        break;
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Other exception thrown: " + e);
                        ThrowMakerError("Texture Loading Error", "Other exception thrown: " + e);
                        break;
                    }
                }
            }
            GetPrefabMap();
        }

        internal string GetTextureName(string textureAddress)
        {
            try
            {
                if (textureAddress.Contains("?"))
                {
                    var splitAddress = textureAddress.Split('?');
                    var addressWithTexName = splitAddress[0];
                    var splitAddressWithTexName = addressWithTexName.Split('/');

                    var textureName = splitAddressWithTexName[splitAddressWithTexName.Length - 1];
                    return textureName;
                }
                else
                {
                    Debug.LogError($"No name parameter detected in texture URL: {textureAddress}");
                    return null;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: trying to load texture name from texture address:{textureAddress}");
                Debug.LogException(e);
                return null;
            }
        }

        /// <summary>
        /// Loads pref from category and behaviour.
        /// </summary>
        /// <remarks>
        /// Adds non-kinematic rigidbodies.
        /// Gets joints in prefab and sets to _prefabJoints.
        /// Gets colliders in prefab and sets to _prefabColliders.
        /// Gets AWBehaviours and sets to _awBehaviours.
        /// </remarks>
        private void GetPrefabMap()
        {
            try
            {
                var prefabObj = PrefabMap.ThingPrefab(ObjectData.category, requestedObject.behaviour);
                ObjectData.instantiatedPrefab = Instantiate(prefabObj) as GameObject;
                TempData.prefabRigidbodies = new List<Rigidbody>();
                var allRBs = ObjectData.instantiatedPrefab.GetComponentsInChildren<Rigidbody>();
                foreach (var rb in allRBs)
                {
                    if (!rb.isKinematic)
                    {
                        TempData.prefabRigidbodies.Add(rb);
                    }
                }

                TempData.prefabColliders = ObjectData.instantiatedPrefab.GetComponentsInChildren<Collider>();
                ActivateRBs(false);
                if (this != null && ObjectData.instantiatedPrefab != null && objHasError != true)
                {
                    ObjectData.instantiatedPrefab.transform.parent = transform;
                }
                else
                {
                    if (AnythingSettings.DebugEnabled) Debug.LogWarning("Mapped prefab destroyed, exiting make process");
                    ThrowMakerError("Error", "Mapped prefab destroyed, exiting make process");
                    return;
                }
            }
            catch (MissingReferenceException e)
            {
                AnythingSafeDestroy.SafeDestroyDelayed(ObjectData.instantiatedPrefab);
                ThrowMakerError("Missing reference Exception while getting prefab map: " + e);
                return;
            }
            catch (Exception e)
            {
                Debug.LogWarning("Exception while getting prefab map: " + e);
                AnythingSafeDestroy.SafeDestroyDelayed(ObjectData.instantiatedPrefab);
                ThrowMakerError("Prefab Map Error", "Exception while getting prefab map: " + e);
                return;
            }

            if (Application.isEditor && !Application.isPlaying)
            {
#if UNITY_EDITOR
                EditorCoroutineUtility.StartCoroutine(LoadObjParts(), this);
#endif
            }
            else
            {
                StartCoroutine(LoadObjParts());
            }
        }

        /// <summary>
        /// Iterates through the key value pairs of part name and OBJ url and requests OBJ from API.
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadObjParts()
        {
            var materialLoader = new MTLLoader();
            var objectMaterial = new Dictionary<string, Material>();
            var matStream = new MemoryStream(Encoding.UTF8.GetBytes(ObjectData.mtlFile));
            if (matStream != null)
            {
                try
                {
                    objectMaterial = materialLoader.Load(matStream, ObjectData.objTextures);
                }
                catch (Exception e)
                {
                    Debug.LogException(new Exception("Error loading material:", e));
                }
            }

            //For each "part" in the "parts" section of the JSON
            foreach (var kvp in ObjectData.objParts)
            {
                var partObjFileAddress = kvp.Value;
                var partName = kvp.Key;
                //Request obj of part using part address.
                var www = UnityWebRequest.Get(partObjFileAddress);
                yield return www.SendWebRequest();

                if (CheckWebRequest.IsError(www))
                {
                    ThrowMakerError("Error loading OBJ part", $"Could not load OBJ for part \"{partName}\"");
                    PrintAwDebugWarning(objError.code, objError.message, $"URL: {partObjFileAddress}");
                    yield break;
                }

                MeshRenderer mRenderer;
                GameObject loadedObj = null;
                try
                {
                    var textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.downloadHandler.text));
                    var loader = new OBJLoader();
                    loadedObj = loader.Load(textStream, objectMaterial);
                    loadedObj.name = "LoadedObj";

                    if (ObjectData.instantiatedPrefab.transform.FindDeepChild(partName))
                    {
                        loadedObj.transform.parent = ObjectData.instantiatedPrefab.transform.FindDeepChild(partName);
                    }
                    loadedObj.AddComponent<LoadedObjectData>();
                    mRenderer = loadedObj.GetComponentInChildren(typeof(MeshRenderer)) as MeshRenderer;
                    if (mRenderer == null)
                    {
                        PrintAwDebugWarning($"Error loading {ObjectData.requestedGuid}", "Mesh renderer not present on loaded object.");
                        continue;
                    }
                    mRenderer.enabled = false;
                    TempData.objRendererList.Add(mRenderer.GetComponent<Renderer>());
                    TempData.objMeshList.Add(mRenderer.GetComponent<MeshFilter>());
                    TempData.objLoadedObjs.Add(loadedObj);
                }
                catch (NullReferenceException e)
                {
                    Debug.LogError("Null reference exception thrown when loading OBJ parts: " + e);
                    AnythingSafeDestroy.SafeDestroyDelayed(loadedObj);
                    ThrowMakerError("Loading Obj Parts Error", "Null reference exception thrown when loading OBJ parts: " + e);
                    yield break;
                }
                catch (MissingReferenceException e)
                {
                    Debug.LogError("Missing reference exception thrown while loading OBJ parts: " + e);
                    AnythingSafeDestroy.SafeDestroyDelayed(loadedObj);
                    ThrowMakerError("Loading Obj Parts Error", "Missing reference exception: " + e);
                    yield break;
                }

                SwitchAnimationShader(mRenderer);
                yield return null;
            }

            try
            {
                CenterAndSizeObjectToBounds();
                if (ObjectData.addDefaultBehaviour)
                {
                    if (Application.isEditor && !Application.isPlaying)
                    {
#if UNITY_EDITOR
                        EditorCoroutineUtility.StartCoroutineOwnerless(AddDefaultBehaviourCreationStage());
#endif
                    }
                    else
                    {
                        StartCoroutine(AddDefaultBehaviourCreationStage());
                    }
                }
                else
                {
                    MarkObjectAsDone();
                }

            }
            catch (Exception e)
            {
                ThrowMakerError($"Exception while finishing object: {e.Message}");
                yield break;
            }
        }

        private void SwitchAnimationShader(MeshRenderer mRenderer)
        {
            switch (requestedObject.behaviour)
            {
                case "swim":
                case "swim3":
                case "swim2":
                    var swimShader = Shader.Find("Anything World/Animation/Fish Vertical Animation");
                    //// TODO: another way, large swimmable category?+
                    ///
                    //if (ObjectData.returnedGuid.IndexOf("whale", StringComparison.Ordinal) != -1)
                    //    swimShader = Shader.Find("Anything World/Animation/Fish Vertical Big Animation");
                    SwitchShader(mRenderer, swimShader, new ShaderPropertyEditing<float>() { property = "WobbleDistance", variable = ObjectData.inputScale });
                    break;

                case "wriggle":
                    SwitchShader(mRenderer, Shader.Find("Anything World/Animation/Wriggle Animation"));
                    break;

                case "crawl":
                    SwitchShader(mRenderer, Shader.Find("Anything World/Animation/Crawler Animation"));
                    break;

                case "slither":
                    SwitchShader(mRenderer, Shader.Find("Anything World/Animation/Slither Animation"));
                    break;

                case "slithervertical":
                    SwitchShader(mRenderer, Shader.Find("Anything World/Animation/Slither Vertical Animation"));
                    break;
            }
        }

        public void SetApiOverride(string url)
        {
            apiOverride = url;
        }

        private void MarkObjectAsDone()
        {
            if (Application.isEditor && !Application.isPlaying)
            {
#if UNITY_EDITOR
                EditorCoroutineUtility.StartCoroutineOwnerless(MarkAsDoneCoroutine());
#endif
            }
            else
            {
                StartCoroutine(MarkAsDoneCoroutine());
            }
        }

        private IEnumerator MarkAsDoneCoroutine()
        {
            //Debug.Log("object done coroutine called");
            onObjectMadeSuccessfullyDelegate?.Invoke();
            onObjectMadeSuccessfullyInstanced?.Invoke(this);
            if (AnythingSettings.Instance.showDebugMessages) Debug.Log($"Object {ObjectData.returnedGuid} marked as made");
            if (AnythingSettings.DebugEnabled) Debug.Log($"Adding creator attribution: {ObjectData.attribution}");
            AnythingSetup.Instance.AddModelAttribution(ObjectData.attribution);
            ActivateRBs(true);
            if (Application.isEditor && !Application.isPlaying)
            {
#if UNITY_EDITOR
                yield return new EditorWaitForSeconds(0.5f);
#endif
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
            //Debug.Log("marked object as done");
            objMade = true;
        }

        #endregion Request Object Pipeline

        #region Object Utils

        public MeshFilter[] GetMeshesInHierarchy()
        {
            var meshes = gameObject.GetComponentsInChildren<MeshFilter>();
            return meshes;
        }

        public Material[] GetSharedMaterials()
        {
            var foundMaterials = new List<Material>();
            foreach (var meshRenderer in gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                foreach (var mat in meshRenderer.sharedMaterials)
                {
                    if (!foundMaterials.Contains(mat)) foundMaterials.Add(mat);
                }
            }
            return foundMaterials.ToArray();
        }
        private void ThrowMakerError(ErrorResponse error)
        {
            this.StopAllCoroutines();
            objHasError = true;
            onObjectFailedDelegate?.Invoke(this);
            objError = error;
        }

        private void ThrowMakerError(string message)
        {
            this.StopAllCoroutines();
            objHasError = true;
            onObjectFailedDelegate?.Invoke(this);
            objError = new ErrorResponse(message, "Error");
        }
        private void ThrowMakerError(string title, string message)
        {
            this.StopAllCoroutines();
            objHasError = true;
            objError = new ErrorResponse
            {
                code = title,
                message = message
            };
        }

        private void PrintAwDebugWarning(string title, string message, string url = "")
        {
            if (AnythingSettings.DebugEnabled)
            {
                Debug.LogWarning($"{title}: {message} \n {url}");
            }
        }

        /// <summary>
        /// Converts string to title case.
        /// </summary>
        /// <param name="stringToConvert">String to convert.</param>
        /// <returns>String with titlecase.</returns>
        private string ToTitleCase(string stringToConvert)
        {
            var firstChar = stringToConvert[0].ToString();
            return (stringToConvert.Length > 0 ? firstChar.ToUpper() + stringToConvert.Substring(1) : stringToConvert);
        }

        /// <summary>
        /// Converts string in format "1.23m" to float.
        /// </summary>
        /// <param name="dimension">String to convert.</param>
        /// <returns>float</returns>
        private float ParseDimension(string dimension)
        {
            var trimmedDim = dimension.Replace("m", "");

            var floatDim = 1f;

            if (float.TryParse(trimmedDim, out var pFloat))
            {
                floatDim = pFloat;
            }

            return floatDim;
        }


        /// <summary>
        /// Builds list of parts from AWParts parts class.
        /// </summary>
        /// <param name="parts">AWParts parts class.</param>
        private void BuildObjParts(AWParts parts)
        {
            var type = parts.GetType();
            var properties = type.GetFields();
            foreach (var property in properties)
            {
                var propName = property.Name.ToString();
                var propValue = property.GetValue(parts).ToString();
                if (propValue.Length <= 1) continue;

                if (!ObjectData.objParts.ContainsKey(propName))
                {
                    ObjectData.objParts.Add(propName, propValue);
                }
            }
        }

        /// <summary>
        /// Set all renderers to be enabled, showing the object as visible.
        /// </summary>
        private void MakeRenderersVisible()
        {
            Debug.Log("make renderers vis awobj");
            foreach (var renderer in TempData.objRendererList)
            {
                var rend = (MeshRenderer)renderer;
                rend.enabled = true;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sRenderer"></param>
        /// <param name="sShader"></param>
        private void SwitchShader(Renderer sRenderer, Shader sShader)
        {
            var allMats = sRenderer.sharedMaterials;

            foreach (var mat in allMats)
            {
                mat.shader = sShader;
            }
        }

        private void SwitchShader<T>(Renderer sRenderer, Shader sShader, ShaderPropertyEditing<T> sPropertyEdit)
        {
            var allMats = sRenderer.sharedMaterials;

            foreach (var mat in allMats)
            {
                mat.shader = sShader;
                switch (sPropertyEdit.variable)
                {
                    case float f:
                        mat.SetFloat(sPropertyEdit.property, f);
                        break;
                    case int i:
                        mat.SetInt(sPropertyEdit.property, i);
                        break;
                    case Color c:
                        mat.SetColor(sPropertyEdit.property, c);
                        break;
                    default:
                        Debug.LogWarning($"Shader Property Editing of type {typeof(T).Name} is not supported");
                        break;
                }

            }
        }

        private Vector3 GetObjectBounds()
        {
            var bounds = new Bounds(Vector3.zero, Vector3.zero);
            foreach (var objRenderer in TempData.objRendererList)
            {
                // Debug.Log("Object bounds: " + objRenderer.bounds);
                bounds.Encapsulate(objRenderer.bounds);
            }
            return bounds.size;
        }

        private Vector3 GetObjectOrigin()
        {
            var totalBounds = new Bounds(Vector3.zero, Vector3.zero);
            // GameObject boundsCenterDebug;
            foreach (var mFilter in TempData.objMeshList)
            {
                var mMesh = mFilter.sharedMesh;
                totalBounds.Encapsulate(mMesh.bounds);
                // Debug.Log("have bounds object : " + mMesh.bounds);
            }

            boundsYOffset = totalBounds.extents.y;
            return totalBounds.center;
        }

        private float CalculateRelativeScaleForDimension(Vector3 currentDimensions, Vector3 desiredDimensions)
        {
            var maxDesiredDimension = GetObjectScalarDimension(desiredDimensions);
            Debug.Log(maxDesiredDimension);
            switch (maxDesiredDimension)
            {
                case ScalingDimension.x:
                    return GetRelativeScale(currentDimensions.x, desiredDimensions.x);
                case ScalingDimension.y:
                    return GetRelativeScale(currentDimensions.y, desiredDimensions.y);
                case ScalingDimension.z:
                    return GetRelativeScale(currentDimensions.z, desiredDimensions.z);
                case ScalingDimension.none:
                default:
                    //If scalar is none, default to 1m on the x axis.
                    return GetRelativeScale(currentDimensions.x, 1);
            }

        }
        private float GetRelativeScale(float currentDimension, float desiredDimension)
        {
            const float scalar = 10;
            Debug.Log("Current:" + currentDimension + "," + "Desired:" + desiredDimension); ;
            return (desiredDimension * scalar) / currentDimension;
        }
        private ScalingDimension GetObjectScalarDimension(Vector3 dimensions)
        {

            var maxDimension = Math.Max(dimensions.z, Math.Max(dimensions.x, dimensions.y));

            if (maxDimension == 0)
            {
                return ScalingDimension.none;
            }

            if (maxDimension == dimensions.x)
            {
                return ScalingDimension.x;
            }
            else if (maxDimension == dimensions.y)
            {
                return ScalingDimension.y;
            }
            else if (maxDimension == dimensions.z)
            {
                return ScalingDimension.z;
            }

            return ScalingDimension.none;
        }
        private enum ScalingDimension
        {
            x,
            y,
            z,
            none
        }
        private void CenterAndSizeObjectToBounds()
        {
            var objectBounds = GetObjectBounds();
            var objectOrigin = GetObjectOrigin();

            if (objectBounds == Vector3.zero)
            {
                throw new Exception($"Object {ObjectData.requestedGuid} has zero bounds, cancelling creation.");
            }
            ObjectData.relativeScale = CalculateRelativeScaleForDimension(objectBounds, ObjectData.inputDimensions);

            if (float.IsNaN(ObjectData.relativeScale) || float.IsPositiveInfinity(ObjectData.relativeScale) || float.IsNegativeInfinity(ObjectData.relativeScale))
            {
                string error = $"Error while calculating bounds scale for {ObjectData.requestedGuid}, calculated bound was {ObjectData.relativeScale.ToString()}";
                Debug.LogError(error);
                ThrowMakerError("Error Centering To Bounds", error);
                return;
            }

            boundsYOffset *= ObjectData.relativeScale;

            foreach (var loadedObj in TempData.objLoadedObjs)
            {
                if (requestedObject.preserveOriginalScale == false)
                {
                    loadedObj.transform.localScale = new Vector3(ObjectData.relativeScale, ObjectData.relativeScale, ObjectData.relativeScale);
                }
                if (requestedObject.preserveOriginalPosition == false)
                {
                    loadedObj.transform.position -= objectOrigin * ObjectData.relativeScale;
                }
                loadedObj.GetComponent<LoadedObjectData>().zeroedPosition = loadedObj.transform.localPosition;
                loadedObj.GetComponent<LoadedObjectData>().zeroedRotation = loadedObj.transform.localRotation;
            }
        }

        private void GatherRbs()
        {
            TempData.prefabRigidbodies = new List<Rigidbody>();
            var allRBs = gameObject.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in allRBs)
            {
                if (!rb.isKinematic)
                {
                    TempData.prefabRigidbodies.Add(rb);
                }
            }

            TempData.prefabColliders = gameObject.GetComponentsInChildren<Collider>();

            var objectTransformArray = gameObject.GetComponentsInChildren<Transform>();
            foreach (var objectTransform in objectTransformArray)
            {
                if (objectTransform.IsSameAs(transform) && !objectTransform.CompareTag("AWThing"))
                {
                    objectTransform.localPosition = Vector3.zero;
                }
            }
        }

        #endregion Object Utils

        #region Behaviour Handling

        /// <summary>
        /// Adds behaviour once has finished being made.
        /// </summary>
        /// <param name="behaviourClassType">String name of AWBehaviour script to be added.</param>
        public void AddBehaviourAsync(System.Type behaviourClassType)
        {
            if (behaviourObject == null) { BuildBehaviourContainer(); }
            try
            {
                if (!behaviourObject.TryGetComponent(behaviourClassType, out var component))
                {
                    var behaviour = (AWBehaviour)behaviourObject.AddComponent(behaviourClassType);
                    behaviour.enabled = false;
                    //behaviour.ControllingAWObj = this;
                    ObjectData.addDefaultBehaviour = false;

                    if (Application.isEditor && !Application.isPlaying)
                    {
#if UNITY_EDITOR
                        var activateBehaviourRoutine = EditorCoroutineUtility.StartCoroutine(ActivateBehaviourWhenMade(behaviour), this);
                        allEditorCoroutines.Add(activateBehaviourRoutine);
#endif
                    }
                    else
                    {
                        StartCoroutine(ActivateBehaviourWhenMade(behaviour));
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception thrown while adding {behaviourClassType.ToString()}: {e.Message}");
            }

            onBehaviourRefreshMethod.Invoke();
        }

        public T AddBehaviour<T>(bool clearExistingBehaviours = true, bool overrideExistingQueuedBehaviour = true) where T : AWBehaviour
        {

            if (IsBehaviourQueued()) return null;

            var existingBehaviours = new List<AWBehaviour>();
            T behaviour = null;

            if (behaviourObject == null)
            {
                BuildBehaviourContainer();
                behaviour = behaviourObject.AddComponent<T>();
                //behaviour.ControllingAWObj = this;
                behaviour.enabled = false;
                ObjectData.addDefaultBehaviour = false;

                if (Application.isEditor && !Application.isPlaying)
                {
#if UNITY_EDITOR
                    if (overrideExistingQueuedBehaviour)
                    {
                        if (behaviourWaitingToActiveCoroutineEditor != null)
                        {
                            EditorCoroutineUtility.StopCoroutine(behaviourWaitingToActiveCoroutineEditor);
                        }

                        behaviourWaitingToActiveCoroutineEditor = EditorCoroutineUtility.StartCoroutine(ActivateBehaviourWhenMade<T>(behaviour), this);
                    }
                    else
                    {
                        if (behaviourWaitingToActiveCoroutineEditor == null)
                        {
                            behaviourWaitingToActiveCoroutineEditor = EditorCoroutineUtility.StartCoroutine(ActivateBehaviourWhenMade<T>(behaviour), this);
                        }
                    }

#endif
                }
                else
                {
                    if (overrideExistingQueuedBehaviour)
                    {
                        if (behaviourWaitingToActivateCoroutine != null)
                        {
                            StopCoroutine(behaviourWaitingToActivateCoroutine);
                        }
                        behaviourWaitingToActivateCoroutine = StartCoroutine(ActivateBehaviourWhenMade(behaviour));
                    }
                    else
                    {
                        if (behaviourWaitingToActivateCoroutine == null)
                        {
                            behaviourWaitingToActivateCoroutine = StartCoroutine(ActivateBehaviourWhenMade(behaviour));
                        }
                    }

                }
            }
            else
            {
                if (clearExistingBehaviours)
                {
                    existingBehaviours = GetExistingBehaviours();
                }
                if (gameObject.TryGetComponent<T>(out var temp))
                {
                    behaviour = temp;
                    existingBehaviours.Remove(temp);
                }
                else
                {
                    behaviour = behaviourObject.AddComponent<T>();
                    //behaviour.ControllingAWObj = this;
                    behaviour.enabled = false;
                    ObjectData.addDefaultBehaviour = false;

                    if (Application.isEditor && !Application.isPlaying)
                    {
#if UNITY_EDITOR
                        if (behaviourWaitingToActiveCoroutineEditor != null)
                        {
                            EditorCoroutineUtility.StopCoroutine(behaviourWaitingToActiveCoroutineEditor);
                        }
                        behaviourWaitingToActiveCoroutineEditor = EditorCoroutineUtility.StartCoroutine(ActivateBehaviourWhenMade<T>(behaviour), this);
#endif
                    }
                    else
                    {
                        if (behaviourWaitingToActivateCoroutine != null)
                        {
                            StopCoroutine(behaviourWaitingToActivateCoroutine);
                        }
                        behaviourWaitingToActivateCoroutine = StartCoroutine(ActivateBehaviourWhenMade(behaviour));
                    }
                }
                if (clearExistingBehaviours)
                {
                    DisableExistingBehaviours(existingBehaviours);
                }
            }
            try
            {
                onBehaviourRefreshMethod.Invoke();
            }
            catch
            {
                if (AnythingSettings.Instance.showDebugMessages) Debug.LogWarning("Failed to invoke behaviour refresh delegate when adding behaviour.");
            }
            return behaviour;
        }

        public bool IsBehaviourQueued()
        {
            if (Application.isEditor && !Application.isPlaying)
            {
#if UNITY_EDITOR
                if (behaviourWaitingToActiveCoroutineEditor != null) return true;
#endif
            }
            else
            {
                if (behaviourWaitingToActivateCoroutine != null) return true;

            }
            return false;
        }
        /// <summary>
        /// Adds default behaviour once OBJ spawned.
        /// </summary>
        public IEnumerator AddDefaultBehaviourCreationStage()
        {
            AddDefaultBehaviourToObject();
            MarkObjectAsDone();
            yield return null;
        }

        public void AddDefaultBehaviourToObject()
        {
            if (IsBehaviourQueued()) return;
            if (ObjectData.addDefaultBehaviour)
            {
                if (behaviourObject == null)
                {
                    BuildBehaviourContainer();
                }

                var isGroupMember = GroupMap.IsObjectFlocking(requestedObject.behaviour, ObjectData.category);

                if (behaviourObject.GetComponent<AWBehaviour>())
                {
                    Destroy(behaviourObject.GetComponent<AWBehaviour>());
                }

                // don't add default behaviour to group members for now as these work differently - GM
                if (ObjectData.hasBehaviourController && !isGroupMember)
                {

                    if (ObjectData.behaviourCategoryName == "vehicle_uniform__drive" || ObjectData.behaviourCategoryName == "vehicle_uniform")
                    {
                        var behaviour = AddBehaviour<RandomMovement>(true, false);
                        behaviour.AddAWAnimator();
                    }
                    else
                    {
                        if (requestedObject.behaviour == "drive") AddBehaviour<VehicleDriveMovement>(true, false);
                        else if (ObjectData.category == "hopper" || requestedObject.behaviour == "hop") AddBehaviour<HoppingMovement>(true, false);
                        else if (requestedObject.behaviour == "fly") AddBehaviour<VehicleFlyerRandomMovement>(true, false);
                        else AddBehaviour<RandomMovement>(true, false);
                    }
                }
                else
                {
                    var behaviour = AddBehaviour<StaticWithAnimation>(true, false);

                }

            }
        }

        /// <summary>
        /// Adds behaviour to AWObj without removing other behaviours.
        /// </summary>
        /// <param name="behaviourName">string name of behaviour script</param>
        /// <returns></returns>
        private IEnumerator AddBehaviourIfMade(string behaviourName)
        {
            ObjectData.addDefaultBehaviour = false;
            while (!objMade)
            {
                yield return new WaitForSeconds(1);
            }
            if (behaviourController != null)
            {
                behaviourController.AddBehaviourScript(behaviourName);
            }
            else
            {
                Debug.LogError("No controller added, cannot add behaviour " + behaviourName + " to " + gameObject.name);
            }
        }

        private IEnumerator AddBehaviourIfMade<T>(T behaviourReference) where T : AWBehaviour
        {
            ObjectData.addDefaultBehaviour = false;
            while (!objMade)
            {
                if (Application.isEditor && !Application.isPlaying)
                {
#if UNITY_EDITOR
                    yield return new EditorWaitForSeconds(1);
#endif
                }
                else
                {
                    yield return new WaitForSeconds(1);
                }
            }

            if (behaviourController != null)
            {
                behaviourReference = (T)behaviourController.AddBehaviourScript<T>();
            }
            else
            {
                Debug.LogError("No controller added, cannot add behaviour" + typeof(T).ToString() + " to " + gameObject.name); ;
            }
            yield return behaviourReference;
        }

#if UNITY_EDITOR
        public EditorCoroutine behaviourWaitingToActiveCoroutineEditor;
#endif 
        public Coroutine behaviourWaitingToActivateCoroutine;
        private IEnumerator ActivateBehaviourWhenMade<T>(T behaviourRef) where T : AWBehaviour
        {
            while (!objMade)
            {
                if (Application.isEditor && !Application.isPlaying)
                {
#if UNITY_EDITOR
                    yield return new EditorWaitForSeconds(1);
#endif
                }
                else
                {
                    yield return new WaitForSeconds(1);
                }
            }

            behaviourRef.enabled = true;
            behaviourRef.behaviourInitialized = true;
            behaviourRef.InitializeAnimator();
            //behaviourRef.ControllingAWObj = this;
            yield return null;
        }

        private IEnumerator ActivateBehaviourWhenMade(AWBehaviour behaviourReference)
        {
            Debug.Log("ActivateBehaviourWhenMade");
            while (!objMade && this != null)
            {
                if (Application.isEditor && !Application.isPlaying)
                {
#if UNITY_EDITOR
                    yield return new EditorWaitForSeconds(1);
#endif
                }
                else
                {
                    yield return new WaitForSeconds(1);
                }
            }
            if (behaviourReference != null)
            {
                behaviourReference.behaviourInitialized = true;
                behaviourReference.enabled = true;
                behaviourReference.InitializeAnimator();
            }
        }

        /// <summary>
        /// Removes other behaviours and adds input behaviour only.
        /// </summary>
        /// <param name="behaviourName">string name of behaviour script.</param>
        /// <returns></returns>
        private IEnumerator SetBehaviourIfMade(string behaviourName)
        {
            while (!objMade)
            {
                yield return new WaitForSeconds(1);
            }
            behaviourController.RemoveActiveBehaviours();
            behaviourController.AddBehaviourScript(behaviourName);
        }

        public void RemoveExistingBehaviours()
        {
            if (behaviourObject != null)
            {
                foreach (var behaviour in behaviourObject.GetComponents<AWBehaviour>())
                {
                    behaviour.RemoveAWAnimator();
                    AnythingSafeDestroy.SafeDestroyImmediate(behaviour, false);
                }
            }
        }

        public List<AWBehaviour> GetExistingBehaviours()
        {
            var behaviourList = new List<AWBehaviour>();
            if (behaviourObject != null)
            {
                foreach (var behaviour in behaviourObject.GetComponents<AWBehaviour>())
                {
                    behaviourList.Add(behaviour);
                }
            }
            return behaviourList;
        }

        private void DisableExistingBehaviours(List<AWBehaviour> behavioursToDisable)
        {
            if (behaviourObject != null)
            {
                foreach (var behaviour in behavioursToDisable)
                {
                    behaviour.enabled = false;
                }
            }
        }

        private void BuildBehaviourContainer()
        {
            behaviourObject = new GameObject();
            behaviourObject.name = "Behaviours";
            behaviourObject.transform.parent = transform;
        }

        public void OnDestroy()
        {
            flaggedDestroyed = true;
            StopAllCoroutines();
            AnythingSafeDestroy.ClearList(TempData.objLoadedObjs);
            if (objHasError == false && !objMade) ThrowMakerError("Destroyed");
        }

        #endregion Behaviour Handling

        public void StopRequestPipeline()
        {
            if (Application.isEditor && !Application.isPlaying)
            {
#if UNITY_EDITOR
                if (allEditorCoroutines != null)
                {
                    foreach (var eRoutine in allEditorCoroutines)
                    {
                        EditorCoroutineUtility.StopCoroutine(eRoutine);
                    }
                }
#endif
            }

            this.StopAllCoroutines();
        }
    }
}