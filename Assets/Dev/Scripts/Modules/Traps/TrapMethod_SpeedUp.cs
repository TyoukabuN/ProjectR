using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PJR
{
    public class TrapMethod_SpeedUp : TrapMethod
    {
        public override TActionType ActionType => TActionType.SpeedUp;
        public override bool HasDirection => true;
        public override Vector3 Direction => Vector3.zero;
        [LabelText("速度系数")] public float speed = 2f;
        [LabelText("持续时间")] public float duration = 3f;
        
    } 
}

