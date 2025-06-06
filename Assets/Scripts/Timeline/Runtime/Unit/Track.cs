using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJR.Timeline
{
    public class Track : SequenceScriptableObject
    {
        [SerializeField]
        private List<Clip> _clips;
        public List<Clip> clips
        {
            get => _clips;
            set=> _clips = value;
        }
        
        public Clip Clip => _clips?.Count > 0 ? _clips[0] : null;
    }
}
