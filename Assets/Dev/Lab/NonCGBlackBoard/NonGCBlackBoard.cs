using PJR.BlackBoard.CachedValueBoard;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class NonGCBlackBoard : SerializedMonoBehaviour, ICachedValueBoardHolder
{
    public CacheableValueBoard cacheableValueBoard;
    CacheableValueBoard ICachedValueBoardHolder.GetCachedValueBoard() => cacheableValueBoard;
    
    public ICachedValueBoardHolder TargetBoard;
        
    [Button]
    public void Test()
    {
        Debug.Log(cacheableValueBoard?.OverrideTo(TargetBoard));
    }


    private bool _doRuntimeGCTest;
    [Button, ShowIf("@EditorApplication.isPlaying")]
    public void RuntimeTest()
    {
        _doRuntimeGCTest = true;
    }

    private void Update()
    {
        if (_doRuntimeGCTest)
        {
            using (new ProfileScope("BoardGCTest"))
            {
                cacheableValueBoard?.OverrideTo(TargetBoard);
            }
        }
    }
}
