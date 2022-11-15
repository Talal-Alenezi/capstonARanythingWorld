using System;
using UnityEngine;

/// <summary>
/// Class used for storing default values for parameters depending on the prefab and behaviour type
/// </summary>
[Serializable]
public class DefaultParameter
{
    /// <summary>
    /// Name of the prefab, name of the behaviour and name of the parameter
    /// </summary>
    [SerializeField]
    public (string, string, string) Key;
    /// <summary>
    /// Default value of the parameter
    /// </summary>
    [SerializeField]
    public float Value;
    public Vector3 Vector3Value;

    public override bool Equals(object obj)
    {
        return Key.Equals(((DefaultParameter)obj).Key);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
