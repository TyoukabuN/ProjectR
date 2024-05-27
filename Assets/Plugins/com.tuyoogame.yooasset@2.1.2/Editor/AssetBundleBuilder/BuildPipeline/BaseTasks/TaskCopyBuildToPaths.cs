using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace YooAsset.Editor
{
    public class TaskCopyBuildToPaths
    {
        /// <summary>
        /// 拷贝资源包到对应路径下以PackageName为名字的文件夹下
        /// </summary>
        internal void CopyBuildToPaths(BuildParametersContext buildParametersContext, PackageManifest manifest)
        {
            EBuildinFileCopyOption copyOption = buildParametersContext.Parameters.BuildinFileCopyOption;
            string packageOutputDirectory = buildParametersContext.GetPackageOutputDirectory();
            string buildinRootDirectory = buildParametersContext.GetBuildinRootDirectory();
            string buildPackageName = buildParametersContext.Parameters.PackageName;
            string buildPackageVersion = buildParametersContext.Parameters.PackageVersion;

            // 复制构建到指定路径
            {
                string[] directories = buildParametersContext.Parameters.CopyBuildToPaths.Split(';');
                foreach (var directory in directories)
                {
                    buildinRootDirectory = $"{directory}/{buildPackageName}";
                    // 默认清空内置文件的目录
                    {
                        EditorTools.ClearFolder(buildinRootDirectory);
                    }

                    // 拷贝补丁清单文件
                    {
                        string fileName = YooAssetSettingsData.GetManifestBinaryFileName(buildPackageName, buildPackageVersion);
                        string sourcePath = $"{packageOutputDirectory}/{fileName}";
                        string destPath = $"{buildinRootDirectory}/{fileName}";
                        EditorTools.CopyFile(sourcePath, destPath, true);
                    }

                    // 拷贝补丁清单哈希文件
                    {
                        string fileName = YooAssetSettingsData.GetPackageHashFileName(buildPackageName, buildPackageVersion);
                        string sourcePath = $"{packageOutputDirectory}/{fileName}";
                        string destPath = $"{buildinRootDirectory}/{fileName}";
                        EditorTools.CopyFile(sourcePath, destPath, true);
                    }

                    // 拷贝补丁清单版本文件
                    {
                        string fileName = YooAssetSettingsData.GetPackageVersionFileName(buildPackageName);
                        string sourcePath = $"{packageOutputDirectory}/{fileName}";
                        string destPath = $"{buildinRootDirectory}/{fileName}";
                        EditorTools.CopyFile(sourcePath, destPath, true);
                    }

                    // 拷贝文件列表（所有文件）
                    {
                        foreach (var packageBundle in manifest.BundleList)
                        {
                            string sourcePath = $"{packageOutputDirectory}/{packageBundle.FileName}";
                            string destPath = $"{buildinRootDirectory}/{packageBundle.FileName}";
                            EditorTools.CopyFile(sourcePath, destPath, true);
                        }
                    }
                }

                BuildLogger.Log($"Copy Build to directory complete: {buildinRootDirectory}");
            }
        }
    }
}