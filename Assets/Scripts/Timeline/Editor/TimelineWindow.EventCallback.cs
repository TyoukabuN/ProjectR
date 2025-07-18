using UnityEditor;

namespace PJR.Timeline.Editor
{
    public partial class TimelineWindow
    {
        void OnPlayModeStateChanged(PlayModeStateChange playModeState)
        {
            // case 923506 - make sure we save view data before switching modes
            if (playModeState == PlayModeStateChange.ExitingEditMode ||
                playModeState == PlayModeStateChange.ExitingPlayMode)
            {
            }

            bool isPlaymodeAboutToChange = playModeState == PlayModeStateChange.ExitingEditMode || playModeState == PlayModeStateChange.ExitingPlayMode;

            // Important to stop the graph on any director so temporary objects are properly cleaned up
            if (isPlaymodeAboutToChange && State != null)
                State.Stop();
        }

        private float? _lastUpdateTime = 0;
        private void OnEditorUpdate()
        {
            if (EditorApplication.isPlaying)
                return;
            if (State.SequencePlayableHandle?.Director?.SequenceRunner == null)
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