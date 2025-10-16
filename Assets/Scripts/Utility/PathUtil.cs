using System.IO;
using UnityEngine;

namespace PJR
{
    public static class PathDefine
    {
        /// <summary>
        /// PlayerBuild的根目录
        /// </summary>
        public static string PlayerBuildRoot = Application.dataPath + "/../.playerBuilds";
        
        /// <summary>
        /// 默认的debug用的PlayerBuild目录
        /// </summary>
        public static string DefaultDebugBuildDirectory = Application.dataPath + "/../.playerBuilds/.defaultDebugBuild";
    }

    public static class PathUtil 
    {
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
        /// <summary>
        /// 将目录分割符换成主要目录分隔符
        /// 即系统文件资源管理器用的目录分隔符
        /// </summary>
        public static string ReplaceToDirectorySeparatorChar(string path)
        {
            return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }
    }
}