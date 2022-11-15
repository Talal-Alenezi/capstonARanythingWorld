using UnityEngine;

public class EnvironmentLights
{
    public Color LightColor = new Color(0.8f, 0.8f, 0.8f);
    public float LightIntensity = 2f;
    public Color FogColor = new Color(0.5f, 0.5f, 0.5f);
    public float FogDensity = 0.01f;
    public FogMode FogType;
    public float FogStart = 10f;
    public float FogEnd = 40f;
    public string SkyBoxMaterial = "AWSkyBoxBlue";

    public EnvironmentLights(string skBxMat, Color lColor, float lIntense, Color fColor, float fDense, float fStart, float fEnd, FogMode fType = FogMode.ExponentialSquared)
    {
        SkyBoxMaterial = skBxMat;
        LightColor = lColor;
        LightIntensity = lIntense;
        FogColor = fColor;
        FogDensity = fDense;
        FogStart = fStart;
        FogEnd = fEnd;
        FogType = fType;
    }
}