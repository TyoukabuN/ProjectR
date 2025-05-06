using System;
using System.Collections.Generic;
using System.IO;
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
            [DisableIf("@true")]
            public OdinEditorWindow Window;
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
                var scriptDirectory = $"Assets/Scripts/Config/{ConfigName}";
                if (!Directory.Exists(scriptDirectory))
                    Directory.CreateDirectory(scriptDirectory);

                for (var i = 0; i < templates.Count; i++)
                {
                    var fileName = Path.GetFileName(templateAssets[i]);
                    fileName = fileName.Replace(TemplateClassName, ConfigName);
                    
                    var classStr = ReplaceTemplate(templates[i].ToString(),ConfigName);
                    CreateCS( $"{scriptDirectory}/{fileName}",classStr);
                }
                
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                Window.Close();
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

            private static string ReplaceTemplate(string templateFileStr, string className)
            {
                var classStr = templateFileStr.ToString();
                classStr = classStr.TrimStart(TemplateTrimStart.ToCharArray());
                classStr = classStr.TrimEnd(TemplateTrimEnd.ToCharArray());
                classStr = classStr.Replace(TemplateClassName, className);
                return classStr;
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


            public static readonly string[] templateAssets = new[]
            {
                "Assets/Resources/OrdinalConfigTemplate/__OrdinalConfig.cs",
                "Assets/Resources/OrdinalConfigTemplate/__OrdinalConfigAsset.cs",
                "Assets/Resources/OrdinalConfigTemplate/__OrdinalConfigItemAsset.cs",
                "Assets/Resources/OrdinalConfigTemplate/__OrdinalConfigIDAttribute.cs",
                "Assets/Resources/OrdinalConfigTemplate/__OrdinalConfigIDAttributeDrawer.cs",
            };

            private List<TextAsset> _templates = null;
            public List<TextAsset> LoadTemplateFile()
            {
                if (_templates == null)
                {
                    _templates = new List<TextAsset>(6);
                    for (var i = 0; i < templateAssets.Length; i++)
                    {
                        var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(templateAssets[i]);
                        if (textAsset == null)
                            throw new Exception($"顺序表配置类模板丢失: {templateAssets[i]}");
                        _templates.Add(textAsset);
                    }
                }
                return _templates;
            }
            
        }

        public static void OrdinalConfigClassDialog()
        {
            var window = new CreateWindow();
            var odinEditorWindow = OdinEditorWindow.InspectObject(window);
            odinEditorWindow.titleContent = new GUIContent("创建顺序表");
            window.Window = odinEditorWindow;
        }
    }
}
