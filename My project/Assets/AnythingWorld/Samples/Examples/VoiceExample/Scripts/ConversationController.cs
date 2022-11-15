using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AnythingWorld.Utilities;
using AnythingWorld.Speech;
using AnythingWorld.Chat;
namespace AnythingWorld.Examples
{

    [RequireComponent(typeof(BehaviourHandler))]
    public class ConversationController : MonoBehaviour
    {
        public TextBubble TextComputerBubble;
        public TextBubble TextUserBubble;
        public Canvas GameCanvas;
        public MicControl MicrophoneVisuals;
        private AnythingChat AWChat;
        private List<TextBubble> textBubbles;
        private TextBubble userTextBubbles;
        private GameObject speechBubble;
        private Text speechBubbleText;
        private CameraFollowUtility camFollow;
        private float letterPause = 0.04f;
        private BehaviourHandler behaviourHandler;
        public enum ConversationMode
        {
            Computer,
            Creature,
            None
        }
        private ConversationMode conversationMode;

        private void Start()
        {
            Init();
        }
        private void Init()
        {
            if (!gameObject.GetComponent<BehaviourHandler>())
            {
                behaviourHandler = gameObject.AddComponent<BehaviourHandler>();
            }
            else
            {
                behaviourHandler = gameObject.GetComponent<BehaviourHandler>();
            }
            conversationMode = ConversationMode.Computer;
            AWChat = new AnythingChat();
            textBubbles = new List<TextBubble>();
            speechBubble = GameObject.Find("SpeechBubble");
            camFollow = GameObject.FindObjectOfType<CameraFollowUtility>();
            speechBubbleText = speechBubble.transform.Find("SpeechText").GetComponent<Text>();
            ShowSpeechBubble(false);
            ComputerSpeak("Hello!");
        }
        public void SendTextMessage(string messageText)
        {
            UserSpeak(messageText);
        }

        private void MessageResponse(AWNaturalLanguageResponse response)
        {
            //Debug.Log($"MessageResponse : {response.text}");


            var respAction = response.queryResult.action;
            var responseNumber = 1;
            var respGenericQuantity = response.queryResult.parameters.fields.generic_quantity.stringValue;

            if (Int32.TryParse(response.queryResult.parameters.fields.number.numberValue, out var itemNum))
            {
                responseNumber = itemNum;
            }
            else
            {
                if (!String.IsNullOrEmpty(respGenericQuantity))
                {
                    responseNumber = UnityEngine.Random.Range(4, 12);
                }
            }

            var respZone = response.queryResult.parameters.fields.zone.stringValue;

            // process action
            if (respAction != "")
            {
                if(AnythingSettings.DebugEnabled) Debug.LogWarning("Have action : " + response.queryResult.action);

                switch (respAction)
                {
                    case "talk":
                        camFollow.TalkTo(response.queryResult.parameters.fields.talk_creature.stringValue);
                        conversationMode = ConversationMode.Creature;
                        break;
                    case "stoptalk":
                        // default:
                        camFollow.SwitchModes(CameraFollowUtility.CameraMode.Follow);
                        conversationMode = ConversationMode.Computer;
                        break;
                    case "chase":
                        //
                        //behaviourHandler.Chase(response.queryResult.parameters.fields.generic_creature_1.stringValue, response.queryResult.parameters.fields.generic_creature_2.stringValue);
                        StartCoroutine(behaviourHandler.ChaseCoroutine(response.queryResult.parameters.fields.generic_creature_1.stringValue, response.queryResult.parameters.fields.generic_creature_2.stringValue));
                        break;
                    case "ride":
                        StartCoroutine(behaviourHandler.RideCoroutine(response.queryResult.parameters.fields.generic_creature.stringValue));
                        break;
                    case "stopriding":
                        behaviourHandler.StopRiding();
                        break;
                    case "throw":
                        StartCoroutine(behaviourHandler.ThrowCoroutine(response.queryResult.parameters.fields.generic_creature_1.stringValue, response.queryResult.parameters.fields.generic_creature_2.stringValue, response.queryResult.parameters.fields.generic_quantity.stringValue));
                        break;
                    case "carry":
                        StartCoroutine(behaviourHandler.CarryCoroutine(response.queryResult.parameters.fields.generic_creature_1.stringValue, response.queryResult.parameters.fields.generic_creature_2.stringValue));
                        break;
                }
            }
            else if (response.@params.Count > 0)
            {
                if (response.@params[0].name != "null")
                {
                    var thingRequested = response.@params[0].name;
                    var objectCounter = 0;
                    while (objectCounter < responseNumber)
                    {
                        if (respZone.Length > 2)
                        {
                            var zoneObjPos = ZonePlacement.Instance.GetZoneObjectPosition(respZone);
                            var zoneObjScale = ZonePlacement.Instance.GetZoneObjectScale();
                            AnythingCreator.Instance.MakeObject(thingRequested, zoneObjPos, Quaternion.Euler(0, UnityEngine.Random.Range(0f, 360f), 0), zoneObjScale, false);
                        }
                        else
                        {
                            AnythingCreator.Instance.MakeObject(thingRequested);
                        }
                        objectCounter++;
                    }
                    camFollow.SwitchModes(CameraFollowUtility.CameraMode.Follow);
                    conversationMode = ConversationMode.Computer;
                }
            }
            ComputerSpeak(response.text);
        }

        public void StartRecording()
        {
            AnythingSpeech.Instance.StartListening(UpdateUserText, RecordingFinished);
            UserSpeak("...", false);
        }

        public void StopRecording()
        {
            AnythingSpeech.Instance.StopListening();
        }

        public void UpdateUserText(string newText)
        {
            userTextBubbles.ShowText(newText);
        }

        public void RecordingFinished(string finalText)
        {
            //Debug.Log($"Recording finished with final text: {finalText}");
            MicrophoneVisuals.SetMicActive(true);
            userTextBubbles.ShowText(finalText);
            GetResponse(userTextBubbles.GetText());
        }

        public void ComputerSpeak(string compText)
        {
            switch (conversationMode)
            {
                case ConversationMode.Computer:
                    var testText = MakeComputerText();
                    testText.ShowText(compText);
                    ShowSpeechBubble(false);
                    break;
                case ConversationMode.Creature:
                    ShowCreatureSpeech(compText);
                    break;
                default:
                    break;
            }

        }

        private void ShowCreatureSpeech(string crtText)
        {
            ShowSpeechBubble(true);
            StartCoroutine(TypeSpeech(crtText));
            AnythingSpeech.Instance.Speak(crtText, "Daniel");
        }

        private void ShowSpeechBubble(bool shouldShow)
        {
            speechBubble.SetActive(shouldShow);
        }

        private bool _runBubbleWait = false;
        private Coroutine _speechBubbleWait;

        private IEnumerator BubbleWaiting()
        {
            _runBubbleWait = true;

            yield return new WaitForSeconds(2f); // arbitrary wait

            speechBubbleText.text = "";
            var fullWaitText = "(⊙.⊙) ◔_◔ (ᴗ˳ᴗ)";
            var charInd = 0;

            while (_runBubbleWait)
            {
                if (charInd == fullWaitText.Length)
                {
                    speechBubbleText.text = "";
                    charInd = 0;
                }
                speechBubbleText.text += fullWaitText[charInd];
                charInd++;
                yield return new WaitForSeconds(0.18f);
            }
        }

        private IEnumerator TypeSpeech(string speech)
        {
            if (_speechBubbleWait != null)
                StopCoroutine(_speechBubbleWait);
            _runBubbleWait = false;

            speechBubbleText.text = "";

            var rowLimit = 14f;

            var parts = speech.Split(' ');
            var tmp = "";
            speechBubbleText.text = "";
            var sentenceCheck = "";
            var textHolder = "";
            for (var j = 0; j < parts.Length; j++)
            {
                tmp = textHolder;

                textHolder += parts[j] + " ";

                sentenceCheck += parts[j] + " ";

                if (sentenceCheck.Length > rowLimit)
                {
                    tmp += "\n";
                    tmp += parts[j] + " ";
                    textHolder = tmp;
                    sentenceCheck = "";
                }

            }

            //isSpelling = true;

            for (var i = 0; i < textHolder.Length; i++)
            {
                yield return new WaitForSeconds(letterPause);
                if (speechBubble != null)
                    speechBubbleText.text += textHolder[i];
            }

            //isSpelling = false;
        }

        public void UserSpeak(string userText, bool getResponse = true)
        {
            userTextBubbles = MakeUserText();
            userTextBubbles.ShowText(userText);
            if (getResponse)
                GetResponse(userText);

            if (conversationMode == ConversationMode.Creature)
                _speechBubbleWait = StartCoroutine(BubbleWaiting());
        }

        public void GetResponse(string userText)
        {
            if (AWChat == null) AWChat = new AnythingChat();
            AWChat.Talk(userText, MessageResponse, this);
        }

        public TextBubble MakeComputerText()
        {
            var conversationComputer = Instantiate(TextComputerBubble, TextComputerBubble.transform.position, Quaternion.identity) as TextBubble;
            conversationComputer.transform.SetParent(GameCanvas.transform, false);
            StartCoroutine(DelayAddComputerBubble(conversationComputer));
            return conversationComputer;
        }

        public TextBubble MakeUserText()
        {
            float userBubbleX = -500;
            var conversationUser = Instantiate(TextUserBubble, Vector3.zero, Quaternion.identity) as TextBubble;
            conversationUser.transform.SetParent(GameCanvas.transform, false);

            var userBubbleRTrans = conversationUser.GetComponent<RectTransform>();
            Vector3 rAncPos = userBubbleRTrans.anchoredPosition;
            rAncPos.x = userBubbleX;
            rAncPos.y = 160;
            userBubbleRTrans.anchoredPosition = rAncPos;

            AddTextBubble(conversationUser);
            return conversationUser;
        }


        private IEnumerator DelayAddComputerBubble(TextBubble compBubble)
        {
            yield return new WaitForSeconds(0.1f); // arbitrary! 
            AddTextBubble(compBubble);
            yield return new WaitForSeconds(0.5f); // arbitrary! 
            AnythingSpeech.Instance.Speak(compBubble.GetText());
        }

        private void AddTextBubble(TextBubble newBubble)
        {
            textBubbles.Insert(0, newBubble);
            for (var i = 0; i < textBubbles.Count; i++)
            {
                if (i < 2)
                {
                    textBubbles[i].MoveUp();
                }
                else
                {
                    textBubbles[i].MoveOut();
                    textBubbles.RemoveAt(2);
                }
            }
        }
    }

}
