using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
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

            [LabelText("是否同步关联类名")] 
            [OnValueChanged("OnSyncRelatedClassNameChanged")] 
            public bool SyncRelatedClassName = true;

            [Button("Create")]
            public void Confirm()
            {
            }
            void OnConfigNameChanged() => DoSyncRelatedClassName();
            void OnSyncRelatedClassNameChanged() => DoSyncRelatedClassName();
            void DoSyncRelatedClassName()
            {
                if (!SyncRelatedClassName)
                    return;
                ConfigAssetName = $"{ConfigName}Asset";
                ConfigItemAssetName = $"{ConfigName}ItemAsset";
            }
        }

        public static void OrdinalConfigClassDialog()
        {
            var window = new CreateWindow();
            OdinEditorWindow.InspectObject(window);
        }
    }
}
