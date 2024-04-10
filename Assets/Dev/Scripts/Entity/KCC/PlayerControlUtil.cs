using PJR.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public static class PlayerControlUtil
    {
        public static void GroundedMove(KCCContext ctx)
        {
            var curSpeed = ctx.inputVelocity.magnitude;
            var output = curSpeed;

            //get taraget speed
            float targetVelocity = ctx.phys.MaxGroundedMoveSpeed;
            if (ctx.inputKeyActive.TryGetValue(RegisterKeys.Run, out var running) && running)
                targetVelocity = ctx.phys.ACCMaxGroundedMoveSpeed;

            if (output > ctx.phys.MaxGroundedMoveSpeed)
            {
                //dampping
                output = Mathf.Lerp(curSpeed, targetVelocity, ctx.phys.SpeedDamping);
            }
            else
            {
                //acc
                output += ctx.deltaTime * ctx.phys.GroundedMoveACCSpeed;
            }

            ctx.outputVelocity = ctx.direction * output;
        }
    }
}
