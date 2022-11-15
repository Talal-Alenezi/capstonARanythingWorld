using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AnythingWorld.Habitat
{
    [CreateAssetMenu(fileName = "AnythingHabitatDictionary", menuName = "AnythingWorld/AnythingHabitatDictionary", order = 2)]
    public class HabitatDictionary : ScriptableObject
    {
        [SerializeField]
        public List<HabitatEntry> habitats = new List<HabitatEntry>();
    }
    [System.Serializable]
    public class HabitatEntry
    {
        [SerializeField]
        public string name = "";
        [SerializeField]
        public string description = "";
    }
}
