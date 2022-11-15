using UnityEngine;

namespace AnythingWorld.Utilities
{
    public class UnderWaterEffect : MonoBehaviour
    {

        //This script enables underwater effects. Attach to main camera.

        //Define variable
        public int underwaterLevel = 7;

        //The scene's default fog settings
        //private bool defaultFog = RenderSettings.fog;
        //private Color defaultFogColor = RenderSettings.fogColor;
        //private float defaultFogDensity = RenderSettings.fogDensity;
        //private Material defaultSkybox = RenderSettings.skybox;
        private Material noSkybox = null;

        private void Start()
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.22f, 0.64f, 0.77f, 0.6f);
            RenderSettings.fogDensity = 0.045f;
            RenderSettings.skybox = noSkybox;
        }
    }
}
