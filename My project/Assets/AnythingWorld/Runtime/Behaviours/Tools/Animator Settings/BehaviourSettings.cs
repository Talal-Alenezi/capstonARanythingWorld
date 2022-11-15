using System.Collections.Generic;
using UnityEngine;
using AnythingWorld.Animation;
using AnythingWorld.Utilities;
/// <summary>
/// Class storing settings related to the movement
/// </summary>
[CreateAssetMenu(fileName = "BehaviourSettings", menuName = "AnythingWorld/BehaviourSettings", order = 1)]
public class BehaviourSettings : SingletonScriptableObject<BehaviourSettings>
{
    #region PrefabBehaviourToAnimationMap
    [Tooltip("List of all needed scripts and their associations to the prefab parts for particular prefab and behaviour.")]
    public List<PrefabBehaviourToAnimationScript> behaviourToAnimationSubscriptDictionary = new List<PrefabBehaviourToAnimationScript>()
    {
        //walking random movement for quadruped
        new PrefabBehaviourToAnimationScript(){Key=("quadruped", "RandomMovement"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key= "parent", Value= new List<string>(){ "AWBodyMovementController" } }
            }
        },        
        
        //walking random movement for quadruped_fat__walk
        new PrefabBehaviourToAnimationScript(){Key=("quadruped_fat__walk", "RandomMovement"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key= "parent", Value= new List<string>(){ "AWBodyMovementController" } }
            }
        },   
        
        //walking random movement for quadruped_fat_shortleg_generic__walk
        new PrefabBehaviourToAnimationScript(){Key=("quadruped_fat_shortleg_generic__walk", "RandomMovement"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key= "parent", Value= new List<string>(){ "AWBodyMovementController" } }
            }
        },          
        
        //walking random movement for quadruped_fat_small_generic__walk
        new PrefabBehaviourToAnimationScript(){Key=("quadruped_fat_small_generic__walk", "RandomMovement"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key= "parent", Value= new List<string>(){ "AWBodyMovementController" } }
            }
        },          
        
        //walking random movement for multileg_crawler
        new PrefabBehaviourToAnimationScript(){Key=("multileg_crawler", "RandomMovement"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key= "parent", Value= new List<string>(){ "AWBodyMovementController" } }
            }
        },           
        
        //walking random movement for multileg_crawler_eight
        new PrefabBehaviourToAnimationScript(){Key=("multileg_crawler_eight", "RandomMovement"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key= "parent", Value= new List<string>(){ "AWBodyMovementController" } }
            }
        },       
        
        //walking random movement for biped__walk2
        new PrefabBehaviourToAnimationScript(){Key=("biped__walk2", "RandomMovement"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key= "parent", Value= new List<string>(){ "AWBodyMovementController" } }
            }
        },  
        
        //walking random movement for biped
        new PrefabBehaviourToAnimationScript(){Key=("biped", "RandomMovement"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key= "parent", Value= new List<string>(){ "AWBodyMovementController" } }
            }
        },

        //walking random movement for quadruped_ungulate
        new PrefabBehaviourToAnimationScript(){Key=("quadruped_ungulate", "RandomMovement"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key= "parent", Value= new List<string>(){ "AWBodyMovementController" } }
            }
        },        
        
        //walking random movement for quadruped_fat__crawl
        new PrefabBehaviourToAnimationScript(){Key=("quadruped_fat__crawl", "RandomMovement"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key= "parent", Value= new List<string>(){ "AWBodyMovementController" } }
            }
        },  
        
        //walking random movement for uniform_slither
        new PrefabBehaviourToAnimationScript(){Key=("uniform_slither", "RandomMovement"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key= "parent", Value= new List<string>(){ "AWBodyMovementController" } }
            }
        },

                //walking random movement for uniform_slither
        new PrefabBehaviourToAnimationScript(){Key=("uniform_swim", "RandomMovement"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key= "parent", Value= new List<string>(){ "AWBodyMovementController" } }
            }
        },



        //flying random movement for vehicle_flyer
        new PrefabBehaviourToAnimationScript(){Key=("vehicle_flyer", "RandomMovement"), Value=
            new List<PrefabPartToAnimationSubscript>()
            {
                new PrefabPartToAnimationSubscript(){Key= "parent", Value= new List<string>(){ "AWFlyingVehicleController" } }
            }
        },
    };
    #endregion

    #region Access Functions
    /// <summary>
    /// Method used for obtaining a list of all the prefab parts and scripts applied to them given name of the prefab nad behaviour
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="behaviour"></param>
    /// <returns></returns>
    public List<PrefabPartToAnimationSubscript> GetScriptsForPrefabBehaviour(string prefab, string behaviour)
    {
        return behaviourToAnimationSubscriptDictionary.Find(x => x.Key == (prefab, behaviour)).Value;
    }
    #endregion


}

