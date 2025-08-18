#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace PJR.Core
{
    public class AssetModificationEventHandle : AssetModificationProcessor
    {
        public static event Action<string> OnWillCreateAssetCall;
        public static event Action<string, RemoveAssetOptions> OnWillDeleteAssetCall;
        public static event Action<string, string> OnWillMoveAssetCall;
        public static event Action<string[]> OnWillSaveAssetsCall;
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
        static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            try
            {
                OnWillDeleteAssetCall?.Invoke(assetPath, options);
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
        static string[] OnWillSaveAssets(string[] paths)
        {
            try
            {
                OnWillSaveAssetsCall?.Invoke(paths);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            return paths;
        }
    }
}

#endif
