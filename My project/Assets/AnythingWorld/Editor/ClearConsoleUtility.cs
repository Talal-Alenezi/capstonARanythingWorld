using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace AnythingWorld.Utilities
{
#if UNITY_EDITOR
    public static class ClearConsoleUtil
    {
        #region Fields
        static MethodInfo clearConsoleMethod;
        static MethodInfo ClearConsoleMethod
        {
            get
            {
                if (clearConsoleMethod == null)
                {
                    Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
                    Type logEntries = assembly.GetType("UnityEditor.LogEntries");
                    clearConsoleMethod = logEntries.GetMethod("Clear");
                }
                return clearConsoleMethod;
            }
        }
        #endregion

        #region Public Methods
        public static void ClearLogConsole()
        {
            ClearConsoleMethod.Invoke(new object(), null);
        }
        #endregion

        #region Private Methods

        #endregion
    }

#endif
}
