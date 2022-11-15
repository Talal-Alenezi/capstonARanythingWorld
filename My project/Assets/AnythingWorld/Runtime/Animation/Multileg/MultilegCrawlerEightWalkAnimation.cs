using System.Collections.Generic;
using UnityEngine;

namespace AnythingWorld.Animation
{
    public class MultilegCrawlerEightWalkAnimation : AnythingAnimationController
    {
        // Behaviour settings
        public float rightFrontLegMovementSpeed = 4;
        public float leftFrontLegMovementSpeed = 4;
        public float rightHindLegMovementSpeed = 4;
        public float leftHindLegMovementSpeed = 4;
        public float leftMiddleFrontLegMovementSpeed = 4;
        public float rightMiddleFrontLegMovementSpeed = 4;
        public float leftMiddleHindLegMovementSpeed = 4;
        public float rightMiddleHindLegMovementSpeed = 4;
        public float rightFrontLegMovementRadius = 0.3f;
        public float leftFrontLegMovementRadius = 0.3f;
        public float rightHindLegMovementRadius = 0.3f;
        public float leftHindLegMovementRadius = 0.3f;
        public float leftMiddleFrontLegMovementRadius = 0.3f;
        public float rightMiddleFrontLegMovementRadius = 0.3f;
        public float leftMiddleHindLegMovementRadius = 0.3f;
        public float rightMiddleHindLegMovementRadius = 0.3f;
        public float rightFrontLegStartDegree = 0;
        public float leftFrontLegStartDegree = 180;
        public float rightHindLegStartDegree = 180;
        public float leftHindLegStartDegree = 0;
        public float leftMiddleFrontLegStartDegree = 0;
        public float rightMiddleFrontLegStartDegree = 180;
        public float leftMiddleHindLegStartDegree = 180;
        public float rightMiddleHindLegStartDegree = 0;

        public override void InitializeAnimationParameters()
        {
            // Obtain settings for this particular animation
            FetchAnimationSettings("multileg_crawler_eight", "walk");

            // Initialize list of parameters used in this behaviour script
            parametersList = new List<Parameter>()
        {
            new Parameter("feet_front_right", "rightFrontLegMovementSpeed","AWFeetMovement", "StepSpeed",rightFrontLegMovementSpeed),
            new Parameter("feet_front_right", "rightFrontLegStartDegree","AWFeetMovement","StepDegree",rightFrontLegStartDegree),
            new Parameter("feet_front_right", "rightFrontLegMovementRadius","AWFeetMovement","StepRadius",rightFrontLegMovementRadius),
            new Parameter("feet_front_left", "leftFrontLegMovementSpeed","AWFeetMovement","StepSpeed",leftFrontLegMovementSpeed),
            new Parameter("feet_front_left", "leftFrontLegStartDegree","AWFeetMovement","StepDegree",leftFrontLegStartDegree),
            new Parameter("feet_front_left", "leftFrontLegMovementRadius","AWFeetMovement","StepRadius",leftFrontLegMovementRadius),
            new Parameter("feet_hind_right", "rightHindLegMovementSpeed","AWFeetMovement","StepSpeed",rightHindLegMovementSpeed),
            new Parameter("feet_hind_right", "rightHindLegStartDegree","AWFeetMovement","StepDegree",rightHindLegStartDegree),
            new Parameter("feet_hind_right", "rightHindLegMovementRadius","AWFeetMovement","StepRadius",rightHindLegMovementRadius),
            new Parameter("feet_hind_left", "leftHindLegMovementSpeed","AWFeetMovement","StepSpeed",leftHindLegMovementSpeed),
            new Parameter("feet_hind_left", "leftHindLegStartDegree","AWFeetMovement","StepDegree",leftHindLegStartDegree),
            new Parameter("feet_hind_left", "leftHindLegMovementRadius","AWFeetMovement","StepRadius",leftHindLegMovementRadius),
            new Parameter("feet_midfront_left", "leftMiddleFrontLegMovementSpeed","AWFeetMovement","StepSpeed",leftMiddleFrontLegMovementSpeed),
            new Parameter("feet_midfront_left", "leftMiddleFrontLegStartDegree","AWFeetMovement","StepDegree",leftMiddleFrontLegStartDegree),
            new Parameter("feet_midfront_left", "leftMiddleFrontLegMovementRadius","AWFeetMovement","StepRadius",leftMiddleFrontLegMovementRadius),
            new Parameter("feet_midfront_right", "rightMiddleFrontLegMovementSpeed","AWFeetMovement","StepSpeed",rightMiddleFrontLegMovementSpeed),
            new Parameter("feet_midfront_right", "rightMiddleFrontLegStartDegree","AWFeetMovement","StepDegree",rightMiddleFrontLegStartDegree),
            new Parameter("feet_midfront_right", "rightMiddleFrontLegMovementRadius","AWFeetMovement","StepRadius",rightMiddleFrontLegMovementRadius),
            new Parameter("feet_midhind_left", "leftMiddleHindLegMovementSpeed","AWFeetMovement","StepSpeed",leftMiddleHindLegMovementSpeed),
            new Parameter("feet_midhind_left", "leftMiddleHindLegStartDegree","AWFeetMovement","StepDegree",leftMiddleHindLegStartDegree),
            new Parameter("feet_midhind_left", "leftMiddleHindLegMovementRadius","AWFeetMovement","StepRadius",leftMiddleHindLegMovementRadius),
            new Parameter("feet_midhind_right", "rightMiddleHindLegMovementSpeed","AWFeetMovement","StepSpeed",rightMiddleHindLegMovementSpeed),
            new Parameter("feet_midhind_right", "rightMiddleHindLegStartDegree","AWFeetMovement","StepDegree",rightMiddleHindLegStartDegree),
            new Parameter("feet_midhind_right", "rightMiddleHindLegMovementRadius","AWFeetMovement","StepRadius",rightMiddleHindLegMovementRadius)
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
            var modifiedParameters = paramControl.CheckParameters(new List<(string, float)>() { ("rightFrontLegMovementSpeed",rightFrontLegMovementSpeed),
            ("rightFrontLegStartDegree",rightFrontLegStartDegree),("rightFrontLegMovementRadius",rightFrontLegMovementRadius),
            ("leftFrontLegMovementSpeed",leftFrontLegMovementSpeed),("leftFrontLegStartDegree",leftFrontLegStartDegree),
            ("leftFrontLegMovementRadius",leftFrontLegMovementRadius), ("rightHindLegMovementSpeed",rightHindLegMovementSpeed),
            ("rightHindLegStartDegree",rightHindLegStartDegree),("rightHindLegMovementRadius",rightHindLegMovementRadius),
            ("leftHindLegMovementSpeed",leftHindLegMovementSpeed), ("leftHindLegStartDegree",leftHindLegStartDegree),
            ("leftHindLegMovementRadius",leftHindLegMovementRadius), ("leftMiddleFrontLegMovementSpeed",leftMiddleFrontLegMovementSpeed), ("leftMiddleFrontLegStartDegree",leftMiddleFrontLegStartDegree),
            ("leftMiddleFrontLegMovementRadius",leftMiddleFrontLegMovementRadius), ("rightMiddleFrontLegMovementSpeed",rightMiddleFrontLegMovementSpeed),
            ("rightMiddleFrontLegStartDegree",rightMiddleFrontLegStartDegree), ("rightMiddleFrontLegMovementRadius",rightMiddleFrontLegMovementRadius),
            ("leftMiddleHindLegMovementSpeed",leftMiddleHindLegMovementSpeed), ("leftMiddleHindLegStartDegree",leftMiddleHindLegStartDegree),
            ("leftMiddleHindLegMovementRadius",leftMiddleHindLegMovementRadius), ("rightMiddleHindLegMovementSpeed",rightMiddleHindLegMovementSpeed),
            ("rightMiddleHindLegStartDegree",rightMiddleHindLegStartDegree), ("rightMiddleHindLegMovementRadius",rightMiddleHindLegMovementRadius)});

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
            rightMiddleHindLegMovementSpeed = speed;
            leftMiddleHindLegMovementSpeed = speed;
            rightMiddleFrontLegMovementSpeed = speed;
            leftMiddleFrontLegMovementSpeed = speed;
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
            rightMiddleHindLegMovementRadius *= scale;
            leftMiddleHindLegMovementRadius *= scale;
            rightMiddleFrontLegMovementRadius *= scale;
            leftMiddleFrontLegMovementRadius *= scale;
        }
    }

}
