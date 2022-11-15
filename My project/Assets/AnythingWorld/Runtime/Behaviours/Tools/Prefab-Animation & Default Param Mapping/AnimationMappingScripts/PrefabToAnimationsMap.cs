using System;
using System.Collections.Generic;
using UnityEngine;


namespace AnythingWorld.Animation
{
    /// <summary>
    /// Class responsible for storing list of AnimationScriptEntry for each type of prefab.
    /// </summary>
    [Serializable]
    public class PrefabToAnimationsMap
    {
        /// <summary>
        /// Name of prefab type.
        /// </summary>
        [SerializeField]
        public string Key;

        /// <summary>
        /// List of animation scripts and keys.
        /// </summary>
        [SerializeField]
        public List<AnimationTypeToScript> Value;

        public override bool Equals(object obj)
        {
            return Key.Equals(((PrefabToAnimationsMap)obj).Key);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}

