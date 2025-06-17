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

        /// <summary>
        /// 检测Selection.activeObject，看能不能从中获取到SequenceAsset
        /// 然后再转成SequenceHandle使用
        /// </summary>
        public void Selection_CheckSelectionChange()
        {
            if (instance?.State == null)
                return;
            if (Selection.activeObject == null)
                return;
            ISequenceHandle handle = null;
            if (IsSequenceAssetSelected(Selection.activeObject, out handle))
            {
                //选中Project里的SequenceAsset
            }
            else if (IsGameObjectSelected(Selection.activeObject, out handle))
            {
                //选中Hierarchy里的MonoSequenceHandle
            }

            if (handle == null)
            {
                //可能有没选中时候的处理
                return;
            }
            Selection_OnSelectedSequenceAsset(handle.Sequence);
        }

        static bool IsSequenceAssetSelected(Object activeObject,out ISequenceHandle handle)
        {
            handle = null;
            if (activeObject is not Sequence)
                return false;
            var sequenceAsset = Selection.activeObject as Sequence;
            if (sequenceAsset == null)
                return false;
            handle = new SequenceHandle(sequenceAsset);
            return true;
        }
        
        static bool IsGameObjectSelected(Object activeObject,out ISequenceHandle handle)
        {
            handle = null;
            if (activeObject is not GameObject)
                return false;
            var gameObject = activeObject as GameObject;
            var director = gameObject?.GetComponent<SequenceDirector>();
            if (director == null)
                return false;
            handle = director;
            return true;
        }

        public static void Selection_OnSelectedSequenceAsset(SequenceHandle sequenceHandle)=>  Selection_OnSelectedSequenceAsset(sequenceHandle.Sequence);
        public static void Selection_OnSelectedSequenceAsset(Sequence sequenceAsset)
        {
            if (sequenceAsset == null)
                return;
            if (instance == null)
                return;
            if (!instance.State.editingSequence.IsEmpty && instance.State.editingSequence.Sequence == sequenceAsset)
                return;
            instance.State.editingSequence = new EditingSequare(sequenceAsset);
            instance.Repaint();
        }
    }
}
