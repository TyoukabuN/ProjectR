using PJR.BlackBoard.CachedValueBoard;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class NonGCBlackBoard : SerializedMonoBehaviour, ICachedValueBoardHolder
{
    public CachedValueBoard CachedValueBoard;
    CachedValueBoard ICachedValueBoardHolder.GetCachedValueBoard() => CachedValueBoard;
    
    public ICachedValueBoardHolder TargetBoard;
        
    [Button]
    public void Test()
    {
        Debug.Log(CachedValueBoard?.OverrideTo(TargetBoard));
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
                CachedValueBoard?.OverrideTo(TargetBoard);
            }
        }
    }
}
