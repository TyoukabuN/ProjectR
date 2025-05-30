using System;
using PJR.Editor;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline.Editor
{
    public partial class TimelineWindow
    {
        public class WindowState
        {
            public EditingSequare editingSequence;
            public bool DisableControlBar() => !AnyEditingSequence();
            public bool AnyEditingSequence() => editingSequence.Sequence != null;
            public bool NonEditingSequence() => !AnyEditingSequence();

            /// <summary>
            /// 时间轴的放大缩小
            /// </summary>
            public int currentPixelPerFrame = Constants.pixelPerFrame;
            /// <summary>
            /// debugging=true会绘制一些额外的GUI
            /// </summary>
            public bool debugging = true;

            public bool requireRepaint = false;

            private TrackGUI trackGUI;
            public TrackGUI TrackGUI => trackGUI ??= new TrackGUI();

#region 一些动态的Rect
            public float trackMenuAreaWidth = Constants.trackMenuDefaultAreaWidth;
            public Rect headerSizeHandleRect;// = instance.headerSizeHandleRect;
#endregion


#region 一些hotspot
            public void ClearHotspot()
            {
                Hotspot?.OnDeselect();
                Hotspot = null;
            }
            
            public Action<TimelineGUIElement, TimelineGUIElement> OnHotspotChanged;
            public void SetHotspot(TimelineGUIElement timelineGUIElement)
            {
                if (timelineGUIElement != null && timelineGUIElement == _hotspot)
                    return;
                OnHotspotChanged?.Invoke(_hotspot,timelineGUIElement);
                _hotspot = timelineGUIElement;
            }
            public void UnSetHotspot(TimelineGUIElement timelineGUIElement)
            {
                if (timelineGUIElement == null)
                    return;
                if (timelineGUIElement != _hotspot)
                    return;
                timelineGUIElement.OnDeselect();
                _hotspot = null;
            }
            private TimelineGUIElement _hotspot;
            public TimelineGUIElement Hotspot
            {
                get => _hotspot;
                set => SetHotspot(value);
            }

            #endregion

            #region 一些单位转换用的方法
            public int PixelToFrame(float pixel)
            {
                return (int)(pixel / currentPixelPerFrame);
            }
            public double PixelToSecond(float pixel)
            {
                return (int)(pixel / currentPixelPerFrame) / CurrentFrameRate;
            }
            public float FrameToPixel(int frames)
            {
                return frames * currentPixelPerFrame;
            }

            public double CurrentFrameRate
            {
                get
                {
                    return Define.FPS_Default;
                }
            }
            public double CurrentSecondPerFrame => 1 / CurrentFrameRate;
            #endregion

            
            public static string Default_UndoName = "Sequence Asset Modify";

            public WindowState()
            {
                AssetProcessor.OnWillSaveAssetsCall -= OnWillSaveAssetsCall;
                AssetProcessor.OnWillSaveAssetsCall += OnWillSaveAssetsCall;
            }

            private void OnWillSaveAssetsCall(string[] paths)
            {
                Debug.Log("OnWillSaveAssetsCall");
                foreach (var path in paths)
                {
                    Debug.Log(path);
                }
            }

            public bool TrySetSequenceAssetDirty(string undoName = null)
            {
                if (editingSequence.SequenceAsset == null)
                    return false;
                undoName ??= Default_UndoName;
                Undo.RecordObject(editingSequence.SequenceAsset, undoName);
                EditorUtility.SetDirty(editingSequence.SequenceAsset);
                instance.hasUnsavedChanges = true;
                return true;
            }
        }

        public struct EditingSequare
        {
            public static EditingSequare Empty = new() { _isEmpty = true };
            private bool _isEmpty;
            public bool IsEmpty => _isEmpty;
            public Sequence Sequence;
            public SequenceAsset SequenceAsset;
            public GameObject GameObject;

            public EditingSequare(SequenceAsset sequenceAsset)
            {
                SequenceAsset = sequenceAsset;
                Sequence = sequenceAsset.Sequence;
                _isEmpty = false;
                GameObject = null;
            }

            public bool Valid
            {
                get=>  Sequence != null && SequenceAsset != null;
            }


           
        }

        public struct HotSpot
        {
            public static HotSpot Empty => new(){};
            private bool _IsEmpty;
            public bool IsEmpty => _IsEmpty;

        }
    }
}
