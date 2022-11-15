using UnityEngine;

namespace AnythingWorld.Animation
{
    /// <summary>
    /// Class representing a single parameter of the behaviour subscript
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Name of the part of the prefab to which the script with this parameter is attached to 
        /// </summary>
        public string PrefabPart { get; }
        /// <summary>
        /// Name of the parameter
        /// </summary>
        public string ParameterName { get; }
        /// <summary>
        /// Name of the script which this parameter is a part of
        /// </summary>
        public string ScriptName { get; }
        /// <summary>
        /// Name of the parameter inside the script
        /// </summary>
        public string ParameterScriptName { get; }
        /// <summary>
        /// Value of the parameter
        /// </summary>
        public float Value { set; get; }
        public Vector3 Vector3Value { set; get; }

        /// <summary>
        /// Constructor for the Parameter class.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="name"></param>
        /// <param name="scriptname"></param>
        /// <param name="paramscriptname"></param>
        /// <param name="value"></param>
        public Parameter(string part, string name, string scriptname, string paramscriptname, float value, Vector3 vector3value = new Vector3()) // NB: was dynamic - GM
        {
            PrefabPart = part;
            ParameterName = name;
            ScriptName = scriptname;
            ParameterScriptName = paramscriptname;
            Value = value;
            Vector3Value = vector3value;
        }
    }
}

