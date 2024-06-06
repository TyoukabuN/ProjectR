using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;

namespace PJR
{
    /// <summary>
    /// 资源描述
    /// </summary>
    public class ResourceDescription
    {
        /// <summary>
        /// 资源地址 [0]:一般,[1]备用
        /// </summary>
        [LabelText("资源地址 [0]一般,[1]备用")]
        public string[] RemoteUrls = { };

        /// <summary>
        /// 渠道
        /// </summary>
        [LabelText("渠道")]
        public string Channel = string.Empty;

        /// <summary>
        /// 资源目标平台<para/>
        /// StandaloneWindows64/Android/iOS 等等
        /// </summary>
        [HorizontalGroup("资源目标平台")]
        [LabelText("资源目标平台")]
        public string Platform = string.Empty;

        /// <summary>
        /// 检查remote server,并更新
        /// </summary>
        [LabelText("需要更新")]
        public bool RequireUpdate = false;

        public ResourceDescription()
        {
#if UNITY_EDITOR
            Platform = EditorUserBuildSettings.activeBuildTarget.ToString();
#endif
        }

#if UNITY_EDITOR
        [HorizontalGroup("资源目标平台", Width = 60)]
        [Button("使用当前")]
        private void Editor_UseSelectPlatform()
        {
            Platform = EditorUserBuildSettings.activeBuildTarget.ToString();
        }

        [HorizontalGroup("资源目标平台", Width = 60)]
        [Button("选择")]
        private void Editor_SelectPlatform()
        {
            GenericMenu menu = new GenericMenu();
            foreach (BuildTarget platform in Enum.GetValues(typeof(BuildTarget)))
            {
                var str = platform.ToString();
                menu.AddItem(new GUIContent(str), str == Platform, () => Platform = str);
            }
            menu.ShowAsContext();
        }

        public static void CreateDesc()
        {
            string filePath = PathUtility.GetDescFilePath();
            if (File.Exists(filePath))
                return;
            string json = JsonUtility.ToJson(new ResourceDescription(), true);
            File.WriteAllText(filePath, json);
        }
#endif
    }
}