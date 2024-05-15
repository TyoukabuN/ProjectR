using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LS.LSEditor
{
    public static class LSAssetAnalysisSetting
    {
        public const string key_show_assetRefCount = "LSAssetAnalysisSetting_show_assetRefCount";

        private static ReferenceFinderData data = new ReferenceFinderData();
        private static bool initializedData = false;

        public static bool ShowAssetRefCount
        {
            set {
                EditorPrefs.SetBool(key_show_assetRefCount, value);
            }
            get
            {
                return EditorPrefs.GetBool(key_show_assetRefCount);
            }
        }
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
    }
}