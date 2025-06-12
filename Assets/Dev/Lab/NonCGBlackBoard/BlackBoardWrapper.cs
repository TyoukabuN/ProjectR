using System;
using PJR.BlackBoard.CachedValueBoard;
using PJR.Editor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine.Serialization;

public class BlackBoardWrapper : SerializedScriptableObject
{
    [FormerlySerializedAs("CachedValueBoard")] public CacheableValueBoard cacheableValueBoard = new();

#if UNITY_EDITOR
    [MenuItem("Assets/PJR/BlackBoard/Wrapper")]
    public static void CreateConstConfigAsset()
    {
        CSConfigHelper.CreateScriptableObject<BlackBoardWrapper>();
    }
#endif
}
