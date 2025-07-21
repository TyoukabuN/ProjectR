using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR.Timeline
{
    [Serializable]
    [TrackCreateMenuItem(nameof(AnimancerClip))]
    public partial class AnimancerClip : Clip
    {
        public AnimationClip animationClip;
        public override string GetClipName() => "播放动画";
        public override string GetClipInfo()
        {
            if (animationClip == null)
                return "[播放动画] null";
            return $"[播放动画] {animationClip?.name ?? string.Empty}";
        }
        public override ClipRunner GetRunner()=> Runner.Get(this);
        public override ClipRunner GetPreviewRunner()=> PreviewRunner.Get(this);

        public override void GetContextMenu(GenericMenu menu)
        {
            menu.AddDisabledItem(new GUIContent("播放动画"));
        }
    }
}
