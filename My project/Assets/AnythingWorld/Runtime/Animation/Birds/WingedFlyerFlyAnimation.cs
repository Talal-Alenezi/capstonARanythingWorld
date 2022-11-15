using System.Collections.Generic;
using UnityEngine;


namespace AnythingWorld.Animation
{
    /// <summary>
    /// Animation controller for the WingedFlyer fly animation.
    /// </summary>
    public class WingedFlyerFlyAnimation : AnythingAnimationController
    {
        #region Fields
        // Animation settings
        public float rightWingMovementSpeed = 4;
        public Vector3 rightWingMovementRotateTo = new Vector3(10, -20, 0);
        public float leftWingMovementSpeed = 4;
        public Vector3 leftWingMovementRotateTo = new Vector3(10, -20, 0);
        public string Descriptor { get; set; } = "Fly";
        #endregion

        public override void InitializeAnimationParameters()
        {
            FetchAnimationSettings("winged_flyer__fly", "fly");

            // Initialize list of parameters used in this animation script
            parametersList = new List<Parameter>()
        {
            new Parameter("wing_right", "rightWingMovementSpeed","AnythingWingRotationAnimationComponent", "RotationSpeed",rightWingMovementSpeed),
            new Parameter("wing_right", "rightWingMovementRotateTo","AnythingWingRotationAnimationComponent","RotateTo",0,rightWingMovementRotateTo),
            new Parameter("wing_left", "leftWingMovementSpeed","AnythingWingRotationAnimationComponent","RotationSpeed",leftWingMovementSpeed),
            new Parameter("wing_left", "leftWingMovementRotateTo","AnythingWingRotationAnimationComponent","RotateTo",0,leftWingMovementRotateTo)
        };

            // Initialize paramControl and _prefabToScript variables
            paramControl = new ParameterController(parametersList);
        }

        /// <summary>
        /// Method used for updating all parameters for the animation
        /// </summary>
        protected override void UpdateParameters()
        {
            // Check which parameters were modified
            var modifiedParameters = paramControl.CheckParameters(new List<(string, float, Vector3)>() {
            ("rightWingMovementSpeed",rightWingMovementSpeed, new Vector3()),
            ("rightWingMovementRotateTo",0,rightWingMovementRotateTo),
            ("leftWingMovementSpeed",leftWingMovementSpeed,new Vector3()),
            ("leftWingMovementRotateTo",0,leftWingMovementRotateTo)
            });

            // Update parmeters value in the proper script
            foreach (var param in modifiedParameters)
            {
                partNameToScript[(param.PrefabPart, param.ScriptName)].ModifyParameter(param);
            }
        }
    }
}

