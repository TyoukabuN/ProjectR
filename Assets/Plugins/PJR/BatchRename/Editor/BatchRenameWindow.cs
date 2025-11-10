using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Globalization;
using System.Text.RegularExpressions;
using CharacterType = BatchRename.Util.CharacterType;
using StripFrom = BatchRename.Util.StripFrom;
using ChangeCaseMethod = BatchRename.Util.ChangeCaseMethod;
using AssetInfo = BatchRename.Util.AssetInfo;
using Object = UnityEngine.Object;

namespace BatchRename
{ 
    public class BatchRenameWindow : OdinEditorWindow
    {
        public enum EAssetGainApproach
        {
            [LabelText("选中Assets")]
            SelectedAssets = 0,
            [LabelText("指定Assets")]
            SpecificAssets = 1,
        }
        
        public static BatchRenameWindow instance;

        [LabelText("Asset获取方法"),ShowInInspector]
        public EAssetGainApproach AssetGainApproach = EAssetGainApproach.SelectedAssets;
        [ShowInInspector, HideReferenceObjectPicker, HideLabel]
        public BatchOperationGroup BatchOpGroup;
        
        [LabelText("指定Assets")]
        [ShowInInspector ,AssetsOnly, ShowIf("@AssetGainApproach == EAssetGainApproach.SpecificAssets")]
        public List<Object> SpecificAssets; 
        
        private List<string> _selectedGUIDs = new List<string>();
        private int _activeMainSelected = -1;
        
        
        
        [MenuItem("Assets/BatchRename")]
        public static void Open()
        {
            instance = GetWindow<BatchRenameWindow>("批量重命名");
            instance.OnRegisterEvent(true);
        }

        public static void OpenForGuidExtension(List<string> assetPaths)
        {
            if (assetPaths is not { Count : > 0 })
                return;
            instance = GetWindow<BatchRenameWindow>("批量重命名");
            instance.OnRegisterEvent(true);
            
            instance.BatchOpGroup = new(new BatchOperationWrap(ERenameType.AddGuidExtension));
            instance.AssetGainApproach = EAssetGainApproach.SpecificAssets;
            instance.SpecificAssets = new List<Object>();
            for (var i = 0; i < assetPaths.Count; i++)
            {
                var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPaths[i]);
                if (asset == null)
                {
                    Debug.LogError($"没有找到Asset: {assetPaths[i]}");
                    continue;
                }
                instance.SpecificAssets.Add(asset);
            }
        }

        void OnRegisterEvent(bool enable)
        {
            Selection.selectionChanged -= this.AssetRenameTool_OnSelectionChange;
            if (enable)
                Selection.selectionChanged += this.AssetRenameTool_OnSelectionChange;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            OnRegisterEvent(true);
            BatchOpGroup ??= new BatchOperationGroup();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnRegisterEvent(false);
        }

        private Vector2 _scrollViewPos = Vector2.zero;

        protected override void OnImGUI()
        {
            base.OnImGUI();

            EditorGUILayout.BeginScrollView(_scrollViewPos);
            AssetRenameTool_DrawSelectedAssets(BatchOpGroup.OnDrawSelectObject);
            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("OK"))
            {
                renameOperationGroup = BatchOpGroup.Perform(GetAssetGuids());
            }
            AssetRenameTool_DrawUndo();
            EditorGUILayout.EndHorizontal();
        }

        public void AssetRenameTool_OnSelectionChange()
        {
            _activeMainSelected = 0;
            _selectedGUIDs = _selectedGUIDs ?? new List<string>();
            _selectedGUIDs.Clear();
            foreach (var guids in Selection.assetGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids);
                if (string.IsNullOrEmpty(assetPath))
                    continue;
                _selectedGUIDs.Add(guids);
            }
        }

        private AssetRenameOperationGroup renameOperationGroup;
        public void AssetRenameTool_DrawUndo()
        {
            if (renameOperationGroup != null && renameOperationGroup.CheckValid())
            {
                GUI.color = Color.red;
                if (GUILayout.Button("撤销"))
                {
                    AssetDatabase.StartAssetEditing();
                    try
                    {
                        renameOperationGroup.Undo();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.ToString());
                    }
                    renameOperationGroup = null;
                    AssetDatabase.StopAssetEditing();
                    AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate);
                }
                GUI.color = Color.white;
            }
        }

        public List<string> GetAssetGuids()
        {
            if(AssetGainApproach == EAssetGainApproach.SelectedAssets)
                return _selectedGUIDs;
            if (AssetGainApproach == EAssetGainApproach.SpecificAssets)
            {
                var guids = new List<string>();
                for (var i = 0; i < SpecificAssets.Count; i++)
                {
                    var assetInfo = new AssetInfo(SpecificAssets[i]);
                    guids.Add(assetInfo.guid);
                }

                return guids;
            }
            return null;
        }

        public void AssetRenameTool_DrawSelectedAssets(Action<int, string> OnDrawItem = null)
        {
            if (AssetGainApproach != EAssetGainApproach.SelectedAssets)
                return;
            EditorGUILayout.LabelField($"选中对象数: {_selectedGUIDs?.Count ?? 0}");

            var guids = GetAssetGuids();
            if (guids != null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    for (int i = 0; i < guids.Count; i++)
                    {
                        string guid = guids[i];
                        string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                        if (string.IsNullOrEmpty(assetPath))
                            continue;
                        string fileName = Path.GetFileName(assetPath);
                        EditorGUILayout.BeginHorizontal();
                        {
                            var _name = $"[{fileName}]".PadRight(32);

                            string content = $"{_name} {assetPath}";

                            EditorGUILayout.LabelField(content);

                            OnDrawItem?.Invoke(i, fileName);

                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }
        }
    }
    [Serializable]
    public class BatchOperationGroup
    {
        [ShowInInspector, HideReferenceObjectPicker]
        [LabelText("操作列表"),ListDrawerSettings(CustomAddFunction = "AddWrap",Expanded = true)]
        private List<BatchOperationWrap> _batchOps;
        public BatchOperationGroup()
        {
            _batchOps ??= new List<BatchOperationWrap>();
            if (_batchOps.Count <= 0)
            {
                _batchOps.Add(new BatchOperationWrap(ERenameType.FindAndReplace));
            }
        }
        public BatchOperationGroup(BatchOperationWrap opWrap)
        {
            _batchOps ??= new List<BatchOperationWrap>();
            _batchOps.Clear();
            _batchOps.Add(opWrap);
        }
        public AssetRenameOperationGroup Perform(List<string> guids) 
        {
            AssetDatabase.StartAssetEditing();
            //
            var group = new AssetRenameOperationGroup();
            try
            {
                Util.ForEachGUID(guids, (index, info) =>
                {
                    var replace = info.fileName;
                    for (int i = 0; i < _batchOps.Count; i++)
                    {
                        var temp = _batchOps[i].Perform(info, index);
                        if (string.IsNullOrEmpty(temp))
                            break;
                        replace = temp;
                        info.fileName = replace;
                        info.fileNameWithoutExtension = Path.GetFileNameWithoutExtension(replace);
                    }
                    group.Add(info.guid, replace);
                });
                group.Perform();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
            //
            AssetDatabase.StopAssetEditing();
            return group;
        }
        public void AddWrap()
        {
            _batchOps.Add(new BatchOperationWrap(ERenameType.FindAndReplace));
        }
        public void OnDrawSelectObject(int index, string guid)
        {

        }
    }

    /// <summary>
    /// 为了仿blender中的重名工具的结构
    /// </summary>
    [Serializable]
    public class BatchOperationWrap
    {
        [LabelText("类型")]
        public ERenameType RenameType = ERenameType.FindAndReplace;

        private BatchOperation _op;

        [ShowInInspector,HideLabel,HideReferenceObjectPicker]
        public BatchOperation OP
        {
            get {
                if (_op == null || _op.RenameType != RenameType)
                {
                    if (RenameType == ERenameType.FindAndReplace)
                        _op = new FindAndReplaceOp();
                    else if (RenameType == ERenameType.SetName)
                        _op = new SetNameOp();
                    else if (RenameType == ERenameType.StripCharacters)
                        _op = new StripCharactersOp();
                    else if (RenameType == ERenameType.ChangeCase)
                        _op = new ChangeCaseOp();
                    else if (RenameType == ERenameType.AddGuidExtension)
                        _op = new AddGuidExtensionOp();
                    else
                        throw new InvalidOperationException();
                }    
                return _op;
            }  
            set { _op = value; }
        }
        public BatchOperationWrap(ERenameType renameType)
        {
            RenameType = renameType;
        }
        public AssetRenameOperationGroup Perform(List<string> guids) 
        { 
            return _op?.Perform(guids);
        }
        public string Perform(AssetInfo info) 
        { 
            return _op?.Perform(info);
        }
        public string Perform(AssetInfo info,int index) 
        { 
            return _op?.Perform(info, index);
        }
    }

    [Serializable]
    public abstract class BatchOperation
    {
        public abstract ERenameType RenameType { get; }
        public abstract AssetRenameOperationGroup Perform(List<string> guids);
        public abstract string Perform(AssetInfo info);
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="info"></param>
        /// <param name="index">有些操作需要知道当前需要改名的Asset的索引,如SetName</param>
        /// <returns></returns>
        public virtual string Perform(AssetInfo info, int index) { return Perform(info); }
    }
    public class FindAndReplaceOp : BatchOperation
    {
        public override ERenameType RenameType  =>  ERenameType.FindAndReplace;
        [LabelText("查找")]
        public string Find;
        [LabelText("替换")]
        public string Replace;
        [LabelText("大小写敏感")]
        public bool CaseSensitive;
        [LabelText("用正则表达式查找")]
        public bool UseRegex;

        public override AssetRenameOperationGroup Perform(List<string> guids) 
        {
            return Util.FindAndReplace(guids, Find, UseRegex, Replace, CaseSensitive);
        }
        public override string Perform(AssetInfo info) 
        {
            return Util.FindAndReplace(info, Find, UseRegex, Replace,false, CaseSensitive);
        }
    }
    public class SetNameOp : BatchOperation
    {
        public override ERenameType RenameType  =>  ERenameType.SetName;

        [EnumToggleButtons,LabelText("方法")]
        public ESetNameMethod SetNameMethod = ESetNameMethod.New;
        [LabelText("文本")]
        public string Name;
        [LabelText("忽略已包含"),ShowIf("@SetNameMethod != ESetNameMethod.New")]
        public bool IgnoreContains;
        public override AssetRenameOperationGroup Perform(List<string> guids) 
        {
            if (SetNameMethod == ESetNameMethod.New)
                return Util.NewName(guids, Name);
            else if (SetNameMethod == ESetNameMethod.Prefix)
                return Util.Prefix(guids, Name, IgnoreContains);
            else if (SetNameMethod == ESetNameMethod.Suffix)
                return Util.Suffix(guids, Name, IgnoreContains);
            return null;
        }
        public override string Perform(AssetInfo info)
        {
            if (SetNameMethod == ESetNameMethod.New)
                return Util.NewName(info, Name);
            else if (SetNameMethod == ESetNameMethod.Prefix)
                return Util.Prefix(info, Name, IgnoreContains);
            else if (SetNameMethod == ESetNameMethod.Suffix)
                return Util.Suffix(info, Name, IgnoreContains);
            return null;
        }
        public override string Perform(AssetInfo info ,int index)
        {
            if (SetNameMethod == ESetNameMethod.New)
                return Util.NewName(info, index, Name);
            else
                return Perform(info);
        }
    }
    public class StripCharactersOp : BatchOperation
    {
        public override ERenameType RenameType  =>  ERenameType.StripCharacters;
        [EnumToggleButtons, LabelText("去除字符")]
        public CharacterType CharacterType = CharacterType.Spaces;
        [EnumToggleButtons, LabelText("从哪里开始")]
        public StripFrom StripFrom = StripFrom.Start;
        public override AssetRenameOperationGroup Perform(List<string> guids) {
            return Util.StripCharacters(guids, CharacterType, StripFrom);
        }
        public override string Perform(AssetInfo info) {
            return Util.StripCharacters(info, CharacterType, StripFrom);
        }
    }
    public class ChangeCaseOp : BatchOperation
    {
        public override ERenameType RenameType  =>  ERenameType.ChangeCase;
        [EnumToggleButtons, LabelText("转换成")]
        public ChangeCaseMethod ChangeCaseMethod = ChangeCaseMethod.UpperCase;
        public override AssetRenameOperationGroup Perform(List<string> guids) {
            return Util.ChangeCase(guids, ChangeCaseMethod);
        }
        public override string Perform(AssetInfo info)
        {
            return Util.ChangeCase(info, ChangeCaseMethod, CultureInfo.InvariantCulture);
        }
    }
    
    /// <summary>
    /// 为asset的名字添加特定位数的guid后缀
    /// </summary>
    public class AddGuidExtensionOp : BatchOperation
    {
        public override ERenameType RenameType  =>  ERenameType.AddGuidExtension;
        [LabelText("Guid后缀位数")]
        public int GuidCharacterCount = 8;
        
        public override AssetRenameOperationGroup Perform(List<string> guids) 
        {
            return Util.Suffix(
                guids, 
                info => info.guid.Substring(0, GuidCharacterCount),
                "_",
                true);
        }
        public override string Perform(AssetInfo info)
        {
            if (string.IsNullOrEmpty(info.guid))
                return null;
            return Util.Suffix(
                info, 
                info.guid.Substring(0, GuidCharacterCount), 
                "_",
                true);
        }
    }

    public enum ERenameType
    {
        [LabelText("查找和替换")]
        FindAndReplace = 0,
        [LabelText("设置名字")]
        SetName = 1,
        [LabelText("字符剥离")]
        StripCharacters = 2,
        [LabelText("改变大小写")]
        ChangeCase = 3,
        [LabelText("添加guid后缀")]
        AddGuidExtension = 4,
    }
    public enum ESetNameMethod
    {
        [LabelText("新命名")]
        New = 0,
        [LabelText("设置前缀")]
        Prefix = 1,
        [LabelText("设置后缀")]
        Suffix = 2,
    }



    /// <summary>
    /// 重命名操作
    /// </summary>
    public class AssetRenameOperationGroup
    {
        //后面可能会改成记录多个Group
        public static int s_guid = -1;

        private int _guid = -1;
        private List<AssetRenameOperation> _operations;
        public int guid => s_guid;
        public List<AssetRenameOperation> Operations => _operations;
        public int Count => _operations?.Count ?? 0;

        public Dictionary<string, List<AssetRenameOperation>> _guid2Ops;

        public AssetRenameOperationGroup()
        {
            _guid = ++s_guid;
            _operations = new List<AssetRenameOperation>();
            _guid2Ops = new Dictionary<string, List<AssetRenameOperation>>();
        }
        public AssetRenameOperation Add(string guid, string newAssetName)
        {
            if (string.IsNullOrEmpty(guid))
                return null;
            if (!AnyFileNameModify(guid, newAssetName))
                return null;
            var op = new AssetRenameOperation(guid, newAssetName);
            _operations.Add(op);

            if (!_guid2Ops.TryGetValue(guid, out var ops))
            { 
                ops = new List<AssetRenameOperation>(4);
                _guid2Ops[guid] = ops;
            }
            ops.Add(op);

            return op;
        }
        public string LastAssetName(string guid, string newAssetName)
        {
            if (!_guid2Ops.TryGetValue(guid, out var ops))
                return null;
            if (ops.Count <= 0)
                return null;
            return ops[ops.Count].newAssetName;
        }
        public void AddRange(AssetRenameOperationGroup group)
        {
            if (group == null)
                return;
            _operations.AddRange(group._operations);
        }
        private static bool AnyFileNameModify(string guid, string newAssetName)
        {
            if (string.IsNullOrEmpty(guid))
                return false;
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(assetPath))
                return false;
            if (Path.GetFileName(assetPath) == newAssetName)
                return false;
            return true;
        }
        public bool CheckValid()
        {
            if (Count <= 0)
                return false;
            bool res = true;
            for (int i = 0; i < _operations.Count; i++)
            {
                var op = _operations[i];
                res = res || op.CheckModifiable();
            }
            return res;
        }
        public void Perform()
        {
            for (int i = 0; i < _operations.Count; i++)
            {
                var op = _operations[i];
                op.Perform();
            }
        }
        public void Undo()
        {
            for (int i = 0; i < _operations.Count; i++)
            {
                var op = _operations[i];
                op.Undo();
            }
        }
        public class AssetRenameOperation
        {
            public string guid = string.Empty;
            public string newAssetName = string.Empty;
            //
            public string preAssetPath = string.Empty;
            public string preAssetName = string.Empty;
            //
            private bool _isValid = false;
            private bool _isPerformed = false;
            private string _error = string.Empty;
            private bool _dontCheckExist = false;
            //
            public bool IsValid => _isValid;
            public bool IsPerformed => _isPerformed;
            public string error => _error;
            //
            public string CurrentAssetPath
            {
                get { return AssetDatabase.GUIDToAssetPath(guid); }
            }
            public string TargetAssetPath
            {
                get
                {
                    return Path.Combine(Path.GetDirectoryName(CurrentAssetPath), newAssetName);
                }
            }
            public AssetRenameOperation(string guid, string newAssetName)
            {
                Init(guid, newAssetName);
            }
            private void Init(string guid, string newAssetName)
            {
                this.guid = guid;
                this.newAssetName = newAssetName;
                //
                _isValid = false;
                if (string.IsNullOrEmpty(guid))
                    return;
                if (string.IsNullOrEmpty(newAssetName))
                    return;
                if (string.IsNullOrEmpty(CurrentAssetPath))
                    return;
                //
                preAssetPath = CurrentAssetPath;
                if (string.IsNullOrEmpty(preAssetPath))
                    return;
                _isPerformed = false;
                _isValid = true;
                preAssetName = Path.GetFileName(preAssetPath);
            }
            public AssetRenameOperation SetDontCheckExist(bool enable)
            {
                _dontCheckExist = enable;
                return this;
            }
            public bool CheckModifiable()
            {
                //如果要改的assetPath已经有asset了就不能改
                if (!_dontCheckExist && !string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(TargetAssetPath)))
                {
                    Debug.LogWarning($"已经存在{TargetAssetPath}");
                    return false;
                }
                return true;
            }
            public void Perform()
            {
                if (!_isValid)
                    return;
                string newAssetName = this.newAssetName;
                if (!CheckModifiable())
                {
                    newAssetName = Path.GetFileName(AssetDatabase.GenerateUniqueAssetPath(Path.Combine(Path.GetDirectoryName(CurrentAssetPath), newAssetName)));
                }
                _isPerformed = true;
                _error = AssetDatabase.RenameAsset(preAssetPath, newAssetName);
                if(string.IsNullOrEmpty(_error))
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                LogError();
            }
            public void Undo()
            {
                if (!_isValid)
                    return;
                if (!_isPerformed)
                    return;
                if (!string.IsNullOrEmpty(_error))
                    return;
                _error = AssetDatabase.RenameAsset(CurrentAssetPath, preAssetName);
                LogError();
            }
            public void LogError()
            {
                if (string.IsNullOrEmpty(_error))
                    return;
                Debug.LogError(_error);
            }
        }
    }

    public static class Util
    {
        public struct AssetInfo
        {
            public string guid;
            public string assetPath;
            public string extension;
            public string fileName;
            public string fileNameWithoutExtension;
            public AssetInfo(string guid)
            {
                this.guid = guid;
                assetPath = AssetDatabase.GUIDToAssetPath(guid);
                extension = Path.GetExtension(assetPath);
                fileName = Path.GetFileName(assetPath);
                fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assetPath);
            }
            public AssetInfo(Object obj)
            {
                assetPath = AssetDatabase.GetAssetPath(obj);
                guid = AssetDatabase.AssetPathToGUID(assetPath);
                extension = Path.GetExtension(assetPath);
                fileName = Path.GetFileName(assetPath);
                fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assetPath);
            }
        }
        public static void ForEachGUID(List<string> guids, Action<AssetInfo> action)
        {
            if (action == null)
                return;
            for (int i = 0; i < guids.Count; i++)
                action(new AssetInfo(guids[i]));
        }
        public static void ForEachGUID(List<string> guids, Action<int ,AssetInfo> action)
        {
            if (action == null)
                return;
            for (int i = 0; i < guids.Count; i++)
                action(i, new AssetInfo(guids[i]));
        }

        /// <summary>
        /// 设置名字
        /// </summary>
        /// <param name="guids">目标Asset GUIDs</param>
        /// <param name="newName">新名字</param>
        /// <param name="activeMainSelected">主Asset索引,主Asset不会有后缀</param>
        /// <returns></returns>
        public static AssetRenameOperationGroup NewName(List<string> guids, string newName)
        {
            var renameOperationGroup = new AssetRenameOperationGroup();

            if (guids == null || guids.Count <= 0)
                return null;
            if (string.IsNullOrEmpty(newName))
                return null;
            int count = 0;
            ForEachGUID(guids,(i, info) =>
            {
                renameOperationGroup.Add(info.guid, NewName(info, $"{newName}_{++count}"));
            });
            renameOperationGroup.Perform();

            return renameOperationGroup;
        }
        public static string NewName(AssetInfo info, string newName)
        {
            return $"{newName}{info.extension}";
        }
        public static string NewName(AssetInfo info, int index, string newName)
        {
            if (index <= 0)
                return NewName(info, newName);
            return $"{newName}_{index}{info.extension}";
        }
        /// <summary>
        /// 设置前缀
        /// </summary>
        /// <param name="guids">目标Asset GUIDs</param>
        /// <param name="prefix">前缀文本</param>
        /// <param name="checkContains">检测是否已包含,已包含就不添加了</param>
        /// <returns></returns>
        public static AssetRenameOperationGroup Prefix(List<string> guids, string prefix, bool checkContains)
        {
            var renameOperationGroup = new AssetRenameOperationGroup();

            if (guids == null || guids.Count <= 0)
                return null;
            if (string.IsNullOrEmpty(prefix))
                return null;
            ForEachGUID(guids, info =>
            {
                renameOperationGroup.Add(info.guid, Prefix(info, prefix, checkContains));
            });
            renameOperationGroup.Perform();

            return renameOperationGroup;
        }
        public static string Prefix(AssetInfo info, string prefix, bool checkContains)
        {
            if (checkContains && info.fileNameWithoutExtension.StartsWith(prefix))
                return null;
            return $"{prefix}{info.fileNameWithoutExtension}{info.extension}";
        }

        /// <summary>
        /// 设置后缀
        /// </summary>
        /// <param name="guids">目标Asset GUIDs</param>
        /// <param name="suffix">后缀文本</param>
        /// <param name="checkContains">检测是否已包含,已包含就不添加了</param>
        /// <returns></returns>
        public static AssetRenameOperationGroup Suffix(List<string> guids, string suffix, bool checkContains)
        {
            var renameOperationGroup = new AssetRenameOperationGroup();

            if (guids == null || guids.Count <= 0)
                return null;
            if (string.IsNullOrEmpty(suffix))
                return null;
            ForEachGUID(guids, info =>
            {
                renameOperationGroup.Add(info.guid, Suffix(info, suffix, checkContains));
            });
            renameOperationGroup.Perform();

            return renameOperationGroup;
        }

        public static AssetRenameOperationGroup Suffix(List<string> guids, Func<AssetInfo, string> getSuffix,
            bool checkContains)
            => Suffix(guids, getSuffix, null, checkContains);

        /// <summary>
        /// 设置后缀
        /// </summary>
        /// <param name="guids">目标Asset GUIDs</param>
        /// <param name="getSuffix">后缀文本的获取方法</param>
        /// <param name="paddingChar">后缀的分隔符</param>
        /// <param name="checkContains">检测是否已包含,已包含就不添加了</param>
        /// <returns></returns>
        public static AssetRenameOperationGroup Suffix(List<string> guids, Func<AssetInfo,string> getSuffix, string paddingChar, bool checkContains)
        {
            var renameOperationGroup = new AssetRenameOperationGroup();

            if (guids == null || guids.Count <= 0)
                return null;
            if (getSuffix == null)
                return null;
            ForEachGUID(guids, info =>
            {
                renameOperationGroup.Add(info.guid, Suffix(info, getSuffix.Invoke(info), paddingChar, checkContains));
            });
            renameOperationGroup.Perform();

            return renameOperationGroup;
        }

        public static string Suffix(AssetInfo info, string suffix, bool checkContains)
            => Suffix(info, suffix, null, checkContains);

        public static string Suffix(AssetInfo info, string suffix, string paddingChar, bool checkContains)
        {
            if (checkContains && info.fileNameWithoutExtension.EndsWith(suffix))
                return null;
            if(string.IsNullOrEmpty(paddingChar))
                return $"{info.fileNameWithoutExtension}{suffix}{info.extension}";
            return $"{info.fileNameWithoutExtension}{paddingChar}{suffix}{info.extension}";
        }

        /// <summary>
        /// 查找和替换
        /// </summary>
        /// <param name="guids">目标Asset GUIDs</param>
        /// <param name="findText">查找文本</param>
        /// <param name="findUseRegex">查找的文本使用正则表达式</param>
        /// <param name="replaceText">替换文本</param>
        /// <param name="replaceUseRegex">替换文本使用正则表达式(没实现)</param>
        /// <param name="caseSensitive">大小写敏感</param>
        /// <returns></returns>
        public static AssetRenameOperationGroup FindAndReplace(List<string> guids, string findText, bool findUseRegex, string replaceText, bool caseSensitive)
        {
            return FindAndReplace(guids, findText, findUseRegex, replaceText, false, caseSensitive);
        }
        public static AssetRenameOperationGroup FindAndReplace(List<string> guids, string findText, bool findUseRegex, string replaceText, bool replaceUseRegex, bool caseSensitive)
        {
            var group = new AssetRenameOperationGroup();
            if (string.IsNullOrEmpty(findText))
                return null;
            ForEachGUID(guids, info =>
            {
                group.Add(info.guid, FindAndReplace(info, findText, findUseRegex, replaceText, replaceUseRegex, caseSensitive));
            });
            group.Perform();
            return null;
        }
        public static string FindAndReplace(AssetInfo info, string findText, bool findUseRegex, string replaceText, bool replaceUseRegex, bool caseSensitive)
        {
            if (!findUseRegex)
            {
                return info.fileNameWithoutExtension.Replace(findText, replaceText);
            }
            else
            {
                //var regex = new Regex(findText, caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
                RegexOptions options = RegexOptions.None;
                if (!caseSensitive)
                    options |= RegexOptions.IgnoreCase;

                var matchEvaluator = new MatchEvaluator(match =>
                {
                    return replaceText;
                });
                return Regex.Replace(info.fileNameWithoutExtension, findText, matchEvaluator, options);
                //TODO:replaceUseRegex 不知道blender里面是怎样用这个的
            }
        }

        #region StripCharacters
        public enum CharacterType
        {
            [LabelText("空格")]
            Spaces,
            [LabelText("数字")]
            Digits,
            [LabelText("标点符号")]
            Punctuation
        }
        public enum StripFrom
        {
            [LabelText("开头")]
            Start,
            [LabelText("结尾")]
            End,
            [LabelText("全部")]
            All,
        }
        public const string STRING_SPACE = " ";
        public const char CHAR_SPACE = ' ';
        //
        public const string PATTERN_SPACE = @"\s";
        public const string PATTERN_SPACE_START = @"^\s+";
        public const string PATTERN_SPACE_END = @"\s+$";
        //
        public const string PATTERN_DIGITS = @"\d";
        public const string PATTERN_DIGITS_START = @"^\d+";
        public const string PATTERN_DIGITS_END = @"\d+$";
        //
        public const string PATTERN_PUNCTUATION = @"[\p{P}\p{S}-[._]]";
        public const string PATTERN_PUNCTUATION_START = @"^[\p{P}\p{S}-[._]]+";
        public const string PATTERN_PUNCTUATION_END = @"[\p{P}\p{S}-[._]]+$";
        public static AssetRenameOperationGroup StripCharacters(List<string> guids, CharacterType characterType, StripFrom stripFrom)
        {
            var group = new AssetRenameOperationGroup();
            ForEachGUID(guids, info =>
            {
                group.Add(info.guid, StripCharacters(info, characterType, stripFrom));
            });
            group.Perform();
            return group;
        }
        public static string StripCharacters(AssetInfo info, CharacterType characterType, StripFrom stripFrom)
        {
            string replaced = string.Empty;
            if (characterType == CharacterType.Spaces)
            {
                if (stripFrom == StripFrom.All)
                    replaced = $"{info.fileNameWithoutExtension.Replace(PATTERN_SPACE, string.Empty)}{info.extension}";
                else
                {
                    replaced = Regex.Replace(info.fileNameWithoutExtension, stripFrom == StripFrom.Start ? PATTERN_SPACE_START : PATTERN_SPACE_END, string.Empty);
                    replaced = $"{replaced}{info.extension}";
                }

            }
            else if (characterType == CharacterType.Digits)
            {
                if (stripFrom == StripFrom.All)
                    replaced = $"{info.fileNameWithoutExtension.Replace(STRING_SPACE, string.Empty)}{info.extension}";
                else
                {
                    replaced = Regex.Replace(info.fileNameWithoutExtension, stripFrom == StripFrom.Start ? PATTERN_DIGITS_START : PATTERN_DIGITS_END, string.Empty);
                    replaced = $"{replaced}{info.extension}";
                }
            }
            else if (characterType == CharacterType.Punctuation)
            {
                if (stripFrom == StripFrom.All)
                {
                    replaced = Regex.Replace(info.fileNameWithoutExtension, PATTERN_PUNCTUATION, string.Empty);
                }
                else
                {
                    replaced = Regex.Replace(info.fileNameWithoutExtension, stripFrom == StripFrom.Start ? PATTERN_PUNCTUATION_START : PATTERN_PUNCTUATION_END, string.Empty);
                }
            }
            if (string.IsNullOrEmpty(replaced) || replaced == info.fileNameWithoutExtension)
                return null;
            return replaced;
        }

        #endregion

        /// <summary>
        /// 
        /// 大小写修改方法
        /// </summary>
        public enum ChangeCaseMethod
        {
            [LabelText("大写")]
            UpperCase,
            [LabelText("小写")]
            LowerCase,
            [LabelText("首字母大写")]
            TitleCase
        }
        public const string EN_WORD_PATTERN = @"\b\w";
        public const string EN_WORD_PATTERN_2 = @"[^_@\-\s]+";
        public static AssetRenameOperationGroup ChangeCase(List<string> guids, ChangeCaseMethod changeCaseMethod)
        {
            return ChangeCase(guids, changeCaseMethod, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// 修改大小写
        /// </summary>
        /// <param name="guids">目标Asset GUIDs</param>
        /// <param name="changeCaseMethod">修改方法</param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static AssetRenameOperationGroup ChangeCase(List<string> guids, ChangeCaseMethod changeCaseMethod, CultureInfo cultureInfo)
        {
            var group = new AssetRenameOperationGroup();
            ForEachGUID(guids, info =>
            {
                if (changeCaseMethod == ChangeCaseMethod.UpperCase)
                    group.Add(info.guid, $"{info.fileNameWithoutExtension.ToUpper(cultureInfo)}{info.extension}").SetDontCheckExist(true);
                else if (changeCaseMethod == ChangeCaseMethod.LowerCase)
                    group.Add(info.guid, $"{info.fileNameWithoutExtension.ToLower(cultureInfo)}{info.extension}").SetDontCheckExist(true);
                else if (changeCaseMethod == ChangeCaseMethod.TitleCase)
                {
                    group.Add(info.guid, ChangeCase(info, changeCaseMethod, cultureInfo)).SetDontCheckExist(true);
                }
            });
            group.Perform();
            return group;
        }
        public static string ChangeCase(AssetInfo info, ChangeCaseMethod changeCaseMethod) => ChangeCase(info,changeCaseMethod, CultureInfo.InvariantCulture);
        public static string ChangeCase(AssetInfo info, ChangeCaseMethod changeCaseMethod, CultureInfo cultureInfo)
        {
            var replaced = Regex.Replace(info.fileNameWithoutExtension, EN_WORD_PATTERN_2, new MatchEvaluator(match =>
            {
                //return match.Value.ToUpper(cultureInfo);
                return CapitalizeWord(match);
            }));
            if (string.IsNullOrEmpty(replaced) || replaced == info.fileNameWithoutExtension)
                return null;
            return replaced;
        }
        static string CapitalizeWord(Match match)
        {
            string word = match.Value;
            if (word.Length > 1)
            {
                // 首字母大写，剩下的字母保持不变
                return char.ToUpper(word[0], CultureInfo.InvariantCulture) + word.Substring(1);
            }
            return word.ToUpper(CultureInfo.InvariantCulture);  // 处理单字母单词
        }
    }
}
