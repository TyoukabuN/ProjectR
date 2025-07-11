using System;
using System.Linq;
using PJR.Editor;
using UnityEditor;
using UnityEngine;

namespace PJR.Timeline.Editor
{
    public partial class TimelineWindow
    {
        /// <summary>
        /// Undo,Selection,Save之类的Sequence的管理都在这
        /// </summary>
        public class WindowState
        {
            public EditingSequare editingSequence;
            public WindowState()
            {
                AssetProcessor.OnWillSaveAssetsCall -= OnWillSaveAssetsCall;
                AssetProcessor.OnWillSaveAssetsCall += OnWillSaveAssetsCall;
                Undo.undoRedoPerformed -= OnUndoRedoPerformed;
                Undo.undoRedoPerformed += OnUndoRedoPerformed;
            }
            private void OnUndoRedoPerformed() => RefreshWindow(true);

            public bool IsControlBarDisabled() => !AnyEditingSequence();
            public bool AnyEditingSequence() => editingSequence.Sequence != null;
            public bool NonEditingSequence() => !AnyEditingSequence();

            /// <summary>
            /// 当前每帧所占像素(px/f)
            /// </summary>
            public float currentPixelPerFrame = Const.DefaultPixelPerFrame;
            /// <summary>
            /// 当前PixelPerFrame缩放系数
            /// </summary>
            public float currentPixelPerFrameScaleFactor = Const.DefaultPixelPerFrameScaleFactor;
            /// <summary>
            /// debugging=true会绘制一些额外的GUI
            /// </summary>
            public bool debugging = false;

            public bool requireRepaint = false;

            private TrackGUI trackGUI;
            public TrackGUI TrackGUI => trackGUI ??= new TrackGUI();

            #region 一些动态的Rect
            public float trackMenuAreaWidth = Const.trackMenuDefaultAreaWidth;
            public Rect headerSizeHandleRect;// = instance.headerSizeHandleRect;
            
            
            #endregion
            
            #region UI Hotspot相关方法
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
                Selection.activeObject = instance;
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
            public int PixelToFrame(float pixel)=>(int)(pixel / currentPixelPerFrame);
            public double PixelToSecond(float pixel)=> (int)(pixel / currentPixelPerFrame) / CurrentFrameRate;
            public float FrameToPixel(int frames)=> frames * currentPixelPerFrame;
            public float TimeToPixel(double time) => (float)(time * CurrentFrameRate * currentPixelPerFrame);

            public double CurrentFrameRate => Define.FPS_Default;
            public double CurrentSecondPerFrame => 1 / CurrentFrameRate;
            #endregion
            
            public static string Default_UndoName = "Sequence Asset Modify";


            /// <summary>
            /// 监听sequenceAsset有没有被保存
            /// </summary>
            /// <param name="paths"></param>
            private void OnWillSaveAssetsCall(string[] paths)
            {
                if (editingSequence.Sequence == null)
                    return;
                    
                var seqPath = AssetDatabase.GetAssetPath(editingSequence.Sequence);
                if (string.IsNullOrEmpty(seqPath))
                    return;
                
                if(!paths.Contains(seqPath))
                    return;
                instance.hasUnsavedChanges = false;
            }

            public void SetHasUnsavedChanges(bool boolean)
            {
                instance.hasUnsavedChanges = boolean;
                instance.Repaint();
            }

            public bool TrySetSequenceAssetDirty(string undoName = null)
            {
                if (editingSequence.Sequence == null)
                    return false;
                undoName ??= Default_UndoName;
                Undo.RecordObject(editingSequence.Sequence, undoName);
                EditorUtility.SetDirty(editingSequence.Sequence);
                instance.hasUnsavedChanges = true;
                return true;
            }
            public bool SaveSequenceAsset(string undoName = null)
            {
                if (!TrySetSequenceAssetDirty(undoName))
                    return false;
                instance.hasUnsavedChanges = false;
                AssetDatabase.SaveAssetIfDirty(editingSequence.Sequence);
                return true;
            }
            
            public void RefreshWindow(bool rightNow = false)
            {
                if (rightNow)
                {
                    instance.Repaint();
                    return;
                }
                requireRepaint = true;
            }

            public bool IsEditingAPrefabAsset() => true;
        }

        public struct EditingSequare
        {
            public static EditingSequare Empty = new() { _isEmpty = true };
            private bool _isEmpty;
            public bool IsEmpty => _isEmpty;
            public Sequence Sequence;
            public GameObject GameObject;

            public EditingSequare(Sequence sequence)
            {
                Sequence = sequence;
                _isEmpty = false;
                GameObject = null;
            }

            public bool Valid => Sequence != null && Sequence != null;
        }
    }
}
