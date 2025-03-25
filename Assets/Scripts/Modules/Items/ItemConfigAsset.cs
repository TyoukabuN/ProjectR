using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public class ItemConfigAsset : TriggerConfigAsset
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
            PJR.Editor.CSConfigHelper.CreateScriptableObject<ItemConfigAsset>();
        }
#endif
        protected override void ExecuteActionEvents(List<EActionEvent> events, LogicEntity trapEntity, LogicEntity targetEntity)
        {
            base.ExecuteActionEvents(events, trapEntity, targetEntity);
        }
    }
}

