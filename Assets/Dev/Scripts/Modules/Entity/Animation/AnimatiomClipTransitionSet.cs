using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using Animancer;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR
{
    //[CreateAssetMenu(menuName = "Custom/AnimatiomClipTransitionSet", fileName = "AnimatiomClipTransitionSet")]
    public class AnimatiomClipTransitionSet : ScriptableObject
    {
        [ShowInInspector]
        public List<AnimatiomClipSet.KeyValuePair<string, ClipTransition>> clips = new List<AnimatiomClipSet.KeyValuePair<string, ClipTransition>>();

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Custom/AnimatiomClipTransitionSet")]
        public static void CreateAnimatiomClipTransitionSet()
        {
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (AssetDatabase.IsValidFolder(assetPath))
            {
                ScriptableObject asset = ScriptableObject.CreateInstance<AnimatiomClipTransitionSet>();
                var uniqueFileName = AssetDatabase.GenerateUniqueAssetPath(assetPath + "/AnimatiomClipTransitionSet.asset");
                UnityEditor.AssetDatabase.CreateAsset(asset, uniqueFileName);
                return;
            }
            //从clipset中创建
            var clipSet = AssetDatabase.LoadAssetAtPath<AnimatiomClipSet>(assetPath) as AnimatiomClipSet;
            if (clipSet != null && clipSet.clips != null)
            {
                AnimatiomClipTransitionSet asset = ScriptableObject.CreateInstance<AnimatiomClipTransitionSet>();
                asset.clips = new List<AnimatiomClipSet.KeyValuePair<string, ClipTransition>>();
                foreach (var pair in clipSet.clips)
                {
                    asset.clips.Add(AnimatiomClipSet.KeyValuePair<string, ClipTransition>.Create(pair.Key, new ClipTransition { Clip = pair.Value }));
                }
                var uniqueFileName = AssetDatabase.GenerateUniqueAssetPath(Path.GetDirectoryName(assetPath) + "/AnimatiomClipTransitionSet.asset");
                UnityEditor.AssetDatabase.CreateAsset(asset, uniqueFileName);
                return;
            }
        }
#endif
    }
}