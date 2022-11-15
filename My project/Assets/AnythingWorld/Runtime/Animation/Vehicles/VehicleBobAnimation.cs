using System.Collections.Generic;
using UnityEngine;

namespace AnythingWorld.Animation
{
    public class VehicleBobAnimation : AnythingAnimationController
    {
        public float bodyShakeMovementSpeed = 0.3f;
        public float bodyShakeMovementFrequency = 0.1f;
        public override void InitializeAnimationParameters()
        {
            //TODO: can we make this dynamic?
            FetchAnimationSettings("vehicle_uniform__drive", "drive");

            parametersList = new List<Parameter>()
        {
            new Parameter("body", "bodyShakeMovementSpeed","AWBodyShakeMovement", "MoveSpeed",bodyShakeMovementSpeed),
            new Parameter("body", "bodyShakeMovementFrequency","AWBodyShakeMovement","Frequency",bodyShakeMovementFrequency),
        };
            // Initialize paramControl and _prefabToScript variables
            paramControl = new ParameterController(parametersList);
        }

        protected override void UpdateParameters()
        {
            var modifiedParameters = paramControl.CheckParameters(new List<(string, float)>()
        {
            ("bodyShakeMovementSpeed",bodyShakeMovementSpeed),
            ("bodyShakeMovementFrequency",bodyShakeMovementFrequency),
        });

            foreach (var param in modifiedParameters)
            {
                partNameToScript[(param.PrefabPart, param.ScriptName)].ModifyParameter(param);
            }
        }

    }
}

