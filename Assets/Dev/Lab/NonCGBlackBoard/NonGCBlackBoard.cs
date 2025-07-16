using System;
using PJR.Core.BlackBoard.CachedValueBoard;
using PJR.BlackBoard.Inspector;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class NonGCBlackBoard : SerializedMonoBehaviour, ICachedValueBoardHolder
{
    public Color color;
    [TitleGroup("黑板1"),OdinSerialize]
    [HideReferenceObjectPicker]
    private CacheableValueBoard cacheableValueBoard;
    
    [TitleGroup("黑板2"),OdinSerialize]
    [HideReferenceObjectPicker]
    private CacheableValueBoard cacheableValueBoard2;
    
    [TitleGroup("黑板Field测试")]
    [NonSerialized,OdinSerialize,GenericTypeFilter]
    [HideReferenceObjectPicker,Button]
    public CacheableField<string> StringValue;
        
    CacheableValueBoard ICachedValueBoardHolder.GetCachedValueBoard() => cacheableValueBoard;

    [TitleGroup("黑板覆写测试")]
    public ICachedValueBoardHolder TargetBoard;
    [Button,TitleGroup("黑板覆写测试")]
    public void Test()
    {
        if (TargetBoard == null)
            return;
        CacheableField<int>.GetFiTypeFilter();
        Debug.Log(cacheableValueBoard?.OverrideTo(TargetBoard));
    }

    // private bool _doRuntimeGCTest;
    // [Button, ShowIf("@EditorApplication.isPlaying"),TitleGroup("黑板测试")]
    // public void RuntimeTest()
    // {
    //     _doRuntimeGCTest = true;
    // }
    // private void Update()
    // {
    //     if (_doRuntimeGCTest)
    //     {
    //         using (new ProfileScope("BoardGCTest"))
    //             cacheableValueBoard?.OverrideTo(TargetBoard);
    //     }
    // }
}
