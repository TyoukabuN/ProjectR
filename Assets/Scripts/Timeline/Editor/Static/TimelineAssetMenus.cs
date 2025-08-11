using UnityEditor;
using UnityEngine;

namespace PJR.Timeline.Editor
{
    public static class TimelineAssetMenus
    {
        [MenuItem("Assets/PJR/Timeline/Create Sequence Asset")]
        public static void CreateSequenceAsset()
        {
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            assetPath = $"{assetPath}/New_Sequence_Asset.asset";
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
            var asset = ScriptableObject.CreateInstance<SequenceAsset>();
            asset.name = "Sequence";
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
        }
    }
}
