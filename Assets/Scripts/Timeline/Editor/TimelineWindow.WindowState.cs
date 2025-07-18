using System;
using System.Linq;
using PJR.Editor;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace PJR.Timeline.Editor
{
    public partial class TimelineWindow
    {
        /// <summary>
        /// Undo,Selection,Save之类的Sequence的管理都在这
        /// </summary>
        public partial class WindowState
        {
            public WindowState()
            {
                AssetProcessor.OnWillSaveAssetsCall -= OnWillSaveAssetsCall;
                AssetProcessor.OnWillSaveAssetsCall += OnWillSaveAssetsCall;
                Undo.undoRedoPerformed -= OnUndoRedoPerformed;
                Undo.undoRedoPerformed += OnUndoRedoPerformed;
            }
            private void OnUndoRedoPerformed() => RefreshWindow(true);

            public bool IsControlBarDisabled() => !AnyEditingSequence();
            public bool AnyEditingSequence() => SequenceHandle?.Valid ?? false;
            public bool NonEditingSequence() => !AnyEditingSequence();

            /// <summary>
            /// 当前每帧所占像素(px/f)
            /// </summary>
            public float currentPixelPerFrame
            {
                get => _currentPixelPerFrame;
                set
                {
                    if()
                    _currentPixelPerFrame = value;
                }
            }
            private float _currentPixelPerFrame = Const.DefaultPixelPerFrame;

            /// <summary>
            /// 当前PixelPerFrame缩放系数，用在鼠标滚轮事件回调修改currentPixelPerFrame那
            /// </summary>
            public float currentPixelPerFrameScaleFactor = Const.DefaultPixelPerFrameScaleFactor;
            /// <summary>
            /// debugging=true会绘制一些额外的GUI
            /// </summary>
            public bool debugging = false;

            private bool _requireRepaint = false;
            public bool RequireRepaint
            {
                get=> _requireRepaint;
                set => _requireRepaint = value;
            }

            private TrackGUI trackGUI;
            public TrackGUI TrackGUI => trackGUI ??= new TrackGUI();

            #region 一些动态的Rect
            public float currentTrackMenuAreaWidth = Const.trackMenuDefaultAreaWidth;
            public Rect headerSizeHandleRect;// = instance.headerSizeHandleRect;
            #endregion
  
            public bool draggingRulerCursor = false;

            public static string Default_UndoName = "Sequence Asset Modify";

            /// <summary>
            /// 监听sequenceAsset有没有被保存
            /// </summary>
            /// <param name="paths"></param>
            private void OnWillSaveAssetsCall(string[] paths)
            {
                if (SequenceHandle.SequenceAsset == null)
                    return;
                    
                var seqPath = AssetDatabase.GetAssetPath(SequenceHandle.SequenceAsset);
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
                if (SequenceHandle.SequenceAsset == null)
                    return false;
                undoName ??= Default_UndoName;
                Undo.RecordObject(SequenceHandle.SequenceAsset, undoName);
                EditorUtility.SetDirty(SequenceHandle.SequenceAsset);
                instance.hasUnsavedChanges = true;
                return true;
            }
            public bool SaveSequenceAsset(string undoName = null)
            {
                if (!TrySetSequenceAssetDirty(undoName))
                    return false;
                instance.hasUnsavedChanges = false;
                AssetDatabase.SaveAssetIfDirty(SequenceHandle.SequenceAsset);
                return true;
            }
            
            public void RefreshWindow(bool rightNow = false)
            {
                if (rightNow)
                {
                    instance.Repaint();
                    return;
                }
                _requireRepaint = true;
            }

            public bool IsEditingAPrefabAsset() => true;
        }

        /// <summary>
        /// 一些单位转换用的方法
        /// </summary>
        public partial class WindowState
        {
            public int PixelToFrame(float pixel)=>(int)(pixel / currentPixelPerFrame);
            public int PixelRoundToFrame(float pixel)=>Mathf.RoundToInt(pixel / currentPixelPerFrame);
            public double PixelToTime(float pixel)=> (int)(pixel / currentPixelPerFrame) / CurrentFrameRate;
            public float FrameToPixel(int frames)=> frames * currentPixelPerFrame;
            public float TimeToPixel(double time) => (float)(time * CurrentFrameRate * currentPixelPerFrame);
            public int ToFrames(double time)=> TimeUtil.ToFrames(time,CurrentFrameRate);
            public double FromFrames(int frames) => TimeUtil.FromFrames(frames, CurrentFrameRate);
            public double CurrentFrameRate => Define.FPS_Default;
            public double CurrentSecondPerFrame => 1 / CurrentFrameRate;
        }

        /// <summary>
        /// Timeline GUI Hotspot
        /// </summary>
        public partial class WindowState
        {
            private TimelineGUIElement _hotspot;

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
            public TimelineGUIElement Hotspot
            {
                get => _hotspot;
                set => SetHotspot(value);
            }
        }

        /// <summary>
        /// 播放相关 
        /// </summary>
        public partial class WindowState
        {
            private ISequenceHandle _sequenceHandle;
            public ISequenceHandle SequenceHandle
            {
                get
                {
                    if ((!_sequenceHandle?.Valid) ?? false)
                        _sequenceHandle = null;
                    return _sequenceHandle ??= SequenceEditHandle.Empty;
                }
                set
                {
                    if (value == null)
                    {
                        _sequenceHandle?.Release();
                        _sequencePlayableHandle = null;
                    }
                    else if(value is ISequencePlayableHandle temp)
                        _sequencePlayableHandle = temp;

                    _sequenceHandle = value;
                }
            }

            private ISequencePlayableHandle _sequencePlayableHandle;
            public ISequencePlayableHandle SequencePlayableHandle => _sequencePlayableHandle;
            public float Time
            {
                set
                {
                    if (_sequencePlayableHandle == null)
                        return;
                    _sequencePlayableHandle.time = value;
                }
            }
            public bool IsPlaying => _sequencePlayableHandle?.IsPlaying() ?? false;
            public void Play()
            {
                if (EditorApplication.isPlaying)
                    return;
                instance._lastUpdateTime = (float)EditorApplication.timeSinceStartup;
                _sequencePlayableHandle?.Play();
            }
            public void Pause()
            {
                if (EditorApplication.isPlaying)
                    return;
                _sequencePlayableHandle?.Pause();
            }
            public void Stop()
            {
                _sequencePlayableHandle?.Stop();
                SequenceHandle = null;
            }
            public void ManualUpdateDirector(float deltaTime, bool force = false)
            {
                SequencePlayableHandle?.SequenceRunner?.OnUpdate(deltaTime,force);
            }
        }
        
        public class SequenceEditHandle : ISequenceHandle
        {
            public static SequenceEditHandle Empty = new() { _isEmpty = true };
            private bool _isEmpty = false;
            public bool IsEmpty => _isEmpty;
            private SequenceAsset _sequenceAssetAsset;
            public UnityEngine.Object Object => _sequenceAssetAsset;
            
            private float _time_test = 0;
            public float time {
                get => _time_test;
                set => _time_test = value;
            }
            public bool Valid => SequenceAsset != null && SequenceAsset != null;

            public SequenceAsset SequenceAsset
            {
                get => _sequenceAssetAsset;
                set => _sequenceAssetAsset = value;
            }
            public double ToGlobalTime(double t) => t;
            public double ToLocalTime(double t)=> t;

            public static SequenceEditHandle Get(SequenceAsset sequenceAssetAsset)
            {
                var temp = UnityEngine.Pool.GenericPool<SequenceEditHandle>.Get();
                temp._sequenceAssetAsset = sequenceAssetAsset;
                temp._isEmpty = false;
                temp._time_test = 0;
                return temp;
            }
            public void Release()
            {
                _sequenceAssetAsset = null;
                _isEmpty = false;
                _time_test = 0;
                UnityEngine.Pool.GenericPool<SequenceEditHandle>.Release(this);
            }
        }
    }
}
