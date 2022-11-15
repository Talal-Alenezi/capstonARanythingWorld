using System.Collections.Generic;
using UnityEngine;
using AnythingWorld.Utilities;

namespace AnythingWorld.Animation
{
    /// <summary>
    /// Class storing settings related to the animations
    /// </summary>
    [CreateAssetMenu(fileName = "AnimationSettings", menuName = "AnythingWorld/AnimationSettings", order = 1)]
    public class AnimationSettings : SingletonScriptableObject<AnimationSettings>
    {
        [Tooltip("List of all needed scripts and their associations to the prefab parts for particular prefab and animation.")]
        public List<PrefabBehaviourToAnimationScript> prefabBehaviourAnimationSettings = new List<PrefabBehaviourToAnimationScript>()
    {
        //walking random movement for quadruped
        new PrefabBehaviourToAnimationScript(){Key=("quadruped", "walk"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key="body", Value= new List<string>(){ "AWBodyShakeMovement" } },
                new PrefabPartToAnimationSubscript(){Key="foot_front_right", Value=new List<string>(){ "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="foot_front_left", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="foot_hind_right", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="foot_hind_left", Value=new List<string>(){ "AWFeetMovement" } }
            }
        },        
        
        //walking random movement for quadruped_fat__walk
        new PrefabBehaviourToAnimationScript(){Key=("quadruped_fat__walk", "walk"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key="body", Value= new List<string>(){ "AWBodyShakeMovement" } },
                new PrefabPartToAnimationSubscript(){Key="leg_front_right", Value=new List<string>(){ "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="leg_front_left", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="leg_hind_right", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="leg_hind_left", Value=new List<string>(){ "AWFeetMovement" } }
            }
        },   
        
        //walking random movement for quadruped_fat_shortleg_generic__walk
        new PrefabBehaviourToAnimationScript(){Key=("quadruped_fat_shortleg_generic__walk", "walk"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key="leg_front_right", Value=new List<string>(){ "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="leg_front_left", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="leg_hind_right", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="leg_hind_left", Value=new List<string>(){ "AWFeetMovement" } }
            }
        },          
        
        //walking random movement for quadruped_fat_small_generic__walk
        new PrefabBehaviourToAnimationScript(){Key=("quadruped_fat_small_generic__walk", "walk"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key="body", Value= new List<string>(){ "AWBodyShakeMovement" } },
                new PrefabPartToAnimationSubscript(){Key="feet_front_right", Value=new List<string>(){ "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="feet_front_left", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="feet_hind_right", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="feet_hind_left", Value=new List<string>(){ "AWFeetMovement" } }
            }
        },         
        
        //walking random movement for winged_standing__walk
        new PrefabBehaviourToAnimationScript(){Key=("winged_standing__walk", "walk"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key="body", Value= new List<string>(){ "AWBodyShakeMovement" } },
                new PrefabPartToAnimationSubscript(){Key="feet_right", Value=new List<string>(){ "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="feet_left", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="tail", Value=new List<string>(){ "AWTurnTail" } },
                new PrefabPartToAnimationSubscript(){Key="head_holder", Value=new List<string>(){ "AWTurnHead" } }
            }
        },      
        
        //walking random movement for winged_standing_small__walk
        new PrefabBehaviourToAnimationScript(){Key=("winged_standing_small__walk", "walk"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key="body", Value= new List<string>(){ "AWBodyShakeMovement" } },
                new PrefabPartToAnimationSubscript(){Key="feet_right", Value=new List<string>(){ "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="feet_left", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="tail", Value=new List<string>(){ "AWTurnTail" } },
                new PrefabPartToAnimationSubscript(){Key="head_holder", Value=new List<string>(){ "AWTurnHead" } }
            }
        },  
        
        //walking random movement for winged_flyer__waddle
        new PrefabBehaviourToAnimationScript(){Key=("winged_flyer__waddle", "walk"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key="body", Value= new List<string>(){ "AWWobble" } },
                new PrefabPartToAnimationSubscript(){Key="wing_right", Value=new List<string>(){ "AnythingWingRotationAnimationComponent" } },
                new PrefabPartToAnimationSubscript(){Key="wing_left", Value=new List<string>(){ "AnythingWingRotationAnimationComponent" } },
                new PrefabPartToAnimationSubscript(){Key="head", Value=new List<string>(){ "AWTurnHeadHorizontal" } }
            }
        },          
        
        //walking random movement for multileg_crawler
        new PrefabBehaviourToAnimationScript(){Key=("multileg_crawler", "walk"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key="feet_front_right", Value=new List<string>(){ "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="feet_front_left", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="feet_hind_right", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="feet_hind_left", Value=new List<string>(){ "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="feet_middle_right", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="feet_middle_left", Value=new List<string>(){ "AWFeetMovement" } }
            }
        },           
        
        //walking random movement for multileg_crawler_eight
        new PrefabBehaviourToAnimationScript(){Key=("multileg_crawler_eight", "walk"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key="feet_front_right", Value=new List<string>(){ "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="feet_front_left", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="feet_hind_right", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="feet_hind_left", Value=new List<string>(){ "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="feet_midfront_right", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="feet_midfront_left", Value=new List<string>(){ "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="feet_midhind_right", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="feet_midhind_left", Value=new List<string>(){ "AWFeetMovement" } }
            }
        },       
        
        //walking random movement for biped__walk2
        new PrefabBehaviourToAnimationScript(){Key=("biped__walk2", "walk"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key="body", Value= new List<string>(){ "AWBodyShakeMovement" } },
                new PrefabPartToAnimationSubscript(){Key="hand_right", Value=new List<string>(){ "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="hand_left", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="foot_right", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="foot_left", Value=new List<string>(){ "AWFeetMovement" } }
            }
        },  
        
        //walking random movement for biped
        new PrefabBehaviourToAnimationScript(){Key=("biped", "walk"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key="body", Value= new List<string>(){ "AWBodyShakeMovement" } },
                new PrefabPartToAnimationSubscript(){Key="foot_right", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="foot_left", Value=new List<string>(){ "AWFeetMovement" } }
            }
        },

        //walking random movement for quadruped_ungulate
        new PrefabBehaviourToAnimationScript(){Key=("quadruped_ungulate", "walk"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key="body", Value= new List<string>(){ "AWBodyShakeMovement" } },
                new PrefabPartToAnimationSubscript(){Key="foot_front_right", Value=new List<string>(){ "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="foot_front_left", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="foot_hind_right", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="foot_hind_left", Value=new List<string>(){ "AWFeetMovement" } }
            }
        },
        new PrefabBehaviourToAnimationScript(){Key=("quadruped_ungulate__walk", "walk"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key="body", Value= new List<string>(){ "AWBodyShakeMovement" } },
                new PrefabPartToAnimationSubscript(){Key="foot_front_right", Value=new List<string>(){ "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="foot_front_left", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="foot_hind_right", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="foot_hind_left", Value=new List<string>(){ "AWFeetMovement" } }
            }
        },

        
        //walking random movement for quadruped_fat__crawl
        new PrefabBehaviourToAnimationScript(){Key=("quadruped_fat__crawl", "walk"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key="feet_front_right", Value=new List<string>(){ "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="feet_front_left", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="feet_hind_right", Value=new List<string>(){  "AWFeetMovement" } },
                new PrefabPartToAnimationSubscript(){Key="feet_hind_left", Value=new List<string>(){ "AWFeetMovement" } }
            }
        },

        new PrefabBehaviourToAnimationScript(){Key=("vehicle_uniform", "drive"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key="body", Value= new List<string>(){ "AWBodyShakeMovement" } },
            }
        },
        new PrefabBehaviourToAnimationScript(){Key=("vehicle_uniform__drive", "drive"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key="body", Value= new List<string>(){ "AWBodyShakeMovement" } },
            }
        }


    };


        #region Access Functions
        /// <summary>
        /// Method used for obtaining a list of all the prefab parts and scripts applied to them given name of the prefab nad behaviour
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        public List<PrefabPartToAnimationSubscript> GetScriptsForPrefabAnimation(string prefab, string behaviour)
        {
            if (prefabBehaviourAnimationSettings.Exists(x => x.Key == (prefab, behaviour)))
            {
                var prefabAnimSettings = prefabBehaviourAnimationSettings.Find(x => x.Key == (prefab, behaviour)).Value;
                return prefabAnimSettings;
            }

            //Debug.LogError("No prefab part scripts found for object " + prefab + " " + behaviour);
            return null;
        }
        #endregion



    }


    //Extension for the List class that prevents items repetition in the list
    public static class CollectionExtensions
    {
        public static void AddItem<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }
    }
}
