using System;
using System.Runtime.InteropServices;
using UnityEngine;


public class MacOSSpeechUnderstanding : MonoBehaviour
{
    //Lets make our calls from the Plugin
    // [DllImport("ASimplePlugin", CallingConvention = CallingConvention.Cdecl)]
    // private static extern int PrintANumber();

    // [DllImport("ASimplePlugin", CallingConvention = CallingConvention.Cdecl)]
    // private static extern IntPtr PrintHello();

    // [DllImport("ASimplePlugin", CallingConvention = CallingConvention.Cdecl)]
    // private static extern int AddTwoIntegers(int i1, int i2);

    // [DllImport("ASimplePlugin", CallingConvention = CallingConvention.Cdecl)]
    // private static extern float AddTwoFloats(float f1, float f2);

    // [DllImport("ASimplePlugin", CallingConvention = CallingConvention.Cdecl)]
    // private static extern IntPtr GetSwiftTestMessage();

    [DllImport("AWMacOSSpeech", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr StartAudioCapture();

    [DllImport("AWMacOSSpeech", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr StartRecognition();

    [DllImport("AWMacOSSpeech", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr GetSpeechTranscript();

    private bool _isReady = false;

    private string _recognisedString;


    void Start()
    {
        // Debug.Log(PrintANumber());

        // Debug.Log(AddTwoIntegers(2, 2));
        // Debug.Log(AddTwoFloats(2.5F, 4F));
        // Debug.Log(GetSwiftTestMessage());
        // Debug.Log(Marshal.PtrToStringAnsi(PrintHello()));
        // Debug.Log(Marshal.PtrToStringAnsi(GetSwiftTestMessage()));

        Debug.Log(Marshal.PtrToStringAnsi(StartAudioCapture()));

        Debug.Log(Marshal.PtrToStringAnsi(StartRecognition()));

        Debug.Log(Marshal.PtrToStringAnsi(GetSpeechTranscript()));

        Invoke("MakeReady", 1f);

    }

    private void MakeReady()
    {
        _isReady = true;
    }

    void Update()
    {
        if (!_isReady)
            return;

        _recognisedString = Marshal.PtrToStringAnsi(GetSpeechTranscript()).ToLower();

        // TODO: what is this??
        // if (_recognisedString.Contains("dog"))
        // {
        //     AnythingCreator.Instance.MakeObject("dog");
        // }

        Debug.Log(Marshal.PtrToStringAnsi(GetSpeechTranscript()));
    }

}
