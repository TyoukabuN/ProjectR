//-----------------------------------------------------------------------
// <copyright file="TempKeyValuePair.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor
{
#pragma warning disable

    [ShowOdinSerializedPropertiesInInspector]
    public class TempKeyValuePair<TKey, TValue>
    {
        [ShowInInspector]
        public TKey Key;

        [ShowInInspector]
        public TValue Value;
    }
}
#endif