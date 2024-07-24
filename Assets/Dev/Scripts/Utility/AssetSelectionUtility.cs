#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

namespace PJR
{
    public static class AssetSelectionUtility
    {
        public static bool GetSeletedFirstFolder(out string assetPath, out Object asset, bool requireFullPath = false)
        {
            assetPath = string.Empty;
            asset = null;
            List<string> paths = new List<string>();
            var objs = Selection.GetFiltered<Object>(SelectionMode.Assets);
            if (objs == null || objs.Length <= 0)
            {
                return false;
            }
            foreach (var obj in objs)
            {
                asset = obj;
                assetPath = AssetDatabase.GetAssetPath(obj);
                if (!AssetDatabase.IsValidFolder(assetPath))
                    continue;
                if (requireFullPath)
                    assetPath = PathUtility.GetFullPath(assetPath);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取选中的第一个文件夹对象
        /// </summary>
        /// <returns>文件夹对象</returns>
        public static Object GetSeletedFirstFolderObject()
        {
            if (!GetSeletedFirstFolder(out string assetPath, out Object asset))
                return null;
            return asset;
        }
        /// <summary>
        /// 获取选中的第一个文件夹路径
        /// </summary>
        /// <param name="requireFullPath">需要完整路径</param>
        /// <returns>文件夹路径</returns>
        public static string GetSeletedFirstFolderPath(bool requireFullPath = false)
        {
            GetSeletedFirstFolder(out string assetPath, out Object asset, requireFullPath);
            return assetPath;
        }


        /// <summary>
        /// 获取选中的多个文件夹对象
        /// </summary>
        /// <param name="folders"></param>
        /// <returns></returns>
        public static bool GetSeletedFolders(out List<Object> folders)
        {
            folders = null;
            var objs = Selection.GetFiltered<Object>(SelectionMode.Assets);
            if (objs == null || objs.Length <= 0)
            {
                return false;
            }

            folders = new List<Object>();

            foreach (var obj in objs)
            {
                var assetPath = AssetDatabase.GetAssetPath(obj);
                if (!AssetDatabase.IsValidFolder(assetPath))
                    continue;
                folders.Add(obj);
            }
            return false;
        }
        /// <summary>
        /// 获取选中的多个文件夹对象
        /// </summary>
        /// <param name="folderPaths"></param>
        /// <returns></returns>
        public static bool GetSeletedFolderPaths(out List<string> folderPaths)
        {
            folderPaths = null;
            var objs = Selection.GetFiltered<Object>(SelectionMode.Assets);
            if (objs == null || objs.Length <= 0)
            {
                return false;
            }

            folderPaths = new List<string>();

            foreach (var obj in objs)
            {
                var assetPath = AssetDatabase.GetAssetPath(obj);
                if (!AssetDatabase.IsValidFolder(assetPath))
                    continue;
                folderPaths.Add(assetPath);
            }
            return folderPaths.Count > 0;
        }

        /// <summary>
        /// 获取选中文件夹下的所有Asset
        /// </summary>
        /// <param name="filter">Project窗口搜索栏一样的类型过滤关键词，如：t:prefab</param>
        /// <returns>Assets的guid</returns>
        public static List<string> GetAllAssetsUnderSelectedFolder(string filter)
        {
            if (!GetSeletedFolderPaths(out var folderPaths))
                return null;
            return AssetDatabase.FindAssets(filter, folderPaths.ToArray()).ToList();
        }

        static string Filter_Prefab = "t:prefab";

        public static List<string> GetAllPrefabsUnderSelectedFolder()
        {
            return GetAllAssetsUnderSelectedFolder(Filter_Prefab);
        }
    }
}
#endif
