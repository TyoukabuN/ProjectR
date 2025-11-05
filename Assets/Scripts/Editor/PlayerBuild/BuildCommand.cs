using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEngine;
using YooAsset;
using YooAsset.Editor;

public static class BuildCommand
{
    static List<BuildSettings> buildSetttings;
    //static List<BuildPrepareAction> actions;
    static BuildCommand()
    {
        //CollectBuildPrepareAction();
        //CollectSuperUnityBuildSettings();
    }

    public static string DefaultPackageName = "Default";

    [MenuItem("Build/BuildPlayer")]
    public static void BuildPlayer()
    {
        //CMD_BuildPlayer("DefaultBuildSetting_DevBuild");
    }

    [MenuItem("Build/BuildAssetbundle")]
    public static void BuildAssetbundle()
    {
        EBuildPipeline BuildPipeline = EBuildPipeline.ScriptableBuildPipeline;
        AssetBundleBuildArgs args = AssetBundleBuildArgs.LoadFromEditorPref(DefaultPackageName, BuildPipeline);
        ExecuteBuildAssetbundle(args, out var result);
    }

    public static void CMD_BuildPlayer()
    {
        //CMD_BuildPlayer(null);
    }
    /// <summary>
    /// 从命令行执行Player的构建
    /// 从命令行中获取对应的BuildSettings资产的名字
    /// 然后找到对应名字的Asset，并构建它
    /// </summary>
    //public static void CMD_BuildPlayer(string buildSettingAssetName)
    //{
    //    //先检查下BuildPrepareAction
    //    RunBuildPrepareAction(BuildPrepareActionArgs.LoadFromCommandLine());
    //    var args = SuperUnityBuildArgs.LoadFromCommandLine();

    //    if (string.IsNullOrEmpty(buildSettingAssetName))
    //    {
    //        if (!args.IsValid())
    //        {
    //            Debug.LogError($"[BuildCommand][CMD_BuildPlayer][Error] 没有在命令行中没有 -buildSettingName");
    //            EditorApplication.Exit(1);
    //            return;
    //        }
    //        buildSettingAssetName = args.BuildSettingName;
    //    }
    //    if (!TryBuildSettingsByName(buildSettingAssetName, out var buildSetting))
    //    {
    //        Debug.LogError($"[BuildCommand][CMD_BuildPlayer][Error] 没有找到对应名字的BuildSettings : {buildSettingAssetName}");
    //        EditorApplication.Exit(1);
    //        return;
    //    }

    //    buildSetting = UnityEngine.Object.Instantiate(buildSetting);
    //    //将这个buildSetting设置成单例，BuildAll方法会直接操作的这个单例
    //    buildSetting.AsStaticInstance(false);
    //    //提前获取buildPath
    //    BuildSettings.basicSettings.buildPath = "CMD_Build";
    //    string buildPath = GenerateBuildPath(BuildSettings.projectConfigurations.BuildAllKeychains()[0], BuildSettings.basicSettings.buildPath);

    //    SuperUnityBuild.BuildTool.BuildProject.BuildAll();

    //    if (!string.IsNullOrEmpty(args.CopyBuildTo))
    //    {
    //        IfNotExistDirectory(args.CopyBuildTo);
    //        ClearOutBoundFolder(args.CopyBuildTo, 2);

    //        string copyBuildTo = $"{args.CopyBuildTo}/{GetDefaultBuildVersion()}";
    //        CopyBuildTo(buildPath, copyBuildTo);
    //    }

    //    EditorApplication.Exit(0);
    //}


    /// <summary>
    /// 将PlayerBuild复制到目标文件夹
    /// </summary>
    /// <param name="buildPath"></param>
    /// <param name="destFolder"></param>
    public static void CopyBuildTo(string buildPath, string destFolder)//,bool combineBuildFolderName = false)
    {
        var folderName = Path.GetFileName(buildPath);
        //if(combineBuildFolderName)
        //    destFolder = $"{destFolder}/{folderName}";
        if (!Directory.Exists(destFolder))
            Directory.CreateDirectory(destFolder);
        else
            EditorTools.ClearFolder(destFolder);
        buildPath = buildPath.TrimEnd('\\').TrimEnd('/');
        destFolder = destFolder.TrimEnd('\\').TrimEnd('/');
        try
        {
            EditorTools.CopyDirectory(buildPath, destFolder);
        }
        catch (Exception e)
        {
            Debug.Log($"[BuildCommand][CopyBuildTo][Failure] 复制Player构建失败!  [Error]: {e}");
        }
    }

    /// <summary>
    /// 清理传入数量外的文件文件夹数
    /// </summary>
    /// <param name="topDirectory">顶层文件夹</param>
    /// <param name="remainFolderCount">保留子文件夹的数量</param>
    public static void ClearOutBoundFolder(string topDirectory, int remainFolderCount)
    {
        var directories = new DirectoryInfo(topDirectory).GetDirectories();

        // 检查是否有子文件夹数量
        if (directories.Length < remainFolderCount)
            return;

        List<DirectoryInfo> dlist = new List<DirectoryInfo>(directories.OrderBy(d => d.CreationTime));
        for (int i = 0; i < dlist.Count; i++)
        {
            if (dlist.Count <= remainFolderCount)
                break;
            var info = dlist[i];
            if (info != null)
            {
                try
                {
                    info.Delete(true);
                    dlist.RemoveAt(i--);
                }
                catch (Exception e)
                {
                    Debug.Log($"[BuildCommand][ClearOutBoundFolder][Failure] 删除文件夹时出错!  [Error]: {e}");
                    break;
                }
            }
        }
    }

    public static void IfNotExistDirectory(string directory)
    {
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
    }

    /// <summary>
    /// 回去BuildSetting的BuildPath
    /// </summary>
    /// <param name="keyChain"></param>
    /// <param name="prototype"></param>
    /// <returns></returns>
    //private static string GenerateBuildPath(string keyChain, string prototype)
    //{
    //    BuildSettings.projectConfigurations.ParseKeychain(keyChain, out BuildReleaseType releaseType, out BuildPlatform platform, out BuildArchitecture arch, out BuildScriptingBackend scriptingBackend, out BuildDistribution dist);
    //    string resolvedPath = TokensUtility.ResolveBuildConfigurationTokens(prototype, releaseType, platform, arch, scriptingBackend, dist, DateTime.Now);
    //    string buildPath = Path.Combine(BuildSettings.basicSettings.baseBuildFolder, resolvedPath);
    //    buildPath = Path.GetFullPath(buildPath).TrimEnd('\\').TrimEnd('/');

    //    return buildPath;
    //}

    /// <summary>
    /// 根据名字获取项目里的BuildSettings资产
    /// </summary>
    /// <param name="name"></param>
    /// <param name="buildSetting"></param>
    /// <returns></returns>
    //public static bool TryBuildSettingsByName(string name, out BuildSettings buildSetting)
    //{
    //    string nameLower = name.ToLower();
    //    buildSetting = buildSetttings.Find(item => Path.GetFileNameWithoutExtension(item.name).ToLower() == nameLower);
    //    if (buildSetting == null)
    //    {
    //        return false;
    //    }
    //    return true;
    //}

    /// <summary>
    /// 从命令执行AssetBundle构建
    /// </summary>
    public static void CMD_ExecuteAssetbundleBuild()
    {
        Debug.Log("////////////////////[CMD_ExecuteAssetbundleBuild]////////////////////");
        ExecuteBuildAssetbundle(AssetBundleBuildArgs.LoadFromCommandLine(), out var buildResult);
        if (buildResult.Success)
        {
            Debug.Log($"[BuildCommand][ExecuteBuildAssetbundle][Success] 构建成功 : {buildResult.OutputPackageDirectory}");
            EditorApplication.Exit(0);
        }
        else
        {
            Debug.LogError($"[BuildCommand][ExecuteBuildAssetbundle][Failure] 构建失败 : {buildResult.ErrorInfo}");
            EditorApplication.Exit(1);
        }
    }

    /// <summary>
    /// 从命令行中执行BuildPrepareAction
    /// </summary>
    //public static void CMD_ExecuteBuildPrepareAction()
    //{
    //    Debug.Log("////////////////////[CMD_ExecuteBuildPrepareAction]////////////////////");
    //    RunBuildPrepareAction(BuildPrepareActionArgs.LoadFromCommandLine());
    //    EditorApplication.Exit(0);
    //}

    /// <summary>
    /// 执行构建
    /// </summary>
    public static bool ExecuteBuildAssetbundle(AssetBundleBuildArgs args, out BuildResult buildResult)
    {
        string PackageName = "Default";
        EBuildPipeline BuildPipeline = EBuildPipeline.ScriptableBuildPipeline;


        ScriptableBuildParameters buildParameters = new ScriptableBuildParameters();
        buildParameters.BuildOutputRoot = args.BuildOutputRoot;
        buildParameters.BuildinFileRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();
        buildParameters.BuildPipeline = BuildPipeline.ToString();
        buildParameters.BuildTarget = args.BuildTarget;
        buildParameters.BuildMode = args.BuildMode;
        buildParameters.PackageName = PackageName;
        buildParameters.PackageVersion = GetDefaultBuildVersion();
        buildParameters.EnableSharePackRule = true;
        buildParameters.VerifyBuildingResult = true;
        buildParameters.FileNameStyle = args.FileNameStyle;
        buildParameters.BuildinFileCopyOption = args.BuildinFileCopyOption;
        buildParameters.BuildinFileCopyParams = args.BuildinFileCopyParams;
        buildParameters.EncryptionServices = CreateEncryptionInstance(PackageName, BuildPipeline);
        buildParameters.CompressOption = args.CompressOption;
        buildParameters.CopyBuildToPaths = args.CopyBuildToPaths;

        ScriptableBuildPipeline pipeline = new ScriptableBuildPipeline();
        buildResult = pipeline.Run(buildParameters, true);
        return buildResult.Success;
    }

    /// <summary>
    /// 创建加密类实例
    /// </summary>
    public static IEncryptionServices CreateEncryptionInstance(string PackageName, EBuildPipeline BuildPipeline)
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
        //int totalSecond = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
        //return DateTime.Now.ToString("yyyy-MM-dd") + "-" + totalSecond;
        return DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
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
        if (!string.IsNullOrEmpty(BuildTargetStr))
            return false;
        try
        {
            buildTarget = (BuildTarget)Enum.Parse(typeof(BuildTarget), BuildTargetStr);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
        return true;
    }

    /// <summary>
    /// 收集项目中的BuildPrepareAction类
    /// </summary>
    //static void CollectBuildPrepareAction()
    //{
    //    if (actions == null || actions.Count == 0)
    //    {
    //        actions = new List<BuildPrepareAction>();
    //        foreach (var t in TypeCache.GetTypesDerivedFrom<BuildPrepareAction>().Where(t => !t.IsAbstract))
    //        {
    //            var a = Activator.CreateInstance(t) as BuildPrepareAction;
    //            actions.Add(a);
    //        }
    //    }
    //}

    /// <summary>
    /// 收集项目中的SuperUnityBuild.BuildTool.BuildSettings资产
    /// </summary>
    //static void CollectSuperUnityBuildSettings()
    //{
    //    buildSetttings = new List<BuildSettings>();
    //    buildSetttings.AddRange(AssetDatabase.FindAssets("t:BuildSettings", new string[] { "Assets/SuperUnityBuild/Settings" }).Select(AssetDatabase.GUIDToAssetPath).Select(x => AssetDatabase.LoadAssetAtPath<BuildSettings>(x)).Where(x => x != null));
    //}

    /// <summary>
    /// 执行对应名字的BuildPrepareAction
    /// </summary>
    /// <param name="actionName"></param>
    //public static void RunBuildPrepareAction(string actionName)
    //{
    //    if (string.IsNullOrEmpty(actionName))
    //        return;
    //    var inst = actions.Find(item => item.GetType().Name == actionName);
    //    if (inst == null)
    //        return;
    //    Debug.LogWarning($"[BuildCommand][RunBuildPrepareAction] Execute {actionName}");
    //    try
    //    {
    //        inst.Execute(new Progress<InfoValue<float>>(x => { }));
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError(e);
    //    }
    //}
    //public static void RunBuildPrepareAction(BuildPrepareActionArgs args)
    //{
    //    if (!args.IsValid())
    //    {
    //        Debug.Log("[BuildCommand][RunBuildPrepareAction] args.IsValid() == false");
    //        return;
    //    }
    //    for (int i = 0; i < args.actionNames.Count; i++)
    //    {
    //        RunBuildPrepareAction(args.actionNames[i]);
    //    }
    //}

    /// <summary>
    /// SuperUnityBuildArgs
    /// </summary>
    public struct SuperUnityBuildArgs
    {
        public string BuildSettingName;
        public string CopyBuildTo;

        public static SuperUnityBuildArgs Default => new SuperUnityBuildArgs()
        {
            BuildSettingName = "DefaultBuildSetting_DevBuild",
            CopyBuildTo = string.Empty,
        };
        public static SuperUnityBuildArgs Empty => new SuperUnityBuildArgs()
        {
            BuildSettingName = string.Empty,
            CopyBuildTo = string.Empty,
        };

        public const string ArgKey_BuildSettingName = "-buildSettingName";
        public const string ArgKey_CopyBuildTo = "-copyBuildTo";
        public const char separator = ';';
        /// <summary>
        /// 从命令行中获取参数
        /// </summary>
        /// <returns></returns>
        public static SuperUnityBuildArgs LoadFromCommandLine()
        {
            Debug.Log("[BuildCommand][SuperUnityBuildArgs.LoadFromCommandLine]");

            SuperUnityBuildArgs temp = Default;

            var args = Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length; i++)
            {
                Debug.Log(args[i]);
                if (args[i].StartsWith(ArgKey_BuildSettingName))
                    temp.BuildSettingName = args[i].Split('=')[1];
                if (args[i].StartsWith(ArgKey_CopyBuildTo))
                    temp.CopyBuildTo = args[i].Split('=')[1];
            }

            return temp;
        }
        public bool ShouldCopyBuildTo()
        {
            return !string.IsNullOrEmpty(CopyBuildTo);
        }

        /// <summary>
        /// 是否有效
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(BuildSettingName))
                return false;
            return true;
        }
    }

    /// <summary>
    /// BuildPrepareAction
    /// </summary>
    //public struct BuildPrepareActionArgs
    //{
    //    /// <summary>
    //    /// BuildPrepareAction的类名
    //    /// </summary>
    //    public List<string> actionNames;

    //    public static BuildPrepareActionArgs Default => new BuildPrepareActionArgs
    //    {
    //        actionNames = new List<string>() {
    //            nameof(ValidateGlobalConfigAssetName),
    //            nameof(ProcessTVEMeshes),
    //            nameof(RefreshECFGAssetReference),
    //        }
    //    };
    //    public static BuildPrepareActionArgs Empty => new BuildPrepareActionArgs
    //    {
    //        actionNames = new List<string>() { },
    //    };

    //    public const string ArgKey_BuildPrepareActionNames = "-buildPrepareActionNames";
    //    /// <summary>
    //    /// 分号Action名字以分号隔开
    //    /// 例如 -buildPrepareActionNames "ValidateGlobalConfigAssetName;ProcessTVEMeshes;RefreshECFGAssetReference"
    //    /// </summary>
    //    public const char separator = ';';

    //    public override string ToString()
    //    {
    //        if (actionNames == null || actionNames.Count <= 0)
    //            return "No Any BuildPrepareNames";
    //        StringBuilder sb = new StringBuilder();
    //        for (int j = 0; j < actionNames.Count; j++)
    //            sb.AppendLine(actionNames[j]);
    //        return sb.ToString();
    //    }

    //    /// <summary>
    //    /// 是否有效
    //    /// </summary>
    //    /// <returns></returns>
    //    public bool IsValid()
    //    {
    //        if (actionNames == null || actionNames.Count <= 0)
    //            return false;
    //        return true;
    //    }

    //    /// <summary>
    //    /// 从命令行中获取参数
    //    /// </summary>
    //    /// <returns></returns>
    //    public static BuildPrepareActionArgs LoadFromCommandLine()
    //    {
    //        Debug.Log("[BuildCommand][BuildPrepareActionArgs.LoadFromCommandLine]");

    //        BuildPrepareActionArgs temp = BuildPrepareActionArgs.Empty;

    //        var args = Environment.GetCommandLineArgs();

    //        for (int i = 0; i < args.Length; i++)
    //        {
    //            Debug.Log(args[i]);
    //            if (args[i].StartsWith(ArgKey_BuildPrepareActionNames))
    //            {
    //                var names = args[i].Split('=')[1].Split(separator, StringSplitOptions.RemoveEmptyEntries);
    //                if (names == null || names.Length <= 0)
    //                    break;
    //                for (int j = 0; j < names.Length; j++)
    //                {
    //                    var name = names[j];
    //                    Debug.Log(name);
    //                    if (string.IsNullOrEmpty(name))
    //                        continue;
    //                    temp.actionNames.Add(name);
    //                }
    //            }
    //        }
    //        return temp;
    //    }
    //}

    /// <summary>
    /// AssetBundle构建参数
    /// </summary>
    public struct AssetBundleBuildArgs
    {
        /// <summary>
        /// 输出路径
        /// 打包可能会出现PathTooLongException
        /// 原因是:windows系统路径的最大长度限制是260个字符(.Net 4.6.2,取消了这个限制)
        /// </summary>
        public string BuildOutputRoot;
        /// <summary>
        /// 目标平台
        /// </summary>
        public BuildTarget BuildTarget;
        /// <summary>
        /// 构建模式
        /// </summary>
        public EBuildMode BuildMode;
        /// <summary>
        /// 文件名字风格
        /// </summary>
        public EFileNameStyle FileNameStyle;
        /// <summary>
        /// 首包(内置)资源的复制设置
        /// </summary>
        public EBuildinFileCopyOption BuildinFileCopyOption;
        /// <summary>
        /// 首包资源标记
        /// </summary>
        public string BuildinFileCopyParams;
        /// <summary>
        /// 压缩选项
        /// </summary>
        public ECompressOption CompressOption;
        /// <summary>
        /// 复制ab文件到一个或多个路径，多路径路径使用;分号分割
        /// </summary>
        public string CopyBuildToPaths;

        public static AssetBundleBuildArgs Default => new AssetBundleBuildArgs()
        {
            BuildMode = EBuildMode.IncrementalBuild,
            FileNameStyle = EFileNameStyle.HashName,
            BuildinFileCopyOption = EBuildinFileCopyOption.ClearAndCopyAll,
            BuildinFileCopyParams = "",
            CompressOption = ECompressOption.LZ4,
            CopyBuildToPaths = ""
        };
        public static AssetBundleBuildArgs LoadFromEditorPref(string PackageName, EBuildPipeline BuildPipeline)
        {
            AssetBundleBuildArgs temp = Default;
            try { temp.BuildOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot(); } catch { }
            try { temp.BuildTarget = EditorUserBuildSettings.activeBuildTarget; } catch { }
            try { temp.BuildMode = AssetBundleBuilderSetting.GetPackageBuildMode(PackageName, BuildPipeline); } catch { }
            try { temp.FileNameStyle = AssetBundleBuilderSetting.GetPackageFileNameStyle(PackageName, BuildPipeline); } catch { }
            try { temp.BuildinFileCopyOption = AssetBundleBuilderSetting.GetPackageBuildinFileCopyOption(PackageName, BuildPipeline); } catch { }
            try { temp.BuildinFileCopyParams = AssetBundleBuilderSetting.GetPackageBuildinFileCopyParams(PackageName, BuildPipeline); } catch { }
            try { temp.CompressOption = AssetBundleBuilderSetting.GetPackageCompressOption(PackageName, BuildPipeline); } catch { }
            try { temp.CopyBuildToPaths = AssetBundleBuilderSetting.GetPackageCopyBuildToPaths(PackageName, BuildPipeline); } catch { }
            return temp;
        }

        public const string ArgKey_PackageName = "-packagePackageNam";
        //
        public const string ArgKey_BuildTarget = "-buildTarget";
        public const string ArgKey_BuildOutputRoot = "-packageBuildOutputRoot";
        public const string ArgKey_BuildMode = "-packageBuildMode";
        public const string ArgKey_FileNameStyle = "-packageFileNameStyle";
        public const string ArgKey_BuildinFileCopyOption = "-packageBuildinFileCopyOption";
        public const string ArgKey_BuildinFileCopyParams = "-packageBuildinFileCopyParams";
        public const string ArgKey_CompressOption = "-packageCompressOption";
        public const string ArgKey_CopyBuildToPaths = "-packageCopyBuildToPaths";

        public const char separator = ' ';
        /// <summary>
        /// 从命令行中获取参数
        /// </summary>
        /// <returns></returns>
        public static AssetBundleBuildArgs LoadFromCommandLine()
        {
            Debug.Log("[BuildCommand][AssetBundleBuildArgs.LoadFromCommandLine]");

            AssetBundleBuildArgs temp = Default;

            var args = Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length; i++)
            {
                Debug.Log(args[i]);
                if (args[i].StartsWith(ArgKey_BuildTarget))
                    temp.BuildTarget = (BuildTarget)Enum.Parse(typeof(BuildTarget), args[i].Split('=')[1]);
                if (args[i].StartsWith(ArgKey_BuildOutputRoot))
                    temp.BuildOutputRoot = args[i].Split('=')[1];
                if (args[i].StartsWith(ArgKey_BuildMode))
                    temp.BuildMode = (EBuildMode)Enum.Parse(typeof(EBuildMode), args[i].Split('=')[1]);
                if (args[i].StartsWith(ArgKey_FileNameStyle))
                    temp.FileNameStyle = (EFileNameStyle)Enum.Parse(typeof(EFileNameStyle), args[i].Split('=')[1]);
                if (args[i].StartsWith(ArgKey_BuildinFileCopyOption))
                    temp.BuildinFileCopyOption = (EBuildinFileCopyOption)Enum.Parse(typeof(EBuildinFileCopyOption), args[i].Split('=')[1]);
                if (args[i].StartsWith(ArgKey_BuildinFileCopyParams))
                    temp.BuildinFileCopyParams = args[i].Split('=')[1];
                if (args[i].StartsWith(ArgKey_CompressOption))
                    temp.CompressOption = (ECompressOption)Enum.Parse(typeof(ECompressOption), args[i].Split('=')[1]);
                if (args[i].StartsWith(ArgKey_CopyBuildToPaths))
                    temp.CopyBuildToPaths = args[i].Split('=')[1];
            }

            if (string.IsNullOrEmpty(temp.BuildOutputRoot))
                temp.BuildOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();

            return temp;
        }
    }
}
