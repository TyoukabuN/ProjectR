//-----------------------------------------------------------------------
// <copyright file="SerializedScriptableObject.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector
{
#pragma warning disable

    using Sirenix.Serialization;
    using UnityEngine;

    /// <summary>
    /// A Unity ScriptableObject which is serialized by the Sirenix serialization system.
    /// </summary>
    [Sirenix.OdinInspector.ShowOdinSerializedPropertiesInInspector]
    public abstract class SerializedScriptableObject : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector]
        private SerializationData serializationData;

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
#if UNITY_EDITOR
            HunterSerializationContext.currentDeserializingObjectContext = this;
#endif
#if LS_PROFILING
            UnityEngine.Profiling.Profiler.BeginSample($"Odin Deserailize SSO Object :{GetType().Name}");
#endif

            UnitySerializationUtility.DeserializeUnityObject(this, ref this.serializationData);
            this.OnAfterDeserialize();
#if LS_PROFILING
            UnityEngine.Profiling.Profiler.EndSample();
#endif
#if UNITY_EDITOR
            if (HunterSerializationContext.currentDeserializingObjectContext == this)
            {
                HunterSerializationContext.currentDeserializingObjectContext = null;
            }
#endif
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR
            HunterSerializationContext.currentSerializingObjectContext = this;
#endif
#if LS_PROFILING
            UnityEngine.Profiling.Profiler.BeginSample($"Odin Serialize SSO Object :{GetType().Name}");
#endif
            this.OnBeforeSerialize();
            UnitySerializationUtility.SerializeUnityObject(this, ref this.serializationData);
#if LS_PROFILING
            UnityEngine.Profiling.Profiler.EndSample();
#endif
#if UNITY_EDITOR
            if (HunterSerializationContext.currentSerializingObjectContext == this)
            {
                HunterSerializationContext.currentSerializingObjectContext = null;
            }
#endif
        }

        /// <summary>
        /// Invoked after deserialization has taken place.
        /// </summary>
        protected virtual void OnAfterDeserialize()
        {
        }

        /// <summary>
        /// Invoked before serialization has taken place.
        /// </summary>
        protected virtual void OnBeforeSerialize()
        {
        }

#if UNITY_EDITOR

        [HideInTables]
        [OnInspectorGUI, PropertyOrder(int.MinValue)]
        private void InternalOnInspectorGUI()
        {
            EditorOnlyModeConfigUtility.InternalOnInspectorGUI(this);
        }

#endif
    }
}