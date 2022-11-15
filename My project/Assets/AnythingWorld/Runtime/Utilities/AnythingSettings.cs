using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace AnythingWorld
{
    [CreateAssetMenu(fileName = "AnythingSettings", menuName = "AnythingWorld/AnythingSettings", order = 1)]
    public class AnythingSettings : ScriptableObject
    {
        public static AnythingSettings Instance
        {
            get
            {

                var instance = Resources.Load<AnythingSettings>("Settings/AnythingSettings");
                if (instance == null)
                {
                    Debug.Log("Instance is null, making new Settings file");
#if UNITY_EDITOR
                    var asset = ScriptableObject.CreateInstance<AnythingSettings>();
                    AssetDatabase.CreateAsset(asset, "Assets/AnythingWorld/Resources/Settings/AnythingSettings.asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    return asset;
#endif
                }
                return instance;
            }
        }


        [Tooltip("Your Anything World API key goes here.")]
        public string apiKey = "";
        [Tooltip("Name of application. Please ensure this value is different for each app.")]
        public string appName = "My App";
        [Tooltip("Your address for electronic mail communications.")]
        public string email = "";
        [Tooltip("Dialogflow project id. Please ensure this value corresponds to the Google project associated with the agent.")]
        public string dialogFlowAgentId = "anything-world-api";
        [Tooltip("Target frame rate - default is 60")]
        public int frameRate = 60;
        [Tooltip("Lower value typically results in smoother and less jittery animations. Higher gives more accurate (but expensive) results.")]
        public int physicsSolverIterations = 2;
        [Tooltip("Joint based animations rely on this - default is 0.1")]
        public float maximumAllowedTimestep = 0.1f;
        public GameObject anythingSetup = null;
        [Tooltip("Show AW Debug Messages")]
        public bool showDebugMessages = false;
        private const string versionNumber = "3.1.20.1";

        //Paths to default shaders
        private const string defaultUnstitchedPath = "Anything World/AnythingLitSG";
        private const string defaultStitchedPath = "Anything World/AnythingStitchedSG";
        private const string defaultUnstitchedTransparentPath = "Anything World/AnythingLitTransparentSG";
        private const string defaultStitchedTransparentPath = "Anything World/AnythingLitTransparentStitchedSG";


        [SerializeField]
        public Shader defaultStitchedShader;
        [SerializeField]
        public Shader defaultUnstitchedShader;
        [SerializeField]
        public Shader defaultUnstitchedTransparentShader;
        [SerializeField]
        public Shader defaultStitchedTransparentShader;
        public static string ApiKey
        {
            get
            {
                return AnythingSettings.Instance.apiKey;
            }
            internal set
            {
                Instance.apiKey = value;
            }
        }
        public static string AppName
        {

            get
            {
                return Instance.appName;
            }
        }

        public static string Email
        {
            get
            {
                return Instance.email;
            }
            internal set
            {
                Instance.email = value;
            }
        }

        public static bool DebugEnabled
        {
            get
            {
                return Instance.showDebugMessages;
            }
        }

        public static string PackageVersion
        {
            get
            {
                return versionNumber;
            }
        }

        public Shader DefaultStitchedShader
        {
            get
            {
                if (defaultStitchedShader == null) defaultStitchedShader = Shader.Find(defaultStitchedPath);
                return defaultStitchedShader;
            }
            set => defaultStitchedShader = value;
        }
        public Shader DefaultUnstitchedShader
        {
            get
            {
                if (defaultUnstitchedShader == null) defaultUnstitchedShader = Shader.Find(defaultUnstitchedPath);
                return defaultUnstitchedShader;
            }
            set => defaultUnstitchedShader = value;
        }
        public Shader DefaultStitchedTransparentShader
        {
            get
            {
                if (defaultStitchedTransparentShader == null) defaultStitchedTransparentShader = Shader.Find(defaultStitchedTransparentPath);
                return defaultStitchedTransparentShader;

            }
            set => defaultStitchedTransparentShader = value;
        }

        public Shader DefaultUnstitchedTransparentShader
        {
            get
            {
                if (defaultUnstitchedTransparentShader == null) defaultUnstitchedTransparentShader = Shader.Find(defaultUnstitchedTransparentPath);
                return defaultUnstitchedTransparentShader;

            }
            set => defaultUnstitchedTransparentShader = value;
        }

        public void ClearSettings()
        {
            apiKey = "";
            appName = "My App";
            email = "";
            dialogFlowAgentId = "anything-world-api";
            frameRate = 60;
            physicsSolverIterations = 2;
            maximumAllowedTimestep = 0.1f;
            anythingSetup = null;
            showDebugMessages = false;
        }


        public bool HasEmail()
        {
            if (email != "")
            {
                return true;
            }
            else
            {
                return false;
            };
        }
        public bool HasAPIKey()
        {
            if (apiKey != "")
            {
                return true;
            }
            else
            {
                return false;
            };
        }
    }
}
