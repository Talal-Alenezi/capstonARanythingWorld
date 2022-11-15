using System.Collections.Generic;
using UnityEngine;

namespace AnythingWorld.Animation
{
    public class FlockManager : MonoBehaviour
    {
        public GameObject[] flockPrefabs;
        public int flockZoneSize = 50;
        public int numMembers = 1;
        public List<GameObject> allMembers;
        public Vector3 goalPos = Vector3.zero;
        private bool isSetup = false;
        public string creatorAttribution = "";

        public List<GameObject> GetAllMembers()
        {
            return allMembers;
        }

        /*
        public void MakeMember()
        {
            for (int i = 0; i < numMembers; i++)
            {
                Vector3 pos = new Vector3(
                    Random.Range(-flockZoneSize, flockZoneSize),
                    Random.Range(-flockZoneSize, flockZoneSize),
                    Random.Range(-flockZoneSize, flockZoneSize)
                );
                GameObject member = (GameObject)Instantiate(
                    flockPrefabs[Random.Range(0, flockPrefabs.Length)], pos, Quaternion.identity);
                member.transform.parent = transform;
                allMembers[i] = member;
            }
        }*/

        public void AddMember(FlockMember newMember)
        {
            if (allMembers == null)
            {
                allMembers = new List<GameObject>();

            }

            if (!isSetup)
            {
                // create reference box collider
                var refArea = gameObject.GetComponent<BoxCollider>();
                if (refArea == null)
                {
                    refArea = gameObject.AddComponent<BoxCollider>();
                    refArea.isTrigger = true;
                }
                float realflockSize = flockZoneSize * 2;
                refArea.size = new Vector3(realflockSize, realflockSize, realflockSize);
                isSetup = true;
            }


            newMember.transform.position = GetRandomPositionInFlockSpace();
            newMember.transform.parent = transform;
            allMembers.Add(newMember.gameObject);
            numMembers++;
        }

        public void Update()
        {
            HandleGoalPos();
        }

        private void HandleGoalPos()
        {
            if (Random.Range(1, 1000) < 10)
            {
                goalPos = GetRandomPositionInFlockSpace();
            }
        }

        private Vector3 GetRandomPositionInFlockSpace()
        {
            var randomPosition = new Vector3(
                    Random.Range(-flockZoneSize, flockZoneSize),
                    Random.Range(-flockZoneSize, flockZoneSize),
                    Random.Range(-flockZoneSize, flockZoneSize)
                );
            var relativePosition = transform.position + randomPosition;
            return relativePosition;
        }

    }
}
