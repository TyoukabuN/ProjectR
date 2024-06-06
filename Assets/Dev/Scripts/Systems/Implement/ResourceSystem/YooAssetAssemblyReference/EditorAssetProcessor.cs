#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace YooAsset
{
    public class EditorAssetProcessor : UnityEditor.AssetModificationProcessor
    {
        public static event Action<string> OnWillCreateAssetCall;
        public static event Action<string> OnWillDeleteAssetCall;
        public static event Action<string, string> OnWillMoveAssetCall;

        static void OnWillCreateAsset(string assetPath)
        {
            try
            {
                OnWillCreateAssetCall?.Invoke(assetPath);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        static AssetDeleteResult OnWillDeleteAsset(string assetPath)
        {
            try
            {
                OnWillDeleteAssetCall?.Invoke(assetPath);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            AssetDeleteResult result = AssetDeleteResult.DidNotDelete;
            return result;
        }
        static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            try
            {
                OnWillMoveAssetCall?.Invoke(sourcePath, destinationPath);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            AssetMoveResult assetMoveResult = AssetMoveResult.DidNotMove;

            return assetMoveResult;
        }
    }
}
#endif
