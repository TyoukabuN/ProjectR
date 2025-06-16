using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline
{
    public class Track : SerializedScriptableObject, ISequenceUnit
    {
        [OdinSerialize]
        private List<Clip> _clips;
        public List<Clip> clips
        {
            get => _clips;
            set=> _clips = value;
        }
        
        public Clip Clip => _clips?.Count > 0 ? _clips[0] : null;
        
        public void MarkDirty()
        {
            if (_clips == null || _clips.Count <= 0)
                return;
            for (var i = 0; i < _clips.Count; i++)
                _clips[i].MarkDirty();
            EditorUtility.SetDirty(this);
        }
        #region ISequenceUnit Impl
        [OdinSerialize, HideInInspector]
        private Sequence _sequence;
        public Sequence Sequence
        {
            get => _sequence;
#if UNITY_EDITOR
            set => _sequence = value;
#endif
        }
        Track ISequenceUnit.Track
        {
            get => this;
            set { }
        }
        #endregion
    }
}


