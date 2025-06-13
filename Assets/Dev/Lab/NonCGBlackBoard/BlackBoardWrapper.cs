using System;
using PJR.BlackBoard.CachedValueBoard;
using PJR.BlackBoard.Inspector;
using PJR.Editor;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine.Serialization;

public class BlackBoardWrapper : SerializedScriptableObject
{
    [LabelText("黑板1"),OdinSerialize]
    [HideReferenceObjectPicker]
    private CacheableValueBoard cacheableValueBoard;
    
    [LabelText("黑板2"),OdinSerialize]
    [HideReferenceObjectPicker]
    private CacheableValueBoard cacheableValueBoard2;
    
    [TitleGroup("黑板Field测试")]
    [NonSerialized,OdinSerialize,GenericTypeFilter]
    [HideReferenceObjectPicker]
    public CacheableField<string> StringValue;

#if UNITY_EDITOR
    [MenuItem("Assets/PJR/BlackBoard/Wrapper")]
    public static void CreateConstConfigAsset()
    {
        CSConfigHelper.CreateScriptableObject<BlackBoardWrapper>();
    }
#endif
}
