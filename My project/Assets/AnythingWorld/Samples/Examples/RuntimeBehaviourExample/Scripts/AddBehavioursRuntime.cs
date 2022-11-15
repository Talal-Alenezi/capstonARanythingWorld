using System.Collections.Generic;
using UnityEngine;
using AnythingWorld;
using AnythingWorld.Habitat;

namespace AnythingWorld.Examples
{
    public class AddBehavioursRuntime : MonoBehaviour
    {
        private List<string> scripts;
        private AWObj myCat;
        //int count=0;
        private AnythingCreator anythingCreator;
        private AnythingHabitat anythingHabitat;
        // Start is called before the first frame update
        void Start()
        {
            anythingHabitat = AnythingHabitat.Instance;
            anythingCreator = AnythingCreator.Instance;
            MakeGameActors();
        }

        private void MakeGameActors()
        {

            anythingCreator.MakeObject("camel").AddBehaviour<RandomMovement>();
            var llama = anythingCreator.MakeObject("llama");
            var follower = llama.AddBehaviour<FollowTarget>();

            var rat = anythingCreator.MakeObject("porcupine");
            rat.AddBehaviour<RandomMovement>().SpeedMultiplier = 3f;

            follower.targetController = rat;
        }
    }
}

