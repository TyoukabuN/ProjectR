using PJR.Core.BlackBoard.CachedValueBoard;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class CachedValueBoardHolder : SerializedMonoBehaviour , ICachedValueBoardHolder
{
    [TitleGroup("黑板1"),OdinSerialize]
    [HideReferenceObjectPicker]
    private CacheableValueBoard cacheableValueBoard;

    public CacheableValueBoard GetCachedValueBoard() => cacheableValueBoard;
}
