using UnityEngine.Networking;

namespace AnythingWorld.Utilities
{
    public static class CheckWebRequest
    {
        public static bool IsSuccess(UnityWebRequest webRequest)
        {

#if UNITY_2020_1_OR_NEWER
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
#else
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                return false;
            }
            else
            {
                return true;
            }
#endif

        }


        public static bool IsError(UnityWebRequest webRequest)
        {
#if UNITY_2020_1_OR_NEWER
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
#else
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                return true;
            }
            else
            {
                return false;
            }
#endif
        }

    }

}
