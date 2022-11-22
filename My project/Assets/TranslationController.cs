using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnythingWorld;
using AnythingWorld.Speech;

public class TranslationController : MonoBehaviour
{
    private Animator anim;
    //public OVRInput.Controller controllerR;
    //public OVRInput.Button buttonB, buttonScndry;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (OVRInput.GetDown(buttonB, controllerR))
        if(Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("Start Listening");
            AnythingSpeech.Instance.StartListening(InProcessListeningHandler, FinishListeningHandler);
        }

        //if (OVRInput.GetDown(buttonB, controllerR))
        //   anim.SetTrigger("hello");
        //if (OVRInput.GetDown(buttonScndry, controllerR))
        //  anim.SetTrigger("ily");
    }// UPDATE END


    private void FinishListeningHandler(string testresponse)
    {
        Debug.Log("The Final response " + testresponse);
        if (testresponse == "hello" || testresponse=="salaam" || testresponse=="assalam alaikum" || testresponse=="hi" || testresponse== "greetings")
        {
            anim.SetTrigger("hello");
        }
        else if (testresponse == "i love you")
        {
            anim.SetTrigger("ily");
        }
        else if (testresponse == "thanks" || testresponse == "thank you" || testresponse=="shukran" || testresponse=="mercy")
        {
            anim.SetTrigger("thx");
        }
    }

    private void InProcessListeningHandler(string testresponse)
    {
        Debug.Log("In Progress:" + testresponse);
    }

}
