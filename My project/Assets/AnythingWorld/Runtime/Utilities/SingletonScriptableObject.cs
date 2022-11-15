using System.Linq;
using UnityEngine;
using AnythingWorld;
namespace AnythingWorld.Utilities
{
    // There must be an in scene reference to a scriptable object or the Resources.FindAllObjectsOfType<T>()
    // that the singleton scriptable object pattern relies on will not work.
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        //static T instance = null;
        public static T Instance
        {
            get
            {
                return Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
            }
        }
    }
}
