using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
#if  UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR.Timeline
{
    public class Track : SerializedScriptableObject, ITrack, ISequenceUnit
    {
        [OdinSerialize]
        private List<Clip> _clips;

        // ITrack 接口实现：只读接口列表
        IReadOnlyList<IClip> ITrack.Clips => _clips as IReadOnlyList<IClip> ?? _clips;

        // 内部/Editor 用：具体类型访问
        public List<Clip> Clips
        {
            get => _clips;
            set => _clips = value;
        }
        public Clip Clip => _clips?.Count > 0 ? _clips[0] : null;
        
#if  UNITY_EDITOR
        public void Editor_MarkDirty()
        {
            if (_clips == null || _clips.Count <= 0)
                return;
            for (var i = 0; i < _clips.Count; i++)
                _clips[i].Editor_MarkDirty();
            EditorUtility.SetDirty(this);
        }
#endif
        #region ISequenceUnit Impl
        [OdinSerialize, HideInInspector]
        private SequenceAsset _sequenceAsset;
        public SequenceAsset sequenceAsset
        {
            get => _sequenceAsset;
            set => _sequenceAsset = value;
        }
        Track ISequenceUnit.Track
        {
            get => this;
            set { }
        }
        #endregion
    }
    
    public interface ITrack
    {
        public IReadOnlyList<IClip> Clips { get; }
    }
}
