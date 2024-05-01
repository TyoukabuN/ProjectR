using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PJR
{
    public class ItemConfig : EntityConfigAsset
    {
        [LabelText("再生道具次数")]
        [Tooltip("<=0为无限次数，>0对应次数")]
        public int CanRegenerateTimes = 0;
        [LabelText("再生间隔")]
        public float interval = 3f;
        [ScriptTypeRestriction(typeof(ItemBase))]
        public Object script;
        //数字先代替
        public int itemType;
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/PJR/创建配置/道具配置/道具配置")]
        public static void CreateAsset()
        {
            CSConfigHelper.CreateScriptableObject<ItemConfig>();
        }
#endif
        
        public void ExecutePhaseEvent(TEntityPhase entityPhase, ItemEntity itemEntity, LogicEntity targetEntity)
        {

            ItemFunc.ExcuteEntrance(itemEntity.config.itemType, itemEntity, targetEntity);
        }
    }
}

