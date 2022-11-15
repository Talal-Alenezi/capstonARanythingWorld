using System.Collections.Generic;
using UnityEngine;


namespace AnythingWorld.Animation
{
    public class QuadrupedFatWalkAnimation : AnythingAnimationController
    {
        // Behaviour settings
        public float bodyShakeMovementSpeed = 1;
        public float bodyShakeMovementFrequency = 0.6f;
        public float rightFrontLegMovementSpeed = 1.5f;
        public float leftFrontLegMovementSpeed = 1.5f;
        public float rightHindLegMovementSpeed = 1.5f;
        public float leftHindLegMovementSpeed = 1.5f;
        public float rightFrontLegMovementRadius = 0.5f;
        public float leftFrontLegMovementRadius = 0.5f;
        public float rightHindLegMovementRadius = 0.5f;
        public float leftHindLegMovementRadius = 0.5f;
        public float rightFrontLegStartDegree = 90;
        public float leftFrontLegStartDegree = 270;
        public float rightHindLegStartDegree = 180;
        public float leftHindLegStartDegree = 0;

        public override void InitializeAnimationParameters()
        {
            // Obtain settings for this particular animation
            FetchAnimationSettings("quadruped_fat__walk", "walk");

            // Initialize list of parameters used in this behaviour script
            parametersList = new List<Parameter>()
        {
            new Parameter("body", "bodyShakeMovementSpeed","AWBodyShakeMovement", "MoveSpeed",bodyShakeMovementSpeed),
            new Parameter("body", "bodyShakeMovementFrequency","AWBodyShakeMovement","Frequency",bodyShakeMovementFrequency),
            new Parameter("leg_front_right", "rightFrontLegMovementSpeed","AWFeetMovement", "StepSpeed",rightFrontLegMovementSpeed),
            new Parameter("leg_front_right", "rightFrontLegStartDegree","AWFeetMovement","StepDegree",rightFrontLegStartDegree),
            new Parameter("leg_front_right", "rightFrontLegMovementRadius","AWFeetMovement","StepRadius",rightFrontLegMovementRadius),
            new Parameter("leg_front_left", "leftFrontLegMovementSpeed","AWFeetMovement","StepSpeed",leftFrontLegMovementSpeed),
            new Parameter("leg_front_left", "leftFrontLegStartDegree","AWFeetMovement","StepDegree",leftFrontLegStartDegree),
            new Parameter("leg_front_left", "leftFrontLegMovementRadius","AWFeetMovement","StepRadius",leftFrontLegMovementRadius),
            new Parameter("leg_hind_right", "rightHindLegMovementSpeed","AWFeetMovement","StepSpeed",rightHindLegMovementSpeed),
            new Parameter("leg_hind_right", "rightHindLegStartDegree","AWFeetMovement","StepDegree",rightHindLegStartDegree),
            new Parameter("leg_hind_right", "rightHindLegMovementRadius","AWFeetMovement","StepRadius",rightHindLegMovementRadius),
            new Parameter("leg_hind_left", "leftHindLegMovementSpeed","AWFeetMovement","StepSpeed",leftHindLegMovementSpeed),
            new Parameter("leg_hind_left", "leftHindLegStartDegree","AWFeetMovement","StepDegree",leftHindLegStartDegree),
            new Parameter("leg_hind_left", "leftHindLegMovementRadius","AWFeetMovement","StepRadius",leftHindLegMovementRadius)
        };


            // Initialize paramControl and _prefabToScript variables
            paramControl = new ParameterController(parametersList);
        }

        /// <summary>
        /// Method used for updating all parameters for the behaviour
        /// </summary>
        protected override void UpdateParameters()
        {
            // Check which parameters were modified
            var modifiedParameters = paramControl.CheckParameters(new List<(string, float)>() {  ("bodyShakeMovementSpeed",bodyShakeMovementSpeed),
            ("bodyShakeMovementFrequency",bodyShakeMovementFrequency),("rightFrontLegMovementSpeed",rightFrontLegMovementSpeed),
            ("rightFrontLegStartDegree",rightFrontLegStartDegree),("rightFrontLegMovementRadius",rightFrontLegMovementRadius),
            ("leftFrontLegMovementSpeed",leftFrontLegMovementSpeed),("leftFrontLegStartDegree",leftFrontLegStartDegree),
            ("leftFrontLegMovementRadius",leftFrontLegMovementRadius), ("rightHindLegMovementSpeed",rightHindLegMovementSpeed),
            ("rightHindLegStartDegree",rightHindLegStartDegree),("rightHindLegMovementRadius",rightHindLegMovementRadius),
            ("leftHindLegMovementSpeed",leftHindLegMovementSpeed), ("leftHindLegStartDegree",leftHindLegStartDegree),
            ("leftHindLegMovementRadius",leftHindLegMovementRadius)});

            // Update parmeters value in the proper script
            foreach (var param in modifiedParameters)
            {
                partNameToScript[(param.PrefabPart, param.ScriptName)].ModifyParameter(param);
            }
        }

       
        /// <summary>
        /// Method used for updating the speed of feet movement in the animation
        /// </summary>
        /// <param name="speed"></param>
        public override void SetMovementSpeed(float speed)
        {
            rightFrontLegMovementSpeed = speed;
            leftFrontLegMovementSpeed = speed;
            rightHindLegMovementSpeed = speed;
            leftHindLegMovementSpeed = speed;

        }


        /// <summary>
        /// Method used for updating the size of feet movement in the animation
        /// </summary>
        /// <param name="scale"></param>
        public override void UpdateMovementSizeScale(float scale)
        {
            rightFrontLegMovementRadius *= scale;
            leftFrontLegMovementRadius *= scale;
            rightHindLegMovementRadius *= scale;
            leftHindLegMovementRadius *= scale;
        }
    }
}
