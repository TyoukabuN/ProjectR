using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace PJR.Editor
{
    public class AssetAnalysisSetting : ScriptableObject
    {
#if UNITY_EDITOR
        [MenuItem("Assets/PJR/创建配置/编辑器配置")]
        public static void CreateAsset()
        {
            CSConfigHelper.CreateScriptableObject<AssetAnalysisSetting>();
        }
#endif
        /// <summary>
        /// 显示被引用数的路径
        /// </summary>
        [LabelText("显示被引用数的路径")]
        public string[] referedCountCheckPath = new[]
        {
        "Assets/__LS/Art",
    };

        [LabelText("显示资产引用数")]
        public bool showAssetRefCount = false;

        [LabelText("显示是否被外部引用")]
        public bool showIsReferedByOutside = false;

        [LabelText("判断乱用引用的根目录")]
        public string rootForOutsideReferedCheck = string.Empty;


        public static string[] ReferedCountCheckPath
        {
            get => GetAsset().referedCountCheckPath;
        }
        public static bool ShowAssetRefCount
        {
            set { GetAsset().showAssetRefCount = value; SaveAsset(); }
            get => GetAsset().showAssetRefCount;
        }
        public static bool ShowIsReferedByOutside
        {
            set { GetAsset().showIsReferedByOutside = value; SaveAsset(); }
            get => GetAsset().showIsReferedByOutside;
        }
        public static string RootForOutsideReferedCheck
        {
            set { GetAsset().rootForOutsideReferedCheck = value; SaveAsset(); }
            get => GetAsset().rootForOutsideReferedCheck;
        }


        public const string assetPath = "Assets/Resources/AssetAnalysisSetting.asset";
        private static AssetAnalysisSetting _curAsset = null;
        public static AssetAnalysisSetting GetAsset()
        {
            if (_curAsset == null)
            {
                _curAsset = AssetDatabase.LoadAssetAtPath<AssetAnalysisSetting>(assetPath);
                if (_curAsset == null)
                {
                    _curAsset = CreateInstance<AssetAnalysisSetting>();
                    AssetDatabase.CreateAsset(_curAsset, assetPath);
                }
            }
            return _curAsset;
        }
        public static void SaveAsset()
        {
            if (_curAsset)
            {
                EditorUtility.SetDirty(_curAsset);
                AssetDatabase.SaveAssets();
            }
        }
        public static void SelectAsset()
        {
            if (_curAsset)
            {
                EditorGUIUtility.PingObject(_curAsset);
                Selection.activeObject = _curAsset;
            }
        }
        public static void OpenAsset()
        {
            if (_curAsset)
            {
                EditorUtility.OpenPropertyEditor(_curAsset);
            }
        }

        public static void PingRootForOutsideReferedCheck()
        {
            var path = GetAsset().rootForOutsideReferedCheck;
            if (string.IsNullOrEmpty(path))
                return;
            if (!AssetDatabase.IsValidFolder(path))
                return;
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<DefaultAsset>(path));
        }

        private static ReferenceFinderData data = new ReferenceFinderData();
        private static bool initializedData = false;

        static void InitReferenceDataIfNeeded()
        {
            if (!initializedData)
            {
                if (!data.ReadFromCache())
                {
                    data.CollectDependenciesInfo();
                }
                initializedData = true;
            }
        }

        public static ReferenceFinderData GetRefFinderData()
        {
            InitReferenceDataIfNeeded();
            return data;
        }

        public static void RefreshReferenceData()
        {
            data = data ?? new ReferenceFinderData();
            data.CollectDependenciesInfo();
        }
        public static void ClearCacheAndRefreshReferenceData()
        {
            string CACHE_PATH = "Library/ReferenceFinderCache";
            if (File.Exists(CACHE_PATH))
                File.Delete(CACHE_PATH);
            RefreshReferenceData();
        }
    }
}