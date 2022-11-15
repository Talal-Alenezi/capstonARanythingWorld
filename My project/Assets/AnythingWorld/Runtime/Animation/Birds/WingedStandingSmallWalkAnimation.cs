using System.Collections.Generic;
using UnityEngine;

namespace AnythingWorld.Animation
{
    public class WingedStandingSmallWalkAnimation : AnythingAnimationController
    {
        #region Fields
        // Animation settings
        public float bodyShakeMovementSpeed = 1;
        public float bodyShakeMovementFrequency = 0.4f;
        public float rightFrontLegMovementSpeed = 2;
        public float leftFrontLegMovementSpeed = 2;
        public float rightFrontLegMovementRadius = 0.5f;
        public float leftFrontLegMovementRadius = 0.5f;
        public float rightFrontLegStartDegree = 180;
        public float leftFrontLegStartDegree = 0;
        public float tailTurnSpeed = 1;
        public float tailTurnFrequency = 70;
        public float headTurnSpeed = 1;
        public float headTurnMaxAngle = 10;

        public string Descriptor { get; set; } = "Walk";
        #endregion


        //Autos
        public override void InitializeAnimationParameters()
        {
            FetchAnimationSettings("winged_standing_small__walk", "walk");

            // Initialize list of parameters used in this animation script
            parametersList = new List<Parameter>()
        {
            new Parameter("body", "bodyShakeMovementSpeed","AWBodyShakeMovement", "MoveSpeed",bodyShakeMovementSpeed),
            new Parameter("body", "bodyShakeMovementFrequency","AWBodyShakeMovement","Frequency",bodyShakeMovementFrequency),
            new Parameter("feet_right", "rightFrontLegMovementSpeed","AWFeetMovement", "StepSpeed",rightFrontLegMovementSpeed),
            new Parameter("feet_right", "rightFrontLegStartDegree","AWFeetMovement","StepDegree",rightFrontLegStartDegree),
            new Parameter("feet_right", "rightFrontLegMovementRadius","AWFeetMovement","StepRadius",rightFrontLegMovementRadius),
            new Parameter("feet_left", "leftFrontLegMovementSpeed","AWFeetMovement","StepSpeed",leftFrontLegMovementSpeed),
            new Parameter("feet_left", "leftFrontLegStartDegree","AWFeetMovement","StepDegree",leftFrontLegStartDegree),
            new Parameter("feet_left", "leftFrontLegMovementRadius","AWFeetMovement","StepRadius",leftFrontLegMovementRadius),
            new Parameter("tail", "tailTurnSpeed","AWTurnTail","TurnSpeed",tailTurnSpeed),
            new Parameter("tail", "tailTurnFrequency","AWTurnTail","Frequency",tailTurnFrequency),
            new Parameter("head_holder", "headTurnSpeed","AWTurnHead","TurnSpeed",headTurnSpeed),
            new Parameter("head_holder", "headTurnMaxAngle","AWTurnHead","MaxRotation",headTurnMaxAngle)
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
            var modifiedParameters = paramControl.CheckParameters(new List<(string, float)>() { ("bodyShakeMovementSpeed",bodyShakeMovementSpeed),
            ("bodyShakeMovementFrequency",bodyShakeMovementFrequency),("rightFrontLegMovementSpeed",rightFrontLegMovementSpeed),
            ("rightFrontLegStartDegree",rightFrontLegStartDegree),("rightFrontLegMovementRadius",rightFrontLegMovementRadius),
            ("leftFrontLegMovementSpeed",leftFrontLegMovementSpeed),("leftFrontLegStartDegree",leftFrontLegStartDegree),
            ("leftFrontLegMovementRadius",leftFrontLegMovementRadius), ("tailTurnSpeed",tailTurnSpeed),
            ("tailTurnFrequency",tailTurnFrequency),("headTurnSpeed",headTurnSpeed),
            ("headTurnMaxAngle",headTurnMaxAngle)});

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
        }

        /// <summary>
        /// Method used for updating the size of feet movement in the animation
        /// </summary>
        /// <param name="scale"></param>
        public override void UpdateMovementSizeScale(float scale)
        {
            rightFrontLegMovementRadius *= scale;
            leftFrontLegMovementRadius *= scale;
        }
    }
}

