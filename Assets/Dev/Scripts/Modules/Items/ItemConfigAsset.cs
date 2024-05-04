using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEditor.PackageManager;
using UnityEditor.UIElements;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PJR
{
    public class ItemConfigAsset : EntityConfigAsset
    {
        [LabelText("再生道具次数")]
        [Tooltip("<=0为无限次数，>0对应次数")]
        public int CanRegenerateTimes = 0;
        [LabelText("再生间隔")]
        public float interval = 3f;

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/PJR/创建配置/道具配置/道具配置")]
        public static void CreateAsset()
        {
            CSConfigHelper.CreateScriptableObject<ItemConfigAsset>();
        }
#endif
        protected override void ExecuteActionEvents(List<EActionEvent> events, LogicEntity trapEntity, LogicEntity targetEntity)
        {
            base.ExecuteActionEvents(events, trapEntity, targetEntity);
        }
    }
}

