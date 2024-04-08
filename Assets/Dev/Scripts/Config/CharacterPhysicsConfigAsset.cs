using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "配置/角色物理配置",fileName = "CharacterPhysicsConfig")]
public class CharacterPhysicsConfigAsset : SerializedScriptableObject
{
    [Title("地面移动")]
    public float GroundedMoveSpeed = 3f;

    [Title("空中移动")]
    public float AirGroundedSpeed = 3f;

    [Title("跳跃")]
    public float JumpUpSpeed = 5f;

    [Title("其他")]
    public Vector3 Gravity = new Vector3(0, -10f, 0);
}
