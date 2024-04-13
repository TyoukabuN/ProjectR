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
    public class EntityPhysicsConfigAsset : SerializedScriptableObject
    {
        [Title("地面移动")]
        [LabelText("地面最大移动速度")] public float MaxGroundedMoveSpeed = 3f;
        [LabelText("地面移动加速度")] public float GroundedMoveACCSpeed = 1f;

        [LabelText("加速时地面最大移动速度")] public float ACCMaxGroundedMoveSpeed = 5f;
        [LabelText("加速时地面移动加速度")] public float ACCGroundedMoveACCSpeed = 3f;

        [Title("空中移动")]
        [LabelText("空中最大移动速度")] public float MaxAirMoveSpeed = 15f;
        [LabelText("空中移动加速度")] public float AirAccelerationSpeed = 15f;

        [Title("跳跃")]
        [LabelText("跳跃上升速度")] public float JumpUpSpeed = 5f;
        [LabelText("可跳跃次数")] public int JumpableTime = 2;

        [Title("其他")]
        [LabelText("重力加速度")] public Vector3 Gravity = new Vector3(0, -10f, 0);
        [LabelText("速度衰减系数")] public float SpeedDamping = 10f;
        [LabelText("转向系数")] public float OrientationSharpness = 10f;
        

#if UNITY_EDITOR
        [MenuItem("Assets/PJR/创建配置/实体物理数值配置")]
        public static void CreateConstConfigAsset()
        {
            CSConfigHelper.CreateScriptableObject<EntityPhysicsConfigAsset>();
        }
#endif
    }
}