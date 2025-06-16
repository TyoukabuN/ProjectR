using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.Utilities;
using static PJR.Timeline.Editor.TimelineWindow;

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
            var asset = ScriptableObject.CreateInstance<Sequence>();
            asset.name = "Sequence";
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
        }
    }
}
