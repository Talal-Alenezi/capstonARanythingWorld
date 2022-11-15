using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AnythingWorld.Examples
{
    public class MicControl : MonoBehaviour
    {
        public Image Speaking;
        public GameObject InputCanvas;

        public ConversationController ConvoController;

        private bool isActive = false;

        private ThinkingAnimation thinkingAnimation;
        private SpeakingAnimation speakingAnimation;

        private float thinkStartTime;

        private Image micImage;
        private Button micButton;
        private Image speakButtonImage;
        private Button speakButton;


        void Start()
        {
            micImage = GetComponent<Image>();
            micButton = GetComponent<Button>();

            thinkingAnimation = GameObject.FindObjectOfType<ThinkingAnimation>();
            speakingAnimation = GameObject.FindObjectOfType<SpeakingAnimation>();

            speakButtonImage = speakingAnimation.gameObject.GetComponent<Image>();
            speakButton = speakingAnimation.gameObject.GetComponent<Button>();

            MakeInitialInactive();
        }

        public void MakeInitialInactive()
        {
            Speaking.gameObject.SetActive(false);
            InputCanvas.SetActive(false);
        }

        public void ToggleTextInput()
        {
            var newState = !InputCanvas.activeSelf;
            var oldState = InputCanvas.activeSelf;
            InputCanvas.SetActive(newState);
            speakingAnimation.gameObject.SetActive(false);
            thinkingAnimation.gameObject.SetActive(false);
            micButton.enabled = micImage.enabled = oldState;
        }

        public void MicPress()
        {
            UIBlocker.Instance.CanvasOneBlocked = true;
            ToggleMicActivation(false);

            PerformMicPress();
        }

        public void OnApplicationPause(bool isPaused)
        {
            if (isPaused)
            {
                ToggleMicActivation(true);
            }
        }

        public void ThinkingPress()
        {
            UIBlocker.Instance.CanvasOneBlocked = true;
            SpeakingPress();
            StartCoroutine(WaitToStopThinking());
        }
  

        private bool _speakingExit = false;

        public void SpeakingPress()
        {
            UIBlocker.Instance.CanvasOneBlocked = true;

            if (_speakingExit == true)
                return;

            _speakingExit = true;

            StartCoroutine(DelayedSpeakingPressResponse());
        }

        private IEnumerator DelayedSpeakingPressResponse()
        {
            yield return new WaitForSeconds(0.2f);
            ConvoController.StopRecording();
            yield return new WaitForSeconds(0.2f);
            ToggleMicActivation(true);
            _speakingExit = false;
        }

        private void PerformMicPress()
        {


            if (isActive)
            {
                ConvoController.StartRecording();
            }
            else
            {
                ConvoController.StopRecording();
            }
        }

        public void MicToThink()
        {
            InputCanvas.SetActive(false);
            thinkStartTime = Time.time;
            micButton.enabled = micImage.enabled = false;
            isActive = false;
            speakingAnimation.StopSpeaking();
            Speaking.gameObject.SetActive(false);

            speakButton.enabled = speakButtonImage.enabled = false;

            thinkingAnimation.StartThinking();

        }

        private void ToggleMicActivation(bool currentActivationStatus)
        {
            var oppositeStatus = currentActivationStatus ? false : true;
            Speaking.gameObject.SetActive(oppositeStatus);
            //Set mic button texture to activatation
            micButton.enabled = micImage.enabled = currentActivationStatus;
            isActive = oppositeStatus;

            if (isActive)
            {
                speakingAnimation.StartSpeaking();
                speakButton.enabled = speakButtonImage.enabled = true;
            }
        }

        public void SetMicActive(bool isActive)
        {

            if (isActive)
            {
                StartCoroutine(WaitToStopThinking());
            }
            else
            {
                Speaking.gameObject.SetActive(true);
                speakingAnimation.StartSpeaking();
                speakButton.enabled = speakButtonImage.enabled = true;
                InputCanvas.SetActive(false);
                micButton.enabled = micImage.enabled = false;
                this.isActive = isActive;
            }
        }

        private IEnumerator WaitToStopThinking()
        {
            var timeThinking = Time.time - thinkStartTime;
            var timeRound = Mathf.Ceil(timeThinking);
            var waitTime = timeRound - timeThinking;

            yield return new WaitForSeconds(waitTime);

            thinkingAnimation.StopThinking();
            speakButton.enabled = speakButtonImage.enabled = false;

            Speaking.gameObject.SetActive(false);

            micButton.enabled = micImage.enabled = true;
            isActive = true;

        }
    }
}

