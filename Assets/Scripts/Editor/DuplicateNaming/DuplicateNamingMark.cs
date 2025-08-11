using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using InfinityCode.UltimateEditorEnhancer;
using InfinityCode.UltimateEditorEnhancer.ProjectTools;
using PJR;
using PJR.ClassExtension;
using UnityEditor;
using UnityEngine;
using static LS.LSEditor.DuplicateNamingMark.Report;
using Object = UnityEngine.Object;
using Resources = UnityEngine.Resources;

namespace LS.LSEditor
{
    [InitializeOnLoad]
    public static partial class DuplicateNamingMark
    {
        public const string LS_Editor_DuplicateNamingMark_Enable = "LS_Editor_DuplicateNamingMark_Enable";
        private static bool? _enable;
        public static bool Enable
        {
            get
            {
                _enable ??= EditorPrefs.GetBool(LS_Editor_DuplicateNamingMark_Enable, false);
                return _enable.Value;
            }
            set
            {
#if UNITY_EDITOR
                _enable = value;
                EditorPrefs.SetBool(LS_Editor_DuplicateNamingMark_Enable, value);
#endif
            }
        }

        static DuplicateNamingMark()
        {
            ProjectItemDrawer.Register("DrawDuplicatedNamingMark", DrawDuplicatedNamingMark, -10);
            RegisterAssetModifyEvent();
        }

        static List<Report> _reports;
        public static bool AnyRequire() =>_reports?.Count>0;

        public static Report RequireCheck(string directoryPath)
        {
            if(string.IsNullOrEmpty(directoryPath))
                return null;
            _reports ??= new List<Report>();

            Report report = null;
            int index = -1;
            for (int i = 0; i < _reports.Count; i++)
            {
                var req = _reports[i];
                if (req == directoryPath)
                    index = i;
                else if (FileUtil.IsWithinDirectory(req.DirectoryPath, directoryPath))
                {
                    _reports.RemoveAt(i--);
                    continue;
                }
            }

            if (index < 0)
            {
                report = new Report(directoryPath);
                _reports.Add(report);
            }
            else
                report = _reports[index];

            report.Analyse();
            return report;
        }

        public static void RequireCheck(List<string> directoryPaths)
        {
            directoryPaths.ForEach(directory => RequireCheck(directory));
        }

        public static void RemoveReport(Report report)
        {
            if(_reports == null)
                return;
            _reports.Remove(report);
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }


        public const string DuplicatedNaming = "重名";
        private static StringBuilder _sb;

        static void DrawDuplicatedNamingMark(ProjectItem item)
        {
            if (!Enable)
                return;
            if (!TryGetFileNode(item.path, out Report report,out var fileNode))
                return;

            var isFolder = fileNode.IsFolder;
            var isRequireTarget = item.path == report.DirectoryPath;
            var isCheckDirectory = isFolder && isRequireTarget;


            //文件icon旁文字
            Rect labetRect = GUIUtil.GetZeroRect(item);
            if (!fileNode.IsFolder && fileNode.NeedToNodify)
            {
                labetRect = GUIUtil.GetLabelRect(item, DuplicatedNaming, Styles.GetLabelStyle());
                GUI.Label(labetRect, DuplicatedNaming);
            }

            //检查目录icon
            Rect iconRect = GUIUtil.GetZeroRect(item);
            if (isCheckDirectory)
            {
                iconRect = GUIUtil.GetIconRect(item);
                Texture folderIcon = report.AnyDuplicatedNamingFile() ? Styles.Icon_RedCrossMark : Styles.Icon_GreenCheckMark;
                GUI.Box(iconRect, new GUIContent(folderIcon,"左键:输出目录下所有重名文件,右键:显工具菜单"), EditorStyles.iconButton);
            }
            //文件icon
            else if (!isFolder && fileNode.NeedToNodify)
            {
                iconRect = GUIUtil.GetIconRect(item);
                Texture folderIcon = Styles.WarningIcon;
                GUI.Box(iconRect, new GUIContent(folderIcon, "左键:输出目录下所有重名文件"), EditorStyles.iconButton);
            }


            //点目录icon + 标题的区域时
            //左键
            if (GUIUtil.LeftClick(GUIUtil.GetRectUnion(labetRect, iconRect)))
            {
                _sb ??= new StringBuilder();

                //主目录
                if (isFolder && isCheckDirectory)
                {
                    foreach (string assetPath in report.GetAllNeedToNotifyUnderDirectory(item.path))
                    {
                        if (string.IsNullOrEmpty(assetPath))
                            continue;
                        _sb.AppendLine(HyperLinkUtil.GetAssetLink(assetPath));
                    }
                }
                //具体文件
                else if(!isFolder)
                {
                    foreach (string assetPath in report.GetAssetPathWithSameName(item.path))
                    {
                        if (string.IsNullOrEmpty(assetPath))
                            continue;
                        _sb.AppendLine(HyperLinkUtil.GetAssetLink(assetPath));
                    }
                }

                if (_sb.Length > 0) Debug.Log(_sb.ToString());
                _sb.Clear();
            }
            //右键
            if (GUIUtil.RightClick(GUIUtil.GetRectUnion(labetRect, iconRect)))
            {
                var menu = new GenericMenu();
                if (isRequireTarget)
                {
                    menu.AddItem("移除此标记", false,()=>RemoveReport(report));
                }
                menu.ShowAsContext();
            }

            //类型显示按钮
            if (isFolder)
            {
                //主目录
                if (isRequireTarget)
                {
                    bool updateNotifyDisplay = false;
                    foreach (var notifyType in report.notifiableManager.GetDisplayTypes())
                    {
                        var rect = GUIUtil.GetIconRect(item);
                        if (GUI.Button(rect, new GUIContent(notifyType.Icon, $"显示:{notifyType.AssetType.Name}"),
                                EditorStyles.iconButton))
                        {
                            notifyType.IsOn = !notifyType.IsOn;
                            updateNotifyDisplay = true;
                        }

                        //类型显示按钮的提示用角标(绿点红点)
                        rect.xMin += 8;
                        rect.yMin += 8;
                        GUI.DrawTexture(rect, notifyType.IsOn ? Styles.Icon_Corner_On : Styles.Icon_Corner_Off);
                    }

                    if (updateNotifyDisplay)
                        report.UpdateNotifiable();
                }
                //子目录
                else
                {
                    subFolderTypesNeedToNotify.Clear();
                    foreach (var node in fileNode.Traverse())
                    {
                        if (!node.IsFolder && node.NeedToNodify)
                        {
                            if (!subFolderTypesNeedToNotify.Contains(node.AssetType))
                                subFolderTypesNeedToNotify.Add(node.AssetType);
                        }
                    }

                    foreach (var type in subFolderTypesNeedToNotify)
                    {
                        var notifyType = AllNotifyAssetTypes.Find(item => item.IsTypeMatch(type));
                        if(notifyType == null)
                            continue;
                        var rect = GUIUtil.GetIconRect(item);
                        GUI.DrawTexture(rect, notifyType.Icon);
                    }
                }
            }
        }

        private static List<Type> subFolderTypesNeedToNotify = new List<Type>();

        private static void RegisterAssetModifyEvent()
        {
            AssetProcessor.OnWillCreateAssetCall -= OnWillCreateAsset;
            AssetProcessor.OnWillCreateAssetCall += OnWillCreateAsset;
            AssetProcessor.OnWillDeleteAssetCall -= OnWillDeleteAsset;
            AssetProcessor.OnWillDeleteAssetCall += OnWillDeleteAsset;
            AssetProcessor.OnWillMoveAssetCall -= OnWillMoveAsset;
            AssetProcessor.OnWillMoveAssetCall += OnWillMoveAsset;
        }
        private static void UnRegisterAssetModifyEvent()
        {
            AssetProcessor.OnWillCreateAssetCall -= OnWillCreateAsset;
            AssetProcessor.OnWillDeleteAssetCall -= OnWillDeleteAsset;
            AssetProcessor.OnWillMoveAssetCall -= OnWillMoveAsset;
        }

        //这个时候是拿不到guid的
        static void OnWillCreateAsset(string assetName)
        {
            //Debug.Log($"OnWillCreateAsset: {assetName}");
            FileUtil.IsMetaFile(assetName, out assetName);
            EditorApplication.delayCall += () => ReAnalyseIfContainsAssetPath(assetName);
        }
        static void OnWillDeleteAsset(string assetName, RemoveAssetOptions options)
        {
            //Debug.Log($"OnWillDeleteAsset: {assetName}, [Option: {options}]");
            //可能是Ctrl+D触发的
            if (options == RemoveAssetOptions.DeleteAssets)
                return;
            EditorApplication.delayCall += () => ReAnalyseIfContainsAssetPath(assetName);
        }
        static void OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            //Debug.Log($"OnWillMoveAsset: {sourcePath} /n {destinationPath}");
            EditorApplication.delayCall += () => ReAnalyseIfContainsAssetPath(sourcePath);
        }

        static void ReAnalyseIfContainsAssetPath(string assetPath)
        {
            if(!AnyRequire())
                return;
            foreach (Report require in _reports)
            {
                if (!require.Valid)
                    continue;
                if (!require.IsWithinDirectory(assetPath))
                    continue;
                require.Analyse();
            }
        }

        static bool NeedToNotify(string filePath, out Report req, out FileNode fileNode)
        {
            return TryGetFileNode(filePath, out req, out fileNode, node => node.NeedToNodify);
        }
        static bool TryGetFileNode(string filePath, out Report req, out FileNode fileNode, Func<FileNode, bool> predicate = null)
        {
            req = null;
            fileNode = null;
            if (_reports == null || _reports.Count <= 0)
                return false;
            if (string.IsNullOrEmpty(filePath))
                return false;
            foreach (Report require in _reports)
            {
                if (!require.Valid)
                    continue;
                if (!require.TryGetFileNode(filePath, out fileNode, predicate))
                    continue;
                req = require;
                return true;
            }
            return false;
        }

        public class AssetProcessor : AssetModificationProcessor
        {
            public static event Action<string> OnWillCreateAssetCall;
            public static event Action<string, RemoveAssetOptions> OnWillDeleteAssetCall;
            public static event Action<string, string> OnWillMoveAssetCall;
            static void OnWillCreateAsset(string assetPath)
            {
                try
                {
                    OnWillCreateAssetCall?.Invoke(assetPath);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
            static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
            {
                try
                {
                    OnWillDeleteAssetCall?.Invoke(assetPath, options);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                AssetDeleteResult result = AssetDeleteResult.DidNotDelete;
                return result;
            }
            static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
            {
                try
                {
                    OnWillMoveAssetCall?.Invoke(sourcePath, destinationPath);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                AssetMoveResult assetMoveResult = AssetMoveResult.DidNotMove;

                return assetMoveResult;
            }
        }
        public class Report
        {
            public static implicit operator string(Report req) => req.DirectoryPath;

            private bool valid;
            private Dictionary<string, List<string>> assetName2AssetPaths = new Dictionary<string, List<string>>();
            public Dictionary<string, List<string>> AssetName2AssetPaths => assetName2AssetPaths;

            //平铺的重名数据
            private List<string> needToNotifyAssetPaths = new List<string>();

            //树状的重名数据
            private FileTree _tree;
            public FileTree Tree => _tree ??= new FileTree(this, DirectoryPath);

            private string _assetFilter = null;
            public string AssetFilter  => _assetFilter;

            private string _directoryPath;
            public string DirectoryPath => _directoryPath;
            public bool Valid => valid;

            public NotifiableManager notifiableManager { get; private set; }

            public Report(string directoryPath, string assetFilter = null)
            {
                _directoryPath = directoryPath;
                _assetFilter = assetFilter;
                assetName2AssetPaths = null;
                valid = false;

                Analyse();
            }

            public void Analyse()
            {
                valid = AssetDatabase.IsValidFolder(_directoryPath);
                if (!valid)
                    return;
                var guids = AssetDatabase.FindAssets(_assetFilter, new string[] { DirectoryPath });
                assetName2AssetPaths ??= new Dictionary<string, List<string>>();
                assetName2AssetPaths.Clear();

                foreach (string guid in guids)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    if (string.IsNullOrEmpty(assetPath))
                        continue;
                    if (AssetDatabase.IsValidFolder(assetPath))
                        continue;

                    string name = Path.GetFileName(assetPath);
                    if (name.StartsWith("__"))
                        continue;
                    if (string.IsNullOrEmpty(name))
                        continue;
                    if (!assetName2AssetPaths.TryGetValue(name, out List<string> guidList))
                    {
                        guidList = new List<string>();
                        assetName2AssetPaths.Add(name, guidList);
                    }
                    guidList.Add(assetPath);
                }

                //获取一个平铺的重名数据
                needToNotifyAssetPaths ??= new List<string>();
                needToNotifyAssetPaths.Clear();
                Action<string> AddNotify = assetPath =>
                {
                    if (!needToNotifyAssetPaths.Contains(assetPath))
                        needToNotifyAssetPaths.Add(assetPath);
                };

                foreach (var pair in assetName2AssetPaths)
                {
                    if (pair.Value.Count <= 1)
                        continue;

                    //Debug.Log(pair.Key);

                    foreach (var assetPath in pair.Value)
                    {
                        AddNotify(assetPath);
                        
                        var directories = FileUtil.GetAllDirectories(assetPath, _directoryPath);
                        foreach (var directory in directories)
                            AddNotify(directory);
                    }
                }

                _tree = new FileTree(this, DirectoryPath);
                notifiableManager = new NotifiableManager(GetNotifyAssetTypes());
            }

            //更新需要显示重名信息的Asset类型
            //只在显示选项改变的时候才更新
            public void UpdateNotifiable()
            {
                foreach (var fileNode in Tree.Traverse())
                    fileNode.Notifiable = notifiableManager.IsTypeNotifiable(fileNode.AssetType);
            }

            /// <summary>
            /// 获取目录所有需要提醒的文件
            /// </summary>
            /// <param name="directory"></param>
            /// <param name="includeFolder"></param>
            /// <returns></returns>
            public IEnumerable<string> GetAllNeedToNotifyUnderDirectory(string directory, bool includeFolder = false)
            {
                if (!AssetDatabase.IsValidFolder(directory))
                    yield return null;
                var res = new List<string>();
                foreach (string assetPath in needToNotifyAssetPaths)
                {
                    if(AssetDatabase.IsValidFolder(assetPath) && !includeFolder)
                        continue;
                    if(FileUtil.IsWithinDirectory(assetPath, directory))
                        yield return assetPath;
                }
            }

            /// <summary>
            /// 获取所有与之同名的文件
            /// </summary>
            /// <param name="assetPath"></param>
            /// <returns></returns>
            public IEnumerable<string> GetAssetPathWithSameName(string assetPath)
            {
                var fileName = Path.GetFileName(assetPath);
                if (assetName2AssetPaths.TryGetValue(fileName, out var assetPaths) && assetPaths != null)
                {
                    foreach (string path in assetPaths)
                        yield return path;
                }
            }

            /// <summary>
            /// 文件是否需要被铁行
            /// </summary>
            /// <param name="assetPath"></param>
            /// <param name="outFileNode"></param>
            /// <returns></returns>
            public bool NeedToNotify(string assetPath, out FileNode outFileNode)
            {
                return TryGetFileNode(assetPath ,out outFileNode, node => node.NeedToNodify);
            }

            public bool TryGetFileNode(string assetPath, out FileNode outFileNode, Func<FileNode, bool> predicate = null)
            {
                outFileNode = null;
                if (!valid)
                    return false;

                foreach (var fileNode in Tree.Traverse())
                {
                    if (fileNode.AssetPath == assetPath)
                    {
                        if (predicate != null && !predicate.Invoke(fileNode))
                            return false;

                        outFileNode = fileNode;
                        return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// 判断文件是否在目录下
            /// </summary>
            /// <param name="filePath"></param>
            /// <returns></returns>
            public bool IsWithinDirectory(string filePath)
            {
                return FileUtil.IsWithinDirectory(filePath, DirectoryPath);
            }

            /// <summary>
            /// 获取某个文件名的重复数量
            /// </summary>
            /// <param name="fileNameWithExtension"></param>
            /// <returns></returns>
            public int GetDuplicatedNamingFileCount(string fileNameWithExtension)
            {
                if (!valid)
                    return 0;
                if (!assetName2AssetPaths.TryGetValue(fileNameWithExtension, out var guids))
                    return 0;
                return guids?.Count ?? 0;
            }

            public bool ThisFileNameHadDuplicate(string fileNameWithExtension)
            {
                return GetDuplicatedNamingFileCount(fileNameWithExtension) > 1;
            }

            public bool AnyDuplicatedNamingFile()
            {
                return needToNotifyAssetPaths.Count > 0;
            }

            public class FileTree
            {
                public FileNode Root;
                public string DirectoryPath;
                public Report Report { get;private set; }

                public FileTree(Report report, string directoryPath)
                {
                    Report = report;
                    DirectoryPath = directoryPath;
                    Root = new FileNode(Report, DirectoryPath, true);
                    BuildTreeRecursive(Root, DirectoryPath);
                }
                public IEnumerable<FileNode> Traverse() => TraverseRecursive(Root);
                private static IEnumerable<FileNode> TraverseRecursive(FileNode fileNode)
                {
                    yield return fileNode;
                    foreach (var child in fileNode.Childs)
                    {
                        foreach (var node in TraverseRecursive(child))
                            yield return node;
                    }
                }
                private void BuildTreeRecursive(FileNode parentNode, string directory, int depth = 0)
                {
                    if (!AssetDatabase.IsValidFolder(directory))
                        return;
                    var subDirectories = Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly);
                    foreach (var subDirectory in subDirectories)
                    {
                        var subNode = new FileNode(Report, subDirectory, true);
                        subNode.ParentNode = parentNode;
                        parentNode.Childs.Add(subNode);
                        BuildTreeRecursive(subNode, subDirectory, depth + 1);
                    }

                    var assetPaths = Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly);
                    foreach (var assetPath in assetPaths)
                    {
                        if (FileUtil.IsMetaFile(assetPath))
                            continue;
                        var node = new FileNode(Report, assetPath, false);
                        node.ParentNode = parentNode;
                        parentNode.Childs.Add(node);
                    }
                }

                public FileNode FindNode(string assetPath)
                {
                    if (string.IsNullOrEmpty(assetPath))
                        return null;
                    foreach (var node in Traverse())
                    {
                        if (node.AssetPath == assetPath)
                            return node;
                    }

                    return null;
                }

                public FileNode FindNode(Func<FileNode,bool> predicate)
                {
                    if (predicate == null)
                        return null;

                    foreach (var node in Traverse())
                    {
                        if (predicate.Invoke(node))
                            return node;
                    }

                    return null;
                }
            }

            public class FileNode 
            {
                public FileNode ParentNode;
                public Report Report { get; private set; }
                public string AssetPath;
                public List<FileNode> Childs;
                public bool IsFolder;

                private string _fileName;
                private Object _asset;
                public string FileName => _fileName ??= Path.GetFileName(AssetPath);
                public Object Asset => _asset ??= AssetDatabase.LoadAssetAtPath<Object>(AssetPath);
                public Type AssetType => Asset.GetType();

                /// <summary>
                /// 不主动更新
                /// </summary>
                private bool _notifiable = true;

                /// <summary>
                /// 可不可以显示提示GUI
                /// </summary>
                public bool Notifiable
                {
                    set => _notifiable = value;
                    get
                    {
                        if (IsFolder)
                            return true;
                        return _notifiable;
                    }
                }

                /// <summary>
                /// 有没有重名的文件
                /// </summary>
                public bool HadDuplicatedNaming = false;


                //需不需要显示提示GUI
                public bool NeedToNodify
                {
                    get
                    {
                        if(!IsFolder)
                            return Notifiable && HadDuplicatedNaming;
                        if (AssetPath == Report.DirectoryPath)
                            return true;
                        foreach (var child in Childs)
                        {
                            if (child.NeedToNodify)
                                return true;
                        }
                        return false;
                    }
                }
                public FileNode(Report report, string assetPath, bool isFolder)
                {
                    AssetPath = FileUtil.NormalizeSeparator(assetPath);
                    IsFolder = isFolder;
                    Childs = new List<FileNode>();
                    Report = report;
                    HadDuplicatedNaming = Report.ThisFileNameHadDuplicate(FileName);
                }
                public IEnumerable<FileNode> Traverse() => TraverseRecursive(this);
                private static IEnumerable<FileNode> TraverseRecursive(FileNode fileNode)
                {
                    yield return fileNode;
                    foreach (var child in fileNode.Childs)
                    {
                        foreach (var node in TraverseRecursive(child))
                            yield return node;
                    }
                }
                public bool AnyDuplicateNameWith()
                {
                    if (!Report.AssetName2AssetPaths.TryGetValue(FileName, out var assetPaths))
                        return false;
                    return assetPaths != null && assetPaths.Count > 1;
                }

                public bool AnyDuplicatedNamingFile() => Report.ThisFileNameHadDuplicate(Path.GetFileName(AssetPath));

                private List<NotifyType> _notifyTypes;
                //<仅限文件夹>获取这个Node下所有通知类型
                public IEnumerable<NotifyType> GetNotifyTypes()
                {
                    if (!IsFolder)
                    {
                        if(!HadDuplicatedNaming)
                            yield break;
                        int index = AllNotifyAssetTypes.FindIndex(item => item.IsTypeMatch(AssetType));
                        if(index >= 0)
                            yield return AllNotifyAssetTypes[index];
                        yield break;
                    }

                    if (_notifyTypes == null)
                    {
                        _notifyTypes = new List<NotifyType>();
                        foreach (var child in Childs)
                        {
                            foreach (var notifyType in child.GetNotifyTypes())
                            {
                                if(_notifyTypes.All(item => item.AssetType != notifyType.AssetType))
                                    _notifyTypes.Add(notifyType.Clone());
                            }
                        }
                    }

                    foreach (var notifyType in _notifyTypes)
                        yield return notifyType;
                }
            }

            public class NotifiableManager
            {
                private List<NotifyType> _notifyTypes;
                public List<NotifyType> NotifyTypes => _notifyTypes;

                public NotifiableManager(List<NotifyType> notifyTypes)
                {
                    _notifyTypes = notifyTypes;
                }

                public IEnumerable<NotifyType> GetDisplayTypes()
                {
                    foreach (var notifyType in _notifyTypes)  
                    {
                        if(!notifyType.HideInMenu)
                            yield return notifyType;
                    }
                }

                public bool IsTypeNotifiable(Type type)
                {
                    foreach (var notifyType in _notifyTypes)
                    {
                        if (notifyType.AssetType == type || notifyType.AssetType.IsAssignableFrom(type))
                        {
                            return notifyType.IsOn;
                        }
                    }

                    return false;
                }
            }

            public class NotifyType
            {
                private string _filter;
                private Texture _icon;

                public string Content;
                public string[] LowCaseFileExtension;
                public bool IsOn;
                public bool HideInMenu;
                public Type AssetType;

                public Texture Icon => _icon;
                public string Filter => _filter;

                public NotifyType(Type assetType,string content, string[] lowCaseFileExtension, bool hideInMenu)
                {
                    _filter = $"t:{content}";
                    this.Content = content;
                    this.LowCaseFileExtension = lowCaseFileExtension;
                    this.IsOn = true;
                    this.AssetType = assetType;
                    this.HideInMenu = hideInMenu;
                    _icon = AssetPreview.GetMiniTypeThumbnail(assetType);
                }

                public NotifyType Clone()
                {
                    return new(AssetType, Content, LowCaseFileExtension, HideInMenu);
                }

                public bool IsTypeMatch(Type type)
                {
                    return type == AssetType || type.IsSubclassOf(AssetType) || AssetType.IsAssignableFrom(type);
                }
            }


            public static List<NotifyType> AllNotifyAssetTypes = new List<NotifyType>() {
                new(typeof(AnimationClip),"AnimationClip",new string[]{".anim"}, false),
                new(typeof(AudioClip),"AudioClip",    new string[]{".ogg"}, false),
                //new(typeof(AudioMixer),"AudioMixer",   new string[]{".mixer"}, false),
                new(typeof(ComputeShader),"ComputeShader",new string[]{".compute"}, false),
                new(typeof(Font),"Font",         new string[]{".tff"}, false),
                //new SearchType(typeof(GUISkin),"GUISkin",      new string[]{}, false),
                new(typeof(Material),"Material",     new string[]{".mat"}, false),
                new(typeof(Mesh),"Mesh",         new string[]{".fbx",".fbc"}, false),
                new(typeof(GameObject),"Model",        new string[]{".fbx",".fbc"}, false),
                new(typeof(PhysicMaterial),"PhysicMaterial",new string[]{}, false),
                new(typeof(GameObject),"Prefab",       new string[]{".prefab"}, false),
                //new(typeof(Scene),"Scene",        new string[]{".unity"}, false),
                //new(typeof(Script),"Script",       new string[]{".cs"}, false),
                new(typeof(Shader),"Shader",       new string[]{".shader"}, false),
                //new(typeof(Sprite),"Sprite",       new string[]{}, false),
                new(typeof(Texture),"Texture",      new string[]{".png"}, false),
                //new(typeof(Folder),"Folder",      new string[]{""}, false),
            };


            /// <summary>
            /// 获取重名文件的所有通知类型
            /// </summary>
            /// <returns></returns>
            protected List<NotifyType> GetNotifyAssetTypes()
            {
                var list = new List<NotifyType>();
                var duplicatedNamingTypes = new List<Type>();
                //找出所有重名文件的类型
                foreach (var fileNode in Tree.Traverse())
                {
                    if(fileNode.IsFolder || !fileNode.HadDuplicatedNaming)
                        continue;
                    if(!duplicatedNamingTypes.Contains(fileNode.AssetType))
                        duplicatedNamingTypes.Add(fileNode.AssetType);
                }
                //根据重名文件类型，获取对应的通知类型
                foreach (var type in duplicatedNamingTypes)
                {
                    var index = AllNotifyAssetTypes.FindIndex(item =>  item.IsTypeMatch(type) );
                    if(index >= 0)
                        list.Add(AllNotifyAssetTypes[index].Clone());
                }

                return list;
            }
        }
        public static class EditorMenu
        {

            [MenuItem("Assets/重名标记/检查此目录", priority = 61)]
            public static void RequireDuplicateNamingCheckMenu()
            {
                if (!AssetSelectionUtil.GetSeletedFolderPaths(out List<string> folderPaths))
                    return;
                RequireCheck(folderPaths);
            }

            [MenuItem("Assets/重名标记/清理所有标记",priority = 62)]
            public static void ClearRequire()
            {
                _reports?.Clear();
                Resources.UnloadUnusedAssets();
                GC.Collect();
            }

            [MenuItem("Assets/重名标记/检查此目录", true)]
            public static bool RequireDuplicateNamingCheckMenu_Validation()
            {
                return AssetSelectionUtil.GetSeletedFolderPaths(out List<string> folderPaths);
            }

            [MenuItem("Assets/重名标记/文档(飞书)", priority = 63)]
            public static void OpenDocument_FeiShuDoc()
            {
                Application.OpenURL("https://n1qngrbrag8.feishu.cn/docx/PTandCrMZok8ILxTlXhcQ0SnnJb");
            }
            //[MenuItem("Assets/重名标记/文档(腾讯文档)", priority = 64)]
            //public static void OpenDocument_TencentDoc()
            //{
            //    Application.OpenURL("https://docs.qq.com/doc/DY1ZvZHpBaWhkS21m");
            //}
        }
        public static class Styles
        {
            public static GUIStyle EntryWarnIconSmallTextless { get; } = new GUIStyle("CN EntryWarnIconSmall");
            public static GUIStyle EntryErrorIconSmallTextless { get; } = new GUIStyle("CN EntryErrorIconSmall");
            public static Texture Icon_Corner_On { get; } = EditorGUIUtility.IconContent("d_winbtn_mac_max").image;
            public static Texture Icon_Corner_Off { get; } = EditorGUIUtility.IconContent("d_winbtn_mac_close").image;
            //public static Texture Icon_GreenCheckMark { get; } = EditorGUIUtility.IconContent("d_GreenCheckmark").image;
            public static Texture Icon_GreenCheckMark { get; } = EditorGUIUtility.IconContent("d_greenLight").image;
            public static Texture Icon_RedCrossMark { get; } = EditorGUIUtility.IconContent("winbtn_mac_close_h").image;
            public static Texture Icon_Warm { get; } = EditorGUIUtility.IconContent("d_console.warnicon.sml").image;

            public static Texture WarningIcon { get; } = EditorGUIUtility.IconContent("d_console.warnicon.sml").image;
            
            public static GUIStyle label;
            public static GUIStyle GetLabelStyle()
            {
                GUIStyle style = null;
                if (label == null)
                {
                    label = new GUIStyle(EditorStyles.label);
                    label.alignment = TextAnchor.MiddleRight;
                    label.padding.right = 10;
                    label.normal.textColor = Color.gray;
                }
                style = label;

                return style;
            }

            public static Texture2D GetIcon(string path)
            {
                Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
                if (obj != null)
                {
                    Texture2D icon = AssetPreview.GetMiniThumbnail(obj);
                    if (icon == null)
                        icon = AssetPreview.GetMiniTypeThumbnail(obj.GetType());
                    return icon;
                }
                return null;
            }
        }
        public static class FileUtil
        {
            public const string MetaExtension = ".meta";
            public static bool IsMetaFile(string assetPath) => Path.GetExtension(assetPath) == MetaExtension;
            public static bool IsMetaFile(string assetPath, out string assetPathWithMetaExtension)
            {
                assetPathWithMetaExtension = assetPath;
                if (!IsMetaFile(assetPath))
                    return false;
                assetPathWithMetaExtension = assetPath.Replace(MetaExtension, string.Empty);
                return true;
            }

            public static bool IsWithinDirectory(string filePath, string directoryPath)
            {
                string fullFilePath = Path.GetFullPath(filePath);
                string fullDirectoryPath = Path.GetFullPath(directoryPath);

                fullDirectoryPath = Path.GetFullPath(fullDirectoryPath + Path.DirectorySeparatorChar);

                return fullFilePath.StartsWith(fullDirectoryPath, StringComparison.OrdinalIgnoreCase);
            }

            public static string NormalizeSeparator(string path)
            {
                if (string.IsNullOrEmpty(path))
                    return path;
                return path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }


            public const string String_Assets = "Assets";
            public static List<string> GetAllDirectories(string assetPath) => GetAllDirectories(assetPath, String_Assets);
            public static List<string> GetAllDirectories(string assetPath, string rootPath)
            {
                List<string> directories = new List<string>();
                string directoryName = assetPath;

                while (true)
                {
                    //Debug.Log(directoryName);
                    if (string.IsNullOrEmpty(directoryName) || directoryName == rootPath)
                        break;

                    directoryName = Path.GetDirectoryName(directoryName);
                    directoryName = NormalizeSeparator(directoryName);

                    directories.Add(directoryName); 
                }

                return directories;
            }

            public static string GetRelativePath(string fullPath)
            {
                if (string.IsNullOrEmpty(fullPath))
                    return fullPath;

                fullPath = NormalizeSeparator(fullPath);

                int index_Assets = fullPath.IndexOf("Assets/");
                if (index_Assets < 0)
                    return fullPath;

                return fullPath.Substring(index_Assets);
            }

        }
        public static class HyperLinkUtil
        {
            static HyperLinkUtil()
            {
                EditorGUI.hyperLinkClicked -= OnHyperLinkClicked;
                EditorGUI.hyperLinkClicked += OnHyperLinkClicked;
            }


            public const string String_Param_PingAssetPath = "pingAssetPath";
            public const string String_Param_PingGUID = "pingGUID";

            static void OnHyperLinkClicked(EditorWindow window, HyperLinkClickedEventArgs args)
            {
                if (args.hyperLinkData.TryGetValue(String_Param_PingGUID, out string guid))
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object)));
                }
                //直接传在args传assetPath可能会出问题，因为assetPath可能包含空格,@等特殊字符，导致参数解析失败
                //所以加了上面的传GUID
                else if (args.hyperLinkData.TryGetValue(String_Param_PingAssetPath, out string assetPath))
                {
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object)));
                }
            }

            public static void LogAssetLink(Object asset) =>
                LogAssetLink(AssetDatabase.GetAssetPath(asset));

            public static void LogAssetLink(string assetPath) => Debug.Log(GetAssetLink(assetPath));

            public static string GetAssetLink(string assetPath)
            {
                if(string.IsNullOrEmpty(assetPath))
                    return null;
                string guid = AssetDatabase.AssetPathToGUID(assetPath);
                if (string.IsNullOrEmpty(guid))
                    return null;
                return $"<a {String_Param_PingGUID}=\"{guid}\">[{Path.GetFileName(assetPath)}]</a> {assetPath}";
            }
        }
        public static class GUIUtil
        {
            public static void EventCheck(Rect rect, EventType eventType, Action<Event> callback) => EventCheck(rect, eventType, callback, false, false);
            public static void EventCheck(Rect rect, EventType eventType, Action<Event> callback, bool ctrl, bool alt)
            {
                if (ctrl && !Event.current.control)
                    return;
                if (alt && !Event.current.alt)
                    return;
                if (Event.current.type == eventType && rect.Contains(Event.current.mousePosition))
                    callback?.Invoke(Event.current);
            }
            public static bool EventCheck(Rect rect, EventType eventType) => EventCheck(rect, eventType, true, false, false);
            public static bool EventCheck(Rect rect, EventType eventType, bool useEvent) => EventCheck(rect, eventType, useEvent, false, false);
            public static bool EventCheck(Rect rect, EventType eventType, bool useEvent, bool ctrl, bool alt)
            {
                var controlID = GUIUtility.GetControlID(FocusType.Passive);

                if (ctrl && !Event.current.control)
                    return false;
                if (alt && !Event.current.alt)
                    return false;
                if (Event.current.GetTypeForControl(controlID) == eventType && rect.Contains(Event.current.mousePosition))
                {
                    if (useEvent)
                        Event.current.Use();
                    return true;
                }
                return false;
            }

            public const int MOUSE_BUTTON_LEFT = 0;
            public const int MOUSE_BUTTON_RIGHT = 1;
            public const int MOUSE_BUTTON_MIDDLE = 2;

            public static bool LeftClick(Rect rect) => LeftClick(rect, false, false);
            public static bool LeftClick(Rect rect, bool ctrl, bool alt)=> MouseDown(rect, MOUSE_BUTTON_LEFT, ctrl, alt);
            public static bool RightClick(Rect rect) => RightClick(rect, false, false);
            public static bool RightClick(Rect rect, bool ctrl, bool alt)=> MouseDown(rect, MOUSE_BUTTON_RIGHT, ctrl, alt);
            public static bool MouseDown(Rect rect, int button, bool ctrl, bool alt)
            {
                if (ctrl && !Event.current.control)
                    return false;
                if (alt && !Event.current.alt)
                    return false;
                if (Event.current.type == EventType.MouseDown &&
                    Event.current.button == button &&
                    rect.Contains(Event.current.mousePosition)
                   )
                {
                    Event.current.Use();
                    return true;
                }
                return false;
            }

            public static Rect GetIconRect(ProjectItem item)
            {
                return GetRect(item, 16);
            }
            public static Rect GetLabelRect(ProjectItem item, string content, GUIStyle style) =>
                GetLabelRect(item, TempContent.Get(content), style);
            public static Rect GetLabelRect(ProjectItem item, GUIContent content, GUIStyle style)
            {
                var size = style.CalcSize(content);
                return GetRect(item, size.x);
            }
            public static Rect GetRect(ProjectItem item, float width)
            {
                Rect r = item.rect;
                r.xMin = r.xMax - width;
                r.height = 16;

                item.rect.xMax -= width;
                return r;
            }
            public static Rect GetRect(ProjectItem item, float width, float height)
            {
                Rect r = item.rect;
                r.xMin = r.xMax - width;
                r.height = height;

                item.rect.xMax -= width;
                return r;
            }

            public static Rect GetZeroRect(ProjectItem item) => GetRect(item, 0);
            public static Rect GetRectUnion(Rect rect1, Rect rect2)
            {
                float xMin = Mathf.Min(rect1.x, rect2.x);
                float yMin = Mathf.Min(rect1.y, rect2.y);
                float xMax = Mathf.Max(rect1.x + rect1.width, rect2.x + rect2.width);
                float yMax = Mathf.Max(rect1.y + rect1.height, rect2.y + rect2.height);

                return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
            }

        }
    }
}