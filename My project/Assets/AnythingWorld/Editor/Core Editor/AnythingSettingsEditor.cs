using AnythingWorld;
using AnythingWorld.Utilities;
using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
[CustomEditor(typeof(AnythingSettings))]
public class AnythingSettingsEditor : Editor
{
    public static string LATEST_VERSION_URL = AnythingApiConfig.ApiUrlStem + "/version";

    [SerializeField]
    protected Texture2D logo;
    public Color GREEN_COLOR = new Color(0.53f, 1f, 0f);

    private VersionCheckData versionCheck;

    public void Awake()
    {
        logo = Resources.Load("Editor/logo_legless_white", typeof(Texture2D)) as Texture2D;
        EditorCoroutineUtility.StartCoroutine(CheckVersion(), this);
        AssetDatabase.Refresh();
    }

    public override void OnInspectorGUI()
    {
        Color guiColor = GUI.color; // Save the current GUI color
        GUI.color = Color.clear; // This does the magic
        GUI.color = guiColor; // Get back to previous GUI color

        EditorGUILayout.Space(60);
        DrawUILine(GREEN_COLOR);

        base.OnInspectorGUI();
        EditorGUILayout.LabelField("Version", AnythingSettings.PackageVersion);


        if (versionCheck != null)
        {
            if (versionCheck.version != AnythingSettings.PackageVersion)
            {
                if (GUILayout.Button("Click Here To Upgrade To " + versionCheck.version, GUILayout.Height(20)))
                {
                    Application.OpenURL(versionCheck.downloadLink);
                }
            }
        }

        EditorGUILayout.Space(10);
        DrawUILine(GREEN_COLOR);


        if (GUILayout.Button("Visit Our Site!", GUILayout.Height(20)))
        {
            Application.OpenURL("https://anything.world/");
        }

    }

    protected void DrawUILine(Color color, int thickness = 1, int padding = 20)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }

    private IEnumerator CheckVersion()
    {
        UnityWebRequest www = UnityWebRequest.Get(LATEST_VERSION_URL);
        yield return www.SendWebRequest();


        if (CheckWebRequest.IsError(www))
        {
            Debug.LogError("Network Error for Anything World version check");
            yield break;
        }

        string result = www.downloadHandler.text;

        //if (AnythingSettings.Instance.showDebugMessages) Debug.Log(result);

        versionCheck = JsonUtility.FromJson<VersionCheckData>(result);
    }
}