using System;
using UnityEngine;

namespace AnythingWorld.Animation
{
    /// <summary>
    /// Class responsible for storing name of animation (key) and name of animationcontroller script (value).
    /// </summary>
    [Serializable]
    public class AnimationTypeToScript
    {
        /// <summary>
        /// Name of animation.
        /// </summary>
        [SerializeField]
        public string Key;

        /// <summary>
        /// Name of animation script.
        /// </summary>
        [SerializeField]
        public string Value;

        public override bool Equals(object obj)
        {
            return Key.Equals(((AnimationTypeToScript)obj).Key);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}
