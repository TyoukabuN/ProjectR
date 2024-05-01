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
        [LabelText("�ٶ�ϵ��")] public float speed = 2f;
        [LabelText("����ʱ��")] public float duration = 3f;
        
    } 
}

