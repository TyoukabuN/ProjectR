using PJR.Timeline;
using PJR.Timeline.Editor;
using UnityEditor;
using UnityEditor.Callbacks;

public static class SeqenceAssetOpenHandler
{
    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceID)
    {
        var asset = AssetDatabase.GetMainAssetTypeAtPath(AssetDatabase.GetAssetPath(instanceID));
        if (asset == typeof(SequenceAsset))
        {
            TimelineWindow.ShowWindow()?.Selection_CheckSelectionChange();
            return true;
        }

        return false;
    }
}
