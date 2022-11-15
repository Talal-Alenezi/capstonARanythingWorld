using System.Collections.Generic;
using UnityEngine;

namespace AnythingWorld.Animation
{
    public class QuadrupedWalkAnimation : AnythingAnimationController
    {
        // Animation settings
        public float bodyShakeMovementSpeed = 1;
        public float bodyShakeMovementFrequency = 0.3f;
        public float rightFrontLegMovementSpeed = 1.5f;
        public float leftFrontLegMovementSpeed = 1.5f;
        public float rightHindLegMovementSpeed = 1.5f;
        public float leftHindLegMovementSpeed = 1.5f;
        public float rightFrontLegMovementRadius = 1;
        public float leftFrontLegMovementRadius = 1;
        public float rightHindLegMovementRadius = 1;
        public float leftHindLegMovementRadius = 1;
        public float rightFrontLegStartDegree = 90;
        public float leftFrontLegStartDegree = 270;
        public float rightHindLegStartDegree = 180;
        public float leftHindLegStartDegree = 0;
        public string Descriptor { get; set; } = "Walk";

        public bool animationReadyToUpdate = false;
        public override void InitializeAnimationParameters()
        {
            FetchAnimationSettings("quadruped", "walk");

            // Initialize list of parameters used in this animation script
            parametersList = new List<Parameter>()
        {
            new Parameter("body", "bodyShakeMovementSpeed","AWBodyShakeMovement", "MoveSpeed",bodyShakeMovementSpeed),
            new Parameter("body", "bodyShakeMovementFrequency","AWBodyShakeMovement","Frequency",bodyShakeMovementFrequency),
            new Parameter("foot_front_right", "rightFrontLegMovementSpeed","AWFeetMovement", "StepSpeed",rightFrontLegMovementSpeed),
            new Parameter("foot_front_right", "rightFrontLegStartDegree","AWFeetMovement","StepDegree",rightFrontLegStartDegree),
            new Parameter("foot_front_right", "rightFrontLegMovementRadius","AWFeetMovement","StepRadius",rightFrontLegMovementRadius),
            new Parameter("foot_front_left", "leftFrontLegMovementSpeed","AWFeetMovement","StepSpeed",leftFrontLegMovementSpeed),
            new Parameter("foot_front_left", "leftFrontLegStartDegree","AWFeetMovement","StepDegree",leftFrontLegStartDegree),
            new Parameter("foot_front_left", "leftFrontLegMovementRadius","AWFeetMovement","StepRadius",leftFrontLegMovementRadius),
            new Parameter("foot_hind_right", "rightHindLegMovementSpeed","AWFeetMovement","StepSpeed",rightHindLegMovementSpeed),
            new Parameter("foot_hind_right", "rightHindLegStartDegree","AWFeetMovement","StepDegree",rightHindLegStartDegree),
            new Parameter("foot_hind_right", "rightHindLegMovementRadius","AWFeetMovement","StepRadius",rightHindLegMovementRadius),
            new Parameter("foot_hind_left", "leftHindLegMovementSpeed","AWFeetMovement","StepSpeed",leftHindLegMovementSpeed),
            new Parameter("foot_hind_left", "leftHindLegStartDegree","AWFeetMovement","StepDegree",leftHindLegStartDegree),
            new Parameter("foot_hind_left", "leftHindLegMovementRadius","AWFeetMovement","StepRadius",leftHindLegMovementRadius)
        };

            // Initialize paramControl and _prefabToScript variables
            paramControl = new ParameterController(parametersList);
        }
        /// <summary>
        /// Method used for updating all parameters for the animation
        /// </summary>
        protected override void UpdateParameters()
        {

            //SetMovementSpeed(speedScalar);
            // Check which parameters were modified

            var modifiedParameters = paramControl.CheckParameters(new List<(string, float)>() { ("bodyShakeMovementSpeed",bodyShakeMovementSpeed),
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
                try
                {
                    partNameToScript[(param.PrefabPart, param.ScriptName)].ModifyParameter(param);
                }
                catch (KeyNotFoundException e)
                {
                    Debug.LogException(e);
                    Debug.LogWarning($"{param.PrefabPart}, {param.ScriptName}");
                }

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
