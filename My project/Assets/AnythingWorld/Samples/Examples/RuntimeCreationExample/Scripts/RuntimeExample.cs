using System.Collections.Generic;
using UnityEngine;
using AnythingWorld.Habitat;

namespace AnythingWorld.Examples
{
    public class RuntimeExample : MonoBehaviour
    {
        [Range(1, 100)]
        public int totalObjects = 3;

        void Start()
        {
            MakeGameHabitat();
            MakeGameActors();
        }
        private void MakeGameHabitat()
        {
            AnythingHabitat.Instance.MakeHabitat("mountain", false);
        }
        private void MakeGameActors()
        {
            var things = new List<string>();
            things.Add("unicycle");
            things.Add("gazelle");
            things.Add("ant");
            things.Add("car");
            things.Add("armadillo");
            things.Add("plane");

            string randomThing;
            int randomIndex;
            for (var i = 0; i < totalObjects; i++)
            {
                randomIndex = Random.Range(0, things.Count);
                randomThing = things[randomIndex];
                AnythingCreator.Instance.MakeObject(randomThing);
            }
        }
    }
}
