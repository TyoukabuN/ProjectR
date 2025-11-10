using System;
using System.Collections.Generic;
using System.Linq;
using BatchRename;
using UnityEditor;
using UnityEngine;
using YooAsset.Editor;

namespace PJR.Editor
{
    public static partial class CorrectAssetTools
    {
        public static void AssetsAddressDeduplication()
        {
            ErrorReport report;

            string PackageName = "Default";
            EBuildPipeline BuildPipeline = EBuildPipeline.ScriptableBuildPipeline;

            var BuildOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
            var BuildTarget = EditorUserBuildSettings.activeBuildTarget;
            var BuildMode = AssetBundleBuilderSetting.GetPackageBuildMode(PackageName, BuildPipeline);
            var FileNameStyle = AssetBundleBuilderSetting.GetPackageFileNameStyle(PackageName, BuildPipeline);
            var BuildinFileCopyOption =
                AssetBundleBuilderSetting.GetPackageBuildinFileCopyOption(PackageName, BuildPipeline);
            var BuildinFileCopyParams =
                AssetBundleBuilderSetting.GetPackageBuildinFileCopyParams(PackageName, BuildPipeline);
            var CompressOption = AssetBundleBuilderSetting.GetPackageCompressOption(PackageName, BuildPipeline);
            var CopyBuildToPaths = AssetBundleBuilderSetting.GetPackageCopyBuildToPaths(PackageName, BuildPipeline);

            ScriptableBuildParameters buildParameters = new ScriptableBuildParameters();
            buildParameters.BuildOutputRoot = BuildOutputRoot;
            buildParameters.BuildinFileRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();
            buildParameters.BuildPipeline = BuildPipeline.ToString();
            buildParameters.BuildTarget = BuildTarget;
            buildParameters.BuildMode = BuildMode;
            buildParameters.PackageName = PackageName;
            buildParameters.PackageVersion = BuildCommand.GetDefaultBuildVersion();
            buildParameters.EnableSharePackRule = true;
            buildParameters.VerifyBuildingResult = true;
            buildParameters.FileNameStyle = FileNameStyle;
            buildParameters.BuildinFileCopyOption = BuildinFileCopyOption;
            buildParameters.BuildinFileCopyParams = BuildinFileCopyParams;
            buildParameters.EncryptionServices = BuildCommand.CreateEncryptionInstance(PackageName, BuildPipeline);
            buildParameters.CompressOption = CompressOption;
            buildParameters.CopyBuildToPaths = CopyBuildToPaths;

            report = new ErrorReport();

            foreach (var package in AssetBundleCollectorSettingData.Setting.Packages)
            {
                CheckPackageAddress(buildParameters, package, ref report);
            }

            if (report.ErrorTerms is not { Count: > 0 })
            {
                Debug.Log("没资源收集错误");
                return;
            }

            foreach (var errorTerm in report.ErrorTerms)
            {
                Debug.Log(errorTerm.ErrorCode);
            }
            
            var assetPaths = new List<string>();
            foreach (var pair in report.AddressToCollectAssetInfos)
            {
                var collectAssetInfoList = pair.Value;
                if (collectAssetInfoList is not { Count: > 1 }) 
                    continue;

                foreach (var collectAssetInfo in collectAssetInfoList)
                {
                    if (collectAssetInfo == null)
                        continue;
                    string assetPath = collectAssetInfo.AssetInfo.AssetPath;
                    if (string.IsNullOrEmpty(assetPath))
                        continue;
                    string guid = AssetDatabase.AssetPathToGUID(assetPath);
                    if (string.IsNullOrEmpty(guid))
                        continue;
                    assetPaths.Add(assetPath);
                }
            }
        
            BatchRenameWindow.OpenForGuidExtension(assetPaths);
        }

        private static void CheckPackageAddress(ScriptableBuildParameters buildParameters,
            AssetBundleCollectorPackage package,
            ref ErrorReport report)
        {
            try
            {
                package.CheckConfigError();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return;
            }

            var buildMode = buildParameters.BuildMode;
            var packageName = buildParameters.PackageName;
            var uniqueBundleName = AssetBundleCollectorSettingData.Setting.UniqueBundleName;

            // 创建资源收集命令
            IIgnoreRule ignoreRule = AssetBundleCollectorSettingData.GetIgnoreRuleInstance(package.IgnoreRuleName);
            CollectCommand command = new CollectCommand(buildMode, packageName,
                package.EnableAddressable,
                package.LocationToLower,
                package.IncludeAssetGUID,
                package.AutoCollectShaders,
                uniqueBundleName, ignoreRule);


            foreach (var group in package.Groups)
            {
                // 检测分组是否激活
                IActiveRule activeRule = AssetBundleCollectorSettingData.GetActiveRuleInstance(group.ActiveRuleName);
                if (!activeRule.IsActiveGroup())
                    continue;

                foreach (var collector in group.Collectors)
                {
                    //////

                    var collectPath = collector.CollectPath;

                    // 收集打包资源路径
                    List<string> findAssets = new List<string>();
                    if (AssetDatabase.IsValidFolder(collectPath))
                    {
                        string collectDirectory = collectPath;
                        string[] findResult = EditorTools.FindAssets(EAssetSearchType.All, collectDirectory);
                        findAssets.AddRange(findResult);
                    }
                    else
                    {
                        string assetPath = collectPath;
                        findAssets.Add(assetPath);
                    }

                    // 收集打包资源信息
                    foreach (string assetPath in findAssets)
                    {
                        var assetInfo = new YooAsset.Editor.AssetInfo(assetPath);
                        if (command.IgnoreRule.IsIgnore(assetInfo) == false &&
                            collector.IsCollectAsset(group, assetInfo))
                        {
                            var collectAssetInfo = collector.CreateCollectAssetInfo(command, group, assetInfo);
                            if (!report.CollectAssetInfoMap.ContainsKey(assetPath))
                            {
                                report.CollectAssetInfoMap.Add(assetPath, collectAssetInfo);
                            }
                            else
                            {
                                report.AddErrorTerm(
                                    assetPath,
                                    "The collecting asset file is existed",
                                    $"The collecting asset file is existed : {assetPath} in collector : {collectPath}",
                                    collectAssetInfo);
                            }
                        }
                    }
                }
            }

            // 检测可寻址地址是否重复
            if (command.EnableAddressable)
            {
                foreach (var collectInfoPair in report.CollectAssetInfoMap)
                {
                    if (collectInfoPair.Value.CollectorType == ECollectorType.MainAssetCollector)
                    {
                        string address = collectInfoPair.Value.Address;
                        string assetPath = collectInfoPair.Value.AssetInfo.AssetPath;
                        var collectAssetInfo = collectInfoPair.Value;
                        if (string.IsNullOrEmpty(address))
                            continue;

                        if (address.StartsWith("Assets/") || address.StartsWith("assets/"))
                        {
                            // report.AddErrorTerm(
                            //     assetPath, 
                            //     "The address can not set asset path in collector",
                            //     $"The address can not set asset path in collector : {CollectPath} \nAssetPath: {assetPath}",
                            //     collectAssetInfo);
                            continue;
                        }

                        if (!report.AddressToCollectAssetInfos.TryGetValue(address, out var collectAssetInfoList))
                        {
                            collectAssetInfoList = new List<CollectAssetInfo>(8);
                            report.AddressToCollectAssetInfos[address] = collectAssetInfoList;
                        }

                        if (collectAssetInfoList.All(x => x.AssetInfo.AssetPath != assetPath))
                            collectAssetInfoList.Add(collectAssetInfo);
                        else
                        {
                            Debug.LogError($"有问题   {assetPath}");
                        }
                    }
                }

                foreach (var pair in report.AddressToCollectAssetInfos)
                {
                    var collectAssetInfoList = pair.Value;
                    if (collectAssetInfoList is not { Count: > 1 })
                        continue;

                    string address = collectAssetInfoList[0].Address;
                    string assetPath = collectAssetInfoList[0].AssetInfo.AssetPath;
                    string assetPaths = string.Empty;

                    for (var i = 0; i < collectAssetInfoList.Count; i++)
                    {
                        assetPaths += $"{collectAssetInfoList[i].AssetInfo.AssetPath}\n".PadLeft(5);
                    }

                    report.AddErrorTerm(
                        assetPath,
                        $"The address is existed ({collectAssetInfoList.Count})",
                        $"The address is existed ({collectAssetInfoList.Count}): {address} \nAssetPath:\n{assetPaths}",
                        null);
                }
            }
        }
    }
}

