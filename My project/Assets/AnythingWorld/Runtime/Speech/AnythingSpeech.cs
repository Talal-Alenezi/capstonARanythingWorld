using UnityEngine;
using System.Collections;
using System;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
using UnityEngine.Windows.Speech;
using SpeechLib;
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
using System.Runtime.InteropServices;
#endif


namespace AnythingWorld.Speech
{
    /// <summary>
    /// Handles voice activation and input.
    /// </summary>
    [System.Serializable]
    public class AnythingSpeech : MonoBehaviour
    {
#region Plugin Methods

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
    [DllImport("AWMacOSSpeech", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr StartAudioCapture();

    [DllImport("AWMacOSSpeech", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr StartRecognition();

    [DllImport("AWMacOSSpeech", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr GetSpeechTranscript();

    [DllImport("AWMacOSSpeech", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr StopRecognition();

    [DllImport("AWMacOSSpeech", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr StopAudioCapture();
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        private DictationRecognizer m_DictationRecognizer;
#endif

#endregion

#region Fields
        private bool DDEBUG = false;
        private string recognisedString;
        private readonly float listeningTimeoutValue = 7f;
        private readonly float intializationTimeout = 7f;


#region Delegates
        public delegate void AWSpeechHandler(string testResponse);
        public event AWSpeechHandler OnSpeechResponse;

        public delegate void AWSpeechDoneHandler(string testResponse);
        public event AWSpeechDoneHandler OnSpeechDone;
#endregion

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        SpVoice _winVoice;
#endif
        private static AnythingSpeech instance;
        public static AnythingSpeech Instance
        {
            get
            {
                if (instance == null)
                {
                    var anythingCreatorGO = new GameObject();
                    anythingCreatorGO.name = "Anything Speech";
                    var anythingCreator = anythingCreatorGO.AddComponent<AnythingSpeech>();
                    instance = anythingCreator;
                }
                return instance;
            }
        }

#endregion
        
        public void Awake()
        {
            VoiceSetup();
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            _winVoice = new SpVoice();
#endif
        }
#region WinTTS Funcs

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || PLATFORM_STANDALONE_WIN
        private void WindowsSTTSetup()
        {
            if (DDEBUG) Debug.Log("Attempting WindowsSTT setup");
            if (m_DictationRecognizer == null)
            {
                if (DDEBUG) Debug.Log("WindowsSTTSetup: Recognizer null, setting up");

                m_DictationRecognizer = new DictationRecognizer(ConfidenceLevel.Low,DictationTopicConstraint.Dictation);
                m_DictationRecognizer.AutoSilenceTimeoutSeconds = listeningTimeoutValue;
                m_DictationRecognizer.InitialSilenceTimeoutSeconds = intializationTimeout;
                if (DDEBUG) Debug.Log("WindowsSTTSetup: Subscribing callbacks to VoiceCreator methods.");

                m_DictationRecognizer.DictationResult += WinResult;

                m_DictationRecognizer.DictationHypothesis += WinHypothesis;

                m_DictationRecognizer.DictationComplete += WinComplete;

                m_DictationRecognizer.DictationError += WinError;

                if (DDEBUG) Debug.Log("WindowsSTTSetup success");
                return;
            }
            else
            {
                if (DDEBUG) Debug.Log("WindowsSTTSetup: Recognizer not null, not setting up");
            }

        }
        private void WindowsSTTDestroy()
        {

            if (m_DictationRecognizer != null)
            {
                //Debug.Log("WindowsSTTDestroy: Recognizer not null, destroying");
                m_DictationRecognizer.DictationResult -= WinResult;

                m_DictationRecognizer.DictationHypothesis -= WinHypothesis;

                m_DictationRecognizer.DictationComplete -= WinComplete;

                m_DictationRecognizer.DictationError -= WinError;

                /* 
                 * DictionRecognizer but always be stopped before calling Dispose() (even if not running)
                 * otherwise a "This value can only be destructed on MainThread!" exception
                 * will be thrown with no stacktrace of useful information.
                 * They will throw a warning if you do it when it's running through, so must be try blocked.
                 * This is a unity and a microsoft bug, may god help us.
                 * For more info: https://forum.unity.com/threads/error-this-value-can-only-be-destructed-on-mainthread-when-doing-dispose.585607/
                */
                try
                {
                    m_DictationRecognizer.Stop();
                }
                catch { }

                m_DictationRecognizer.Dispose();
                m_DictationRecognizer = null;
            }
            else
            {
                Debug.Log("WindowsSTTDestroy: Recognizer null, not destroying");
            }

        }
        private void WinResult(string text, ConfidenceLevel confidence)
        {
            if(DDEBUG) Debug.LogWarning($"Dictation win result: {text}");
            recognisedString = text;
            StopListening();
        }
        private void WinHypothesis(string text)
        {
            //if (DDEBUG) Debug.LogWarning($"Dictation win hypothesis: {text}"); ;
            recognisedString = text;
        }
        private void WinComplete(DictationCompletionCause completionCause)
        {
            if (completionCause != DictationCompletionCause.Complete)
            {
                if(DDEBUG) Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
            }
            else
            {
                if(DDEBUG) Debug.Log("Dictation completed successfully");
            }
            StopListening();
        }
        private void WinError(string error, int hresult)
        {
            Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
            StopListening();
        }
#endif

        /// <summary>
        /// Activates voice input and starts listening to user.
        /// </summary>
        /// <param name="responseCallback"></param>
        /// <param name="doneCallback"></param>
        public void StartListening(AWSpeechHandler responseCallback, AWSpeechDoneHandler doneCallback)
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            WindowsSTTSetup();
            OnSpeechResponse = responseCallback;
            OnSpeechDone = doneCallback;
            m_DictationRecognizer.Start();

#endif
        }
        public void StartListening(AWSpeechDoneHandler doneCallback)
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            WindowsSTTSetup();
            OnSpeechDone = doneCallback;
            if (DDEBUG) Debug.Log("Listening started.");
            m_DictationRecognizer.Start();
#endif
        }

        /// <summary>
        /// Deactivate voice input and stop listening.
        /// </summary>
        public void StopListening()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            if (DDEBUG) Debug.Log("Stopping listening and destroying STT instance.");
            m_DictationRecognizer.Stop();
            WindowsSTTDestroy();
            OnSpeechDone?.Invoke(recognisedString);
            OnSpeechDone = null;
            OnSpeechResponse = null;
#endif
        }
#endregion
        /// <summary>
        /// Use speech synthesis to say input string.
        /// </summary>
        /// <param name="thingToSay">String to vocalise.</param>
        /// <param name="voiceName">Name of voice</param>
        public void Speak(string thingToSay, string voiceName = "none")
        {
            if (DDEBUG) Debug.Log("Speak string '" + thingToSay + "' requested.");
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        if(DDEBUG) Debug.Log("Speak using MACOSSpeechSynth.");
        if (voiceName == "none")
        {
            MacOSSpeechSynth.Instance.Speak(thingToSay);
        }
        else
        {
            MacOSSpeechSynth.Instance.Speak(thingToSay,voiceName);
        }
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            if (DDEBUG) Debug.Log("Speak using winVoice");
            if (_winVoice == null)
                _winVoice = new SpVoice();
            _winVoice.Speak(thingToSay, SpeechVoiceSpeakFlags.SVSFlagsAsync);
#endif
        }
        /// <summary>
        /// Setup voice input.
        /// </summary>
        private void VoiceSetup()
        {
            recognisedString = "";
        }
    }
}