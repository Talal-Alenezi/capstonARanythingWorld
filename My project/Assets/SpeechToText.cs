using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnythingWorld;
using AnythingWorld.Speech;

public class SpeechToText : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("Start Listening");
            AnythingSpeech.Instance.StartListening(InProcessListeningHandler, FinishListeningHandler);
            // AnythingSpeech.Instance.TimeOutValue;
        }
    }

    private void FinishListeningHandler(string testresponse)
    {
        Debug.Log("The Final response " + testresponse);
    }

    private void InProcessListeningHandler(string testresponse)
    {
        Debug.Log("In Progress: " + testresponse);
    }
}