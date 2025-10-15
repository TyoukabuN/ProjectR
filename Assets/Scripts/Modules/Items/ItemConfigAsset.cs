using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
using PJR.Editor;
#endif

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
        [MenuItem("Assets/PJR/创建配置/道具配置/道具配置")]
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

