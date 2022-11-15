using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnythingApiConfig
{
    public static string ApiUrlStem
    {
        get
        {
            return AW_API_STEM;
        }
    }
    private const string AW_API_STEM = "https://api.anything.world";
}

