using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public abstract class KCCLogicBase
    {
        public virtual void OnUpdateVelocity(KCContext context) { }
        public virtual void OnUpdateRotation(KCContext context) { }
    }
}
