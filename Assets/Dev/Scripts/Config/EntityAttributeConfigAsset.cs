using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PJR;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR
{
    public class EntityAttributeConfigAsset : ScriptableObject
    {

        [Title("攻击")]
        [LabelText("角色攻击伤害")] public float PlayerAtk = 0;
        [LabelText("角色攻击之间的间隔")] public float PlayerAtkInterval = 0;

        [Title("受身")]
        [LabelText("硬直时间")] public float HitStunTime = 0;
        [LabelText("击退时间")] public float RepelTime = 0;
        [LabelText("被击飞时间")] public float BlowUpTime = 0;
        [LabelText("起身无敌时间")] public float GetUpImbaTime = 0;
        [LabelText("无敌时间")] public float ImbaTime = 0;


#if UNITY_EDITOR
        [MenuItem("Assets/PJR/创建配置/实体属性配置")]
        public static void CreateConstConfigAsset()
        {
            CSConfigHelper.CreateScriptableObject<EntityAttributeConfigAsset>();
        }
#endif
    }
}
