using UnityEditor;
using UnityEngine;

namespace PJR.Timeline
{
    public interface IClip
    {
        Define.EFrameRate FrameRateType { get; set; }
        bool Mute { get; set; }
        string Description { get; set; }
        double start { get; set; }
        double end { get; set; }
        double duration { get; }
        int TotalFrame { get; }
        int StartFrame { get; set;}
        int EndFrame { get; set;}
        string GetClipName();
        string GetClipInfo();
        public ClipRunner GetRunner();
        public Color GetClipColor();
#if UNITY_EDITOR
        public ClipRunner Editor_GetPreviewRunner();
        public void GetContextMenu(GenericMenu menu){}
#endif
    }
}