using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Custom/AnimatiomClipSet", fileName = "AnimatiomClipSet")]
public class AnimatiomClipSet : SerializedScriptableObject
{
    [NonSerialized, OdinSerialize] 
    [DictionaryDrawerSettings(KeyLabel = "Name", ValueLabel = "Clip")]
    public Dictionary<string, AnimationClip> clips = new Dictionary<string, AnimationClip>();
}
