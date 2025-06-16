using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline
{
    public static class SequenceUndo
    {
        public static bool undoEnabled = true;

        private static string UndoName_Prefix = "PJR.Timeline";
        private static string UndoName(string name) => $"{UndoName_Prefix} {name}";
        
        
        [Conditional("UNITY_EDITOR")]
        public static void PushUndo(Object[] thingsToDirty, string operation)
        {
#if UNITY_EDITOR
            if (thingsToDirty == null || !undoEnabled)
                return;

            for (var i = 0; i < thingsToDirty.Length; i++)
            {
                if (thingsToDirty[i] is Track track)
                    track.MarkDirty();
                EditorUtility.SetDirty(thingsToDirty[i]);
            }

            Undo.RegisterCompleteObjectUndo(thingsToDirty, UndoName(operation));
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void PushUndo(Object thingToDirty, string operation)
        {
#if UNITY_EDITOR
            if (thingToDirty != null && undoEnabled)
            {
                var track = thingToDirty as Track;
                if (track != null)
                    track.MarkDirty();

                EditorUtility.SetDirty(thingToDirty);
                Undo.RegisterCompleteObjectUndo(thingToDirty, UndoName(operation));
            }
#endif
        }
    }
}
