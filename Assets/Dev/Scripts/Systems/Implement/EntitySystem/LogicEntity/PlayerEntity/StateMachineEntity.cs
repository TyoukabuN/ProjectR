using PJR.ScriptStates.Player;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using PJR.Systems;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR
{
    public partial class StateMachineEntity : LogicEntity
    {
        public override string entityName => "StateMachineEntity";

        public EntityPhysicsConfigAsset PhysicsConfig => physicsConfig;

        public override void Update()
        {
            base.Update();
             
            Update_State();
        }
        public override void LateUpdate()
        {
            base.LateUpdate();
        }

        public override void Destroy()
        {
            base.Destroy();

            Destroy_State();

            EntitySystem.DestroyPhysEntity(physEntity);
        }

#if UNITY_EDITOR
        private GUIStyle GizmosGUIStyle = null;
        StringBuilder builder;

        public override void OnDrawGizmos()
        {
            if (GizmosGUIStyle == null)
            {
                GUIStyle style = new GUIStyle();
                style.richText = true;
                GizmosGUIStyle = style;
            }
            if (builder == null)
            {
                builder = new StringBuilder();
            }
            builder.Clear();

            //builder.AppendLine(string.Format("<color=red>跳跃:{0}/{1}</color>", jumpCounter.ToString(), jumpableTimes.ToString()));
            //builder.AppendLine(string.Format("<color=red>着地:{0}</color>", Grounded.ToString()));
            //builder.AppendLine(string.Format("<color=red>移动输入:{0}</color>", inputAxi.ToString()));
            builder.AppendLine(string.Format("<color=red>速度:{0}</color>", physEntity == null ? 0 : physEntity.motor.BaseVelocity.ToString()));
            //builder.AppendLine(string.Format("<color=red>XZAnima:{0}</color>", inputAxi));
            if(scriptStateMachine != null)
                builder.AppendLine(string.Format("<color=red>状态:{0}</color>", (EPlayerState)scriptStateMachine?.CurrentEState));

            Handles.Label(transform.position, builder.ToString(), GizmosGUIStyle);
            Handles.color = Color.red;

            Gizmos.color = Color.blue;
            var from = transform.position;
            from.y += 0.5f;
            Gizmos.DrawLine(from, from + transform.forward);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(from, from + transform.right);
            //Gizmos.DrawWireSphere(transform.position + groundedOffset, groundedRadius);

        }
#endif
    }
}
