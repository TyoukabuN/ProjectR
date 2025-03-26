using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
            
            public OdinEditorWindow Window;
        }

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
