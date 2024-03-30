//-----------------------------------------------------------------------
// <copyright file="UndoTrackerStateContainer.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor
{
#pragma warning disable

    using Sirenix.OdinInspector.Editor.Internal;
    using Sirenix.Serialization;
    using System.Collections.Generic;
    using UnityEngine;

    [ShowOdinSerializedPropertiesInInspector]
    internal class UndoTrackerStateContainer : SessionSingletonSO<UndoTrackerStateContainer>, ISerializationCallbackReceiver
    {
        public int LatestIndex;
        public Dictionary<int, List<UnityEngine.Object>> UndoGroups = new Dictionary<int, List<UnityEngine.Object>>();

        [HideInInspector]
        private SerializationData serializationData;

        public void OnAfterDeserialize()
        {
            UnitySerializationUtility.DeserializeUnityObject(this, ref this.serializationData);
        }

        public void OnBeforeSerialize()
        {
            UnitySerializationUtility.SerializeUnityObject(this, ref this.serializationData);
        }
    }
}
#endif