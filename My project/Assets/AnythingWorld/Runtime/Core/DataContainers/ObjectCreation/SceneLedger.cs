using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnythingWorld.DataContainers
{
    [Serializable]
    public class SceneLedger
    {
        [SerializeField]
        private List<ThingGroup> thingGroups = null;
        public List<ThingGroup> ThingGroups
        {
            get
            {
                if (thingGroups == null)
                {
                    thingGroups = new List<ThingGroup>();
                }
                return thingGroups;
            }
        }

        public SceneLedger()
        {
            thingGroups = new List<ThingGroup>();
        }
        public ThingGroup NewThingGroup(string name)
        {
            var newGroup = new ThingGroup(name);
            thingGroups.Add(newGroup);
            return newGroup;
        }

        public void RemoveGroup(ThingGroup group)
        {
            group.ClearObjects();
            thingGroups.Remove(group);
        }
    }
}