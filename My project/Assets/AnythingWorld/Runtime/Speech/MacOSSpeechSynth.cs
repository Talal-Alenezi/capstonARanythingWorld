using UnityEngine;
using UnityEngine.Events;
public class MacOSSpeechSynth : MonoBehaviour
{
    #region Fields
    public int outputChannel = 0;
    public UnityEvent onStartedSpeaking;
    public UnityEvent onStoppedSpeaking;
    System.Diagnostics.Process speechProcess;
    bool wasSpeaking;
    private static MacOSSpeechSynth instance;
    public static MacOSSpeechSynth Instance
    {
        get
        {
            if (instance == null)
            {
                var anythingCreatorGO = new GameObject();
                anythingCreatorGO.name = "Anything Speech Synth";
                var speechSynth = anythingCreatorGO.AddComponent<MacOSSpeechSynth>();
                instance = speechSynth;
            }
            return instance;
        }
    }
    #endregion

    #region Unity Callbacks
    void Start()
    {
        // Invoke("DelaySpeak",1f);
    }
    // void Update() {
    //     bool isSpeaking = (speechProcess != null && !speechProcess.HasExited);
    //     if (isSpeaking != wasSpeaking) {
    //         if (isSpeaking) onStartedSpeaking.Invoke();
    //         else onStoppedSpeaking.Invoke();
    //         wasSpeaking = isSpeaking;
    //     }
    // }
    #endregion

    #region Public Methods
    public void Speak(string text, string voice = "Samantha")
    {
        var cmdArgs = string.Format("-a {2} -v {0} \"{1}\"", voice, text.Replace("\"", ","), outputChannel);
        speechProcess = System.Diagnostics.Process.Start("/usr/bin/say", cmdArgs);
    }
    #endregion

    #region Private Methods
    private void DelaySpeak()
    {

        Speak("Welcome To Anything World");
    }

    #endregion








}