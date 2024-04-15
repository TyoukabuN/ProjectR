using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using UnityEditor;

namespace PJR
{
    [CreateAssetMenu(menuName = "Custom/AnimatiomClipSet", fileName = "AnimatiomClipSet")]
    public class AnimatiomClipSet : ScriptableObject
    {
        [ShowInInspector]
        public List<KeyValuePair<string, AnimationClip>> clips = new List<KeyValuePair<string, AnimationClip>>();

        [Serializable]
        public struct KeyValuePair<TKey, TValue>
        {
            public TKey Key;
            public TValue Value;
            public static KeyValuePair<TKey, TValue> Create(TKey key, TValue value)=> new KeyValuePair<TKey, TValue> { Key = key, Value = value };
        }

        [Button("Save")]
        void Save()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}
