using System.Collections;
using System.Collections.Generic;
using NPOI.SS.Formula.Functions;
using PJR.Core.Pooling;
using UnityEngine;

namespace PJR.Core.StateMachine
{
    public class OnFinish<TContext> : FsmTransition<TContext>
    {
        public override bool CanTransition()
        {
            return From?.IsFinish ?? false;
        }
        public static OnFinish<TContext> Get() => GenerialPool<OnFinish<TContext>>.Get();
        public override void Release() => GenerialPool<OnFinish<TContext>>.Release(this);
    }
}
