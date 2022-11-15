using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
namespace AnythingWorld.Utilities
{
    [InitializeOnLoad]
    public class SessionManager
    {
        static SessionManager()
        {
#if UNITY_EDITOR
            EditorApplication.wantsToQuit -= SessionManager.LogSessionEnd;
            EditorApplication.wantsToQuit += SessionManager.LogSessionEnd;
#endif
        }
        public static bool LogSessionEnd()
        {
            //must not block as this will block 
            try
            {
                if (AnythingWorld.AnythingSettings.ApiKey != null &&
                    AnythingWorld.AnythingSettings.ApiKey != ""
                    )
                {
                    var seconds = EditorApplication.timeSinceStartup;
                    var hours = Math.Floor(seconds / 60 / 60);
                    var minutes = Math.Floor((seconds / 60) - (hours * 60));
                    UploadSessionLogData(hours.ToString(), minutes.ToString(), AnythingWorld.AnythingSettings.ApiKey, AnythingWorld.AnythingSettings.AppName);
                    return true;
                }
                else
                {
                    //not logged in so may not have have accepted our terms and conditions
                    return true;
                }
            }
            catch
            {
                return true;
            }
        }
        private static void UploadSessionLogData(string hours, string minutes, string apiKey, string appName)
        {
            string encodedAppName = System.Uri.EscapeUriString(appName);
            string url = $"{AnythingApiConfig.ApiUrlStem}/session-length";
            string data = $"?key={apiKey}&platform=unity&app={encodedAppName}&hours={hours}&minutes={minutes}";
            string request = url + data;
            var www = UnityWebRequest.Post(request, "");
            www.SendWebRequest();
        }
    }
}