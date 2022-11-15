using System;
using System.Collections.Generic;
using UnityEngine;


namespace AnythingWorld.Utilities
{
    /// <summary>
    /// Automatically object destruction for editor and runtime with one call.
    /// </summary>
    public class AnythingSafeDestroy
    {
        public static void SafeDestroyImmediate(GameObject _object, bool _nullify = true)
        {
            if (_object.TryGetComponent<AWObj>(out var component))
            {
                component.StopRequestPipeline();
                component.flaggedDestroyed = true;
            }


            if (Application.isEditor && !Application.isPlaying)
            {
#if UNITY_EDITOR
                try
                {
                    UnityEngine.Object.DestroyImmediate(_object);
                    if (_nullify) _object = null;
                }
                catch (Exception e)
                {
                    Debug.Log($"Exception thrown while trying to safe destroy object: {e}");
                }
#endif
            }
            else
            {
                UnityEngine.Object.Destroy(_object);
                if (_nullify) _object = null;
            }
        }

        public static void SafeDestroyImmediate(UnityEngine.Object _object, bool _nullify = true)
        {

            if (Application.isEditor && !Application.isPlaying)
            {
#if UNITY_EDITOR
                Debug.Log("safe destroy immediate");
                try
                {
                    UnityEngine.Object.DestroyImmediate(_object);
                    
                    if (_nullify) _object = null;
                }
                catch (Exception e)
                {
                    Debug.Log($"Exception thrown while trying to safe destroy object: {e}");
                }
#endif
            }
            else
            {
                UnityEngine.Object.Destroy(_object);
                if (_nullify) _object = null;
            }
        }

        public static void SafeDestroyImmediate(AWObj _object, bool _nullify = true)
        {
            _object.StopRequestPipeline();
            _object.flaggedDestroyed = true;
            if (Application.isEditor && !Application.isPlaying)
            {
#if UNITY_EDITOR
                try
                {
                    UnityEngine.Object.DestroyImmediate(_object);
                    if (_nullify) _object = null;
                }
                catch (Exception e)
                {
                    Debug.Log($"Exception thrown while trying to safe destroy object: {e}");
                }
#endif
            }
            else
            {
                UnityEngine.Object.Destroy(_object);
                if (_nullify) _object = null;
            }
        }
        public static void SafeDestroyDelayed(AnythingWorld.AWObj[] _objects, bool _nullify = true)
        {
            foreach(var _object in _objects)
            {
                SafeDestroyDelayed(_object);
            }
        }

        public static void SafeDestroyDelayed(GameObject _object, bool _nullify = true)
        {
            if (_object == null) return;
            if(_object.TryGetComponent<AWObj>(out var component))
            {
                component.StopRequestPipeline();
                component.flaggedDestroyed = true;
            }

            if (Application.isEditor && !Application.isPlaying)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall += () =>
                {

                    try
                    {
                        UnityEngine.Object.DestroyImmediate(_object);
                        if (_nullify) _object = null;
                    }
                    catch (Exception e)
                    {
                        Debug.Log($"Exception thrown while trying to safe destroy object: {e}");
                    }
                };
#endif
            }
            else
            {
                UnityEngine.Object.Destroy(_object);
                if (_nullify) _object = null;
            }
        }
        public static void SafeDestroyDelayed(AnythingWorld.AWObj _object, bool _nullify = true)
        {
            var name = _object.ModelName;
            _object.flaggedDestroyed = true;
            _object.StopRequestPipeline();
            if (Application.isEditor && !Application.isPlaying)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall += () =>
                {

                    try
                    {
                        UnityEngine.Object.DestroyImmediate(_object);
                        if (_nullify) _object = null;
                    }
                    catch (Exception e)
                    {
                        Debug.Log($"Exception thrown while trying to safe destroy object: {e}");
                    }
                };
#endif
            }
            else
            {
                UnityEngine.Object.Destroy(_object);
                if (_nullify) _object = null;
            }
        }

        /// <summary>
        /// Safe version of the Destroy call that will work during editor and runtime.
        /// Destroy does not usually work during editor mode as the call execution is different. 
        /// Destroy immediate is used at the end of the editor loop to replicate Destroy.
        /// </summary>
        /// <param name="_object"></param>
        /// <param name="_nullify"></param>
        public static void SafeDestroyDelayed(UnityEngine.Object _object, bool _nullify = true, bool debug=false)
        {
            var name = _object.name;
            if (Application.isEditor && !Application.isPlaying)
            {
#if UNITY_EDITOR

                UnityEditor.EditorApplication.delayCall += () =>
                {
                    try
                    {
                        UnityEngine.Object.DestroyImmediate(_object);
                        if (_nullify) _object = null;
                    }
                    catch(Exception e)
                    {
                        Debug.Log($"Exception thrown while trying to safe destroy object: {e}");
                    }
                };
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
#endif
            }
            else
            {
                UnityEngine.Object.Destroy(_object);
                if (_nullify) _object = null;
            }
        }

        public static void SafeDestroyDelayed(UnityEngine.Object[] _objects, bool _nullify = true)
        {
            foreach (var _object in _objects)
            {
                SafeDestroyDelayed(_object, _nullify);
            }
        }
        public static void ClearList<T>(List<T> list)
        {
            if (Application.isEditor && !Application.isPlaying)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    list.Clear();
                };
#endif
            }
            else
            {
                list.Clear();
            }
        }

        public static void DestroyListOfObjects(List<UnityEngine.Object> objects)
        {
            if (Application.isEditor && !Application.isPlaying)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    foreach(var obj in objects)
                    {
                        UnityEngine.Object.DestroyImmediate(obj);
                    }
                };
#endif
            }
            else
            {
                foreach(var obj in objects)
                {
                    UnityEngine.Object.Destroy(obj);
                }
            }
        }
        public static void DestroyListOfObjects(UnityEngine.Object[] objects)
        {
            if (Application.isEditor && !Application.isPlaying)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    foreach (var obj in objects)
                    {
                        UnityEngine.Object.DestroyImmediate(obj);
                    }
                };
#endif
            }
            else
            {
                foreach (var obj in objects)
                {
                    UnityEngine.Object.Destroy(obj);
                }
            }
        }
    }
}



