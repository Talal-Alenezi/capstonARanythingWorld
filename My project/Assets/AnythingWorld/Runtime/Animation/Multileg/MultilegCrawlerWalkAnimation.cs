using System.Collections.Generic;
using UnityEngine;


namespace AnythingWorld.Animation
{
    public class MultilegCrawlerWalkAnimation : AnythingAnimationController
    {

        // Behaviour settings
        public float rightFrontLegMovementSpeed = 4;
        public float leftFrontLegMovementSpeed = 4;
        public float rightHindLegMovementSpeed = 4;
        public float leftHindLegMovementSpeed = 4;
        public float leftMiddleLegMovementSpeed = 4;
        public float rightMiddleLegMovementSpeed = 4;
        public float rightFrontLegMovementRadius = 0.6f;
        public float leftFrontLegMovementRadius = 0.6f;
        public float rightHindLegMovementRadius = 0.6f;
        public float leftHindLegMovementRadius = 0.6f;
        public float leftMiddleLegMovementRadius = 0.6f;
        public float rightMiddleLegMovementRadius = 0.6f;
        public float rightFrontLegStartDegree = 0;
        public float leftFrontLegStartDegree = 180;
        public float rightHindLegStartDegree = 0;
        public float leftHindLegStartDegree = 180;
        public float leftMiddleLegStartDegree = 0;
        public float rightMiddleLegStartDegree = 180;

        public override void InitializeAnimationParameters()
        {
            // Obtain settings for this particular animation
            FetchAnimationSettings("multileg_crawler", "walk");

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
            new Parameter("feet_middle_left", "leftMiddleLegMovementSpeed","AWFeetMovement","StepSpeed",leftMiddleLegMovementSpeed),
            new Parameter("feet_middle_left", "leftMiddleLegStartDegree","AWFeetMovement","StepDegree",leftMiddleLegStartDegree),
            new Parameter("feet_middle_left", "leftMiddleLegMovementRadius","AWFeetMovement","StepRadius",leftMiddleLegMovementRadius),
            new Parameter("feet_middle_right", "rightMiddleLegMovementSpeed","AWFeetMovement","StepSpeed",rightMiddleLegMovementSpeed),
            new Parameter("feet_middle_right", "rightMiddleLegStartDegree","AWFeetMovement","StepDegree",rightMiddleLegStartDegree),
            new Parameter("feet_middle_right", "rightMiddleLegMovementRadius","AWFeetMovement","StepRadius",rightMiddleLegMovementRadius)
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
            ("leftHindLegMovementRadius",leftHindLegMovementRadius), ("leftMiddleLegMovementSpeed",leftMiddleLegMovementSpeed), ("leftMiddleLegStartDegree",leftMiddleLegStartDegree),
            ("leftMiddleLegMovementRadius",leftMiddleLegMovementRadius), ("rightMiddleLegMovementSpeed",rightMiddleLegMovementSpeed), ("rightMiddleLegStartDegree",rightMiddleLegStartDegree),
            ("rightMiddleLegMovementRadius",rightMiddleLegMovementRadius)});

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
            rightMiddleLegMovementSpeed = speed;
            leftMiddleLegMovementSpeed = speed;
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
            rightMiddleLegMovementRadius *= scale;
            leftMiddleLegMovementRadius *= scale;
        }
    }
}

