using UnityEditor;

namespace PJR.Timeline.Editor
{
    public partial class TimelineWindow
    {
        //进入PlayMode之前清理下，预览相关的
        void OnPlayModeStateChanged(PlayModeStateChange playModeState)
        {
            if (playModeState == PlayModeStateChange.ExitingEditMode 
                || playModeState == PlayModeStateChange.ExitingPlayMode)
            {
            }

            bool isPlaymodeAboutToChange = 
                playModeState == PlayModeStateChange.ExitingEditMode 
                || playModeState == PlayModeStateChange.ExitingPlayMode;

            if (isPlaymodeAboutToChange && State != null)
                State.Stop();
        }

        //EditMode下的Update tick
        private float? _lastUpdateTime = 0;
        private void OnEditorUpdate()
        {
            if (EditorApplication.isPlaying)
                return;
            if (State.SequencePlayableHandle?.Director?.Runner == null)
                return;
            if (_lastUpdateTime == null)
                _lastUpdateTime = (float)EditorApplication.timeSinceStartup;
            float currentTime = (float)EditorApplication.timeSinceStartup;
            var deltaTime = currentTime - _lastUpdateTime.Value;
            _lastUpdateTime = currentTime;
            State.ManualUpdateDirector(deltaTime);
        }
    }
}