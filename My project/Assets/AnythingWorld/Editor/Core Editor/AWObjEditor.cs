using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnythingWorld.AWObj))]
public class AWObjEditor : Editor
{
    private AnythingWorld.AWObj owner;
    public void OnEnable()
    {
        owner = (AnythingWorld.AWObj)target;
    }
    public override void OnInspectorGUI()
    {

        if (GUILayout.Button("Serialize Meshes"))
        {
            var meshFilters = owner.GetMeshesInHierarchy();
            var sharedMaterials = owner.GetSharedMaterials();

            /// Directory of folder to store assets, relative to Assets
            string assetName = $"{owner.name}.asset";
            string folderPath = $"Assets/AnythingWorldSerializedAssets/{owner.name}";
            /*
            if (!AssetDatabase.IsValidFolder("Assets/StreamingAssets"))
            {
                AssetDatabase.CreateFolder("Assets", "StreamingAssets");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            */
            if (!AssetDatabase.IsValidFolder("Assets/AWSerializedAssets"))
            {
                AssetDatabase.CreateFolder("Assets", "AWSerializedAssets");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                string folderGUID = AssetDatabase.CreateFolder("Assets/AWSerializedAssets", owner.name);
                folderPath = AssetDatabase.GUIDToAssetPath(folderGUID);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            Debug.Log($"Serialized asset at {folderPath}");

            int count = 0;
            //Save mesh filters
            foreach(var filter in meshFilters)
            {
                
                if (filter.sharedMesh != null)
                {
                    if (AssetDatabase.Contains(filter.sharedMesh))
                    { 
                        Debug.Log($"Mesh {owner.name}_{filter.sharedMesh.name} already serialized within database.");
                        continue;
                    }
                    var safeFilterName = GenerateSafeFilePath(filter.sharedMesh.name);
                    var assetPath = $"{folderPath}/{owner.name}_{safeFilterName}_{count}.asset";
                    if (!AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D)))
                    {
                        try
                        {
                            AssetDatabase.CreateAsset(filter.sharedMesh, assetPath);
                            EditorUtility.SetDirty(filter.sharedMesh);
                            AssetDatabase.SaveAssets();
                            count++;
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }

                }
                else
                {
                    Debug.Log($"filter SharedMesh is null: {filter.gameObject.name}");
                }
            }
            count = 0;
            //Save texture
            foreach (var mat in sharedMaterials)
            {

                Texture2D[] texList = GetSharedTexturesFromShader(mat);
                foreach (Texture2D tex in texList)
                {

                    if (AssetDatabase.Contains(tex))
                    {
                        Debug.Log($"Mesh {owner.name}_{tex.name} already serialized within database.");
                        continue;
                    }
                    if (tex != null)
                    {
                        var safeTexName = GenerateSafeFilePath(tex.name);
                        var assetPath = $"{folderPath}/{owner.name}_{safeTexName}_{count}.asset";
                        if (!AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D)))
                        {
                            try
                            {
                                AssetDatabase.CreateAsset(tex, assetPath);
                                EditorUtility.SetDirty(tex);
                                AssetDatabase.SaveAssets();
                                count++;
                            }
                            catch (System.Exception e)
                            {
                                Debug.LogException(e);
                            }
                        }
                    }
                    else
                    {
                        Debug.Log($"Texture is null: {tex.name}");
                    }
                }
            }
            count = 0;
            //Saved material instances
            foreach (Material mat in sharedMaterials)
            {
                if (mat != null)
                {

                    if (AssetDatabase.Contains(mat))
                    {
                        Debug.Log($"Mesh {owner.name}_{mat.name} already serialized within database.");
                        continue;
                    }
                    var safeMatName = GenerateSafeFilePath(mat.name);
                    var assetPath = $"{folderPath}/{owner.name}_{safeMatName}__{count}.mat";
                    if(!AssetDatabase.LoadAssetAtPath(assetPath, typeof(Material)))
                    {
                        try
                        {
                            EditorUtility.SetDirty(mat);
                            AssetDatabase.CreateAsset(mat, assetPath);
                            AssetDatabase.SaveAssets();
                            count++;
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                    else
                    {
                        Debug.Log($"Did not save asset, already serialized at {assetPath}");
                    }
                   
                }
                else
                {
                    Debug.Log($"Material is null: {mat.name}");
                }
                
            }
            AssetDatabase.Refresh();
        }
        base.OnInspectorGUI();
    }

    private static string GenerateSafeFilePath(string inputPath)
    {
        string illegalChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        Regex r = new Regex(string.Format("[{0}]", Regex.Escape(illegalChars)));
        var safePath = r.Replace(inputPath, "");
        return safePath;
    }
    static void DeleteAllFilesInFolder(string folderPath)
    {
        // Check file is valid for deletion
        if (!AssetDatabase.IsValidFolder(folderPath)) return;
        string[] unusedFolder = { folderPath };
        foreach (var asset in AssetDatabase.FindAssets("", unusedFolder))
        {
            var path = AssetDatabase.GUIDToAssetPath(asset);
            AssetDatabase.DeleteAsset(path);
        }
    }
    private Texture2D[] GetSharedTexturesFromShader(Material mat)
    {
        List<Texture2D> textureList = new List<Texture2D>();
        string[] textureProps = mat.GetTexturePropertyNames();
        foreach(var propertyName in textureProps)
        {

            Texture2D tex = mat.GetTexture(propertyName) as Texture2D;
            if (tex != null)
            {
                //Debug.Log($"Non null tex found: {propertyName}");
                tex.name = propertyName;
                textureList.Add(tex);
            }
        }

        return textureList.ToArray();
    }
}
