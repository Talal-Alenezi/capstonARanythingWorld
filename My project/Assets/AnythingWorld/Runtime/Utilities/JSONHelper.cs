using System;
using UnityEngine;

namespace AnythingWorld.Utilities
{
    public class JSONHelper
    {
        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items = null;

        }
        public static T[] GetJSONArray<T>(string json)
        {
            var newJson = "{ \"Items\": " + json + "}";
            var wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);

            return wrapper.Items;
        }
        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            var wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        public static string ToJson<T>(T[] array)
        {
            var wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static T[] FromJson<T>(string json)
        {
            var wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

    }
}
