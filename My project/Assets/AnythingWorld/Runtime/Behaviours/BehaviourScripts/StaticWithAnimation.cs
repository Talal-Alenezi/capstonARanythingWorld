using AnythingWorld;
using AnythingWorld.Behaviours;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticWithAnimation : AWBehaviour
{
    protected override string[] targetAnimationType { get; set; } = { "default" };
    void Start()
    {
        InitializeAnimator();
    }

}
