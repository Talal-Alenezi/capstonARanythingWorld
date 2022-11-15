using UnityEngine;
using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
using Unity.EditorCoroutines.Editor;
#endif
#if UNITY_EDITOR_WIN
using UnityEngine.Windows.Speech;
#elif UNITY_EDITOR_OSX
using System.Runtime.InteropServices;
#endif
using AnythingWorld.Speech;
using AnythingWorld.Habitat;

namespace AnythingWorld
{
#if UNITY_EDITOR
    /// <summary>
    /// Custom editor for the AWVoiceCreator.
    /// </summary>
    [Serializable]
    public class AnythingVoiceCreator : MonoBehaviour
    {
        #region Plugin Methods
#if UNITY_EDITOR_OSX
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
#elif UNITY_EDITOR_WIN
        private static DictationRecognizer m_DictationRecognizer = null;
        public static string RecogniserStatus
        {
            get
            {
                if (m_DictationRecognizer != null)
                {
                    return m_DictationRecognizer.Status.ToString();
                }
                else
                {
                    return "Recogniser not initialised.";
                }
            }
        }
#else
     public static string RecogniserStatus
    {
        get
        {
            return "Recogniser only present in windows 10";
        }
    }
#endif

        #endregion

        private bool DDEBUG = false;
        #region Fields
        private const double VOICE_SWITCH_DELAY = 0.5f;
        //private bool voiceInitialised = false;
        private bool listenForVoice = false;
        private string recognisedString;
        public string RecognisedString
        {
            get
            {
                return recognisedString;
            }
        }
        [SerializeField]
        private CreatureKeywords creatureKeywords;
        [SerializeField]
        private HabitatKeywords habitatKeywords;
        private int processInd;
        private string processingString;
        private string previousProcessingString;
        private Tuple<int, int, string, string> resultTuple;
        private Tuple<int, string> resultTupleHabitat;
        private double lastUtteranceTime;
        private double lastVoiceSwitchTime;
        private bool isMaking;
        int frameCount = 0;
        #region Delegates
        public delegate void VoiceInputDelegate(string detectedText);
        VoiceInputDelegate UpdateRecognisedInput;
        public delegate void ListeningStatusDelegate(bool listening);
        ListeningStatusDelegate ListeningStatusChange;
        public delegate void ListeningEndedDelegate();
        ListeningEndedDelegate ListeningEnded;


        #endregion


        public string progressStatus = "";
        public double initTimeout = 10.0;
        public double listeningTimeOut = 10.0;

        private bool voiceInitialized = false;
        //time for recognizer to start up
        public double initTimeoutCounter = 0.0;
        public double listeningTimeOutCounter = 0.0;


        #region Instancing
        [SerializeField]
        private static AnythingVoiceCreator instance;
        public static AnythingVoiceCreator Instance
        {
            get
            {
                if (instance == null)
                {
                    if (FindObjectOfType<AnythingVoiceCreator>())
                    {
                        instance = FindObjectOfType<AnythingVoiceCreator>();
                    }
                    else
                    {
                        instance = AnythingCreator.Instance.gameObject.AddComponent<AnythingVoiceCreator>();
                    }

                }
                return instance;
            }
        }

        #endregion
        public bool ListenForVoice
        {
            get
            {
                return listenForVoice;
            }
        }

        private string selectedInputMic;
        public string SelectedInputMic
        {
            get => selectedInputMic;
            set => selectedInputMic = value;
        }

        #endregion
        private AnythingVoiceCreator()
        {
            RefreshCreatureKeywords();
        }

        private void RefreshCreatureKeywords()
        {
            if (creatureKeywords == null)
                creatureKeywords = new CreatureKeywords();
            if (habitatKeywords == null)
                habitatKeywords = new HabitatKeywords();
        }
        public void SubscribeOutputText(VoiceInputDelegate delegateFunc)
        {
            UpdateRecognisedInput -= delegateFunc;
            UpdateRecognisedInput += delegateFunc;
        }
        public void SubscribeToListeningStatus(ListeningStatusDelegate delegateFunc)
        {
            ListeningStatusChange -= delegateFunc;
            ListeningStatusChange += delegateFunc;
        }
        public void SubscribeToListeningEnded(ListeningEndedDelegate delegateFunc)
        {
            ListeningEnded -= delegateFunc;
            ListeningEnded += delegateFunc;
        }

        public IEnumerator UpdateLoop()
        {
#if UNITY_EDITOR_WIN
            m_DictationRecognizer.Start();
            var loopStart = EditorApplication.timeSinceStartup;
            while (m_DictationRecognizer.Status == SpeechSystemStatus.Stopped)
            {
                progressStatus = "Waiting for recogniser to start";

                var timeWaiting = EditorApplication.timeSinceStartup - loopStart;
                initTimeoutCounter = timeWaiting;
                if (timeWaiting > initTimeout)
                {
                    progressStatus = "Timed out waiting for recogniser to start.";
                    ListeningStatusChange?.Invoke(false);
                    ActivateVoiceInput(false);
                    yield break;
                }
                yield return new EditorWaitForSeconds(0.01f);
            }


            voiceInitialized = true;
            progressStatus = "Recogniser started.";
            if (DDEBUG) Debug.Log("VOICELOG: Status = " + m_DictationRecognizer.Status);
#else
        voiceInitialized = true;
#endif
            if (voiceInitialized)
            {
                AnythingBase.CheckAwSetup();
                if (DDEBUG) Debug.Log("VOICELOG: Entering listening Update loop");

                listenForVoice = true;
                if (TryStartRecording())
                {
                    ListeningStatusChange?.Invoke(true);
                    lastUtteranceTime = EditorApplication.timeSinceStartup;
                    while (listenForVoice)
                    {
                        TextUpdate();
                        yield return new EditorWaitForSeconds(0.01f);

                        var timeInactive = EditorApplication.timeSinceStartup - lastUtteranceTime;
                        listeningTimeOutCounter = timeInactive;
                        if (timeInactive > listeningTimeOut)
                        {
                            progressStatus = "Recogniser timed out, exiting listen loop.";
                            if (DDEBUG)
                                Debug.Log("VOICELOG: Deactivating voice due to timeout.");
                            ActivateVoiceInput(false);
                            listenForVoice = false;
                        }
                    }
                }
                else
                {
                    ActivateVoiceInput(false);
                    listenForVoice = false;
                    yield break;
                }



            }
            if (DDEBUG) Debug.Log("VOICELOG: Update loop exit");
        }



        #region WinTTS Funcs
#if UNITY_EDITOR_WIN
        private void WindowsSTTSetup()
        {
            if (m_DictationRecognizer == null)
            {
                if (DDEBUG) Debug.Log("WindowsSTTSetup: Recognizer null, setting up");

                m_DictationRecognizer = new DictationRecognizer(ConfidenceLevel.Medium);

                if (DDEBUG) Debug.Log("WindowsSTTSetup: Subscribing callbacks to VoiceCreator methods.");

                m_DictationRecognizer.DictationResult += WinResult;

                m_DictationRecognizer.DictationHypothesis += WinHypothesis;

                m_DictationRecognizer.DictationComplete += WinComplete;

                m_DictationRecognizer.DictationError += WinError;
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
            if (AnythingSettings.Instance.showDebugMessages) Debug.LogWarning($"Dictation win result: {text}");
            recognisedString = text;
            ActivateVoiceInput(false);
        }

        private void WinHypothesis(string text)
        {
            if (AnythingSettings.Instance.showDebugMessages) Debug.LogWarning($"Dictation win hypothesis: {text}"); ;
            recognisedString = text;
        }

        private void WinComplete(DictationCompletionCause completionCause)
        {
            if (completionCause != DictationCompletionCause.Complete)
            {
                if (AnythingSettings.Instance.showDebugMessages) Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
            }
            else
            {
                if (AnythingSettings.Instance.showDebugMessages) Debug.Log("Dictation completed successfully");
            }
            ActivateVoiceInput(false);
        }

        private void WinError(string error, int hresult)
        {
            Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
            ActivateVoiceInput(false);
        }
#endif
        #endregion


        /// <summary>
        /// Initializes DictionRecognizer and returns true if successful.
        /// </summary>
        /// <returns>Boolean denoting status of m_DictationRecognizer</returns>
        private bool VoiceSetup()
        {
#if UNITY_EDITOR_WIN
            if (AnythingSettings.Instance.showDebugMessages) Debug.Log("Setting up new windows diction recognizer");
            try
            {
                if (m_DictationRecognizer != null)
                {
                    if (DDEBUG) Debug.Log("m_DictionRecognizer not null on VoiceSetup, destroying");
                    WindowsSTTDestroy();
                }
                WindowsSTTSetup();
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error initializing diction recognizer.:");
                Debug.LogException(e);
                return false;
            }
            if (m_DictationRecognizer == null)
            {
                if (DDEBUG) Debug.Log("m_DictionRecognizer failed to setup, exiting");
                return false;
            }
            else if (m_DictationRecognizer.Status == SpeechSystemStatus.Failed)
            {
                if (DDEBUG) Debug.LogWarning("Speech system failed to setup");
                return false;
            }
            return true;
#else
        ResetProcessedStringVariables();
        ResetRecognisedString();

        lastVoiceSwitchTime = EditorApplication.timeSinceStartup;
        isMaking = false;
        listenForVoice = false;
        return true;

#endif
        }

        /// <summary>
        /// Resets recognised string and invokes the UpdateRecognisedInput delegate func.
        /// </summary>
        private void ResetRecognisedString()
        {
            recognisedString = "...";
            UpdateRecognisedInput?.Invoke(recognisedString);
        }

        /// <summary>
        /// Reset process index and process string before receiving new dictation.
        /// </summary>
        private void ResetProcessedStringVariables()
        {
            processInd = 0;
            processingString = "empty";
            recognisedString = "";
        }

#if UNITY_EDITOR
        /// <summary>
        /// Initializes voice recognition and begins listening.
        /// Starts listening UpdateLoop coroutine if initialization successful.
        /// </summary>
        public void StartVoiceInput()
        {
            if (AnythingSettings.Instance.showDebugMessages) Debug.Log("Starting voice input");

            ResetProcessedStringVariables();


#if UNITY_EDITOR_OSX
            if(VoiceSetup())
            {
                Debug.Log("start voice");
                Marshal.PtrToStringAnsi(StartRecognition());
            }
            else
            {
                if (DDEBUG) Debug.LogWarning("Failed to setup voice");
                return;
            }
           
#elif UNITY_EDITOR_WIN
            if (VoiceSetup())
            {
                m_DictationRecognizer.Start();
            }
            else
            {
                if (DDEBUG) Debug.LogWarning("Failed to setup voice");
                return;
            }
#endif


            if (Application.isEditor && !Application.isPlaying)
            {
#if UNITY_EDITOR
                if (voiceListeningLoopEditorCoroutine != null)
                    EditorCoroutineUtility.StopCoroutine(voiceListeningLoopEditorCoroutine);
                voiceListeningLoopEditorCoroutine = EditorCoroutineUtility.StartCoroutine(UpdateLoop(), this);
#endif
            }
            else
            {

                if (voiceListeningCoroutine != null)
                    StopCoroutine(UpdateLoop());
                voiceListeningCoroutine = StartCoroutine(UpdateLoop());
            }
            return;
        }
#endif
#if UNITY_EDITOR
        private EditorCoroutine voiceListeningLoopEditorCoroutine;
#endif
        private Coroutine voiceListeningCoroutine;

        /// <summary>
        /// Stops speech recognition and updates recognised string.
        /// UpdateRecognisedInput delegate invoked with recognised string and then reset.
        /// </summary>
        public void StopVoiceInput()
        {
            UpdateRecognisedInput?.Invoke(recognisedString);
            if (AnythingSettings.Instance.showDebugMessages) Debug.Log("Stop voice input");
#if UNITY_EDITOR_OSX
            Debug.Log("stop voice");
            Debug.Log(Marshal.PtrToStringAnsi(StopRecognition()));
#elif UNITY_EDITOR_WIN
            if (m_DictationRecognizer != null)
            {
                WindowsSTTDestroy();
            }
#endif
            if (recognisedString == "")
            {
                ResetRecognisedString();
            }



            StopRecordingInput();
            AnythingSetup.Instance.ShowLoading(false);

            if (Application.isEditor && !Application.isPlaying)
            {
#if UNITY_EDITOR
                if (voiceListeningLoopEditorCoroutine != null)
                    EditorCoroutineUtility.StopCoroutine(voiceListeningLoopEditorCoroutine);
#endif
            }
            else
            {

                if (voiceListeningCoroutine != null)
                    StopCoroutine(UpdateLoop());
            }

            ListeningStatusChange?.Invoke(false);
            ListeningEnded?.Invoke();

            if (AnythingSettings.Instance.showDebugMessages) Debug.Log("Voice output: " + recognisedString);
            ListeningEnded();


        }

        public void ActivateVoiceInput(bool shouldActivate)
        {

            if ((EditorApplication.timeSinceStartup - lastVoiceSwitchTime) > VOICE_SWITCH_DELAY)
            {
                lastVoiceSwitchTime = EditorApplication.timeSinceStartup;
            }
            else
            {
                return;
            }

            if (AnythingSettings.Instance.showDebugMessages) Debug.Log("ActivateVoiceInput : " + shouldActivate);

            if (shouldActivate)
            {
                StartVoiceInput();
            }
            else
            {
                StopVoiceInput();
                listenForVoice = false;
            }
            listenForVoice = shouldActivate;
            ListeningStatusChange?.Invoke(shouldActivate);
        }

        #region Monitor Volume
        private string micDeviceName;
        private AudioClip recordedClip;
        public bool TryStartRecording()
        {
            if (TryGetActiveMicrophoneName(out var micDeviceName))
            {
                if (AnythingSettings.Instance.showDebugMessages) Debug.Log($"Starting recording with {micDeviceName}");
                if (Microphone.IsRecording(micDeviceName))
                {
                    StopRecordingInput();
                }

                Microphone.GetDeviceCaps(micDeviceName, out var minSupportedFreq, out var maxSupportedFreq);
                try
                {
                    recordedClip = Microphone.Start(micDeviceName, true, 999, maxSupportedFreq);
                    if (recordedClip != null) { return true; } else { return false; }
                }
                catch (Exception e) { Debug.LogException(e); return false; }
            }
            else { Debug.LogWarning("Could not start recording, no microphone device found."); return false; }
        }
        public float GetMaxLevel()
        {
            return LevelMax();
        }



        public bool TryGetActiveMicrophoneName(out string micDeviceName)
        {
            if (selectedInputMic != null)
            {
                micDeviceName = selectedInputMic;
                return true;
            }


            if (Microphone.devices != null && Microphone.devices.Length > 0)
            {
                try
                {
                    Debug.Log("Attempting to find microphone device");
                    micDeviceName = Microphone.devices[0];
                    Debug.Log(Microphone.devices.Length);
                }
                catch (Exception e)
                {

                    throw new Exception($"{e.GetType()}: Could not assign microphone to micDevice");

                }

                if (AnythingSettings.Instance.showDebugMessages) Debug.Log($"Mic device used: {micDeviceName}");
                return true;

            }
            else
            {
                throw new InvalidOperationException("No microphone devices recognised by unity.");
            }

        }
        private int sampleWindow = 128;
        private float LevelMax()
        {
            if (recordedClip != null)
            {
                float levelMax = 0;
                var waveData = new float[sampleWindow];
                var micPosition = Microphone.GetPosition(null) - (sampleWindow + 1); // null means the first microphone
                if (micPosition < 0) return 0;
                recordedClip.GetData(waveData, micPosition);
                // Getting a peak on the last 128 samples
                for (var i = 0; i < sampleWindow; i++)
                {
                    var wavePeak = waveData[i] * waveData[i];
                    if (levelMax < wavePeak)
                    {
                        levelMax = wavePeak;
                    }
                }
                return levelMax;
            }
            else
            {

                return -1;
            }

        }
        private void StopRecordingInput()
        {
            if (micDeviceName == null) return;
            else
            {
                if (Microphone.IsRecording(micDeviceName))
                {
                    Microphone.End(micDeviceName);
                }
            }

        }
        #endregion


        #region Keyword Processing
        public void CheckForKeyWordMatch()
        {
            if (DDEBUG) Debug.Log("Check for key word match()");
            if (processingString.Length > 2)
            {
                resultTuple = creatureKeywords.ProcessUtterance(processingString);
                if (AnythingSettings.DebugEnabled)
                {
                    var creaturesString = "";
                    foreach (var name in creatureKeywords.Creatures)
                    {
                        creaturesString += name + "\n";
                    }
                    Debug.Log($"Keyword found: {creaturesString}");
                }


                // have object keyword match
                if (resultTuple.Item1 != -1)
                {
                    if (AnythingSettings.Instance.showDebugMessages) Debug.Log("object: resultTuple.Item1 = " + resultTuple.Item1);
                    EditorCoroutineUtility.StartCoroutine(MakeObjectWithVoice(resultTuple.Item4, resultTuple.Item3, resultTuple.Item2), this);
                    processInd += resultTuple.Item1;
                }
                else
                {
                    resultTupleHabitat = habitatKeywords.ProcessUtterance(processingString);
                    if (resultTupleHabitat.Item1 != -1)
                    {
                        if (AnythingSettings.Instance.showDebugMessages) Debug.Log("habitat: resultTuple.Item1 = " + resultTuple.Item1);
                        var habitatKeyword = resultTupleHabitat.Item2;
                        AnythingHabitat.Instance.MakeHabitat(habitatKeyword);
                        processInd += resultTupleHabitat.Item1;
                    }
                }



            }
        }

        private void ProcessRecognisedString()
        {
            if (DDEBUG) Debug.Log("ProcessRecognisedString()");
            if (creatureKeywords == null)
                creatureKeywords = new CreatureKeywords();
            if (habitatKeywords == null)
                habitatKeywords = new HabitatKeywords();

            if (DDEBUG) Debug.Log("Trying ProcessUtterance with string " + recognisedString);
            resultTuple = creatureKeywords.ProcessUtterance(recognisedString);
        }
        #endregion

        #region Voice Creation
        public void TryCreate()
        {
            if (DDEBUG) Debug.Log("TryCreate()");
            if (resultTuple != null)
            {
                if (AnythingSettings.Instance.showDebugMessages) Debug.Log("resultTuple.Item1 = " + resultTuple.Item1);
                ProcessRecognisedString();
            }
        }
        private IEnumerator MakeObjectWithVoice(string objectName, string zoneName, int quantity)
        {

            if (AnythingSettings.Instance.showDebugMessages) Debug.Log($"make object with voice : {objectName}, {zoneName}, {quantity}");

            while (isMaking)
                yield return new EditorWaitForSeconds(0.05f);

            isMaking = true;

            if (string.IsNullOrEmpty(zoneName))
            {

                for (var i = 0; i < quantity; i++)
                {
                    AnythingCreator.Instance.MakeObject(objectName);
                    yield return null;
                }

            }
            else
            {
                var zoneObjPos = ZonePlacement.Instance.GetZoneObjectPosition(zoneName);
                var zoneObjScale = ZonePlacement.Instance.GetZoneObjectScale();

                for (var i = 0; i < quantity; i++)
                {
                    AnythingCreator.Instance.MakeObject(objectName, zoneObjPos, Quaternion.Euler(0, UnityEngine.Random.Range(0f, 360f), 0), zoneObjScale, false);
                }
            }
            isMaking = false;
        }

        /* private IEnumerator MakeHabitatWithVoice(string hName)
         {
             if (awHabitat == null)
             {
                 GameObject habitatCreator = new GameObject();
                 habitatCreator.name = "Habitat Creator";
                 awHabitat = habitatCreator.AddComponent<AWHabitat>();
             }


             awHabitat.MakeHabitat(hName);

             while (!awHabitat.AWHabitatReady)
             {
                 yield return new EditorWaitForSeconds(0.01f);
             }
         }*/
        #endregion

        #region Handle Text Output
        public void ClearOutput()
        {
            if (listenForVoice)
            {
                ActivateVoiceInput(false);
            }
            recognisedString = "...";
            UpdateRecognisedInput(recognisedString);
        }
        void TextUpdate()
        {
            if (listenForVoice)
            {
#if UNITY_EDITOR_OSX
            recognisedString = Marshal.PtrToStringAnsi(GetSpeechTranscript()).ToLower();
            Debug.Log("recognisedString = " + recognisedString);
#endif
                if (processingString == "empty")
                    processingString = recognisedString;

                if (processingString != previousProcessingString)
                {
                    if(DDEBUG) Debug.Log("Checking for keyword since processedString. " +
                        $" Last: {previousProcessingString} " +
                        $" Current: {processingString} ");
                    lastUtteranceTime = EditorApplication.timeSinceStartup;
                    CheckForKeyWordMatch();
                    previousProcessingString = processingString;
                }

                if (processInd > recognisedString.Length)
                    processInd = recognisedString.Length - 1;

                processingString = recognisedString.Substring(processInd);
                frameCount++;
                UpdateRecognisedInput?.Invoke(recognisedString);
                SceneView.RepaintAll();
            }

        }

        public string[] GetMicrophoneArray()
        {
            if (Microphone.devices != null)
            {
                return Microphone.devices;
            }
            else
            {
                return null;
            }
        }


        #endregion




    }
#endif
}
