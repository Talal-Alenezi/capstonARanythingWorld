using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using AnythingWorld.Speech;
using AnythingWorld;

/// <summary>
/// 
/// </summary>
namespace AnythingWorld.Chat
{
    public class AnythingChat
    {
        #region Fields
        public string API_LOCATION_BASE = AnythingApiConfig.ApiUrlStem + "/df/";
        public const string API_LOCATION_TEXT = "textRequest";
        public const string API_LOCATION_AUDIO = "audioRequest";
        private AWNaturalLanguageResponse awResponse;
        public delegate void AWResponseHandler(AWNaturalLanguageResponse awResponse);
        public event AWResponseHandler OnAWResponse;
        private bool DDEBUG = false;
        #endregion

        /// <summary>
        /// Get a conversational response to a string, powered by the DialogFlow chatbot.
        /// </summary>
        /// <param name="message">Input string to generate response from.</param>
        /// <param name="responseCallback">Reference to callback that will receive AWNaturalLanguageResponse response from DialogFlow.</param>
        /// <param name="routineObj">Reference to MonoBehaviour function owning the new coroutine.</param>
        public void Talk(string message, AWResponseHandler responseCallback, MonoBehaviour routineObj)
        {
            OnAWResponse += responseCallback;
            routineObj.StartCoroutine(SendText(message, awReply =>
            {
                OnAWResponse(awResponse);
                OnAWResponse -= responseCallback;
            }));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator SendText(string message, System.Action<AWNaturalLanguageResponse> callback)
        {

            var APICall = API_LOCATION_BASE + API_LOCATION_TEXT;
            var awMessage = new AWTextMessageInput(message);
            var postData = JsonUtility.ToJson(awMessage);
            if (DDEBUG) Debug.Log(postData);
            var bytes = Encoding.UTF8.GetBytes(postData);
            var byteString = "{";
            foreach(var b in bytes){
                byteString += b.ToString();
                byteString += ",";
            }
            byteString += "}";
            var uwr = UnityWebRequest.Put(APICall, bytes);
            uwr.SetRequestHeader("Content-Type", "application/json");
            uwr.method = "POST";
            uwr.timeout = 20;
            yield return uwr.SendWebRequest();

            if (AnythingWorld.Utilities.CheckWebRequest.IsError(uwr))
            {
                Debug.LogError("Network Error for API : " + uwr.error);
                yield break;
            }
            if (String.IsNullOrEmpty(AnythingSettings.Instance.dialogFlowAgentId))
            {
                AnythingSettings.Instance.dialogFlowAgentId = "anything-world-api";

            }
            if (DDEBUG) Debug.Log("->: " + uwr.downloadHandler.text);
            awResponse = JsonUtility.FromJson<AWNaturalLanguageResponse>(uwr.downloadHandler.text);
            callback(awResponse);
        }
    }
}

