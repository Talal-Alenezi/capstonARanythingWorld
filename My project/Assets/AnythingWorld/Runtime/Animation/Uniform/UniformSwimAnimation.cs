using System.Collections.Generic;
using UnityEngine;
namespace AnythingWorld.Animation
{
    public class UniformSwimAnimation : AnythingAnimationController
    {
        #region Fields
        // Shader default parameters
        public float WobbleDistance = 0.1f;
        public float WobbleSpeed = 3;
        public float WobbleFrequency = 0.5f;
        public float CreatureScale = 1;

        #endregion
        private void Reset()
        {
            ActivateShader();
            UpdateParameters();
        }
        private void OnValidate()
        {
            UpdateParameters();
           
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
            if (GetComponentInParent<AWObj>())
            {
                var data = GetComponentInParent<AWObj>().ObjectData;
                if (data.returnedGuid.Contains("octopus")) CreatureScale = 10;
                else
                {
                    CreatureScale = data.inputScale;

                }
            }
            if (TryGetComponent<FlockMember>(out var flock))
            {
                flock.maxSpeed = 0.3f;
                flock.minSpeed = 0.1f;
            }
            if (gameObject.GetComponentsInChildren<Renderer>().Length > 0)
            {
                var renderers = gameObject.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    foreach (var material in renderer.sharedMaterials)
                    {
                        material.SetInt("HorizontalMovement", 1);
                        //Debug.Log("updating wobble distance");
                        material.SetFloat("WobbleDistance", WobbleDistance * (material.GetInt("HorizontalMovement") == 0 ? renderer.bounds.size.z : renderer.bounds.size.x) / CreatureScale);
                        material.SetFloat("WobbleSpeed", WobbleSpeed);
                        material.SetFloat("WobbleFrequency", WobbleFrequency / (material.GetInt("HorizontalMovement") == 0 ? renderer.bounds.size.z : renderer.bounds.size.x));
                    }
                }
            }



        }

        protected override void ActivateShader()
        {
            if (gameObject.GetComponentsInChildren<Renderer>().Length > 0)
            {
                var renderers = gameObject.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    foreach (var material in renderer.sharedMaterials)
                    {
                        material.SetInt("HorizontalMovement", 1);
                        //Debug.Log("updating wobble distance");
                        material.SetFloat("WobbleDistance", WobbleDistance * (material.GetInt("HorizontalMovement") == 0 ? renderer.bounds.size.z : renderer.bounds.size.x) / CreatureScale);
                        material.SetFloat("WobbleSpeed", WobbleSpeed);
                        material.SetFloat("WobbleFrequency", WobbleFrequency / (material.GetInt("HorizontalMovement") == 0 ? renderer.bounds.size.z : renderer.bounds.size.x));
                    }
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

    }
}