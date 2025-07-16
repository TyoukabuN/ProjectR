#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline
{
    public static class SequenceUnitCreateHelper
    {
        public static bool CreateTrack(SequenceAsset sequenceAssetAsset,Type clipType)
        {
            var clipScriptableObject = ScriptableObject.CreateInstance(clipType);
            clipScriptableObject.name = clipType.Name;
            var clip = clipScriptableObject as Clip;
            
            if (clip == null)
            {
                Debug.Log($"创建Clip失败,[类型:{clipType.Name}]");
                return false;
            }
            //创建Track
            Track track = ScriptableObject.CreateInstance<Track>();
            track.name = "Track";
            track.sequenceAsset = sequenceAssetAsset;
            track.hideFlags |= HideFlags.HideInInspector;
            //创建clip
            clip.FrameRateType = sequenceAssetAsset.FrameRateType;
            clip.StartFrame = 0;
            clip.EndFrame = 5;
            clip.sequenceAsset = sequenceAssetAsset;
            clip.hideFlags |= HideFlags.HideInInspector;
            clip.Track = track;
            
            //内部引用
            track.clips = new List<Clip>{ clip };
            //内部SubAsset
            AssetDatabase.AddObjectToAsset(track, sequenceAssetAsset);
            AssetDatabase.AddObjectToAsset(clipScriptableObject, sequenceAssetAsset);

            sequenceAssetAsset.Tracks.Add(track);
            if(sequenceAssetAsset != null)
                EditorUtility.SetDirty(sequenceAssetAsset);
            AssetDatabase.SaveAssets();
            return true;
        }

        public static bool DeleteTrack(Track track)
        {
            if (track == null)
                return false;
            if (track.sequenceAsset == null)
                return false;
            //移除内部引用
            if (track.sequenceAsset.Tracks?.Contains(track) ?? false)
                track.sequenceAsset.Tracks.Remove(track);
            //删除ClipAsset
            foreach (var clip in track.clips)
                if (AssetDatabase.Contains(clip) && AssetDatabase.Contains(clip))
                    AssetDatabase.RemoveObjectFromAsset(clip);
            //删除TrackAsset
            if (AssetDatabase.Contains(track) && AssetDatabase.Contains(track))
                AssetDatabase.RemoveObjectFromAsset(track);
            return true;
        }
        public static bool DeleteClip(Clip clip)
        {
            if (clip == null)
                return false;
            if (clip.Track == null)
                return false;
            //移除内部引用
            if (clip.Track.clips?.Contains(clip) ?? false)
                clip.Track.clips.Remove(clip);
            //删除Asset
            if (AssetDatabase.Contains(clip) && AssetDatabase.Contains(clip))
                AssetDatabase.RemoveObjectFromAsset(clip);
            return true;
        }
    }    
}

#endif
