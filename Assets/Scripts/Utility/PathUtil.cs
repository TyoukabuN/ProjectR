using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public static class PathUtil 
    {
        /// <summary>
        /// 项目内配置根目录
        /// </summary>
        public const string ConfigRoot = "Assets/__LS/Config";
        /// <summary>
        /// 项目内json配置文件根目录
        /// </summary>
        public const string JsonConfigRoot = ConfigRoot + "/json";
        /// <summary>
        /// 配置源文件路径
        /// </summary>
        public static string RawConfigRoot = Application.dataPath + "/../../LS_Config";
        /// <summary>
        /// xlxs转json batch文件
        /// </summary>
        public static string Xlsx2JsonBatch = RawConfigRoot + "/xlsx2json.bat";


        /// <summary>
        /// 资源描述文件的路径
        /// </summary>
        /// <returns></returns>
        public static string GetDescFilePath()
        {
            return $"{Application.streamingAssetsPath}/{nameof(ResourceDescription)}";
        }

        /// <summary>
        /// </summary>
        /// <param name="relativePath">assets里的相对路径</param>
        /// <returns>asset在硬盘里的fullPath</returns>
        public static string GetFullPath(string relativePath)
        {
            // 获取项目的根目录
            string projectRoot = Application.dataPath;

            // 如果传入的相对路径包含 "Assets" 开头，则去掉 "Assets"
            if (relativePath.StartsWith("Assets/"))
            {
                relativePath = relativePath.Substring("Assets/".Length);
            }

            // 拼接完整路径
            string fullPath = $"{projectRoot}/{relativePath}";

            // 确保路径格式正确
            //fullPath = Path.GetFullPath(fullPath);

            return fullPath;
        }

        /// <summary>
        /// 路径归一化
        /// </summary>
        public static string RegularPath(string path)
        {
            return path.Replace('\\', '/').Replace("\\", "/");
        }
    }
}