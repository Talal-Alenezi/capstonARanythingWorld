using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif
using UnityEngine.Networking;


namespace AnythingWorld.Speech
{
    public class CreatureKeywords
    {
        private readonly bool DDEBUG = false;
        private readonly string objectListUrl = AnythingApiConfig.ApiUrlStem + "/names";
        [SerializeField]
        private List<string> creatures;

        public CreatureKeywords(MonoBehaviour routineRunner = null)
        {
            LoadCreatureCache(routineRunner);
        }
        public List<string> Creatures
        {
            get
            {
                return creatures;
            }
        }

        private List<string> _numberWords = new List<string>()
    {
        "zero",
        "one",
        "two",
        "three",
        "four",
        "five",
        "six",
        "seven",
        "eight",
        "nine",
        "ten",
        "some" // TODO: some = 11 for now :D - do this properly
        };

        private List<string> _zoneWords = new List<string>()
    {
        "background",
        "middleground",
        "foreground"
    };

        private List<string> _splitZoneWords = new List<string>()
    {
        "back ground",
        "middle ground",
        "fore ground"
    };

        public Tuple<int, int, string, string> ProcessUtterance(string utterance)
        {
            if(DDEBUG) Debug.Log("ProcessUtterance : " + utterance);
            if (utterance == null) return null;
            // replace split zone strings
            utterance = utterance.Replace(_splitZoneWords[0], _zoneWords[0]).Replace(_splitZoneWords[1], _zoneWords[1]).Replace(_splitZoneWords[2], _zoneWords[2]);
            var utteranceArray = utterance.Split(' ');
            var foundCreature = "";
            var foundAtIndice = -1;
            var quantity = 1;
            var zoneWord = "";

            for (var i = 0; i < utteranceArray.Length; i++)
            {
                var word = utteranceArray[i];
                var lastWord = "";
                if (word.Length > 2)
                {
                    if (i > 0)
                    {
                        lastWord = utteranceArray[i - 1];

                        if (_numberWords.Contains(lastWord.ToLower()))
                        {
                            if (lastWord == "some")
                            {
                                lastWord = UnityEngine.Random.Range(20, 50).ToString();
                            }
                            else
                            {
                                lastWord = _numberWords.IndexOf(lastWord.ToLower()).ToString();
                            }
                        }
                        if (_zoneWords.Contains(lastWord.ToLower()))
                        {
                            zoneWord = lastWord.ToLower();
                        }
                    }
                    int parsedInt;

                    var havePreferredCreature = IsPreferredCreature(word);


                    // TODO: list of forced gotchas here!
                    if (havePreferredCreature == "desert") havePreferredCreature = null;
                    if (havePreferredCreature == "jungle") havePreferredCreature = null;
                    if (havePreferredCreature == "wale") havePreferredCreature = "whale";
                    if (havePreferredCreature == "shock") havePreferredCreature = "shark";
                    if (havePreferredCreature == "shark") havePreferredCreature = "shark#0000";
                    if (havePreferredCreature != null)
                    {

                        //Debug.LogWarning("havePref : " + word + " : " + havePreferredCreature);


                        foundCreature = havePreferredCreature;
                        foundAtIndice = utterance.IndexOf(word) + word.Length;
                        if (int.TryParse(lastWord, out parsedInt))
                        {
                            quantity = parsedInt;
                        }
                        break;
                    }
                }
            }
            return new Tuple<int, int, string, string>(foundAtIndice, quantity, zoneWord, foundCreature);
        }

        public string IsPreferredCreature(string thingName)
        {

            string havePreferred = null;

            if (creatures == null)
                return havePreferred;

            if (creatures.Contains(thingName))
                havePreferred = thingName;

            // try plurals
            if (havePreferred == null && thingName[thingName.Length - 1] == 's')
            {
                var singularThing = thingName.Substring(0, thingName.Length - 1);
                if (creatures.Contains(singularThing))
                {
                    havePreferred = singularThing;
                }
            }

            return havePreferred;
        }

        private IEnumerator LoadCreatures()
        {

            var www = UnityWebRequest.Get(objectListUrl);
            yield return www.SendWebRequest();

            if (AnythingWorld.Utilities.CheckWebRequest.IsError(www))
            {
                if (AnythingSettings.Instance.showDebugMessages) Debug.LogError("Error requesting things!");
                yield break;
            }

            var result = www.downloadHandler.text;
            result.Trim(new Char[] { ' ', ']', '.' });
            result = result.Replace("\"", "");
            creatures = new List<string>(result.Split(','));
            creatures = creatures.ConvertAll(d => d.ToLower());
            creatures = creatures.ConvertAll(d => d.Split('#')[0]);


            creatures.Add("wale");
        }


        private void LoadCreatureCache(MonoBehaviour routineRunner = null)
        {
            if (creatures == null)
            {
#if UNITY_EDITOR
                EditorCoroutineUtility.StartCoroutine(LoadCreatures(), this);
#else
        if (routineRunner != null)
        {
            routineRunner.StartCoroutine(LoadCreatures());
        }
#endif
            }

        }
        private void RefreshCreatureCache(MonoBehaviour routineRunner = null)
        {
#if UNITY_EDITOR
            EditorCoroutineUtility.StartCoroutine(LoadCreatures(), this);
#else
        if (routineRunner != null)
        {
            routineRunner.StartCoroutine(LoadCreatures());
        }
#endif
        }
    }
}

