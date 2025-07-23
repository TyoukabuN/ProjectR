using System;
using PJR.Core.BlackBoard.CachedValueBoard;
using PJR.BlackBoard.Inspector;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

/// <summary>
/// 这个类是用来展示怎样使用黑板的<br/>
/// 可以看到怎样定义一个黑板<br/>
/// 可以看到怎样定义一个CacheableField<br/>
/// 它们需要什么Attrubute标记<br/>
/// BBOverrideToTest可以看到怎样进行黑板的覆盖
/// </summary>
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
    [NonSerialized,OdinSerialize]//禁用掉unity的序列化,只用Odin序列化,因为有些字段用到内部引用
    [GenericTypeFilter]
    [HideReferenceObjectPicker]
    public CacheableField<string> StringValue;
        
    CacheableValueBoard ICachedValueBoardHolder.GetCachedValueBoard() => cacheableValueBoard;

    [TitleGroup("黑板覆写测试")]
    public ICachedValueBoardHolder TargetBoard;
    [Button,TitleGroup("黑板覆写测试")]
    public void BBOverrideToTest()
    {
        if (TargetBoard == null)
            return;
        Debug.Log(cacheableValueBoard?.OverrideTo(TargetBoard));
    }

    [Button]
    public void Test2()
    {
        string str = StringValue; 
        Debug.Log($"str: {str}");
        Debug.Log($"StringValue.Value: {StringValue.Value}");
    }
}
