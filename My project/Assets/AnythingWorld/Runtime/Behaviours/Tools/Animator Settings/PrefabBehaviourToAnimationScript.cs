using System;
using System.Collections.Generic;
using UnityEngine;


namespace AnythingWorld.Animation
{
    /// <summary>
    /// Class responsible for storing a list of subscripts and their connection to the prefab's part used by a particular animation applied to a particular prefab type.
    /// </summary>
    [Serializable]
    public class PrefabBehaviourToAnimationScript
    {
        /// <summary>
        /// A Tuple storing name of the prefab and name of the animator
        /// </summary>
        [SerializeField]
        public (string, string) Key;

        /// <summary>
        /// List of all the parts of the prefab with subscripts attached to them
        /// </summary>
        [SerializeField]
        public List<PrefabPartToAnimationSubscript> Value;

        public override bool Equals(object obj)
        {
            return Key.Equals(((PrefabBehaviourToAnimationScript)obj).Key);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }

}
