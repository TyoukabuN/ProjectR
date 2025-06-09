using System;
using PJR.BlackBoard.CachedValueBoard;
using PJR.Editor;
using Sirenix.OdinInspector;
using UnityEditor;

public class BlackBoardWrapper : SerializedScriptableObject
{
    public CachedValueBoard CachedValueBoard;

#if UNITY_EDITOR
    [MenuItem("Assets/PJR/BlackBoard/Wrapper")]
    public static void CreateConstConfigAsset()
    {
        CSConfigHelper.CreateScriptableObject<BlackBoardWrapper>();
    }
#endif
}
