using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
namespace AnythingWorld.Editors
{
    public class AnythingSettingsPanelEditor : AnythingEditor
    {
        #region Fields
        private string awKey;
        private string appName;
        private string agentID;
        private string email;
        private bool debug;
        private Shader unstitchedShader;
        private Shader stitchedShader;

        private Shader transparentUnstitchedShader;
        private Shader transparentStitchedShader;

        private bool showAdvanced = false;

        #endregion
        public static void Init()
        {
            Resources.LoadAll<AnythingWorld.AnythingSettings>("Settings");


            if (AnythingSettings.Instance != null)
            {
                try
                {
                    ShowWindow();
                    EditorUtility.SetDirty(AnythingSettings.Instance);


                    if (HasOpenInstances<AnythingSettingsPanelEditor>())
                    {
                        AnythingSettingsPanelEditor window = GetWindow<AnythingSettingsPanelEditor>() as AnythingSettingsPanelEditor;
                        window.awKey = AnythingSettings.Instance.apiKey;
                        window.appName = AnythingSettings.Instance.appName;
                        window.agentID = AnythingSettings.Instance.dialogFlowAgentId;
                        window.email = AnythingSettings.Instance.email;
                        window.debug = AnythingSettings.Instance.showDebugMessages;
                        window.unstitchedShader = AnythingSettings.Instance.DefaultUnstitchedShader;
                        window.stitchedShader = AnythingSettings.Instance.DefaultStitchedShader;
                        window.transparentUnstitchedShader = AnythingSettings.Instance.DefaultUnstitchedTransparentShader;
                        window.transparentStitchedShader = AnythingSettings.Instance.DefaultStitchedTransparentShader;
                        EditorUtility.SetDirty(window);
                    }

                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error opening settings window : {e.Message}");
                    CloseWindow();
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Could not find instance of AnythingSettings scriptable object.", "Ok");
                CloseWindow();
            }
        }

        [MenuItem("Anything World/Settings", false, 21)]
        public static void MenuLoad()
        {
            Init();
        }

        public static void ShowWindow()
        {

            AnythingSettingsPanelEditor window = CreateInstance(typeof(AnythingSettingsPanelEditor)) as AnythingSettingsPanelEditor;
            GUIContent windowContent = new GUIContent("AnythingWorld Settings");
            window.titleContent = windowContent;
            window.position = new Rect(10f, 10f, 500f, 500f);
            window.minSize = new Vector2(500, 300);
            window.ShowUtility();


            GetWindow<AnythingSettingsPanelEditor>("position");

        }
        public static void CloseWindow()
        {
            if (HasOpenInstances<AnythingSettingsPanelEditor>())
            {
                AnythingSettingsPanelEditor window = GetWindow<AnythingSettingsPanelEditor>() as AnythingSettingsPanelEditor;
                window.Close();
            }
        }
        private void OnGUI()
        {
            InitializeResources();
            SettingsWindow();
        }

        private void SettingsWindow()
        {

            var logoRect = new Rect(10, 10, 64, 64);
            var titleRect = new Rect(logoRect.xMax + 10, 0, position.width - logoRect.xMax + 10, 90);
            var bannerRect = new Rect(0, 0, position.width, logoRect.yMax + 10);
            GUI.DrawTexture(bannerRect, thumbnailBackgroundActive);

            GUI.DrawTexture(logoRect, EditorGUIUtility.isProSkin ? WhiteAnythingGlobeLogo : BlackAnythingGlobeLogo, ScaleMode.StretchToFill);
            int textSize = 14;
            GUIContent labelContent = new GUIContent("ANYTHING WORLD SETTINGS");
            GUI.Label(titleRect, labelContent, BuildStyle(EditorStyles.label, PoppinsStyle.Bold, 20, TextAnchor.MiddleLeft, EditorGUIUtility.isProSkin ? Color.white : Color.black));
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical();

            GUILayoutUtility.GetRect(100, bannerRect.yMax+10);
            GUIStyle fieldStyle = new GUIStyle(EditorStyles.textField);
            fieldStyle.font = POPPINS_REGULAR;
            fieldStyle.fixedWidth = 300;

            GUILayout.BeginHorizontal();
            DrawCustomText("Version number ", textSize);
            GUILayout.FlexibleSpace();
            DrawCustomText(AnythingSettings.PackageVersion, textSize, 300);
            //awKey = GUILayout.TextField(awKey, fieldStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            DrawCustomText("API Key ", textSize);
            awKey = GUILayout.TextField(awKey, fieldStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            DrawCustomText("App Name ", textSize);
            appName = GUILayout.TextField(appName, fieldStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            DrawCustomText("Dialogflow Agent ID ", textSize);
            agentID = GUILayout.TextField(agentID, fieldStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            DrawCustomText("Email ", textSize);
            email = GUILayout.TextField(email, fieldStyle);
            GUILayout.EndHorizontal();


            showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Advanced", BuildStyle(EditorStyles.foldout, PoppinsStyle.Regular, 12, TextAnchor.MiddleLeft));

            if (showAdvanced)
            {         
                GUILayout.BeginHorizontal();
                DrawCustomText("Show experimental debug logs ", textSize, 300);
                debug = GUILayout.Toggle(debug, "");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                DrawCustomText("Default unstitched shader ", textSize, 300);
                unstitchedShader = EditorGUILayout.ObjectField(unstitchedShader, typeof(Shader), true, GUILayout.MinWidth(300)) as Shader;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                DrawCustomText("Default stitched shader ", textSize, 300);
                stitchedShader = EditorGUILayout.ObjectField(stitchedShader, typeof(Shader), true, GUILayout.MinWidth(300)) as Shader;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                DrawCustomText("Default transparent unstitched shader ", textSize, 300);
                transparentUnstitchedShader = EditorGUILayout.ObjectField(transparentUnstitchedShader, typeof(Shader), true, GUILayout.MinWidth(300)) as Shader;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                DrawCustomText("Default transparent stitched shader ", textSize, 300);
                transparentStitchedShader = EditorGUILayout.ObjectField(transparentStitchedShader, typeof(Shader), true, GUILayout.MinWidth(300)) as Shader;
                GUILayout.EndHorizontal();

                GUILayout.FlexibleSpace();




            }
            else
            {
                GUILayout.FlexibleSpace();
            }


            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply & Close", searchButtonStyle))
            {
                if (AnythingSettings.Instance == null)
                {
                    Debug.LogError("No AnythingSettings instance located.");
                    CloseWindow();
                }
                else
                {
                    var settingsSerializedObject = new SerializedObject(AnythingSettings.Instance);
                    settingsSerializedObject.FindProperty("apiKey").stringValue = awKey;
                    settingsSerializedObject.FindProperty("appName").stringValue = appName;
                    settingsSerializedObject.FindProperty("dialogFlowAgentId").stringValue = agentID;
                    settingsSerializedObject.FindProperty("email").stringValue = email;
                    settingsSerializedObject.FindProperty("showDebugMessages").boolValue = debug;
                    settingsSerializedObject.FindProperty("defaultStitchedShader").objectReferenceValue = stitchedShader;
                    settingsSerializedObject.FindProperty("defaultUnstitchedShader").objectReferenceValue = unstitchedShader;
                    settingsSerializedObject.FindProperty("defaultStitchedTransparentShader").objectReferenceValue = transparentStitchedShader;
                    settingsSerializedObject.FindProperty("defaultUnstitchedTransparentShader").objectReferenceValue = transparentUnstitchedShader;
                    settingsSerializedObject.ApplyModifiedProperties();
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    
                    Undo.RecordObject(AnythingSettings.Instance, "Changed AnythingSetttings");
                    EditorUtility.SetDirty(AnythingSettings.Instance);

   
                    CloseWindow();
                }

            }
            if (GUILayout.Button("Reset", searchButtonStyle))
            {
                Init();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

        }
    }
}

#endif