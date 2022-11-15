using System.Collections.Generic;
using UnityEngine;


namespace AnythingWorld.Animation
{
    /// <summary>
    /// Animation controller for the biped walk animation.
    /// </summary>
    public class BipedWalkAnimation : AnythingAnimationController
    {
        // Behaviour settings
        public float bodyShakeMovementSpeed = 1;
        public float bodyShakeMovementFrequency = 0.4f;
        public float rightLegMovementSpeed = 1;
        public float leftLegMovementSpeed = 1;
        public float rightLegMovementRadius = 1;
        public float leftLegMovementRadius = 1;
        public float rightLegStartDegree = 180;
        public float leftLegStartDegree = 0;

        public override void InitializeAnimationParameters()
        {
            // Obtain settings for this particular animation
            FetchAnimationSettings("biped", "walk");

            // Initialize list of parameters used in this behaviour script
            parametersList = new List<Parameter>()
        {
            new Parameter("body", "bodyShakeMovementSpeed","AWBodyShakeMovement", "MoveSpeed",bodyShakeMovementSpeed),
            new Parameter("body", "bodyShakeMovementFrequency","AWBodyShakeMovement","Frequency",bodyShakeMovementFrequency),
            new Parameter("foot_right", "rightLegMovementSpeed","AWFeetMovement","StepSpeed",rightLegMovementSpeed),
            new Parameter("foot_right", "rightLegStartDegree","AWFeetMovement","StepDegree",rightLegStartDegree),
            new Parameter("foot_right", "rightLegMovementRadius","AWFeetMovement","StepRadius",rightLegMovementRadius),
            new Parameter("foot_left", "leftLegMovementSpeed","AWFeetMovement","StepSpeed",leftLegMovementSpeed),
            new Parameter("foot_left", "leftLegStartDegree","AWFeetMovement","StepDegree",leftLegStartDegree),
            new Parameter("foot_left", "leftLegMovementRadius","AWFeetMovement","StepRadius",leftLegMovementRadius)
        };
            // Initialize paramControl and _prefabToScript variables
            paramControl = new ParameterController(parametersList);
        }

        /// <summary>
        /// Method used for updating the speed of feet movement in the animation
        /// </summary>
        /// <param name="speed"></param>
        public override void SetMovementSpeed(float speed)
        {
            leftLegMovementSpeed = speed;
            rightLegMovementSpeed = speed;

        }

        /// <summary>
        /// Method used for updating the size of feet movement in the animation
        /// </summary>
        /// <param name="scale"></param>
        public override void UpdateMovementSizeScale(float scale)
        {
            rightLegMovementRadius *= scale;
            leftLegMovementRadius *= scale;
        }

        /// <summary>
        /// Method used for updating all parameters for the behaviour
        /// </summary>
        protected override void UpdateParameters()
        {
            // Check which parameters were modified
            var modifiedParameters = paramControl.CheckParameters(new List<(string, float, Vector3)>() {  ("bodyShakeMovementSpeed",bodyShakeMovementSpeed,new Vector3()),
            ("bodyShakeMovementFrequency",bodyShakeMovementFrequency,new Vector3()),("leftLegMovementRadius",leftLegMovementRadius,new Vector3()),
            ("leftLegStartDegree",leftLegStartDegree,new Vector3()),("leftLegMovementSpeed",leftLegMovementSpeed,new Vector3()),
            ("rightLegMovementRadius",rightLegMovementRadius,new Vector3()),("rightLegStartDegree",rightLegStartDegree,new Vector3()),
            ("rightLegMovementSpeed",rightLegMovementSpeed,new Vector3())});

            // Update parmeters value in the proper script
            foreach (var param in modifiedParameters)
            {
                partNameToScript[(param.PrefabPart, param.ScriptName)].ModifyParameter(param);
            }
        }

    }

}
