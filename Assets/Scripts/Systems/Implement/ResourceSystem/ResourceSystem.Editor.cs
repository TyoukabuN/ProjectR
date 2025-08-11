#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using YooAsset;
using Object = UnityEngine.Object;

namespace PJR.Systems
{
    public partial class ResourceSystem
    {
        private static EditorAssetManager _editorAssetMgr;
        public static EditorAssetManager EditorAssetMgr
        {
            get
            {
                if (_editorAssetMgr == null)
                    _editorAssetMgr = new EditorAssetManager();
                return _editorAssetMgr;
            }
        }

        private static void CheckInit()
        {
            if (_editorAssetMgr == null)
                _editorAssetMgr = new EditorAssetManager();
        }
        public static void ResetEditorAssetManager()
        {
            _editorAssetMgr = new EditorAssetManager();
        }
        public static bool Editor_TryGetAssetInfoByName(string assetName, out AssetInfo assetInfo)
        {
            CheckInit();
            assetInfo = _editorAssetMgr.ConvertLocationToAssetInfo(assetName, typeof(Object));
            return assetInfo != null;
        }
        public static bool Editor_TryGetAssetInfoByGUID(string assetGUID, out AssetInfo assetInfo)
        {
            CheckInit();
            assetInfo = _editorAssetMgr.ConvertAssetGUIDToAssetInfo(assetGUID, typeof(Object));
            return assetInfo != null;
        }

        /// <summary>
        /// 通过GUID加载Editor下asset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetGUID"></param>
        /// <returns></returns>
        public static T EditotLoadAssetByGUID<T>(string assetGUID) where T : Object
        {
            return (T)EditotLoadAssetByGUID(assetGUID, typeof(T));
        }
        public static Object EditotLoadAssetByGUID(string assetGUID, Type type)
        {
            GetGUIDInfo(assetGUID, out assetGUID, out string subAssetName);
            var assetInfo = _editorAssetMgr.ConvertAssetGUIDToAssetInfo(assetGUID, type);
            return AssetDatabase.LoadAssetAtPath(assetInfo.AssetPath, type);
        }

        /// <summary>
        /// 通过名字加载Editor下asset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static T EditorLoadAsset<T>(string assetName) where T : Object
        {
            return (T)EditorLoadAsset(assetName, typeof(T));
        }

        public static Object EditorLoadAsset(string assetName, Type type)
        {
            CheckInit();
            //
            Debug.Log($"AssetSystem.EditorLoadAsset [assetName]:{assetName}");
            if (string.IsNullOrEmpty(assetName))
                return null;
            var assetInfo = _editorAssetMgr.ConvertLocationToAssetInfo(assetName, type);
            return AssetDatabase.LoadAssetAtPath(assetInfo.AssetPath, type);
        }
    }
}
#endif
