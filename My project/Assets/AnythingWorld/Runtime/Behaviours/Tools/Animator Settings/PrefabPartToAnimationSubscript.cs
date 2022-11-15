using System;
using System.Collections.Generic;
using UnityEngine;


namespace AnythingWorld.Animation
{
    /// <summary>
    /// Class storing list of subscripts attached to a certain prefab part.
    /// </summary>
    [Serializable]
    public class PrefabPartToAnimationSubscript
    {
        /// <summary>
        /// Name of the prefab part
        /// </summary>
        [SerializeField]
        public string Key;
        /// <summary>
        /// List of the animation subscripts
        /// </summary>
        [SerializeField]
        public List<string> Value;


        public override bool Equals(object obj)
        {
            return Key.Equals(((PrefabPartToAnimationSubscript)obj).Key);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}
