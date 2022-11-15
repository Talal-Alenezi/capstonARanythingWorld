using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnythingWorld.DataContainers
{
    /// <summary>
    /// Queue object that allows object data from API to be passed into the anythingQueue in <c>MakeObjectProcess</c>.
    /// </summary>
    public class AnythingQueueObject
    {
        #region Fields
        public string objName;
        public bool autoLayout = true;
        public bool hasBehaviour = true;
        public bool hasCollider = true;
        public bool positionGlobally = false;
        public Vector3 objectPos = Vector3.zero;
        public Quaternion objectRot = Quaternion.identity;
        public Vector3 objectScale = Vector3.one;
        public Transform parentTransform;
        public GameObject gameObjectReference = null;
        public int makeAttempts=0;
        public float startWaitTime = 0;
        #endregion

        #region Public Methods
        public AnythingQueueObject(string _objectName, bool _autoLayout, bool _hasBehaviour, bool _hasCollider, Vector3 _objectPosition, Quaternion _objectRotation, Vector3 _objectScale, Transform _parentTransform)
        {
            objName = _objectName;
            autoLayout = _autoLayout;
            hasBehaviour = _hasBehaviour;
            hasCollider = _hasCollider;
            objectPos = _objectPosition;
            objectRot = _objectRotation;
            objectScale = _objectScale;
            parentTransform = _parentTransform;
        }

        public AnythingQueueObject(string _objectName, bool _autoLayout, bool _hasBehaviour, bool _hasCollider, Vector3 _objectPosition, Quaternion _objectRotation, Vector3 _objectScale, Transform _parentTransform, GameObject _objRef)
        {
            objName = _objectName;
            autoLayout = _autoLayout;
            hasBehaviour = _hasBehaviour;
            hasCollider = _hasCollider;
            objectPos = _objectPosition;
            objectRot = _objectRotation;
            objectScale = _objectScale;
            parentTransform = _parentTransform;
            gameObjectReference = _objRef;
        }

        public AnythingQueueObject(string _objectName, bool _autoLayout, bool _hasBehaviour, bool _hasCollider, bool _positionGlobally, Vector3 _objectPosition, Quaternion _objectRotation, Vector3 _objectScale, Transform _parentTransform, GameObject _objRef)
        {
            objName = _objectName;
            autoLayout = _autoLayout;
            hasBehaviour = _hasBehaviour;
            hasCollider = _hasCollider;
            objectPos = _objectPosition;
            objectRot = _objectRotation;
            objectScale = _objectScale;
            parentTransform = _parentTransform;
            gameObjectReference = _objRef;
            positionGlobally = _positionGlobally;
        }
        #endregion
    }
}
