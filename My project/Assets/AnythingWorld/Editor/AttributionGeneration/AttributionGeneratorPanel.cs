using AnythingWorld.Editors;
using System;
using UnityEditor;
using UnityEngine;

public class AttributionGeneratorPanel : AnythingEditor
{

    [MenuItem("Anything World/Generate Attributions", false, 22)]
    public static void ShowWindow()
    {
        AttributionGeneratorPanel window = ScriptableObject.CreateInstance(typeof(AttributionGeneratorPanel)) as AttributionGeneratorPanel;
        GUIContent windowContent = new GUIContent("Attribution Generation");
        window.titleContent = windowContent;
        window.minSize = new Vector2(200, 200);
        window.position = new Rect(0, 0, 420, 520);
        window.ShowUtility();
    }
    private string[] fetchedAttributionArray;
    private Vector2 scrollPos;
    private Vector2 textboxScrollPos;
    private string message = " ";
    private string lastFilePath = "";
    private string lastFolderPath = "";
    private Color messageColor = Color.white;
    private void OnGUI()
    {
        InitializeResources();
        DrawBoldTextHeader("Generate Attribution", 16);


        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Refresh Attributions", submitButtonStyle, GUILayout.Width(180)))
        {
            fetchedAttributionArray =  AnythingWorld.AnythingSetup.GenerateAttributionsFromScene();
            messageColor = Color.white;
            message = $"{fetchedAttributionArray.Length} model attributions found in scene.";
            lastFilePath = " ";
        }


        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("Attributions:", BuildStyle(EditorStyles.label, PoppinsStyle.Medium, 12, TextAnchor.MiddleLeft), GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        textboxScrollPos = GUILayout.BeginScrollView(textboxScrollPos);
        GUILayout.TextArea(AnythingWorld.AnythingSetup.ModelAttributionStringBlock,/*BuildStyle(EditorStyles.textArea, PoppinsStyle.Regular, 12, TextAnchor.UpperLeft)*/ GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) /*GUILayout.MinHeight(100)*/);
        GUILayout.EndScrollView();
        GUILayout.Space(10);
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);

        GUILayout.BeginVertical();
        GUILayout.Label(message, BuildStyle(EditorStyles.label, PoppinsStyle.Medium, 12, TextAnchor.MiddleLeft, messageColor));
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        if (GUILayout.Button(lastFilePath, BuildStyle(EditorStyles.label, PoppinsStyle.Medium, 10, TextAnchor.MiddleLeft, Color.grey)))
        {

            try
            {
                System.Diagnostics.Process.Start(lastFolderPath);
            }
            catch (Exception e)
            {
                //The system cannot find the file specified...
                Debug.Log(e);
            }
        }
        //GUILayout.Label(messagePath, BuildStyle(EditorStyles.label, PoppinsStyle.Medium, 10, TextAnchor.MiddleLeft, Color.grey));
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.Space(10);
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Export To .txt", submitButtonStyle, GUILayout.Width(180)))
        {
            SaveToFile(AnythingWorld.AnythingSetup.ModelAttributionStringBlock, "txt");

        }

        if (GUILayout.Button("Export To .csv", submitButtonStyle, GUILayout.Width(180)))
        {
            SaveToFile(BuildCSVString(AnythingWorld.AnythingSetup.ModelAttributionStringBlock), "csv");

        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    public void SaveToFile(string content, string filetype)
    {
        if (content == null || content == "")
        {
            message = "No attributions found to export, try refreshing.";
            lastFilePath = "";
            messageColor = Color.red;
            return;
        }


            // The target file path e.g.
#if UNITY_EDITOR
        var folder = Application.streamingAssetsPath;
        if (!System.IO.Directory.Exists(folder)) System.IO.Directory.CreateDirectory(folder);

        string chosenPath = EditorUtility.OpenFolderPanel("Choose output directory", folder, "");

#else
    var folder = Application.persistentDataPath;
#endif
        var myUniqueFileName = string.Format(@"attribution_generation_{0}.{1}", System.DateTime.Now.Ticks, filetype);

        var filePath = System.IO.Path.Combine(chosenPath, myUniqueFileName);

        using (var writer = new System.IO.StreamWriter(filePath, false))
        {
            writer.Write(content);
        }

        message = "Succesfully saved attribution file";
        lastFilePath = filePath;
        messageColor = Color.green;
        lastFolderPath = chosenPath;
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    public string BuildCSVString(string content)
    {
        if (content == null || content == "")
        {
            return null;
        }


        var sb = new System.Text.StringBuilder("Name, Author\n");
        foreach (var myString in content.Split('\n'))
        {
            if (myString == string.Empty) continue;
            //remove instance of by between author and model name
            var cleanString = myString.Replace("by", "");
            //get model name
            var modelName = cleanString.Split(' ')[0];

            //get author name
            var authorName = cleanString.Replace(modelName, "").Trim();
            sb.Append($"{modelName},{authorName}\n");
        }
        return sb.ToString();
    }

}
