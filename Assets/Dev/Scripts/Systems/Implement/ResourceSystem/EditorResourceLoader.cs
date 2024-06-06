using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
namespace PJR
{
    public class EditorResourceLoader : ResourceLoader
    {
        public EditorResourceLoader(string assetFullName, Type assetType) : base(assetFullName, assetType)
        {
            isEditor = true;
        }
        public override void Update()
        {
            if (isDone)
                return;
            string assetPath = ResourceSystem.EditorAssetMgr.ConvertLocationToAssetPath(assetFullName);
            asset = AssetDatabase.LoadAssetAtPath(assetPath, assetType);
            if (asset == null)
            {
                error = $"[EditorResourceLoader] Find not asset \"{assetName}\": \n [type]: {assetType.FullName} \n [fullName]:{assetFullName} ";
                LogSystem.LogError(error);
            }
            phase = Phase.Done;
        }
    }

    public class EditorResourceLoader<T> : ResourceLoader where T : System.Type
    {
        public EditorResourceLoader(T assetType,string assetFullName) : base(assetFullName, assetType)
        {
            isEditor = true;
        }
    }
}
#endif
