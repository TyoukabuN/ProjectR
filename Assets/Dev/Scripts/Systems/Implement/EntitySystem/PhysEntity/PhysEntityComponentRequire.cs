using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

namespace PJR
{
    /// <summary>
    /// 给物理实体的组件请求列表
    /// </summary>
    public struct PhysEntityComponentRequire
    {
        public bool kinematicCharacterMotor;
        public bool animancer;
        public bool collider;
        public static PhysEntityComponentRequire Default => new PhysEntityComponentRequire{
            kinematicCharacterMotor = true,
            animancer = true,
            collider = true,
        };
    }
}