using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LoadedObjectData : MonoBehaviour
{
    /// <summary>
    /// Position of loaded object at instantiation.
    /// </summary>
    [SerializeField]
    public Vector3 zeroedPosition;
    [SerializeField]
    public Quaternion zeroedRotation;
}
