using System.Collections.Generic;
using UnityEngine;

namespace AnythingWorld.Animation
{
    /// <summary>
    /// Class responsible for monitoring changes in parameters and extracting parameters that have been modified.
    /// </summary>
    public class ParameterController
    {
        /// <summary>
        /// List of all the parameters for particular behaviour
        /// </summary>
        Dictionary<string, Parameter> _parameters = new Dictionary<string, Parameter>();

        /// <summary>
        /// Constructor of the class ParameterController taking a list of parameters for the particular behaviour
        /// </summary>
        /// <param name="parameters"></param>
        public ParameterController(List<Parameter> parameters)
        {
            foreach (var param in parameters)
            {
                _parameters.Add(param.ParameterName, param);
            }
        }

        /// <summary>
        /// Method responsible for checking whether given parameters were modified. It returns a list of modified parameters with new values.
        /// </summary>
        /// <param name="newParams"></param>
        /// <returns></returns>
        public List<Parameter> CheckParameters(List<Parameter> newParams)
        {
            var modified = new List<Parameter>();
            for (var i = 0; i < newParams.Count; i++)
            {
                if (_parameters[newParams[i].ParameterName].Value != newParams[i].Value)
                {
                    modified.Add(newParams[i]);
                    _parameters[newParams[i].ParameterName].Value = newParams[i].Value;
                }
            }

            return modified;
        }

        /// <summary>
        /// Method responsible for checking whether given parameters were modified. It returns a list of modified parameters with new values.
        /// </summary>
        /// <param name="newParams"></param>
        /// <returns></returns>
        public List<Parameter> CheckParameters(List<(string, float, Vector3)> newParams)
        {
            var modified = new List<Parameter>();
            for (var i = 0; i < newParams.Count; i++)
            {
                if (_parameters[newParams[i].Item1].Value != newParams[i].Item2)
                {
                    _parameters[newParams[i].Item1].Value = newParams[i].Item2;
                    modified.Add(_parameters[newParams[i].Item1]);
                }
                else if (_parameters[newParams[i].Item1].Vector3Value != newParams[i].Item3)
                {
                    _parameters[newParams[i].Item1].Vector3Value = newParams[i].Item3;
                    modified.Add(_parameters[newParams[i].Item1]);
                }
            }

            return modified;
        }

        public List<Parameter> CheckParameters(List<(string, float)> newParams)
        {
            var modified = new List<Parameter>();
            for (var i = 0; i < newParams.Count; i++)
            {
                if (_parameters[newParams[i].Item1].Value != newParams[i].Item2)
                {
                    _parameters[newParams[i].Item1].Value = newParams[i].Item2;
                    modified.Add(_parameters[newParams[i].Item1]);
                }
            }

            return modified;
        }
    }

}
