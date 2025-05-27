using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline.Editor
{
    public partial class TimelineWindow
    {
        private void Selection_OnSelectionChange()
        {
            Selection_CheckSelectionChange();
        }

        public void Selection_CheckSelectionChange()
        {
            if (instance?.state == null)
                return;

            //from ProjectWindow
            SequenceAsset sequenceAsset = Selection.activeObject as SequenceAsset;
            if (sequenceAsset != null)
            {
                Selection_OnSelectedSequenceAsset(sequenceAsset);
                return;
            }

            if (sequenceAsset == null)
            {
                //可能有没选中时候的处理
            }

            //需要加个从Hierarchy选中的功能
        }

        public void Selection_OnSelectedSequenceAsset(SequenceAsset sequenceAsset)
        {
            if (sequenceAsset == null)
                return;
            if (!state.editingSequence.IsEmpty && state.editingSequence.SequenceAsset == sequenceAsset)
                return;
            state.editingSequence = new EditingSequare(sequenceAsset);
            Repaint();
        }
    }
}
