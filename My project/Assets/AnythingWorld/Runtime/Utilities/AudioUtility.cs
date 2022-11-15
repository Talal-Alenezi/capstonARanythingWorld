using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AnythingWorld.Utilities
{
    public static class AudioUtility
    {
#if UNITY_EDITOR

        public static void PlayClip(AudioClip clip, int startSample, bool loop)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "PlayClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[] {
                typeof(AudioClip),
                typeof(Int32),
                typeof(Boolean)
            },
            null
            );
            method.Invoke(
                null,
                new object[] {
                clip,
                startSample,
                loop
            }
            );
        }

        public static void PlayClip(AudioClip clip, int startSample)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "PlayClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[] {
                typeof(AudioClip),
                typeof(Int32)
            },
            null
            );
            method.Invoke(
                null,
                new object[] {
                clip,
                startSample
            }
            );
        }

        public static void PlayClip(AudioClip clip)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "PlayClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[] {
                typeof(AudioClip)
            },
            null
            );
            method.Invoke(
                null,
                new object[] {
                clip
            }
            );
        }

        public static void StopClip(AudioClip clip)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "StopClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[] {
                typeof(AudioClip)
            },
            null
            );
            method.Invoke(
                null,
                new object[] {
                clip
            }
            );
        }

        public static void PauseClip(AudioClip clip)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "PauseClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[] {
                typeof(AudioClip)
            },
            null
            );
            method.Invoke(
                null,
                new object[] {
                clip
            }
            );
        }

        public static void ResumeClip(AudioClip clip)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "ResumeClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[] {
                typeof(AudioClip)
            },
            null
            );
            method.Invoke(
                null,
                new object[] {
                clip
            }
            );
        }

        public static void LoopClip(AudioClip clip, bool on)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "LoopClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[] {
                typeof(AudioClip),
                typeof(bool)
            },
            null
            );
            method.Invoke(
                null,
                new object[] {
                clip,
                on
            }
            );
        }

        public static bool IsClipPlaying(AudioClip clip)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "IsClipPlaying",
                BindingFlags.Static | BindingFlags.Public
                );

            var playing = (bool)method.Invoke(
                null,
                new object[] {
                clip,
            }
            );

            return playing;
        }

        public static void StopAllClips()
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "StopAllClips",
                BindingFlags.Static | BindingFlags.Public
                );

            method.Invoke(
                null,
                null
                );
        }

        public static float GetClipPosition(AudioClip clip)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "GetClipPosition",
                BindingFlags.Static | BindingFlags.Public
                );

            var position = (float)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return position;
        }

        public static int GetClipSamplePosition(AudioClip clip)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "GetClipSamplePosition",
                BindingFlags.Static | BindingFlags.Public
                );

            var position = (int)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return position;
        }

        public static void SetClipSamplePosition(AudioClip clip, int iSamplePosition)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "SetClipSamplePosition",
                BindingFlags.Static | BindingFlags.Public
                );

            method.Invoke(
                null,
                new object[] {
                clip,
                iSamplePosition
            }
            );
        }

        public static int GetSampleCount(AudioClip clip)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "GetSampleCount",
                BindingFlags.Static | BindingFlags.Public
                );

            var samples = (int)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return samples;
        }

        public static int GetChannelCount(AudioClip clip)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "GetChannelCount",
                BindingFlags.Static | BindingFlags.Public
                );

            var channels = (int)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return channels;
        }

        public static int GetBitRate(AudioClip clip)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "GetChannelCount",
                BindingFlags.Static | BindingFlags.Public
                );

            var bitRate = (int)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return bitRate;
        }

        public static int GetBitsPerSample(AudioClip clip)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "GetBitsPerSample",
                BindingFlags.Static | BindingFlags.Public
                );

            var bits = (int)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return bits;
        }

        public static int GetFrequency(AudioClip clip)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "GetFrequency",
                BindingFlags.Static | BindingFlags.Public
                );

            var frequency = (int)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return frequency;
        }

        public static int GetSoundSize(AudioClip clip)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "GetSoundSize",
                BindingFlags.Static | BindingFlags.Public
                );

            var size = (int)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return size;
        }

        public static Texture2D GetWaveForm(AudioClip clip, int channel, float width, float height)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "GetWaveForm",
                BindingFlags.Static | BindingFlags.Public
                );

            var path = AssetDatabase.GetAssetPath(clip);
            var importer = (AudioImporter)AssetImporter.GetAtPath(path);

            var texture = (Texture2D)method.Invoke(
                null,
                new object[] {
                clip,
                importer,
                channel,
                width,
                height
            }
            );

            return texture;
        }

        public static Texture2D GetWaveFormFast(AudioClip clip, int channel, int fromSample, int toSample, float width, float height)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "GetWaveFormFast",
                BindingFlags.Static | BindingFlags.Public
                );

            var texture = (Texture2D)method.Invoke(
                null,
                new object[] {
                clip,
                channel,
                fromSample,
                toSample,
                width,
                height
            }
            );

            return texture;
        }

        public static void ClearWaveForm(AudioClip clip)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "ClearWaveForm",
                BindingFlags.Static | BindingFlags.Public
                );

            method.Invoke(
                null,
                new object[] {
                clip
            }
            );
        }

        public static bool HasPreview(AudioClip clip)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "GetSoundSize",
                BindingFlags.Static | BindingFlags.Public
                );

            var hasPreview = (bool)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return hasPreview;
        }

        public static bool IsCompressed(AudioClip clip)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "IsCompressed",
                BindingFlags.Static | BindingFlags.Public
                );

            var isCompressed = (bool)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return isCompressed;
        }

        public static bool IsStramed(AudioClip clip)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "IsStramed",
                BindingFlags.Static | BindingFlags.Public
                );

            var isStreamed = (bool)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return isStreamed;
        }

        public static double GetDuration(AudioClip clip)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "GetDuration",
                BindingFlags.Static | BindingFlags.Public
                );

            var duration = (double)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return duration;
        }

        public static int GetFMODMemoryAllocated()
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "GetFMODMemoryAllocated",
                BindingFlags.Static | BindingFlags.Public
                );

            var memoryAllocated = (int)method.Invoke(
                null,
                null
            );

            return memoryAllocated;
        }

        public static float GetFMODCPUUsage()
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "GetFMODCPUUsage",
                BindingFlags.Static | BindingFlags.Public
                );

            var cpuUsage = (float)method.Invoke(
                null,
                null
                );

            return cpuUsage;
        }

        public static bool Is3D(AudioClip clip)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "Is3D",
                BindingFlags.Static | BindingFlags.Public
                );

            var is3D = (bool)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return is3D;
        }

        public static bool IsMovieAudio(AudioClip clip)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "IsMovieAudio",
                BindingFlags.Static | BindingFlags.Public
                );

            var isMovieAudio = (bool)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return isMovieAudio;
        }

        public static bool IsMOD(AudioClip clip)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "IsMOD",
                BindingFlags.Static | BindingFlags.Public
                );

            var isMOD = (bool)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return isMOD;
        }

        public static int GetMODChannelCount()
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "GetMODChannelCount",
                BindingFlags.Static | BindingFlags.Public
                );

            var channels = (int)method.Invoke(
                null,
                null
            );

            return channels;
        }

        public static AnimationCurve GetLowpassCurve(AudioLowPassFilter lowPassFilter)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "GetLowpassCurve",
                BindingFlags.Static | BindingFlags.Public
                );

            var curve = (AnimationCurve)method.Invoke(
                null,
                new object[] {
                lowPassFilter
            }
            );

            return curve;
        }

        public static Vector3 GetListenerPos()
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "GetListenerPos",
                BindingFlags.Static | BindingFlags.Public
                );

            var position = (Vector3)method.Invoke(
                null,
                null
            );

            return position;
        }

        public static void UpdateAudio()
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "UpdateAudio",
                BindingFlags.Static | BindingFlags.Public
                );

            method.Invoke(
                null,
                null
                );
        }

        public static void SetListenerTransform(Transform t)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "SetListenerTransform",
                BindingFlags.Static | BindingFlags.Public
                );

            method.Invoke(
                null,
                new object[] {
                t
            }
            );
        }

        public static AudioType GetClipType(AudioClip clip)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "GetClipType",
                BindingFlags.Static | BindingFlags.Public
                );

            var type = (AudioType)method.Invoke(
                null,
                new object[] {
                clip
            }
            );

            return type;
        }

        public static bool HaveAudioCallback(MonoBehaviour behaviour)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "HaveAudioCallback",
                BindingFlags.Static | BindingFlags.Public
                );

            var hasCallback = (bool)method.Invoke(
                null,
                new object[] {
                behaviour
            }
            );

            return hasCallback;
        }

        public static int GetCustomFilterChannelCount(MonoBehaviour behaviour)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "GetCustomFilterChannelCount",
                BindingFlags.Static | BindingFlags.Public
                );

            var channels = (int)method.Invoke(
                null,
                new object[] {
                behaviour
            }
            );

            return channels;
        }

        public static int GetCustomFilterProcessTime(MonoBehaviour behaviour)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "GetCustomFilterProcessTime",
                BindingFlags.Static | BindingFlags.Public
                );

            var processTime = (int)method.Invoke(
                null,
                new object[] {
                behaviour
            }
            );

            return processTime;
        }

        public static float GetCustomFilterMaxIn(MonoBehaviour behaviour, int channel)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "GetCustomFilterMaxIn",
                BindingFlags.Static | BindingFlags.Public
                );

            var maxIn = (float)method.Invoke(
                null,
                new object[] {
                behaviour,
                channel
            }
            );

            return maxIn;
        }

        public static float GetCustomFilterMaxOut(MonoBehaviour behaviour, int channel)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "GetCustomFilterMaxOut",
                BindingFlags.Static | BindingFlags.Public
                );

            var maxOut = (float)method.Invoke(
                null,
                new object[] {
                behaviour,
                channel
            }
            );

            return maxOut;
        }

#endif
    }
}