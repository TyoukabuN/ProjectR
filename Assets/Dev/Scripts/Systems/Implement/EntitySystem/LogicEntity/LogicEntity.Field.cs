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
        public virtual KCContext InputKCContent { get => null; }
        public virtual OrientationMethod OrientationMethod { get => OrientationMethod.TowardsCamera; }
    }
}
