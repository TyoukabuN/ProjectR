using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline.Editor
{
    public partial class TimelineWindow
    {
        private void OnSelectionChange()
        {
            CheckSelectionChange();
        }

        public void CheckSelectionChange()
        {
            if (instance?.state == null)
                return;

            //from ProjectWindow
            SequenceAsset asset = Selection.activeObject as SequenceAsset;
            if (asset != null)
            {
                state.editingSequence = new EditingSequare()
                {
                    Asset = asset,
                    Sequence = asset.Sequence,
                };
                Repaint();
                return;
            }

            if (asset == null)
            {
                state.editingSequence = EditingSequare.Empty;
                Repaint();
                return;
            }
            //from Hierarchy
        }
    }
}
