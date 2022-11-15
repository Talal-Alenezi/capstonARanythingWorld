using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace AnythingWorld.Utilities
{
    [InitializeOnLoad]
    public class AutoRunUtil
    {
        static AutoRunUtil()
        {
            if(!SessionState.GetBool("AutoRunUtilCalled", false))
            {
                EditorApplication.update += RunOnce;
                SessionState.SetBool("AutoRunUtilCalled", true);
            }
        }

        static void RunOnce()
        {
            VersionCheckEditor.DisplayUpdateDialogue();
            EditorApplication.update -= RunOnce;

        }
    }
}

