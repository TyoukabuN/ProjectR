using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
namespace PJR.Systems
{
    public partial class ResourceSystem
    {
        public class EditorResourceLoader : ResourceLoader
        {
            public EditorResourceLoader(string assetName, Type assetType) : base(assetName, assetType)
            {
                IsEditor = true;
            }
            public override void Update()
            {
                if (isDone)
                    return;
                string assetPath = EditorAssetMgr.ConvertLocationToAssetPath(AssetFullName);
                AssetObject = AssetDatabase.LoadAssetAtPath(assetPath, AssetType);
                if (AssetObject == null)
                {
                    error = $"[EditorResourceLoader] Find not asset \"{AssetName}\": \n [type]: {AssetType.FullName} \n [fullName]:{AssetFullName} ";
                    LogSystem.LogError(error);
                }
                State = LoaderState.Done;
            }
        }

        public class EditorResourceLoader<T> : ResourceLoader where T : Type
        {
            public EditorResourceLoader(T assetType, string assetName) : base(assetName, assetType)
            {
                IsEditor = true;
            }
        }
    }
}
#endif
