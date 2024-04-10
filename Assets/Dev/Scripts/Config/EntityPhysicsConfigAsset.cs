using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "配置/角色物理配置", fileName = "EntityPhysicsConfig")]
public class EntityPhysicsConfigAsset : SerializedScriptableObject
{
    [Title("地面移动")]
    [LabelText("地面最大移动速度")] public float MaxGroundedMoveSpeed = 3f;
    [LabelText("地面移动加速度")] public float GroundedMoveACCSpeed = 1f;

    [LabelText("加速时地面最大移动速度")] public float ACCMaxGroundedMoveSpeed = 3f;
    [LabelText("加速时地面移动加速度")] public float ACCGroundedMoveACCSpeed = 1f;

    [Title("空中移动")]
    [LabelText("空中最大移动速度")] public float MaxAirMoveSpeed = 15f;
    [LabelText("空中移动加速度")] public float AirAccelerationSpeed = 15f;

    [Title("跳跃")]
    [LabelText("跳跃上升速度")] public float JumpUpSpeed = 5f;

    [Title("其他")]
    [LabelText("重力加速度")] public Vector3 Gravity = new Vector3(0, -10f, 0);
    [LabelText("速度衰减系数")] public float SpeedDamping = 0.1f;
}
