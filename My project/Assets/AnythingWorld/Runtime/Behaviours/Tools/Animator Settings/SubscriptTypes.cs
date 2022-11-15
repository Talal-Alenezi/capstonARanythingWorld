using System;
using UnityEngine;

/// <summary>
/// Class responsible for storing subscripts types
/// </summary>
[Serializable]
public class SubscriptTypes
{
    /// <summary>
    /// Name of the subscript
    /// </summary>
    [SerializeField]
    public string Key;
    /// <summary>
    /// Type of the subscript
    /// </summary>
    [SerializeField]
    public Type Value;

    public override bool Equals(object obj)
    {
        return Key.Equals(((SubscriptTypes)obj).Key);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
