using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

namespace PJR
{
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