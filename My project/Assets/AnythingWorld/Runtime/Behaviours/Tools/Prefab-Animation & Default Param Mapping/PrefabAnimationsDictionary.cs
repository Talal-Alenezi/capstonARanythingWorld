using System.Collections.Generic;
using UnityEngine;


namespace AnythingWorld.Animation
{
    /// <summary>
    /// Class storing settings related to prefabs and animations.
    /// </summary>
    [CreateAssetMenu(fileName = "PrefabAnimationsDictionary", menuName = "AnythingWorld/PrefabAnimationsDictionary", order = 3)]
    public class PrefabAnimationsDictionary : ScriptableObject
    {
        #region Prefab Type to Animation Script Map 
        public List<PrefabToAnimationsMap> prefabAnimationMapping = new List<PrefabToAnimationsMap>()
    {
        new PrefabToAnimationsMap(){Key="quadruped", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value="QuadrupedWalkAnimation" },
                new AnimationTypeToScript(){Key="walk", Value="QuadrupedWalkAnimation" }
            }
        },
        new PrefabToAnimationsMap(){Key="quadruped__walk", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value="QuadrupedWalkAnimation"},
                new AnimationTypeToScript(){Key="walk", Value="QuadrupedWalkAnimation" }
            }
        },
        new PrefabToAnimationsMap(){Key="quadruped_ungulate", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value="QuadrupedUngulateWalkAnimation" },
                new AnimationTypeToScript(){Key="walk", Value="QuadrupedUngulateWalkAnimation" }
            }
        },
        new PrefabToAnimationsMap(){Key="quadruped_ungulate__walk", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value="QuadrupedUngulateWalkAnimation" },
                new AnimationTypeToScript(){Key="walk", Value="QuadrupedUngulateWalkAnimation" }
            }
        },
        new PrefabToAnimationsMap(){Key="quadruped_fat__crawl", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value="QuadrupedFatCrawlWalkAnimation" },
                new AnimationTypeToScript(){Key="walk", Value="QuadrupedFatCrawlWalkAnimation" }
            }
        },
        new PrefabToAnimationsMap(){Key="quadruped_fat_shortleg_generic__walk", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value="QuadrupedFatShortlegWalkAnimation" },
                new AnimationTypeToScript(){Key="walk", Value="QuadrupedFatShortlegWalkAnimation" }
            }
        },
        new PrefabToAnimationsMap(){Key="quadruped_fat_small_generic__walk", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value="QuadrupedFatSmallWalkAnimation" },
                new AnimationTypeToScript(){Key="walk", Value="QuadrupedFatSmallWalkAnimation" }
            }
        },
        new PrefabToAnimationsMap(){Key="quadruped_fat__walk", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value="QuadrupedFatWalkAnimation" },
                new AnimationTypeToScript(){Key="walk", Value="QuadrupedFatWalkAnimation" }
            }
        },
        new PrefabToAnimationsMap(){Key="biped__walk", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value="BipedWalkAnimation" },
                new AnimationTypeToScript(){Key="walk", Value="BipedWalkAnimation" }
            }
        },
        new PrefabToAnimationsMap(){Key="biped__walk2", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value="BipedWalk2WalkAnimation" },
                new AnimationTypeToScript(){Key="walk", Value="BipedWalk2WalkAnimation" }
            }
        },
        new PrefabToAnimationsMap(){Key="winged_flyer", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value="WingedFlyerWalkAnimation" },
                new AnimationTypeToScript(){Key="walk", Value="WingedFlyerWalkAnimation" }
            }
        },
        new PrefabToAnimationsMap(){Key="winged_flyer__fly", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value="WingedFlyerFlyAnimation" },
                new AnimationTypeToScript(){Key="fly", Value="WingedFlyerFlyAnimation" }
            }
        },
        new PrefabToAnimationsMap(){Key="winged_flyer__waddle", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value="WingedFlyerWalkAnimation" },
                new AnimationTypeToScript(){Key="walk", Value="WingedFlyerWalkAnimation" }
            }
        },
        new PrefabToAnimationsMap(){Key="winged_standing", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value="WingedStandingWalkAnimation" },
                new AnimationTypeToScript(){Key="walk", Value="WingedStandingWalkAnimation" }
            }
        },
        new PrefabToAnimationsMap(){Key="winged_standing_small", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value="WingedStandingSmallWalkAnimation" },
                new AnimationTypeToScript(){Key="walk", Value="WingedStandingSmallWalkAnimation" }
            }
        },

        new PrefabToAnimationsMap(){Key="winged_standing_small__walk", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value="WingedStandingSmallWalkAnimation" },
                new AnimationTypeToScript(){Key="walk", Value="WingedStandingSmallWalkAnimation" }
            }
        },

        new PrefabToAnimationsMap(){Key="winged_standing__walk", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value="WingedStandingWalkAnimation" },
                new AnimationTypeToScript(){Key="walk", Value="WingedStandingWalkAnimation" }
            }
        },


        new PrefabToAnimationsMap(){Key="multileg_crawler_eight__walk", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value="MultilegCrawlerEightWalkAnimation" },
                new AnimationTypeToScript(){Key="walk", Value="MultilegCrawlerEightWalkAnimation" }
            }
        },
        new PrefabToAnimationsMap(){Key="multileg_crawler__walk", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value="MultilegCrawlerWalkAnimation" },
                new AnimationTypeToScript(){Key="walk", Value="MultilegCrawlerWalkAnimation" }
            }
        },
        new PrefabToAnimationsMap(){Key="multileg_crawler_big__walk", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value="MultilegCrawlerWalkAnimation" },
                new AnimationTypeToScript(){Key="walk", Value="MultilegCrawlerWalkAnimation" }
            }
        },
        new PrefabToAnimationsMap(){Key="vehicle_uniform", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value=null },
                new AnimationTypeToScript(){Key="drive", Value="VehicleBobAnimation" }
            }
        },
        new PrefabToAnimationsMap(){Key="vehicle_uniform__drive", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value=null },
                new AnimationTypeToScript(){Key="drive", Value="VehicleBobAnimation" }
            }
        },
        new PrefabToAnimationsMap(){Key="vehicle_one_wheel__drive", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value=null },
                new AnimationTypeToScript(){Key="drive", Value=null }
            }
        },
        new PrefabToAnimationsMap(){Key="vehicle_three_wheel__drive", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value=null },
                new AnimationTypeToScript(){Key="drive", Value=null }
            }
        },

        new PrefabToAnimationsMap(){Key="vehicle_four_wheel__drive", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value=null },
                new AnimationTypeToScript(){Key="drive", Value=null }
            }
        },

         new PrefabToAnimationsMap(){Key="vehicle_four_wheel", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value=null },
                new AnimationTypeToScript(){Key="drive", Value=null }
            }
        },

        new PrefabToAnimationsMap(){Key="vehicle_two_wheel__drive", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value=null },
                new AnimationTypeToScript(){Key="drive", Value=null }
            }
        },

         new PrefabToAnimationsMap(){Key="vehicle_load__drive", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value=null },
                new AnimationTypeToScript(){Key="drive", Value=null }
            }
         },

          new PrefabToAnimationsMap(){Key="uniform__slithervertical", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value="UniformSlitherVerticalAnimation" },
                new AnimationTypeToScript(){Key="Move", Value="UniformSlitherVerticalAnimation" },
            }
        },

           new PrefabToAnimationsMap(){Key="uniform__swim", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value="UniformSwimAnimation" }
            }
        },

           new PrefabToAnimationsMap(){Key="uniform__swim2", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value="UniformSwim2Animation" }
            }
        },
        new PrefabToAnimationsMap(){Key="hopper__hop", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value=null },
                new AnimationTypeToScript(){Key="hop", Value=null }
            }
        },
        new PrefabToAnimationsMap(){Key="hopper__walk", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value=null },
                new AnimationTypeToScript(){Key="hop", Value=null }
            }
        },
        new PrefabToAnimationsMap(){Key="biped__squat", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value=null },
                new AnimationTypeToScript(){Key="hop", Value=null },
                new AnimationTypeToScript(){Key="walk", Value="BipedWalk2WalkAnimation"}
            }
        },
        new PrefabToAnimationsMap(){Key="winged_standing__hop", Value=
            new List<AnimationTypeToScript>()
            {
                new AnimationTypeToScript(){Key="default", Value=null },
                new AnimationTypeToScript(){Key="hop", Value=null }
            }
        }
    };
        #endregion

        #region Default Parameter Values For Scripts

        [Tooltip("List of default parameters for particular prefab and behaviour.")]
        public List<DefaultParameter> prefabBehaviourDefaultParameterValue = new List<DefaultParameter>()
    {
        // Default settings for prefab:quadruped behaviour:RandomMovement
        new DefaultParameter(){Key=("quadruped__walk", "RandomMovement","moveSpeed"),Value= 5 },
        new DefaultParameter(){Key=("quadruped__walk", "RandomMovement","turnSpeed"),Value= 1 },

        // Default setting for prefab:quadruped_fat__walk behaviour:RandomMovement
        new DefaultParameter(){Key=("quadruped_fat__walk", "RandomMovement","moveSpeed"),Value= 3 },
        new DefaultParameter(){Key=("quadruped_fat__walk", "RandomMovement","turnSpeed"),Value= 2 },

        // Default settings for prefab:quadruped_fat__crawl behaviour:RandomMovement
        new DefaultParameter(){Key=("quadruped_fat__crawl", "RandomMovement","moveSpeed"),Value= 1 },
        new DefaultParameter(){Key=("quadruped_fat__crawl", "RandomMovement","turnSpeed"),Value= 1 },

        // Default settings for prefab:biped__walk2 behaviour:RandomMovement
        new DefaultParameter(){Key=("biped__walk2", "RandomMovement","moveSpeed"),Value= 2 },
        new DefaultParameter(){Key=("biped__walk2", "RandomMovement","turnSpeed"),Value= 1 },

        new DefaultParameter(){Key=("biped__squat", "RandomMovement","moveSpeed"),Value= 2 },
        new DefaultParameter(){Key=("biped__squat", "RandomMovement","turnSpeed"),Value= 1 },

        // Default settings for prefab:quadruped_ungulate__walk behaviour:RandomMovement
        new DefaultParameter(){Key=("quadruped_ungulate__walk", "RandomMovement","moveSpeed"),Value= 5 },
        new DefaultParameter(){Key=("quadruped_ungulate__walk", "RandomMovement","turnSpeed"),Value= 1 },

        // Default settings for prefab:biped__walk behaviour:RandomMovement
        new DefaultParameter(){Key=("biped__walk", "RandomMovement","moveSpeed"),Value= 3 },
        new DefaultParameter(){Key=("biped__walk", "RandomMovement","turnSpeed"),Value= 1 },

        // Default settings for prefab:biped__walk behaviour:RandomMovement
        new DefaultParameter(){Key=("winged_flyer__waddle", "RandomMovement","moveSpeed"),Value= 3 },
        new DefaultParameter(){Key=("winged_flyer__waddle", "RandomMovement","turnSpeed"),Value= 1 },

        // Default settings for prefab:quadruped_fat_small_generic__walk behaviour:RandomMovement
        new DefaultParameter(){Key=("quadruped_fat_small_generic__walk", "RandomMovement","moveSpeed"),Value= 3 },
        new DefaultParameter(){Key=("quadruped_fat_small_generic__walk", "RandomMovement","turnSpeed"),Value= 2 },

        // Default settings for prefab:quadruped_fat_shortleg_generic__walk behaviour:RandomMovement
        new DefaultParameter(){Key=("quadruped_fat_shortleg_generic__walk", "RandomMovement","moveSpeed"),Value= 3 },
        new DefaultParameter(){Key=("quadruped_fat_shortleg_generic__walk", "RandomMovement","turnSpeed"),Value= 2 },

        // Default settings for prefab:multileg_crawler__walk behaviour:RandomMovement
        new DefaultParameter(){Key=("multileg_crawler__walk", "RandomMovement","moveSpeed"),Value= 10 },
        new DefaultParameter(){Key=("multileg_crawler__walk", "RandomMovement","turnSpeed"),Value= 2 },

        // Default settings for prefab:multileg_crawler_big__walk behaviour:RandomMovement
        new DefaultParameter(){Key=("multileg_crawler_big__walk", "RandomMovement","moveSpeed"),Value= 10 },
        new DefaultParameter(){Key=("multileg_crawler_big__walk", "RandomMovement","turnSpeed"),Value= 2 },

        // Default settings for prefab:multileg_crawler_eight__walk behaviour:RandomMovement
        new DefaultParameter(){Key=("multileg_crawler_eight__walk", "RandomMovement","moveSpeed"),Value= 10 },
        new DefaultParameter(){Key=("multileg_crawler_eight__walk", "RandomMovement","turnSpeed"),Value= 2 },

        // Default settings for prefab:uniform__slither behaviour:RandomMovement
        new DefaultParameter(){Key=("uniform__slither", "RandomMovement","moveSpeed"),Value= 7 },
        new DefaultParameter(){Key=("uniform__slither", "RandomMovement","turnSpeed"),Value= 1 },

        // Default settings for prefab:uniform__wriggle behaviour:RandomMovement
        new DefaultParameter(){Key=("uniform__wriggle", "RandomMovement","moveSpeed"),Value= 2 },
        new DefaultParameter(){Key=("uniform__wriggle", "RandomMovement","turnSpeed"),Value= 1 },

         // Default settings for prefab:uniform__wriggle behaviour:RandomMovement
        new DefaultParameter(){Key=("winged_standing", "RandomMovement","moveSpeed"),Value= 2 },
        new DefaultParameter(){Key=("winged_standing", "RandomMovement","turnSpeed"),Value= 1 },

        // Default settings for prefab:uniform__wriggle behaviour:RandomMovement
        new DefaultParameter(){Key=("winged_standing__walk", "RandomMovement","moveSpeed"),Value= 2 },
        new DefaultParameter(){Key=("winged_standing__walk", "RandomMovement","turnSpeed"),Value= 1 },
        // Default settings for prefab:uniform__wriggle behaviour:RandomMovement
        new DefaultParameter(){Key=("winged_standing_small__walk", "RandomMovement","moveSpeed"),Value= 2 },
        new DefaultParameter(){Key=("winged_standing_small__walk", "RandomMovement","turnSpeed"),Value= 1 },

        // Default settings for prefab:uniform__wriggle behaviour:RandomMovement
        new DefaultParameter(){Key=("uniform__wriggle", "RandomMovement","moveSpeed"),Value= 2 },
        new DefaultParameter(){Key=("uniform__wriggle", "RandomMovement","turnSpeed"),Value= 1 },


        // Default settings for prefab:vehicle_four_wheel behaviour:RandomDrive
        new DefaultParameter(){Key=("vehicle_four_wheel__drive", "VehicleDriveMovement","enginePower"),Value= 1500 },
        new DefaultParameter(){Key=("vehicle_four_wheel__drive", "VehicleDriveMovement","rotationSpeed"),Value= 5 },
        new DefaultParameter(){Key=("vehicle_four_wheel__drive", "VehicleDriveMovement","power"),Value= -1500 },
        new DefaultParameter(){Key=("vehicle_four_wheel__drive", "VehicleDriveMovement","brake"),Value= 0 },
        new DefaultParameter(){Key=("vehicle_four_wheel__drive", "VehicleDriveMovement","steer"),Value= 0 },
        new DefaultParameter(){Key=("vehicle_four_wheel__drive", "VehicleDriveMovement","maxSteer"),Value= 50 },
        new DefaultParameter(){Key=("vehicle_four_wheel__drive", "VehicleDriveMovement","VehicleCenterOfMass"),Value= 0,Vector3Value= new Vector3(0,-0.5f,0) },

        // Default settings for prefab:vehicle_uniform behaviour:RandomDrive
        new DefaultParameter(){Key=("vehicle_uniform", "RandomMovement","moveSpeed"),Value= 2 },
        new DefaultParameter(){Key=("vehicle_uniform", "RandomMovement","turnSpeed"),Value= 1 },

        // Default settings for prefab:vehicle_uniform__drive behaviour:RandomDrive
        new DefaultParameter(){Key=("vehicle_uniform__drive", "RandomMovement","moveSpeed"),Value= 2 },
        new DefaultParameter(){Key=("vehicle_uniform__drive", "RandomMovement","turnSpeed"),Value= 1 },

         // Default settings for prefab:vehicle_uniform__drive behaviour:RandomDrive
        new DefaultParameter(){Key=("vehicle_uniform__bob", "RandomMovement","moveSpeed"),Value= 2f },
        new DefaultParameter(){Key=("vehicle_uniform__bob", "RandomMovement","turnSpeed"),Value= 0.3f },

        // Default settings for prefab:vehicle_one_wheel behaviour:RandomDrive
        new DefaultParameter(){Key=("vehicle_one_wheel__drive", "VehicleDriveMovement","enginePower"),Value= 300 },
        new DefaultParameter(){Key=("vehicle_one_wheel__drive", "VehicleDriveMovement","rotationSpeed"),Value= 5 },
        new DefaultParameter(){Key=("vehicle_one_wheel__drive", "VehicleDriveMovement","power"),Value= 0 },
        new DefaultParameter(){Key=("vehicle_one_wheel__drive", "VehicleDriveMovement","brake"),Value= 0 },
        new DefaultParameter(){Key=("vehicle_one_wheel__drive", "VehicleDriveMovement","steer"),Value= 0 },
        new DefaultParameter(){Key=("vehicle_one_wheel__drive", "VehicleDriveMovement","maxSteer"),Value= 25 },
        new DefaultParameter(){Key=("vehicle_one_wheel__drive", "VehicleDriveMovement","VehicleCenterOfMass"),Value= 0,Vector3Value= new Vector3(0,-12,0) },

        // Default settings for prefab:vehicle_three_wheel behaviour:RandomDrive
        new DefaultParameter(){Key=("vehicle_three_wheel__drive", "VehicleDriveMovement","enginePower"),Value= 150 },
        new DefaultParameter(){Key=("vehicle_three_wheel__drive", "VehicleDriveMovement","rotationSpeed"),Value= 5 },
        new DefaultParameter(){Key=("vehicle_three_wheel__drive", "VehicleDriveMovement","power"),Value= 0 },
        new DefaultParameter(){Key=("vehicle_three_wheel__drive", "VehicleDriveMovement","brake"),Value= 0 },
        new DefaultParameter(){Key=("vehicle_three_wheel__drive", "VehicleDriveMovement","steer"),Value= 0 },
        new DefaultParameter(){Key=("vehicle_three_wheel__drive", "VehicleDriveMovement","maxSteer"),Value= 25 },
        new DefaultParameter(){Key=("vehicle_three_wheel__drive", "VehicleDriveMovement","VehicleCenterOfMass"),Value= 0,Vector3Value= new Vector3(0,-0.5f,0) },

        // Default settings for prefab:vehicle_two_wheel behaviour:RandomDrive
        new DefaultParameter(){Key=("vehicle_two_wheel__drive", "VehicleDriveMovement","enginePower"),Value= 150 },
        new DefaultParameter(){Key=("vehicle_two_wheel__drive", "VehicleDriveMovement","rotationSpeed"),Value= 5 },
        new DefaultParameter(){Key=("vehicle_two_wheel__drive", "VehicleDriveMovement","power"),Value= 0 },
        new DefaultParameter(){Key=("vehicle_two_wheel__drive", "VehicleDriveMovement","brake"),Value= 0 },
        new DefaultParameter(){Key=("vehicle_two_wheel__drive", "VehicleDriveMovement","steer"),Value= 0 },
        new DefaultParameter(){Key=("vehicle_two_wheel__drive", "VehicleDriveMovement","maxSteer"),Value= 25 },
        new DefaultParameter(){Key=("vehicle_two_wheel__drive", "VehicleDriveMovement","VehicleCenterOfMass"),Value= 0,Vector3Value= new Vector3(0,-0.5f,0) },

        // Default settings for prefab:hopper behaviour:RandomHop
        new DefaultParameter(){Key=("hopper__hop", "HoppingMovement","MovementSpeed"),Value= 7 },
        new DefaultParameter(){Key=("hopper__hop", "HoppingMovement","TurnSpeed"),Value= 2 },
        new DefaultParameter(){Key=("hopper__hop", "HoppingMovement","JumpAmount"),Value= 5 },
        new DefaultParameter(){Key=("hopper__hop", "HoppingMovement","JumpTime"),Value= 0.3f },

        // Default settings for prefab:biped__walk behaviour:RandomMovement
        new DefaultParameter(){Key=("hopper__walk", "HoppingMovement","MovementSpeed"),Value= 30 },
        new DefaultParameter(){Key=("hopper__walk", "HoppingMovement","TurnSpeed"),Value= 2 },
        new DefaultParameter(){Key=("hopper__walk", "HoppingMovement","JumpAmount"),Value= 16 },
        new DefaultParameter(){Key=("hopper__walk", "HoppingMovement","JumpTime"),Value= 0.3f },

        // Default settings for prefab:biped__squat behaviour:HoppingMovement
        new DefaultParameter(){Key=("biped__squat", "HoppingMovement","MovementSpeed"),Value= 8 },
        new DefaultParameter(){Key=("biped__squat", "HoppingMovement","TurnSpeed"),Value= 2 },
        new DefaultParameter(){Key=("biped__squat", "HoppingMovement","JumpAmount"),Value= 14 },
        new DefaultParameter(){Key=("biped__squat", "HoppingMovement","JumpTime"),Value= 0.3f },

        // Default settings for prefab:winged_standing__hop behaviour:HoppingMovement
        new DefaultParameter(){Key=("winged_standing__hop", "HoppingMovement","MovementSpeed"),Value= 6 },
        new DefaultParameter(){Key=("winged_standing__hop", "HoppingMovement","TurnSpeed"),Value= 6 },
        new DefaultParameter(){Key=("winged_standing__hop", "HoppingMovement","JumpAmount"),Value= 1.5f },
        new DefaultParameter(){Key=("winged_standing__hop", "HoppingMovement","JumpTime"),Value= 0.4f },
    };
        #endregion

        #region Access Functions
        private List<AnimationTypeToScript> GetAnimationList(string prefabCategory)
        {
            var animList = new List<AnimationTypeToScript>();
            if (prefabAnimationMapping.Exists(x => x.Key == (prefabCategory)))
            {
                animList = prefabAnimationMapping.Find(x => x.Key == (prefabCategory)).Value;
                return animList;
            }
            else
            {
                if (AnythingSettings.Instance.showDebugMessages) Debug.Log("No animation list found for " + prefabCategory + ".");
            }
            return animList;
        }

        /// <summary>
        /// Get an animation script for the provided prefab category and animation type.
        /// </summary>
        /// <param name="prefabCategory">Category of prefab.</param>
        /// <param name="animationType">String describing type of animation.</param>
        /// <returns>String name of animation script.</returns>
        public string GetAnimationScriptName(string prefabCategory, string animationType)
        {
            if (prefabCategory != null && animationType != null)
            {
                var animList = GetAnimationList(prefabCategory);
                //If animation list returned is populated, iterate through
                if (animList != null && animList.Count > 0)
                {
                    //If animation exists in 
                    if (animList.Exists(x => x.Key == (animationType)))
                    {
                        var scriptName = animList.Find(x => x.Key == (animationType)).Value;
                        //Debug.Log("Script found");
                        return scriptName;
                    }
                    else
                    {
                        if (AnythingSettings.DebugEnabled) Debug.LogWarning("No matching AWAnimation script for " + prefabCategory + " " + animationType);
                    }
                }
                else
                {
                    if (AnythingSettings.DebugEnabled) Debug.LogWarning("No animation list returned to GetAnimationScriptName");
                }

                return null;
            }
            else
            {

                if (AnythingSettings.DebugEnabled) Debug.LogWarning("Animation or prefab script not present.");
                return null;
            }
        }


        /// <summary>
        /// Method used for obtaining default value of the parameter given prefab's name and behaviour
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="behaviour"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public float GetDefaultParameterValue(string prefab, string behaviour, string parameter) // return type changed from dynamic to float! - GM
        {
            // Debug.Log(prefab + " + " + behaviour + "+" + parameter);

            if (prefabBehaviourDefaultParameterValue.Exists(x => x.Key == (prefab, behaviour, parameter)))
            {
                var defaultParamVal = prefabBehaviourDefaultParameterValue.Find(x => x.Key == (prefab, behaviour, parameter)).Value;
                return defaultParamVal;
            }
            //Debug.Log("No default " + parameter + " parameter value found for " + prefab + " " + behaviour + ".");
            return 0f;
        }

        public Vector3 GetDefaultParameterVector3Value(string prefab, string behaviour, string parameter) // new method to allow Vector3s to be returned - GM
        {
            // Debug.Log(prefab + " + " + behaviour + "+" + parameter);

            if (prefabBehaviourDefaultParameterValue.Exists(x => x.Key == (prefab, behaviour, parameter)))
            {
                var defaultParamVal = prefabBehaviourDefaultParameterValue.Find(x => x.Key == (prefab, behaviour, parameter)).Vector3Value;
                return defaultParamVal;
            }
            //Debug.Log("No default " + parameter + " parameter value found for " + prefab + " " + behaviour + ".");
            return new Vector3();
        }
        #endregion

    }
}
