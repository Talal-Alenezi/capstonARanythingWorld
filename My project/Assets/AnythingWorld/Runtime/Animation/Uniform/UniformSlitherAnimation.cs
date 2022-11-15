using System.Collections.Generic;
using UnityEngine;
using AnythingWorld;

namespace AnythingWorld.Animation
{
    /// <summary>
    /// Animation controller for the slither animation for "uniform" type prefabs.
    /// </summary>
    public class UniformSlitherAnimation : AnythingAnimationController
    {
        #region Fields
        private float WobbleDistance = 15;
        private float WobbleSpeed = 5;
        /// ParameterController object used for monitoring parameters' changes
        ParameterController paramController;
        #endregion

        public override void InitializeAnimationParameters()
        {
            FetchAnimationSettings("uniform__slither", "Move");

            // Initialize list of parameters used in this behaviour script
            parametersList = new List<Parameter>();

            // Initialize paramControl variable
            paramController = new ParameterController(parametersList);
        }
       
        protected override void ActivateShader()
        {
            var shaders = gameObject.GetComponentsInChildren<Renderer>();
            foreach (var shader in shaders)
            {
                Debug.Log("Activating shader for uniform slither");
                shader.sharedMaterial.SetFloat("WobbleDistance", WobbleDistance*transform.parent.localScale.x);
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

        /// <summary>
        /// Method used for updating all parameters for the behaviour
        /// </summary>
        protected override void UpdateParameters()
        {
            // Check which parameters were modified
            var modifiedParameters = paramController.CheckParameters(new List<(string, float)>());

            // Update parmeters value in the proper script
            foreach (var param in modifiedParameters)
            {
                partNameToScript[(param.PrefabPart, param.ScriptName)].ModifyParameter(param);
            }
        }
    }

}
