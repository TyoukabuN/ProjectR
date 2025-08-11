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
            if (instance == null)
                return;
            if (Selection.activeObject == null)
                return;
            Selection_CheckSpecificObject(Selection.activeObject);
        }
        
        public void Selection_CheckSpecificObject(Object obj)
        {
            ISequenceHandle handle = null;
            if (IsSequenceAssetSelected(obj, out handle))
            {
                //选中Project里的SequenceAsset
            }
            if (handle == null)
            {
                //可能有没选中时候的处理
                return;
            }
            
            instance.State.SequenceHandle = handle;
            instance.State.RefreshWindow(true);
        }


        static bool IsSequenceAssetSelected(Object activeObject,out ISequenceHandle holder)
        {
            holder = null;
            if (activeObject is not SequenceAsset)
                return false;
            var sequenceAsset = Selection.activeObject as SequenceAsset;
            if (sequenceAsset == null)
                return false;
            holder = SequenceEditHandle.Get(sequenceAsset);
            return true;
        }
        
        static bool IsGameObjectSelected(Object activeObject,out ISequenceHandle holder)
        {
            holder = null;
            if (activeObject is not GameObject)
                return false;
            var gameObject = activeObject as GameObject;
            var director = gameObject.GetComponent<SequenceDirector>();
            if (director == null)
                return false;
            if (director.Sequence == null)
            {
                UnityEngine.Debug.LogWarning("SequenceDirector没有有效的Sequence");
                return false;
            }
            holder = director.GetHandle();
            return true;
        }
        public void Selection_TrySelectObject(Object obj)
        {
            ISequenceHandle handle = null;

            if (!Selection_TryGetSequenceHandle(obj, out handle))
                return;
            
            instance.State.SequenceHandle = handle;
            instance.State.RefreshWindow(true);
        }
        public bool Selection_TryGetSequenceHandle(Object obj,out ISequenceHandle handle)
        {
            if (IsSequenceAssetSelected(obj, out handle))
                return true;
            if (IsGameObjectSelected(obj, out handle))
                return true;
            return false;
        }

        public static bool Selection_IsObjectFocused<T>(T obj) where T : Object
        {
            if (ReferenceEquals(obj,null))
                return false;
            if (instance == null)
                return false;
            return instance.State.SequenceHandle?.Object == obj;
        }
    }
}
