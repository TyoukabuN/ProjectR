using PJR.BlackBoard.CachedValueBoard;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class NonGCBlackBoard : SerializedMonoBehaviour, CachedValueBoardHolder
{
    public CachedValueBoard CachedValueBoard;
    CachedValueBoard CachedValueBoardHolder.GetCachedValueBoard() => CachedValueBoard;
    
    public CachedValueBoardHolder TargetBoard;
        
    [Button]
    public void Test()
    {
        var board = TargetBoard.GetCachedValueBoard();
        if (board == null)
            return;
        Debug.Log(CachedValueBoard?.OverrideTo(board));
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
                CachedValueBoard _board;
                using (new ProfileScope("1"))
                    _board = TargetBoard.GetCachedValueBoard();
                using (new ProfileScope("2"))
                    CachedValueBoard.OverrideTo(_board);
            }
        }
    }
}

public interface CachedValueBoardHolder
{
    public CachedValueBoard GetCachedValueBoard();
}
