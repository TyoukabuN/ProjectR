using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Linq;
using System.IO;
using System;

namespace PJR.Editor
{
    public class FlagManageWindow : OdinMenuEditorWindow
    {
        public static FlagManageWindow instance = null;
        public static FlagManageWindow Instance
        {
            get
            {
                instance = instance ?? GetWindow<FlagManageWindow>();
                return instance;
            }
        }

        [MenuItem("PJR/配置/Tools/Flag/Flag管理窗口")]
        public static void OpenWindow()
        {
            FlagConfig.Editor_RefreshConfig();
            instance = GetWindow<FlagManageWindow>();
            instance.titleContent = new GUIContent("Flag管理窗口");
        }

        [MenuItem("PJR/配置/Tools/Flag/刷新Flag配置")]
        public static void Editor_RefreshConfig()
        {
            FlagConfig.Editor_RefreshConfig();
        }

        [MenuItem("PJR/配置/Tools/Flag/生成Flag静态索引(程序用)")]
        public static void Editor_ConvertFlagDefineSetToStaticClass()
        {
            string scriptAssetPath = Helper.Editor_ConvertFlagDefineSetToStaticClass_GetAssetPath();
            if (UnityEditor.EditorUtility.DisplayDialog("提示", $"生成Flag索引,可以让程序在代码里只用引用到策划配置的flag\n{scriptAssetPath}", "ok"))
            {
                Helper.Editor_ConvertFlagDefineSetToStaticClass();
            }
        }

        [MenuItem("PJR/配置/Tools/Flag/上传Flag配置文件(TortoiseGit)")]
        public static void Editor_CommitFlagConfigAsset()
        {
            TortoiseGitUtil.SVNCommit(FlagConfig.FlagConfigFileRoot);
        }

        [MenuItem("PJR/配置/Tools/Flag/文档")]
        public static void Editor_Doc()
        {
            Application.OpenURL("https://docs.qq.com/doc/DY0xvb1R2TkJpVFpx");
        }

        /// <summary>
        /// 加号图标
        /// </summary>
        public static Texture AddedRemote
        {
            get
            {
                return EditorGUIUtility.IconContent("d_P4_AddedRemote").image;
            }
        }

        private OdinMenuTree tree = null;

        protected override OdinMenuTree BuildMenuTree()
        {
            tree = new OdinMenuTree();
            //tree.Config.SelectFirstWhenBuildTree = false;
            tree.Selection.SelectionChanged += OnItemSelectionChange;

            tree.Add("添加大类", new AddCategoryMenuItem(), AddedRemote);

            for (int i = 0; i < FlagConfig.Editor_SortedDefineSet.Count; i++)
            {
                var defineSet = FlagConfig.Editor_SortedDefineSet[i];
                var info = FlagIDInfo.GetInfo(defineSet.ID);
                if (info == null)
                    continue;
                if (!info.IsValid())
                    continue;
                var flagDefines = FlagConfig.GetFlagDefinesByCategory(info.categoryID);
                if (flagDefines == null || flagDefines.Count <= 0)
                    continue;

                var categoryFlag = FlagConfig.GetFlagDefine(info.categoryID);
                var categorySet = FlagConfig.GetFlagDefineSet(info.categoryID);

                if (FlagConfig.IsCategoryID(defineSet.ID))
                {
                    tree.Add(defineSet.Name, defineSet);
                    tree.Add($"{defineSet.Name}/添加Flag至[{defineSet.Name}]", new AddTypeMenuItem(categorySet, defineSet), AddedRemote);
                }


                for (int j = 0; j < flagDefines.Count; j++)
                {
                    var flagDefine = flagDefines[j];
                    if (flagDefine == null)
                        continue;
                    if (FlagConfig.IsCategoryID(flagDefine.ID))
                        continue;
                    tree.Add(FlagConfig.Editor_GetMenuName(flagDefine.ID), flagDefine);
                }
                if (!FlagConfig.IsCategoryID(defineSet.ID) && categorySet != null)
                {
                    tree.Add($"{categorySet.Name}/{defineSet.Name}", defineSet);
                    tree.Add($"{categorySet.Name}/{defineSet.Name}/添加Flag至[{defineSet.Name}]", new AddTypeMenuItem(categorySet, defineSet), AddedRemote);
                }
                if (FlagConfig.IsCategoryID(defineSet.ID))
                {
                    tree.Add($"{defineSet.Name}/添加小类至[{defineSet.Name}]", new AddKindMenuItem(defineSet), AddedRemote);
                }
            }

            tree.EnumerateTree();
            return tree;
        }

        public void OnItemSelectionChange(SelectionChangedType selectionChangedType)
        {
            //Debug.Log(selectionChangedType);
            if (selectionChangedType == SelectionChangedType.ItemAdded)
            {
                if (tree.Selection.SelectedValue == null)
                    return;
                //FlagDefine item = (FlagDefine)tree.Selection.SelectedValue;
                //bool selected = false;
            }
        }

        protected override void OnImGUI()
        {
            if (refreshNextFrame)
            {
                RefreshConfig();
                refreshNextFrame = false;
                return;
            }

            base.OnImGUI();

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("保存配置"))
            {
                FlagConfig.Editor_SetDirtyAndSaveAllConfig();
            }
            if (GUILayout.Button("刷新配置"))
            {
                FlagConfig.Editor_RefreshConfig();
                ForceMenuTreeRebuild();
            }
            if (GUILayout.Button("生成Flag静态索引(程序用)"))
            {
                Editor_ConvertFlagDefineSetToStaticClass();
            }
            if (GUILayout.Button("测试选择菜单"))
            {
                FlagConfig.Editor_ShowFlagGenericMenu(flagDefine => { });
            }
            if (GUILayout.Button("上传Flag配置(TortoiseGit)"))
            {
                Editor_CommitFlagConfigAsset();
            }
            if (GUILayout.Button("文档"))
            {
                Editor_Doc();
            }
            EditorGUILayout.EndHorizontal();
        }

        public static void RefreshConfig()
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            FlagConfig.Editor_RefreshConfig();
            if (Instance != null)
            {
                Instance.ForceMenuTreeRebuild();
            }
        }

        public static void ShowNotification(string context)
        {
            if (Instance != null)
            {
                Instance.ShowNotification(new GUIContent(context));
            }
        }

        private bool refreshNextFrame = false;
        public static void MarkRefreshNextFrame()
        {
            if (Instance != null)
            {
                Instance.refreshNextFrame = true;
            }
        }

        /// <summary>
        /// 添加大类
        /// </summary>
        class AddCategoryMenuItem
        {
            [LabelText("ID"), DisableIf("@true")]
            public int id = -1;
            [LabelText("名字(菜单选项)")]
            public string nameStr = "比如:角色动作";
            [LabelText("Key(脚本生成用)")]
            public string Key = "填全英文比如: EnemyAction";
            [LabelText("文件名")]
            public string fileName = string.Empty;
            [HideIf("@true")]
            public string error = string.Empty;

            public AddCategoryMenuItem()
            {
                id = FlagConfig.Editor_GetMaximumCategoryID() + FlagConfig.categoryInterval;
                string prefix = $"Flag_{{类型}}_{{Key}}_{{ID}} 比如：Flag_Input_EmemyAction";
                fileName = $"{prefix}_{id}";
            }

            [OnInspectorGUI]
            public void OnGUI()
            {
                if (GUILayout.Button("创建"))
                {
                    if (!string.IsNullOrEmpty(error))
                    {
                        ShowNotification(error);
                    }
                    else
                    {
                        string assetPath = $"{FlagConfig.FlagConfigFileRoot}/{fileName}.asset";
                        var asset = ScriptableObject.CreateInstance<FlagDefineSet>();
                        asset.id = id;
                        asset.nameStr = nameStr;
                        asset.FlagDefines = new List<FlagDefine>
                {
                    //Category Flag 大类Flag
                    new FlagDefine
                    {
                        id = asset.id,
                        Key = asset.Key,
                        nameStr = $"[CategoryFlag]{nameStr}",
                    }
                };

                        try
                        {
                            var uniqueFileName = AssetDatabase.GenerateUniqueAssetPath(assetPath);
                            AssetDatabase.CreateAsset(asset, uniqueFileName);

                            UnityEditor.EditorUtility.SetDirty(asset);
                            AssetDatabase.SaveAssets();

                            ShowNotification($"成功创建大类{nameStr}");
                            RefreshConfig();
                        }
                        catch (Exception e)
                        { 
                            Debug.LogError(e);
                        }
                    }
                }

                DrawErrorTips();
            }

            public void DrawErrorTips()
            {
                error = string.Empty;
                if (string.IsNullOrEmpty(nameStr))
                    error = "名字不能为空";
                else if (string.IsNullOrEmpty(Key))
                    error = "Key不能为空";
                else if (string.IsNullOrEmpty(fileName))
                    error = "文件名不能为空";

                if (string.IsNullOrEmpty(error))
                {
                    var exist = FlagConfig.GetFlagDefineByKey(Key);
                    if (exist != null)
                        error = $"已存Key: {Key}  [{exist.ID}][{FlagConfig.Editor_GetMenuName(exist.ID)}]";
                }

                if (!string.IsNullOrEmpty(error))
                    EditorGUILayout.HelpBox(error, MessageType.Warning);
            }
        }

        /// <summary>
        /// 添加小类
        /// </summary>
        class AddKindMenuItem
        {
            [LabelText("ID"), DisableIf("@true")]
            public int id = -1;
            [LabelText("名字(菜单选项)")]
            public string nameStr = "例如:林猪王";
            [LabelText("Key(脚本生成用)")]
            public string Key = "不用填";
            [LabelText("文件名")]
            public string fileName = string.Empty;
            [HideIf("@true")]
            public string error = string.Empty;

            [DisableIf("@true")]
            public FlagDefineSet flagDefineSet = null;
            public AddKindMenuItem(FlagDefineSet flagDefineSet)
            {
                this.flagDefineSet = flagDefineSet;
                var info = FlagIDInfo.GetInfo(flagDefineSet.GetMaximumID());
                if (!info.IsValid())
                {
                    MarkRefreshNextFrame();
                    return;
                }
                id = FlagConfig.Editor_GetMaximumKindID(info.categoryID) + FlagConfig.kindInterval * (info.kind + 1);
                //nameStr = $"{flagDefineSet.Name}/";
                string prefix = flagDefineSet.name.Replace(info.categoryID.ToString(), string.Empty).TrimEnd('_');
                fileName = $"{prefix}_{id}_填个名字";
            }

            [OnInspectorGUI]
            public void OnGUI()
            {
                if (GUILayout.Button("创建"))
                {
                    if (!string.IsNullOrEmpty(error))
                    {
                        ShowNotification(error);
                    }
                    else
                    {
                        string assetPath = $"{FlagConfig.FlagConfigFileRoot}/{fileName}.asset";
                        var asset = ScriptableObject.CreateInstance<FlagDefineSet>();
                        asset.id = id;
                        asset.nameStr = nameStr;
                        asset.FlagDefines = new List<FlagDefine>();
                        var uniqueFileName = AssetDatabase.GenerateUniqueAssetPath(assetPath);

                        AssetDatabase.CreateAsset(asset, uniqueFileName);
                        AssetDatabase.SaveAssets();

                        ShowNotification($"成功创建小类{nameStr}");
                        RefreshConfig();
                    }
                }

                DrawErrorTips();
            }

            public void DrawErrorTips()
            {
                error = string.Empty;
                if (string.IsNullOrEmpty(nameStr))
                    error = "名字不能为空";
                else if (string.IsNullOrEmpty(fileName))
                    error = "文件名不能为空";
                else if (id <= 0)
                    error = "配置发生变动请刷新配置";

                if (!string.IsNullOrEmpty(error))
                    EditorGUILayout.HelpBox(error, MessageType.Warning);
            }
        }

        /// <summary>
        /// 添加Flag
        /// </summary>
        class AddTypeMenuItem
        {
            [LabelText("ID"), DisableIf("@true")]
            public int id = -1;
            [LabelText("名字(菜单选项)")]
            public string nameStr = null;
            [LabelText("Key(脚本生成用)")]
            public string Key = "英文开头如食人魔大风车: Ogre_windMill_State";
            [HideIf("@true")]
            public string error = string.Empty;

            [DisableIf("@true")]
            public FlagDefineSet categoryFlagDefineSet = null;
            [DisableIf("@true")]
            public FlagDefineSet flagDefineSet = null;

            public AddTypeMenuItem(FlagDefineSet categoryFlagDefineSet, FlagDefineSet flagDefineSet)
            {
                this.categoryFlagDefineSet = categoryFlagDefineSet;
                this.flagDefineSet = flagDefineSet;
                id = flagDefineSet.GetMaximumID() + 1;
                nameStr = $"{flagDefineSet.Name}/";
            }

            [OnInspectorGUI]
            public void OnGUI()
            {
                if (GUILayout.Button("创建"))
                {
                    if (flagDefineSet.FlagDefines.Any(flagDef => flagDef.id == id))
                    {
                        Debug.LogWarning($"{flagDefineSet.Name}已存在id: {id}");
                    }
                    else if (!string.IsNullOrEmpty(error))
                    {
                        ShowNotification(error);
                    }
                    else
                    {
                        flagDefineSet.FlagDefines.Add(new FlagDefine()
                        {
                            id = id,
                            nameStr = nameStr,
                            Key = Key,
                        });
                        UnityEditor.EditorUtility.SetDirty(flagDefineSet);
                        AssetDatabase.SaveAssets();

                        ShowNotification($"成功创建类型{nameStr}");
                        RefreshConfig();
                    }
                }

                DrawErrorTips();
            }

            public void DrawErrorTips()
            {
                error = string.Empty;
                if (string.IsNullOrEmpty(nameStr))
                    error = "名字不能为空";
                else if (string.IsNullOrEmpty(Key))
                    error = "Key不能为空";

                if (string.IsNullOrEmpty(error))
                {
                    var exist = FlagConfig.GetFlagDefineByKey(Key);
                    if (exist != null)
                        error = $"已存Key: {Key}  [{exist.ID}][{FlagConfig.Editor_GetMenuName(exist.ID)}]";
                }

                if (!string.IsNullOrEmpty(error))
                    EditorGUILayout.HelpBox(error, MessageType.Warning);
            }
        }

        public static class Helper
        {

            public const string bitwiseGenScriptRoot = "Assets/Plugins/BitwiseFlags/Gen";
            public const string className = "FlagHandleReference";
            private const string Extension = ".cs";
            private const string FlagConfigRoot = "Assets/Dev/Prefabs/ConfigAssets/Flag";


            //[MenuItem("Assets/Flag/Convert/将FlagDefineSet转换成代码可以引用的Static字段")]
            public static void Editor_ConvertFlagDefineSetToStaticClass()
            {
                if (FlagConfig.Editor_SortedCategoryDefines == null)
                    return;

                string filePath = Path.Combine(bitwiseGenScriptRoot, $"{className}.Gen{Extension}");

                var builder = new CSharpScriptBuilder(filePath);
                builder.AppendLine($"//FlagManageWindow.Helper.Editor_ConvertFlagDefineSetToStaticClass根据{FlagConfigRoot}下配置生成");
                builder.AppendLine($"//为了在代码里可以直接引用到对应的Flag");

                using (builder.BeginClass(className))
                {
                    var sortedCategoryDefines = FlagConfig.Editor_SortedCategoryDefines;
                    for (int i = 0; i < sortedCategoryDefines.Count; i++)
                    {
                        var categoryDefine = sortedCategoryDefines[i];
                        var subClassName = categoryDefine.Key;
                        Debug.Log(categoryDefine);

                        var flagDefines = FlagConfig.GetFlagDefinesByCategory(categoryDefine.id);
                        flagDefines.Sort((a, b) => a.ID.CompareTo(b.ID));

                        using (builder.BeginClass($"{categoryDefine.Key}", "public static"))
                        {
                            for (int j = 0; j < flagDefines.Count; j++)
                            {
                                var define = flagDefines[j];
                                string fieldName = define.Key;

                                //没有填Key
                                if (string.IsNullOrEmpty(fieldName))
                                    continue;

                                if (FlagConfig.IsCategoryID(define.id))
                                {
                                    fieldName = "Category";
                                }
                                else if (fieldName.Contains(subClassName))
                                {
                                    fieldName = fieldName.Replace(subClassName, string.Empty);
                                    fieldName = fieldName.TrimStart('.');
                                }

                                builder.AppendLine($"public static FlagHandle512 {fieldName} = new FlagHandle512({define.id});");
                            }
                        }
                    }
                }

                builder.Gen();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }

            public static void Editor_ConvertFlagDefineSetToStaticClass_PingScriptAsset()
            {
                string assetPath = Path.Combine(bitwiseGenScriptRoot, $"{className}.Gen{Extension}");
                var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                if (asset == null)
                    return;
                EditorGUIUtility.PingObject(asset);
            }
            public static string Editor_ConvertFlagDefineSetToStaticClass_GetAssetPath()
            {
                return Path.Combine(bitwiseGenScriptRoot, $"{className}.Gen{Extension}");
            }
        }
    }
}