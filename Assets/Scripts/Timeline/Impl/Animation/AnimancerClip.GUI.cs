using PJR.Timeline.Editor;
#if UNITY_EDITOR
#endif

namespace PJR.Timeline
{
    public class AnimancerClip_ClipGUI : ClipGUI
    {
        public override ClipDrawer ClipDrawer => new AnimancerClipDrawer(Clip);
    }
    public class AnimancerClipDrawer : ClipDrawer
    {
        public AnimancerClipDrawer(IClip clip) : base(clip)
        {
        }
    }
}
