using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnythingWorld;
using AnythingWorld.Speech;

public class TranslationController : MonoBehaviour
{
    private Animator anim;
    public OVRInput.Controller controllerR;
    public OVRInput.Button buttonB, buttonScndry;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(buttonScndry, controllerR))
        {
            anim.SetTrigger("hello");
        }
        if(Input.GetKeyUp(KeyCode.Space) || (OVRInput.GetDown(buttonB, controllerR)))
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
        var words = testresponse.Split(' ');
        
        StartCoroutine(Timer(words));
        

    }

    // ANIMATION PLAYER
    void playAnim(string testresponse) // This Test Response is after spliting
    {
        if (testresponse == "in" || testresponse == "on" || testresponse == "and" || testresponse == "the" || testresponse == "an") 
        return;
        else if (testresponse == "hello" || testresponse == "salaam" || testresponse == "assalam alaikum" || testresponse == "hi" || testresponse == "greetings")
        {
            anim.SetTrigger("hello");
        }
        else if (testresponse == "i love you")
        {
            anim.SetTrigger("ily");
        }
        else if (testresponse == "thanks" || testresponse == "thank you" || testresponse == "shukran" || testresponse == "mercy")
        {
            anim.SetTrigger("thx");
        }
        else if (testresponse == "engineer" || testresponse == "engineering")
        {
            anim.SetTrigger("engineer");
        }
        else if (testresponse == "student")
        {
            anim.SetTrigger("student");
        }
        else if (testresponse == "there is an exam next lecture" || testresponse == "exam next lecture" || testresponse == "there is an exam next class" || testresponse == "exam next class")
        {
            anim.SetTrigger("exam next class");
        }
        else if (testresponse == "10 to the power of 0 equal to 1" || testresponse == "10^0=1" || testresponse == "10^0 equal 1" || testresponse == "10^0 equal one" || testresponse == "10 power 0 equal 1")
        {
            anim.SetTrigger("10 to power of 0");
        }
        else if (testresponse == "book")
        {
            anim.SetTrigger("book");
        }
        else if (testresponse == "calculus")
        {
            anim.SetTrigger("calculus");
        }
        else if (testresponse == "focus")
        {
            anim.SetTrigger("focus");
        }
        else if (testresponse == "find the value of x" || testresponse == "find value of x" || testresponse == "find the value of X" || testresponse == "find value of X")
        {
            anim.SetTrigger("find the value of x");
        }
        else if (testresponse == "i'm hungry")
        {
            anim.SetTrigger("im hungry");
        }
        else if (testresponse == "projector")
        {
            anim.SetTrigger("projector");
        }
        else if (testresponse == "raise your hand")
        {
            anim.SetTrigger("raise your hand");
        }
        else if (testresponse == "solve the problem")
        {
            anim.SetTrigger("solve the problem");
        } else if (testresponse == "one" || testresponse == "1")
        {
            anim.SetTrigger("one");
        } else if (testresponse == "two" || testresponse == "2" || testresponse == "to")
        {
            anim.SetTrigger("two");
        } else if (testresponse == "three" || testresponse == "3")
        {
            anim.SetTrigger("three");
        } else if (testresponse == "four" || testresponse == "4")
        {
            anim.SetTrigger("four");
        } else if (testresponse == "five" || testresponse == "5")
        {
            anim.SetTrigger("five");
        } else if (testresponse == "plus" || testresponse == "+")
        {
            anim.SetTrigger("plus");
        } else if (testresponse == "equal")
        {
            anim.SetTrigger("equal");
        }else if (testresponse == "exam")
        {
            anim.SetTrigger("exam");
        }else if (testresponse == "open")
        {
            anim.SetTrigger("open");
        }else if (testresponse == "i" || testresponse == "im" || testresponse == "i'm" || testresponse == "I" || testresponse == "Im" || testresponse == "I'm")
        {
            anim.SetTrigger("i");
        }
        else if (testresponse == "thursday")
        {
            anim.SetTrigger("thursday");
        }else if (testresponse == "tuesday")
        {
            anim.SetTrigger("tuesday");
        }else if (testresponse == "unknown")
        {
            anim.SetTrigger("unknown");
        }else if (testresponse == "x" || testresponse == "X")
        {
            anim.SetTrigger("x");
        }else if (testresponse == "find")
        {
            anim.SetTrigger("find");
        }else if (testresponse == "next")
        {
            anim.SetTrigger("next");
        }else if (testresponse == "name")
        {
            anim.SetTrigger("name");
        }


        //test
        else if (testresponse == "test")
        {
            anim.SetTrigger("test");
        }
    }

    private void InProcessListeningHandler(string testresponse)
    {
        Debug.Log("In Progress: " + testresponse);
    }



    // ENUMERATOR
    IEnumerator Timer(string[] words)
    {
        //string[] words = { "thanks", "book", "hello" };
        for (int i = 0; i < words.Length; i++)
        {
            playAnim(words[i]);
            print(words[i]);
            yield return new WaitForSeconds(2.57f);
            print("coroutine is here");
        }
       
    }

}
