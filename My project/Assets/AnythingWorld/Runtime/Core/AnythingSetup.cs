using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif
using AnythingWorld.Utilities;

namespace AnythingWorld
{
    /// <summary>
    /// Initializes the scene ready for creating objects with AnythingCreator.
    /// </summary>
    [ExecuteAlways]
    [Serializable]
    public class AnythingSetup : MonoBehaviour
    {
        #region Fields
        public DisplayWaitingSwirl waitingGUI;
        [SerializeField]
        public GameObject canvas;
        // private const int MAX_ATTRIBUTION_LINES = 10;
        [SerializeField]
        private Text attributionUIText;

        [SerializeField]
        private static List<string> modelAttributionList;
        [SerializeField]
        private static string modelAttributionStringBlock;

        public static string ModelAttributionStringBlock
        {
            get
            {
                if (modelAttributionStringBlock == null)
                {
                    return "";
                }
                else
                {
                    return modelAttributionStringBlock;
                }

            }
        }
        public static List<string> ModelAttributionList
        {
            get
            {
                if (modelAttributionList == null)
                {
                    modelAttributionList = new List<string>();
                    if(AnythingSettings.DebugEnabled) Debug.Log("Initialized model attribution list in property");
                }
                return modelAttributionList;
            }
            set
            {
                modelAttributionList = value;
            }
        }
        public static string ModelAttributionString
        {
            get
            {
                var val = "";
                if (modelAttributionList != null)
                {
                    foreach (var line in modelAttributionList)
                    {
                        val += line;
                        val += "\n";
                    }
                }
                return val;
            }
        }

        public AnythingSettings anythingSettings;
        //public BehaviourSettings behaviourSettings;

        public bool IsShowingLoading
        {
            get
            {
                return isShowingLoading;
            }
        }
        private static AnythingSetup instance;
        private bool isShowingLoading;

        public static AnythingSetup Instance
        {
            get
            {
                if (instance == null)
                {
                    if (AnythingBase.CheckAwSetup())
                    {
                        instance = GameObject.FindObjectOfType<AnythingSetup>();
                        if (instance == null)
                        {
                            AnythingBase.CheckAwSetup();
                            Debug.LogError("No anything setup in scene");
                            return null;
                        }
                        else
                        {
                            instance.ApplySettings();
                            return instance;
                        }
                    }
                    else
                    {
                        Debug.LogError("No anything setup in scene");
                        return null;
                    }


                }
                else
                {
                    instance.ApplySettings();
                    return instance;
                }

            }
        }
        #endregion

        #region Unity Callbacks
        public void Awake()
        {
            Init();
            instance = this;
            ApplySettings();
            GenerateAttributionsFromScene();
        }

        
        public void Start()
        {
            GenerateAttributionsFromScene();
        }
        private void Init()
        {
            // There must be an in scene reference to a scriptable object or the Resources.FindAllObjectsOfType<T>()
            // that the singleton scriptable object pattern relies on will not work.
            try
            {
                anythingSettings = AnythingBase.Settings;
            }
            catch
            {
                Debug.LogWarning("Error loading settings");
                return;
            }

            var attributionObject = GameObject.FindGameObjectWithTag("AWAttribution");
            if (attributionObject != null)
            {
                attributionUIText = attributionObject.GetComponent<Text>();
            }

            if (canvas == null)
            {
                if (GetComponentInChildren<Canvas>())
                {
                    canvas = GetComponentInChildren<Canvas>().gameObject;
                }
            }
            if (waitingGUI == null)
            {
                if (canvas)
                {
                    waitingGUI = canvas.transform.GetComponentInChildren<DisplayWaitingSwirl>(true);
                }

            }

        }


        #endregion

        #region Private Methods
        private void ApplySettings()
        {
            if (AnythingSettings.Instance != null)
            {
                Physics.defaultSolverIterations = AnythingSettings.Instance.physicsSolverIterations;
                Time.maximumDeltaTime = AnythingSettings.Instance.maximumAllowedTimestep;
                Application.targetFrameRate = AnythingSettings.Instance.frameRate;
                var defaultLayer = LayerMask.NameToLayer("Default");
                var raycastLayer = LayerMask.NameToLayer("Ignore Raycast");
                var animationLayer = LayerMask.NameToLayer("Animation");
                Physics.IgnoreLayerCollision(defaultLayer, raycastLayer, true);
                Physics.IgnoreLayerCollision(defaultLayer, animationLayer, true);
            }
            else
            {
                Debug.Log("Error accesing anything settings");
            }


        }

        #endregion

        public void ResetAttributionList()
        {
            ModelAttributionList = new List<string>();
            if (attributionUIText != null)
            {
                attributionUIText.text = "";
            }
        }
        public void AddModelAttribution(string creatorText)
        {
            //Debug.Log($"Add model attribution: {creatorText}");
            if (ModelAttributionList == null)
            {
                if(AnythingSettings.DebugEnabled) Debug.Log("Model attribution list is null, resetting");
                ResetAttributionList();
            }

            try
            {
                //If attribution list doesn't already contain the attribution add it
                if (!ModelAttributionList.Contains(creatorText))
                {
                    ModelAttributionList.Add(creatorText);
                    UpdateModelAttributionTags(ModelAttributionList);
                }


            }
            catch (Exception e)
            {
                Debug.LogError($"Error updating attribution list with {creatorText}:");
                Debug.LogException(e);
            }
        }

        public static void UpdateModelAttributionTags(List<string> attributionStrings)
        {
            var textBlock = GenerateAttributionTextBlock(attributionStrings);
            //Debug.Log(textBlock);
            var tags = FindObjectsOfType<AttributionTag>();


            foreach (var tag in tags)
            {
                if (tag != null)
                {
                    tag.ClearTextTag();
                    tag.UpdateTextTag(textBlock);
                }
            }
        }

        public static string GenerateAttributionTextBlock(List<string> attributionStrings)
        {
            modelAttributionStringBlock = "";
            foreach (var attr in attributionStrings)
            {
                modelAttributionStringBlock += attr;
                modelAttributionStringBlock += "\n";
            }
            return modelAttributionStringBlock;
        }
        public void ShowLoading(bool showLoading)
        {
            /*
            if (waitingGUI != null)
            {
                if (waitingGUI.gameObject.activeSelf == showLoading)
                    return;
                if (showLoading)
                    waitingGUI.gameObject.SetActive(showLoading);
                waitingGUI.StartSwirly(showLoading);
                if (!showLoading)
                    waitingGUI.gameObject.SetActive(showLoading);
                isShowingLoading = showLoading;
            }
            */
        }

        public static string[] GenerateAttributionsFromScene()
        {
            var sceneObjects = FindObjectsOfType<AWObj>();
            var sceneObjectAttributions = new List<string>();
            foreach (var awobj in sceneObjects)
            {
                if (!sceneObjectAttributions.Contains(awobj.CreatorAttribution))
                {
                    sceneObjectAttributions.Add(awobj.CreatorAttribution);
                }
            }

            UpdateModelAttributionTags(sceneObjectAttributions);
            return sceneObjectAttributions.ToArray();
        }

#if UNITY_EDITOR
        static void InitHierarchyMonitoring()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        static void OnHierarchyChanged()
        {
            var all = Resources.FindObjectsOfTypeAll(typeof(GameObject));
            var numberVisible =
                all.Where(obj => (obj.hideFlags & HideFlags.HideInHierarchy) != HideFlags.HideInHierarchy).Count();
            Debug.LogFormat("There are currently {0} GameObjects visible in the hierarchy.", numberVisible);
        }
#endif

    }

#if UNITY_EDITOR

    [InitializeOnLoadAttribute]
    public static class HierarchyMonitor
    {
        static HierarchyMonitor()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        static void OnHierarchyChanged()
        {
            AnythingSetup.GenerateAttributionsFromScene();
        }
    }
#endif
}