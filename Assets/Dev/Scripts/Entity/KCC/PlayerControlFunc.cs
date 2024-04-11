using PJR.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    /// <summary>
    /// 玩家控制方法
    /// </summary>
    public static class PlayerControlFunc
    {
        /// <summary>
        /// 地面上移动
        /// </summary>
        /// <param name="ctx"></param>
        public static void GroundedMove(KCCContext ctx)
        {
            var velocoty = ctx.inputVelocity.magnitude;

            //get taraget speed
            float targetVelocity = ctx.cfg.MaxGroundedMoveSpeed;
            if (ctx.inputHandle.HasAnyFlag(RegisterKeys.Run))
                targetVelocity = ctx.cfg.ACCMaxGroundedMoveSpeed;

            if (velocoty > ctx.cfg.MaxGroundedMoveSpeed)
            {
                //dampping
                velocoty = Mathf.Lerp(velocoty, targetVelocity, ctx.cfg.SpeedDamping);
            }
            else
            {
                //acc
                velocoty += ctx.deltaTime * ctx.cfg.GroundedMoveACCSpeed;
            }

            ctx.outputVelocity = ctx.direction * velocoty;
        }

        /// <summary>
        /// 进入空中
        /// </summary>
        /// <param name="ctx"></param>
        public static void EnterToAir(KCCContext ctx)
        {
            var output = ctx.inputVelocity;
            var motor = ctx.motor;

            float JumpUpSpeed = ctx.cfg.JumpUpSpeed;

            Vector3 jumpDirection = ctx.motor.CharacterUp;
            if (motor.GroundingStatus.FoundAnyGround && !motor.GroundingStatus.IsStableOnGround)
            {
                jumpDirection = ctx.motor.GroundingStatus.GroundNormal;
            }

            output += (jumpDirection * JumpUpSpeed) - Vector3.Project(output, motor.CharacterUp);
            ctx.outputVelocity = output;

            motor.ForceUnground();
        }

        /// <summary>
        /// 空中移动
        /// </summary>
        /// <param name="ctx"></param>
        public static void OnAir(KCCContext ctx)
        {
            var output = ctx.inputVelocity;
            var cfg = ctx.cfg;

            output += cfg.Gravity * ctx.deltaTime;
            output *= (1f / (1f + (cfg.SpeedDamping * ctx.deltaTime)));
            ctx.outputVelocity = output;
        }
    }
}
