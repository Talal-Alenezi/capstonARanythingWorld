using AnythingWorld.Habitat;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AnythingWorld.Speech
{
    public class HabitatKeywords
    {

        private List<string> _habitats;

        public List<string> Habitats
        {
            get
            {
                return _habitats;
            }
        }

        private readonly bool showDebugLog = false;
        public Tuple<int, string> ProcessUtterance(string utterance)
        {
            // Debug.Log($"Processutterance {utterance}");
            if (_habitats == null)
                PopulateHabitats();

            var words = utterance.Split(' ');
            var findHabitat = "none";
            var findInd = -1;
            
            for (var i = 0; i < words.Length; i++)
            {
                
                var word = words[i];
                if(word == "tundra" || word == "arctic" || word == "nice" || word =="ice")
                {
                    word = "icescape";
                }  
                Debug.Log(word);
                if (IsPreferredHabitat(word))
                {
                    findHabitat = word;
                    findInd = utterance.IndexOf(word) + word.Length;
                    break;
                }
            }


            if (showDebugLog) Debug.Log($"HabitatKeywords ProcessUtterance returning {findInd},{findHabitat}");
            return new Tuple<int, string>(findInd, findHabitat);
        }

        public bool IsPreferredHabitat(string thingName)
        {
            var havePreferred = false;
            if (_habitats.Contains(thingName))
                havePreferred = true;
            return havePreferred;
        }

        private void PopulateHabitats()
        {
            var habitatList = HabitatMap.GetHabitats();
            _habitats = new List<string>();
            foreach (var kvp in habitatList)
            {
                _habitats.Add(kvp.Key);
            }
        }


    }
}

