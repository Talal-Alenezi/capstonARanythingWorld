using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnythingWorld.Habitat
{
    public static class HabitatMap
    {
        private static SortedDictionary<string, HabitatDescription> habitatDictionary;
        private static string[] habitatNameArray;
        private static string[] habitatDescriptionArray;
        private static void VerifyDictionarySetup()
        {
            if (habitatDictionary == null)
            {
                RefreshHabitatDictionary();
            }
        }
        public static void RefreshHabitatDictionary()
        {
            habitatDictionary = new SortedDictionary<string, HabitatDescription>();
            var dictionary = Resources.Load<HabitatDictionary>("Settings/AnythingHabitatDictionary");
            foreach(var entry in dictionary.habitats)
            {
                habitatDictionary.Add(entry.name, new HabitatDescription(entry.name, entry.description));
            }

        }
        private static string[] GenerateHabitatDescriptionArray()
        {
            VerifyDictionarySetup();
            var descriptions = new List<string>();
            foreach (var val in HabitatDictionary.Values)
            {
                descriptions.Add(val.Description);
            }
            habitatDescriptionArray = descriptions.ToArray();
            return descriptions.ToArray();

        }
        private static string[] GenerateHabitatNameArray()
        {
            VerifyDictionarySetup();
            var names = new List<string>();
            foreach (var val in HabitatDictionary.Values)
            {
                names.Add(val.Name);
            }
            habitatNameArray = names.ToArray();
            return names.ToArray();
        }
        public static SortedDictionary<string, HabitatDescription> HabitatDictionary
        {
            get
            {
                VerifyDictionarySetup();
                return GetHabitats();
            }
        }
        public static string GetHabitatAtIndex(int index)
        {
            VerifyDictionarySetup();
            if (index < habitatDictionary.Count)
            {
                return habitatDictionary.ElementAt(index).Key;
            }
            return null;

        }
        public static SortedDictionary<string, HabitatDescription> GetHabitats()
        {
            VerifyDictionarySetup();
            return habitatDictionary;
        }
        public static string[] HabitatNameArray
        {
            get
            {
                if (habitatNameArray == null) return GenerateHabitatNameArray();
                return habitatNameArray;
            }
        }
        public static string[] HabitatDesriptionArray
        {
            get
            {
                if (habitatNameArray == null) return GenerateHabitatNameArray();
                return habitatDescriptionArray;
            }
        }
    }

    public struct HabitatDescription
    {
        public string Name;
        public string Description;

        public HabitatDescription(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
