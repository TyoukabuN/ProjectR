using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using YooAsset.Editor;
using YooAsset;
using PJR;

public static class BuildCommand
{
    public static string DefaultPackageName = "Default";

    [MenuItem("PJR/打包/BuildPlayerTest")]
    public static void BuildPlayerTest()
    {
        BuildPlayer();
    }
    public static void BuildPlayer()
    {
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, GetBuildPlayerLocation(), EditorUserBuildSettings.activeBuildTarget, BuildOptions.CleanBuildCache | BuildOptions.StrictMode);
    }

    [MenuItem("PJR/打包/BuildAssetbundleTest")]
    public static void BuildAssetbundle()
    {
        EBuildPipeline BuildPipeline = EBuildPipeline.ScriptableBuildPipeline;
        AssetBundleBuildArgs args = AssetBundleBuildArgs.LoadFromEditorPref(DefaultPackageName, BuildPipeline);
        ExecuteBuildAssetbundle(args);
    }

    /// <summary>
    /// 从命令执行AssetBundle构建
    /// </summary>
    public static void CMD_ExecuteAssetbundleBuild()
    {
        LogSystem.Log("/////////////////////////////////////////////////////////////////////");
        LogSystem.Log("////////////////////[CMD_ExecuteAssetbundleBuild]////////////////////");
        LogSystem.Log("/////////////////////////////////////////////////////////////////////");
        ExecuteBuildAssetbundle(AssetBundleBuildArgs.LoadFromCommandLine());
    }

    /// <summary>
    /// 执行构建
    /// </summary>
    public static void ExecuteBuildAssetbundle(AssetBundleBuildArgs args)
    {
        string PackageName = "Default";
        EBuildPipeline BuildPipeline = EBuildPipeline.ScriptableBuildPipeline;


        ScriptableBuildParameters buildParameters = new ScriptableBuildParameters();
        buildParameters.BuildOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
        buildParameters.BuildinFileRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();
        buildParameters.BuildPipeline = BuildPipeline.ToString();
        buildParameters.BuildTarget = args.buildTarget;
        buildParameters.BuildMode = args.buildMode;
        buildParameters.PackageName = PackageName;
        buildParameters.PackageVersion = GetDefaultBuildVersion();
        buildParameters.EnableSharePackRule = true;
        buildParameters.VerifyBuildingResult = true;
        buildParameters.FileNameStyle = args.fileNameStyle;
        buildParameters.BuildinFileCopyOption = args.buildinFileCopyOption;
        buildParameters.BuildinFileCopyParams = args.buildinFileCopyParams;
        buildParameters.EncryptionServices = CreateEncryptionInstance(PackageName, BuildPipeline);
        buildParameters.CompressOption = args.compressOption;

        ScriptableBuildPipeline pipeline = new ScriptableBuildPipeline();
        var buildResult = pipeline.Run(buildParameters, true);
        if (buildResult.Success)
        {
            LogSystem.Log($"[ExecuteBuildAssetbundle][Success] 构建成功 : {buildResult.OutputPackageDirectory}");
        }
        else
        {
            LogSystem.LogError($"[ExecuteBuildAssetbundle][Failure] 构建失败 : {buildResult.ErrorInfo}");
        }
    }

    /// <summary>
    /// 创建加密类实例
    /// </summary>
    private static IEncryptionServices CreateEncryptionInstance(string PackageName, EBuildPipeline BuildPipeline)
    {
        var encyptionClassName = AssetBundleBuilderSetting.GetPackageEncyptionClassName(PackageName, BuildPipeline);
        var encryptionClassTypes = EditorTools.GetAssignableTypes(typeof(IEncryptionServices));
        var classType = encryptionClassTypes.Find(x => x.FullName.Equals(encyptionClassName));
        if (classType != null)
            return (IEncryptionServices)Activator.CreateInstance(classType);
        else
            return null;
    }
    /// <summary>
    /// 获取打包程序执行文件名字
    /// </summary>
    /// <returns></returns>
    private static string GetBuildPlayerLocation()
    {
        string buildTarget = EditorUserBuildSettings.activeBuildTarget.ToString();
        return $"{Application.dataPath}/../__builds/{buildTarget}/build_{GetDefaultBuildVersion()}/main.exe";
    }
    /// <summary>
    /// 获取打包版本号
    /// </summary>
    /// <returns></returns>
    private static string GetDefaultBuildVersion()
    {
        int totalSecond = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
        return DateTime.Now.ToString("yyyy-MM-dd") + "-" + totalSecond;
    }

    /// <summary>
    /// 从命令行中获取BuildTarget
    /// </summary>
    /// <param name="buildTarget"></param>
    /// <returns></returns>
    public static bool TryGetBuildTargetFromArgs(out BuildTarget buildTarget)
    {
        buildTarget = BuildTarget.StandaloneWindows64;
        string BuildTargetStr = CMDUtility.GetArgumentValue("BuildTarget");
        if(!string.IsNullOrEmpty(BuildTargetStr))
            return false;
        try { 
            buildTarget = (BuildTarget)Enum.Parse(typeof(BuildTarget), BuildTargetStr);
        }catch(Exception e)
        {
            LogSystem.LogError(e);
            return false;
        }
        return true;
    }

    /// <summary>
    /// AssetBundle构建参数
    /// </summary>
    public struct AssetBundleBuildArgs
    {
        public BuildTarget buildTarget;
        public EBuildMode buildMode;
        public EFileNameStyle fileNameStyle;
        public EBuildinFileCopyOption buildinFileCopyOption;
        public string buildinFileCopyParams;
        public ECompressOption compressOption;

        public static AssetBundleBuildArgs Default => new AssetBundleBuildArgs()
        {
            buildMode = EBuildMode.IncrementalBuild,
            fileNameStyle = EFileNameStyle.HashName,
            buildinFileCopyOption = EBuildinFileCopyOption.ClearAndCopyAll,
            buildinFileCopyParams = "",
            compressOption = ECompressOption.LZ4
        };

        public static AssetBundleBuildArgs LoadFromEditorPref(string PackageName, EBuildPipeline BuildPipeline)
        {
            AssetBundleBuildArgs temp = AssetBundleBuildArgs.Default;
            try { temp.buildTarget = EditorUserBuildSettings.activeBuildTarget; } catch {}
            try { temp.buildMode = AssetBundleBuilderSetting.GetPackageBuildMode(PackageName, BuildPipeline); } catch {}
            try { temp.fileNameStyle = AssetBundleBuilderSetting.GetPackageFileNameStyle(PackageName, BuildPipeline); } catch {}
            try { temp.buildinFileCopyOption = AssetBundleBuilderSetting.GetPackageBuildinFileCopyOption(PackageName, BuildPipeline); } catch {}
            try { temp.buildinFileCopyParams = AssetBundleBuilderSetting.GetPackageBuildinFileCopyParams(PackageName, BuildPipeline); } catch {}
            try { temp.compressOption = AssetBundleBuilderSetting.GetPackageCompressOption(PackageName, BuildPipeline); } catch { }
            return temp;
        }

        public const string ArgKey_PackageName = "-packagePackageNam";
        //
        public const string ArgKey_BuildTarget = "-buildTarget";
        public const string ArgKey_BuildMode = "-packageBuildMode";
        public const string ArgKey_FileNameStyle = "-packageFileNameStyle";
        public const string ArgKey_BuildinFileCopyOption = "-packageBuildinFileCopyOption";
        public const string ArgKey_BuildinFileCopyParams = "-packageBuildinFileCopyParams";
        public const string ArgKey_CompressOption = "-packageCompressOption";

        public const char separator = ' ';
        /// <summary>
        /// 从命令行中获取参数
        /// </summary>
        /// <returns></returns>
        public static AssetBundleBuildArgs LoadFromCommandLine()
        {
            AssetBundleBuildArgs temp = AssetBundleBuildArgs.Default;

            var args = Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length; i++)
            {
                LogSystem.Log(args[i]);
                if (args[i].StartsWith(ArgKey_BuildTarget))
                    temp.buildTarget = (BuildTarget)Enum.Parse(typeof(BuildTarget), args[++i]);
                if (args[i].StartsWith(ArgKey_BuildMode))
                    temp.buildMode = (EBuildMode)Enum.Parse(typeof(EBuildMode), args[++i]);
                if (args[i].StartsWith(ArgKey_FileNameStyle))
                    temp.fileNameStyle = (EFileNameStyle)Enum.Parse(typeof(EFileNameStyle), args[++i]);
                if (args[i].StartsWith(ArgKey_BuildinFileCopyOption))
                    temp.buildinFileCopyOption = (EBuildinFileCopyOption)Enum.Parse(typeof(EBuildinFileCopyOption), args[++i]);
                if (args[i].StartsWith(ArgKey_BuildinFileCopyParams))
                    temp.buildinFileCopyParams = args[++i];
                if (args[i].StartsWith(ArgKey_CompressOption))
                    temp.compressOption = (ECompressOption)Enum.Parse(typeof(ECompressOption), args[++i]);
            }

            return temp;
        }
    }
}
