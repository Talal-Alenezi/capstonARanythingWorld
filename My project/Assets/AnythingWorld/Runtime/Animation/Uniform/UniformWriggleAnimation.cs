using System.Collections.Generic;
using UnityEngine;

namespace AnythingWorld.Animation
{
    /// <summary>
    /// Animation script defining the wriggle animation for the "Uniform" prefab type.
    /// </summary>
    public class UniformWriggleAnimation : AnythingAnimationController
    {
        #region Fields
        /// Shader default parameters
        private float WobbleDistance = 4;
        private float WobbleSpeed = 3;
        #endregion

        public override void InitializeAnimationParameters()
        {

            FetchAnimationSettings("uniform__wriggle", "Move");

            // Initialize list of parameters used in this behaviour script
            parametersList = new List<Parameter>();

            // Initialize paramControl variable
            paramControl = new ParameterController(parametersList);
        }

        /// <summary>
        /// Method used for updating all parameters for the behaviour
        /// </summary>
        protected override void UpdateParameters()
        {
            // Check which parameters were modified
            var modifiedParameters = paramControl.CheckParameters(new List<(string, float, Vector3)>());

            // Update parmeters value in the proper script
            foreach (var param in modifiedParameters)
            {
                partNameToScript[(param.PrefabPart, param.ScriptName)].ModifyParameter(param);
            }
        }

        protected override void ActivateShader()
        {
            var shaders = gameObject.GetComponentsInChildren<Renderer>();
            foreach (var shader in shaders)
            {
                shader.sharedMaterial.SetFloat("WobbleDistance", WobbleDistance);
                shader.sharedMaterial.SetFloat("WobbleSpeed", WobbleSpeed);
            }
        }

        protected override void DeactivateShader()
        {
            var shaders = gameObject.GetComponentsInChildren<Renderer>();
            foreach (var shader in shaders)
            {
                shader.sharedMaterial.SetFloat("WobbleDistance", 0);
                shader.sharedMaterial.SetFloat("WobbleSpeed", 0);
            }
        }
    }
}

