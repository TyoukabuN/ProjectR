using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.Profiling;
using System;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Unity.EditorCoroutines.Editor;
using UnityEngine.SceneManagement;
//

public partial class AssetAnalysisTool : AssetAnalysisBase
{
    public enum ERenameType
    {
        FineAndReplace = 0,
        SetName = 1,
        StripCharacters = 2,
        ChangeCase = 3,
    }
    public enum ERenameTypeCN
    {
        查找和替换 = 0,
        设置名字 = 1,
        字符剥离 = 2,
        改变大小写 = 3,
    }
    public enum ESetNameMethod
    {
        New = 0,
        Prefix = 1,
        Suffix = 2,
    }
    public enum ESetNameMethodCN
    {
        新命名 = 0,
        设置前缀 = 1,
        设置后缀 = 2,
    }
    public static AnimBoolHandle animBool_resource_rename;
    //
    public static IntHandle RenameType;
    public static IntHandle RenameMethodType;
    public static StringHandle RenameName;
    public static BoolHandle Rename_ShowReferedCount;

    public static List<string> _selectedGUIDs = new List<string>();
    public static int activeMainSelected = -1;

    public static AssetRenameOperationGroup renameOperationGroup;

    public void AssetRenameTool_OnEnable()
    {
        animBool_resource_rename = new AnimBoolHandle(this.GetType().Name + "animBool_resource_rename", false);
        animBool_resource_rename.valueChanged.AddListener(Repaint);
    }
    /// <summary>
    /// 资源重命名工具
    /// </summary>
    public void AssetRenameTool()
    {
        if (RenameType == null) RenameType = new IntHandle("AssetRenameTool_RenameType", (int)ERenameType.SetName);
        if (RenameMethodType == null) RenameMethodType = new IntHandle("AssetRenameTool_RenameMethodType", (int)ESetNameMethod.New);
        if (RenameName == null) RenameName = new StringHandle("AssetRenameTool_RenameName", string.Empty);
        if (Rename_ShowReferedCount == null) Rename_ShowReferedCount = new BoolHandle("AssetRenameTool_Rename_AssetOption_ShowReferedCount", true);
        //
        animBool_resource_rename.target = Foldout(animBool_resource_rename.target, "资源重命名工具");
        if (EditorGUILayout.BeginFadeGroup(animBool_resource_rename.faded))
        {
            RenameType.value = (int)Enum.ToObject(typeof(ERenameType), EditorGUILayout.EnumPopup("Type", (ERenameTypeCN)RenameType.value));
            //画不同的重命名方法
            if (((ERenameType)RenameType.value) == ERenameType.SetName)
            {
                AssetRenameTool_SetName();
            }
            else
            {
                EditorGUILayout.HelpBox("这个类型还没有弄", MessageType.Warning);
            }
        }
        EditorGUILayout.EndFadeGroup();
    }


    /// <summary>
    /// Type=设置名字
    /// </summary>
    public void AssetRenameTool_SetName()
    {
        RenameMethodType.value = (int)Enum.ToObject(typeof(ESetNameMethod), EditorGUILayout.EnumPopup("Method", (ESetNameMethodCN)RenameMethodType.value));
        RenameName.DrawDefaultGUI("Name");
        Rename_ShowReferedCount.DrawDefaultGUI("显示资源被引用数");
        if (_selectedGUIDs != null)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                for (int i = 0; i < _selectedGUIDs.Count; i++)
                {
                    string guid = _selectedGUIDs[i];
                    string assetPath = AssetDatabase.GUIDToAssetPath(_selectedGUIDs[i]);
                    if (string.IsNullOrEmpty(assetPath))
                        continue;
                    string fileName = Path.GetFileName(assetPath);
                    EditorGUILayout.BeginHorizontal();
                    {
                        var _name = $"[{fileName}]".PadRight(32);


                        string content = $"{_name} {assetPath}";

                        //显示引用数
                        if (Rename_ShowReferedCount.value)
                        {
                            int refCount = 0;
                            if (GetRefFinderData().GetAssetState(guid, out var assetDesc))
                                refCount = assetDesc.references.Count;
                            string refCountStr = $"[被引用数]:{refCount}";
                            content = $"{content}   {refCountStr}";
                        }
                        EditorGUILayout.LabelField(content);

                        AssetRenameTool_UseNameButton(fileName);
                        AssetRenameTool_AsMainButton(i);

                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }
        if (GUILayout.Button("OK"))
        {
            AssetDatabase.StartAssetEditing();
            try
            {
                if (RenameMethodType.ToEnum<ESetNameMethod>() == ESetNameMethod.New)
                    AssetRenameTool_RenameAsset_Method_New();
                else if (RenameMethodType.ToEnum<ESetNameMethod>() == ESetNameMethod.Prefix)
                    AssetRenameTool_RenameAsset_Method_Prefix();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
            AssetDatabase.StopAssetEditing();
        }
        if (renameOperationGroup != null && GUILayout.Button("撤销"))
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

        }
    }

    public void AssetRenameTool_GetReferedCountContent(string guid)
    { 

    }

    /// <summary>
    /// 使用选中文件名字的名字
    /// </summary>
    /// <param name="fileName"></param>
    public void AssetRenameTool_UseNameButton(string fileName)
    {
        if (GUILayout.Button("UseName", GUILayout.Width(72f)))
        { 
            RenameName.value = Path.GetFileNameWithoutExtension(fileName);
        }
    }

    /// <summary>
    /// 画设置MainAsset的按钮
    /// MainAsset是重命名files的主文件，主文件不重命名，其他文件按顺序加后缀
    /// </summary>
    /// <param name="selectedItemIndex"></param>
    public void AssetRenameTool_AsMainButton(int selectedItemIndex)
    {
        if (selectedItemIndex == activeMainSelected)
        {
            GUI.color = Color.yellow;
            EditorGUILayout.LabelField("MainAsset", GUILayout.Width(60f));
            GUI.color = Color.white;
        }
        else if (GUILayout.Button("As Main", GUILayout.Width(60f)))
        {
            activeMainSelected = selectedItemIndex;
        }
    }

    /// <summary>
    /// 新命名的ok按钮
    /// </summary>
    public void AssetRenameTool_RenameAsset_Method_New()
    {
        renameOperationGroup = new AssetRenameOperationGroup();

        if (_selectedGUIDs == null || _selectedGUIDs.Count <= 0)
            return;
        if (string.IsNullOrEmpty(RenameName.value))
            return;
        if (activeMainSelected < 0)
            return;
        int count = 0;
        for (int i = 0; i < _selectedGUIDs.Count; i++)
        {
            string guid = _selectedGUIDs[i];
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var ext = Path.GetExtension(assetPath);
            if (i == activeMainSelected)
            {
                renameOperationGroup.AddRenameOp(guid, $"{RenameName.value}{ext}");
            }
            else
            {
                renameOperationGroup.AddRenameOp(guid, $"{RenameName.value}_{++count}{ext}");
            }
        }
        renameOperationGroup.Perform();
    }

    /// <summary>
    /// 前缀的ok按钮
    /// </summary>
    public void AssetRenameTool_RenameAsset_Method_Prefix()
    {
        renameOperationGroup = new AssetRenameOperationGroup();

        if (_selectedGUIDs == null || _selectedGUIDs.Count <= 0)
            return;
        if (string.IsNullOrEmpty(RenameName.value))
            return;
        for (int i = 0; i < _selectedGUIDs.Count; i++)
        {
            string guid = _selectedGUIDs[i];
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var ext = Path.GetExtension(assetPath);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assetPath);
            renameOperationGroup.AddRenameOp(guid, $"{RenameName.value}{fileNameWithoutExtension}{ext}");
        }
        renameOperationGroup.Perform();
    }

    public void AssetRenameTool_OnRegisterEvent()
    {
        Selection.selectionChanged -= this.AssetRenameTool_OnSelectionChange;
        Selection.selectionChanged += this.AssetRenameTool_OnSelectionChange;
    }

    public void AssetRenameTool_OnSelectionChange()
    {
        activeMainSelected = 0;
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

        public AssetRenameOperationGroup()
        {
            _guid = ++s_guid;
            _operations = new List<AssetRenameOperation>();
        }
        public void AddRenameOp(string guid, string newAssetName)
        {
            var op = new AssetRenameOperation(guid, newAssetName);
            _operations.Add(op);
        }
        public bool CheckValid()
        {
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
            public bool CheckModifiable()
            {
                //如果要改的assetPath已经有asset了就不能改
                if (!string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(TargetAssetPath)))
                {
                    Debug.LogWarning($"已经存在{TargetAssetPath}");
                    return false;
                }
                return true;
            }
            public void Perform(bool checkModifiable = true)
            {
                if (!_isValid)
                    return;
                if (!CheckModifiable())
                    return;
                _isPerformed = true;
                _error = AssetDatabase.RenameAsset(preAssetPath, newAssetName);
                LogError();
            }
            public void Undo()
            {
                if (!_isValid)
                    return;
                if (!_isPerformed)
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

}
