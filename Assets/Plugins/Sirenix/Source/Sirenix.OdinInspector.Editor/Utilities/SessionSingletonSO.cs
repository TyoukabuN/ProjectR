//-----------------------------------------------------------------------
// <copyright file="UndoTracker.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Internal
{
#pragma warning disable

    using Sirenix.Utilities;
    using UnityEditor;
    using UnityEngine;

    public abstract class SessionSingletonSO<T> : ScriptableObject 
        where T : SessionSingletonSO<T>
    {
        public static string Key = $"session_so_{typeof(T).GetNiceName()}";

        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    int instanceID = SessionState.GetInt(Key, 0);

                    if (instanceID == 0)
                    {
                        instance = ScriptableObject.CreateInstance<T>();
                        instance.hideFlags = HideFlags.HideAndDontSave;
                        SessionState.SetInt(Key, instance.GetInstanceID());
                    }
                    else
                    {
                        instance = EditorUtility.InstanceIDToObject(instanceID) as T;

                        if (instance == null)
                        {
#if SIRENIX_INTERNAL
                            Debug.LogError($"Failed to persist session singleton SO {typeof(T).GetNiceName()} from instance ID " + instanceID);
#endif
                            
                            var instances = Resources.FindObjectsOfTypeAll<T>();
                            
                            if (instances != null && instances.Length > 0)
                            {
                                instance = instances[0];
                            }
                        }

                        if (instance == null)
                        {
#if SIRENIX_INTERNAL
                            Debug.LogError($"All session singleton fallbacks failed as no objects of that type were loaded at all; creating new instance of {typeof(T).GetNiceName()}");
#endif

                            instance = ScriptableObject.CreateInstance<T>();
                            instance.hideFlags = HideFlags.HideAndDontSave;
                            SessionState.SetInt(Key, instance.GetInstanceID());
                        }
                    }
                }

                return instance;
            }
        }
    }
}
#endif