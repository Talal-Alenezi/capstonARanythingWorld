using System.Collections.Generic;
using UnityEngine;

namespace AnythingWorld.Animation
{
    /// <summary>
    /// Animation controller for slithering animation for "Uniform" type prefabs.
    /// </summary>
    public class UniformSlitherVerticalAnimation : AnythingAnimationController
    {
        #region Fields
        // Shader default parameters
        private float WobbleDistance = 5;
        private float WobbleSpeed = 5;

        #endregion
        private void Reset()
        {
            ActivateShader();
        }

        public override void InitializeAnimationParameters()
        {
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
            if (gameObject.GetComponentsInChildren<Renderer>().Length>0)
            {
                var shaders = gameObject.GetComponentsInChildren<Renderer>();
                foreach (var shader in shaders)
                {
                    //Debug.Log("updating wobble distance");
                    shader.sharedMaterial.SetFloat("WobbleDistance", WobbleDistance * transform.parent.localScale.x);
                    shader.sharedMaterial.SetFloat("WobbleSpeed", WobbleSpeed);
                }
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

        public override void SetMovementSpeed(float speed)
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateMovementSizeScale(float scale)
        {
            throw new System.NotImplementedException();
        }
    }

}
