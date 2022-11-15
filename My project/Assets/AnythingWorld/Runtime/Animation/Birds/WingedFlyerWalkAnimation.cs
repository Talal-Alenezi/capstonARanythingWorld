using System.Collections.Generic;
using UnityEngine;


namespace AnythingWorld.Animation
{
    /// <summary>
    /// Animation controller for the WingedFlyer walk animation.
    /// </summary>
    public class WingedFlyerWalkAnimation : AnythingAnimationController
    {
        #region Fields
        // Animation settings
        public float bodyWobbleMovementSpeed = 4;
        public float bodyWobbleMaxAngle = 4;
        public float rightLegMovementSpeed = 3;
        public float leftLegMovementSpeed = 2.4f;
        public float rightWingMovementSpeed = 5;
        public float leftWingMovementSpeed = 4;
        public Vector3 rightLegMovementRotateTo = new Vector3(10, -20, 0);
        public Vector3 leftLegMovementRotateTo = new Vector3(10, 20, 0);
        public Vector3 rightWingMovementRotateTo = new Vector3(0, 0, -40);
        public Vector3 leftWingMovementRotateTo = new Vector3(0, 0, 40);
        public float headTurnSpeed = 3;
        public float headTurnMaxAngle = 30;
        public string Descriptor { get; set; } = "Walk";
        #endregion

        public override void InitializeAnimationParameters()
        {
            FetchAnimationSettings("winged_flyer__waddle", "walk");

            // Initialize list of parameters used in this animation script
            parametersList = new List<Parameter>()
        {
            new Parameter("body", "bodyWobbleMovementSpeed","AWWobble", "MoveSpeed",bodyWobbleMovementSpeed),
            new Parameter("body", "bodyWobbleMaxAngle","AWWobble","Frequency",bodyWobbleMaxAngle),
            new Parameter("wing_right", "rightWingMovementSpeed","AnythingWingRotationAnimationComponent", "RotationSpeed",rightWingMovementSpeed),
            new Parameter("wing_right", "rightWingMovementRotateTo","AnythingWingRotationAnimationComponent","RotateTo",0,rightWingMovementRotateTo),
            new Parameter("wing_left", "leftWingMovementSpeed","AnythingWingRotationAnimationComponent","RotationSpeed",leftWingMovementSpeed),
            new Parameter("wing_left", "leftWingMovementRotateTo","AnythingWingRotationAnimationComponent","RotateTo",0,leftWingMovementRotateTo),
            //new Parameter("leg_right", "rightLegMovementSpeed","AnythingWingRotationAnimationComponent","RotationSpeed",rightLegMovementSpeed),
            //new Parameter("leg_right", "rightLegMovementRotateTo","AnythingWingRotationAnimationComponent","RotateTo",0,rightLegMovementRotateTo),
            //new Parameter("leg_left", "leftLegMovementSpeed","AnythingWingRotationAnimationComponent","RotationSpeed",leftLegMovementSpeed),
            //new Parameter("leg_left", "leftLegMovementRotateTo","AnythingWingRotationAnimationComponent","RotateTo",0,leftLegMovementRotateTo),
            new Parameter("head", "headTurnSpeed","AWTurnHeadHorizontal","TurnSpeed",headTurnSpeed),
            new Parameter("head", "headTurnMaxAngle","AWTurnHeadHorizontal","MaxRotation",headTurnMaxAngle)
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
            rightLegMovementSpeed = speed;
            leftLegMovementSpeed = speed - (0.2f * speed);
        }
     
        /// <summary>
        /// Method used for updating all parameters for the animation
        /// </summary>
        protected override void UpdateParameters()
        {
            // Check which parameters were modified
            var modifiedParameters = paramControl.CheckParameters(new List<(string, float, Vector3)>() { ("bodyWobbleMovementSpeed",bodyWobbleMovementSpeed,new Vector3()),
            ("bodyWobbleMaxAngle",bodyWobbleMaxAngle,new Vector3()),("rightWingMovementSpeed",rightWingMovementSpeed,new Vector3()),
            ("rightWingMovementRotateTo",0,rightWingMovementRotateTo),("leftWingMovementSpeed",leftWingMovementSpeed,new Vector3()),
            ("leftWingMovementRotateTo",0,leftWingMovementRotateTo),("headTurnSpeed",headTurnSpeed,new Vector3()),
            ("headTurnMaxAngle",headTurnMaxAngle,new Vector3())});

            // Update parmeters value in the proper script
            foreach (var param in modifiedParameters)
            {
                partNameToScript[(param.PrefabPart, param.ScriptName)].ModifyParameter(param);
            }
        }
    }

}
