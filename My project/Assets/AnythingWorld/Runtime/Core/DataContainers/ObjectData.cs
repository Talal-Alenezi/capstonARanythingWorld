using System;
using System.Collections.Generic;
using UnityEngine;

namespace AnythingWorld.DataContainers
{
    [Serializable]
    public class ObjectData
    {
        #region Model Characteristics
        /// <summary>
        /// Author attribution.
        /// </summary>
        [SerializeField]
        public string attribution;
        /// <summary>
        /// Guid requested by user.
        /// </summary>
        [SerializeField]
        public string requestedGuid;
        /// <summary>
        /// Guid returned by server;
        /// </summary>
        [SerializeField]
        public string returnedGuid;
        [SerializeField]
        public string category;
        [SerializeField]
        public string behaviourCategoryName;
        [SerializeField]
        public Vector3 inputDimensions = Vector3.zero;
        [SerializeField]
        public string behaviour;
        /// <summary>
        /// User requested behaviour controller added.
        /// </summary>
        public bool hasBehaviourController = true;
        /// <summary>
        /// User requested that colliders are added.
        /// </summary>
        public bool hasColliderGenerator = true;
        /// <summary>
        /// Scale input by user in MakeAwObj call.
        /// </summary>
        public float inputScale = 1;

        /// <summary>
        /// The object scaled to 
        /// </summary>
        [SerializeField]
        public float relativeScale = 1;

        /// <summary>
        /// User requested that default behaviours are added.
        /// </summary>
        public bool addDefaultBehaviour = true;
        #endregion

        #region Model Resources
        public Dictionary<string, Texture> objTextures = new Dictionary<string, Texture>();
        public Dictionary<string, string> objParts = new Dictionary<string, string>();
        public string mtlFile;
        #endregion

        public GameObject instantiatedPrefab;

    }
}