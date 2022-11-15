using System.Collections;
using UnityEditor;
using UnityEngine.Networking;
using UnityEngine;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif

namespace AnythingWorld.Utilities
{
    public class VersionCheckEditor : Editor
    {

        public static void DisplayUpdateDialogue()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorCoroutineUtility.StartCoroutineOwnerless(GetVersion("http://anything-world-api.appspot.com/version"));
            }
        }

        private static IEnumerator GetVersion(string uri)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(uri))
            {
                yield return uwr.SendWebRequest();



                if (AnythingWorld.Utilities.CheckWebRequest.IsSuccess(uwr))
                {
                    VersionResponse response = VersionResponse.CreateFromJson(uwr.downloadHandler.text);

                    if (response.version != AnythingSettings.PackageVersion && !AnythingSettings.PackageVersion.Contains("b"))
                    {
                        if (EditorUtility.DisplayDialog("Update Anything World", response.message, "Get Now", "Exit"))
                        {
                            Application.OpenURL(response.downloadLink);
                        }
                    }
                    else
                    {
                        //Debug.Log("Current version: " + AnythingSettings.VERSION_NUMBER + "\n LATEST VERSION: " + response.version);
                    }
                }




            }


            yield return null;
        }

        [System.Serializable]
        public class VersionResponse
        {
            public string downloadLink;
            public string version;
            public string message;

            public static VersionResponse CreateFromJson(string jsonString)
            {
                return JsonUtility.FromJson<VersionResponse>(jsonString);
            }
        }

    }

}

