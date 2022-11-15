using System.Collections.Generic;
using UnityEngine;


namespace AnythingWorld.Animation
{
    public class VehicleFourWheelAnimation : AnythingAnimationController
    {

        // Animation settings
        public float enginePower = 500;
        public float rotationSpeed = 10;
        public float power = -1500;
        public float brake = 0;
        public float steer = 0;
        public float maxSteer = 60;
        public Vector3 centerOfMass = new Vector3(0, -0.5f, 0);

        //Autos
        public string Descriptor { get; set; } = "Drive";

        public override void InitializeAnimationParameters()
        {
            FetchAnimationSettings("vehicle_four_wheel", "drive");
        }

        /// <summary>
        /// Method used for updating all parameters for the animation
        /// </summary>
        protected override void UpdateParameters()
        {
            // Check which parameters were modified
            var modifiedParameters = paramControl.CheckParameters(new List<(string, float, Vector3)>() { ("enginePower",enginePower,new Vector3()),
            ("rotationSpeed",rotationSpeed,new Vector3()),("power",power,new Vector3()),("brake",brake,new Vector3()),("steer",steer,new Vector3()),("maxSteer",maxSteer,new Vector3()),("centerOfMass",0,centerOfMass)});

            // Update parmeters value in the proper script
            foreach (var param in modifiedParameters)
            {
                partNameToScript[(param.PrefabPart, param.ScriptName)].ModifyParameter(param);
            }
        }
    }
}

