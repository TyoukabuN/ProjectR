using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace PJR.Timeline.Editor
{
    [InitializeOnLoad]
    public static class Shortcut
    {
        public const string play = "PJR/Timeline/Play";

        private static TimelineWindow.WindowState State => TimelineWindow.instance?.State;
        
        [Shortcut(play, typeof(TimelineWindow), KeyCode.Space)]
        public static void Play(ShortcutArguments args)
        {
            if (State == null) 
                return;
            if (State.SequencePlayableHandle == null)
                return;
            if(State.SequencePlayableHandle.IsPlaying())
                State.SequencePlayableHandle.Pause();
            else
                State.SequencePlayableHandle.Play();
        }
    }
}