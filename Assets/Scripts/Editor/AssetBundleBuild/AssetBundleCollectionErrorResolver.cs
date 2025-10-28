using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using YooAsset.Editor;


namespace PJR.Editor
{
    public class AssetBundleCollectionErrorResolver : OdinEditorWindow
    {

        [MenuItem("Debug/AssetBundleCollectionErrorResolver", priority = 400)]
        static void OpenWindow()
        {
            var inst = GetWindow<AssetBundleCollectionErrorResolver>();
        }

        protected override void OnImGUI()
        {
            base.OnImGUI();

            if (GUILayout.Button("Test"))
            {

                var packageNames = GetBuildPackageNames();
                if (packageNames is not { Count: > 0 })
                {
                    Debug.Log("packageNames is not { Count: > 0 }");
                    return;
                }
                var packageName = packageNames[0];
                var buildMode = EBuildMode.SimulateBuild;
                var collectResult = AssetBundleCollectorSettingData.Setting.GetPackageAssets(buildMode, packageName);
            }
        }
        
        private List<string> GetBuildPackageNames()
        {
            List<string> result = new List<string>();
            foreach (var package in AssetBundleCollectorSettingData.Setting.Packages)
            {
                result.Add(package.PackageName);
            }
            return result;
        }
    }
}
