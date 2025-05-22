using System;
using System.Collections.Generic;
using System.IO;
using PJR.Config;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace PJR.Editor
{
    public static class OrdinalConfigClassHelper
    {
        [LabelText("顺序表类创建窗口")]
        public class CreateWindow
        {
            [OnValueChanged("OnConfigNameChanged")] 
            public string ConfigName;
            [DisableIf("@true")]
            [LabelText("配置类Asset类名")]
            public string ConfigAssetName;
            [LabelText("配置类ItemAsset类名")]
            [DisableIf("@true")] 
            public string ConfigItemAssetName;
            // [LabelText("是否同步关联类名")] 
            // [OnValueChanged("OnSyncRelatedClassNameChanged")] 
            // public bool SyncRelatedClassName = true;
            
            protected string _error = string.Empty;

            [OnInspectorGUI]
            protected void OnGUI()
            {
                DrawErrorTips();
            }
            
            public void DrawErrorTips()
            {
                _error = string.Empty;
                if (ExistClass(ConfigName))
                    _error = "已存在相同顺序表类";

                if (!string.IsNullOrEmpty(_error))
                    EditorGUILayout.HelpBox(_error, MessageType.Error);
            }

            void OnConfigNameChanged() => DoSyncRelatedClassName();
            void OnSyncRelatedClassNameChanged() => DoSyncRelatedClassName();
            void DoSyncRelatedClassName()
            {
                // if (!SyncRelatedClassName)
                //     return;
                ConfigAssetName = $"{ConfigName}Asset";
                ConfigItemAssetName = $"{ConfigName}ItemAsset";
            }

            private const string ConfigScriptRoot = "Assets/Scripts/Config";
            
            [Button("Create")]
            public void Confirm()
            {
                var templates = LoadTemplateFile();
                if (templates == null)
                    return;
                if (ExistClass(ConfigName))
                {
                    EditorUtility.DisplayDialog("Tips", $"已存在同名类{GetClassFullName(ConfigName)}","ok");
                    return;
                }
                var scriptDirectory = $"{ConfigScriptRoot}/{ConfigName}";
                if (!Directory.Exists(scriptDirectory))
                    Directory.CreateDirectory(scriptDirectory);

                for (var i = 0; i < templates.Count; i++)
                {
                    var templateFileConfig = templates[i].TemplateFileConfig;
                    var fileName = Path.GetFileName(templateFileConfig.assetPath);
                    fileName = fileName.Replace(TemplateClassName, ConfigName);
                    
                    var classStr = ReplaceTemplate(templates[i].TextAsset.ToString(),ConfigName, templateFileConfig.ignoreIfEditorScope);
                    CreateCS( $"{scriptDirectory}/{fileName}",classStr);
                }
                
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                Window.Close();
            }

            private static string ReplaceTemplate(string templateFileStr, string className, bool ignoreIfEditorScope = false)
            {
                var classStr = templateFileStr.ToString();
                if (!ignoreIfEditorScope)
                {
                    classStr = classStr.TrimStart(TemplateTrimStart.ToCharArray());
                    classStr = classStr.TrimEnd(TemplateTrimEnd.ToCharArray());
                }
                classStr = classStr.Replace(TemplateClassName, className);
                return classStr;
            }
            
            public string GetClassFullName(string className) => $"{GetConfigAssemblyName()}.Config.{className}";
            public string GetConfigAssemblyName() => "PJR";
            public bool ExistClass(string className)
            {
                if (TryGetLoadedAssemblyByName(GetConfigAssemblyName(), out var assembly))
                    return assembly.GetType(GetClassFullName(className)) != null;

                var type = Type.GetType(GetClassFullName(className));
                return type != null;
            }
            bool TryGetLoadedAssemblyByName(string assemblyName,out Assembly res)
            {
                res = null;
                // 获取所有已加载的程序集
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
    
                // 查找匹配的程序集（不包含版本等信息的基本名称）
                foreach (Assembly assembly in assemblies)
                {
                    string name = assembly.GetName().Name;
                    if (name.Equals(assemblyName, StringComparison.OrdinalIgnoreCase))
                    {
                        res = assembly;
                        return true;
                    }
                }
    
                return false;
            }

            public const string TemplateTrimStart = "#if UNITY_EDITOR";
            public const string TemplateTrimEnd = "#endif";
            public const string TemplateClassName = "__OrdinalConfig";
            static void CreateCS(string filePath, string content)
            {
                try
                {
                    File.WriteAllText(filePath, content);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"保存文件时发生错误: {ex.Message}");
                }
            }


            public struct TemplateFileConfig
            {
                public string assetPath;
                public bool ignoreIfEditorScope;
                public bool Valid => !string.IsNullOrEmpty(assetPath);
                public TemplateFileConfig(string assetPath, bool ignoreIfEditorScope = false)
                {
                    this.assetPath = assetPath;
                    this.ignoreIfEditorScope = ignoreIfEditorScope;
                }
            }
            public static readonly TemplateFileConfig[] templateFileConfigs = new[]
            {
                new TemplateFileConfig("Assets/Resources/OrdinalConfigTemplate/__OrdinalConfig.cs"),
                new TemplateFileConfig("Assets/Resources/OrdinalConfigTemplate/__OrdinalConfigAsset.cs"),
                new TemplateFileConfig("Assets/Resources/OrdinalConfigTemplate/__OrdinalConfigItemAsset.cs"),
                new TemplateFileConfig("Assets/Resources/OrdinalConfigTemplate/__OrdinalConfigIDAttribute.cs"),
                new TemplateFileConfig("Assets/Resources/OrdinalConfigTemplate/__OrdinalConfigIDAttributeDrawer.cs",true),
            };

            public class TemplateFileInfo
            {
                public TemplateFileConfig TemplateFileConfig;
                public TextAsset TextAsset;
            }

            private List<TemplateFileInfo> _templates = null;
            public List<TemplateFileInfo> LoadTemplateFile()
            {
                if (_templates == null)
                {
                    _templates = new List<TemplateFileInfo>(6);
                    for (var i = 0; i < templateFileConfigs.Length; i++)
                    {
                        var templateAsset = templateFileConfigs[i];
                        var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(templateAsset.assetPath);
                        if (textAsset == null)
                            throw new Exception($"顺序表配置类模板丢失: {templateAsset.assetPath}");
                        _templates.Add(new TemplateFileInfo()
                        {
                            TemplateFileConfig = templateAsset,
                            TextAsset = textAsset,
                        });
                    }
                }
                return _templates;
            }

    

            [HideInInspector]
            public OdinEditorWindow Window;
        }

        [RequireConfigMenuItem("Tools/创建顺序表类")]
        public static void OrdinalConfigClassDialog()
        {
            var window = new CreateWindow();
            var odinEditorWindow =OdinEditorWindow.InspectObject(window);
            window.Window = odinEditorWindow;
        }

        public abstract class Skeleton
        {
            public abstract Transform GetBoneByName(string name);
        }
        public class AvatarPart
        {
            public void ReSkinned(Skeleton skeleton, SkinnedMeshRenderer ownSkinnedMeshRenderer)
            {
                var temp = new List<Transform>();
                for (var i = 0; i < ownSkinnedMeshRenderer.bones.Length; i++)
                {
                    var originBone = ownSkinnedMeshRenderer.bones[i];
                    var targetBone = skeleton.GetBoneByName(originBone.name);
                    if (targetBone == null)
                        throw new Exception($"found not [bone:{originBone.name} in skeleton]");
                    temp.Add(targetBone);
                }

                ownSkinnedMeshRenderer.bones = temp.ToArray();
            }
        }
    }
}
