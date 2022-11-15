using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnythingWorld.Utilities
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// Returns true is component is same object as argument component. 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        public static bool IsSameAs(this UnityEngine.Component self, UnityEngine.Component component)
        {
            if (self == null) return false;
            if (component == null) return false;

            return self == component;
        }
        /// <summary>
        /// Returns true if component is different object to the argument component.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        public static bool IsNotSameAs(this UnityEngine.Component self, UnityEngine.Component component)
        {
            if (self == null) return false;
            if (component == null) return false;

            return self != component;
        }

    }
}

