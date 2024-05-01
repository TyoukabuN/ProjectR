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
        [LabelText("�������ߴ���")]
        [Tooltip("<=0Ϊ���޴�����>0��Ӧ����")]
        public int CanRegenerateTimes = 0;
        [LabelText("�������")]
        public float interval = 3f;
        [ScriptTypeRestriction(typeof(ItemBase))]
        public Object script;
        //�����ȴ���
        public int itemType;
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/PJR/��������/��������/��������")]
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

