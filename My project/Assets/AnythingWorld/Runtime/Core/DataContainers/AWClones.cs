using UnityEngine;
#if UNITY_EDITOR
#endif
using System.Collections.Generic;
using System;


namespace AnythingWorld
{
    [Serializable]
    public class AWClones
    {
        [SerializeField]
        public string Key;
        [SerializeField]
        public List<GameObject> ClonedThings;
        [SerializeField]
        public List<int> UniqueIDs;
    }
}
