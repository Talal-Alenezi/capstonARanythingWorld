using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnythingWorld.Utilities
{
    public static class ErrorUtility
    {
        public static void LogErrorResponse(ErrorResponse errorResponse)
        {
            Debug.LogError($"{errorResponse.code}: { errorResponse.message}");
        }
        public static void LogWarningResponse(ErrorResponse errorResponse)
        {
            Debug.LogWarning($"{errorResponse.code}: {errorResponse.message}");
        }
        public static void LogErrorResponseAW(ErrorResponse errorResponse)
        {
            if (AnythingSettings.DebugEnabled)
            {
                Debug.LogError($"{errorResponse.code}: { errorResponse.message}");
            }
        }
        public static void LogWarningResponseAW(ErrorResponse errorResponse)
        {
            if (AnythingSettings.DebugEnabled)
            {
                Debug.LogWarning($"{errorResponse.code}: {errorResponse.message}");
            }
        }
        public static ErrorResponse CreateErrorResponse(string errorString)
        {
            try
            {
                var response = JsonUtility.FromJson<ErrorResponse>(errorString);
                return response;
            }
            catch
            {
                Debug.LogWarning($"Could not format string into ErrorResponse: {errorString}");
                return null;
            }

        }
        public static ErrorResponse HandleErrorResponse(string errorString, bool logError = false, bool awDebug = true)
        {
            var response = CreateErrorResponse(errorString);
            if (response != null)
            {
                if (logError==true)
                {
                    LogErrorResponse(response);
                }
                else
                {
                    LogWarningResponse(response);
                }
            }
            return response;
        }
        public static ErrorResponse HandleErrorResponseAWDebug(string errorString, bool logError = false)
        {
            var response = CreateErrorResponse(errorString);
            if (response != null)
            {
                if (logError == true)
                {
                    LogErrorResponseAW(response);
                }
                else
                {
                    LogWarningResponseAW(response);
                }
            }
            return response;
        }
    }
}

