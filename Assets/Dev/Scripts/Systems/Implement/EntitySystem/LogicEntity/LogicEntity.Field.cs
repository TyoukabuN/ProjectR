using System.Collections.Generic;
using PJR.LogicUnits;
using PJR.Systems;
using UnityEngine;
using System.Linq;
using static PJR.StateMachineEntity;

namespace PJR
{
    public abstract partial class LogicEntity
    {
        protected KCContext inputKCContent;
        public virtual KCContext InputKCContent => inputKCContent ??= new KCContext();
        public virtual OrientationMethod OrientationMethod { get => OrientationMethod.TowardsCamera; }
    }
}
