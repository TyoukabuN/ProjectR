using PJR.ScriptStates.Player;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Sirenix.Utilities.Editor;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR
{
    public partial class PlayerEntity : LogicEntity
    {
        public EntityPhysicsConfigAsset PhysicsConfig => physicsConfig;
        public override void OnCreate(EntityContext context)
        { 
            var physEntity = EntitySystem.CreatePhysEntity();

            context.avatarAssetNames = new AvatarAssetNames()
            {
                modelName = "Avatar_Slime.prefab",
                animationClipSet = "Slime_ClipTransitionSet.asset"
            };

            if (ResourceSystem.TryGetAsset("EntityPhysicsConfig.asset", out var loader))
                physicsConfig = loader.GetRawAsset<EntityPhysicsConfigAsset>();
            else
                LogSystem.LogError("PlayerEntity.OnCreate 加载 EntityPhysicsConfig 失败");

            //
            physEntity.CreateAvatar(this);
            physEntity.logicEntity = this;
            physEntity.onAvatarLoadDone += OnAvatarLoadDone;
            physEntity.onDrawGizmos += OnDrawGizmos;
            //
            this.physEntity = physEntity;
            this.gameObject = physEntity.gameObject;
            this.transform = physEntity.transform;

#if UNITY_EDITOR
            GUIHelper.PingObject(physEntity);
#endif
        }
        void OnAvatarLoadDone(PhysEntity physEntity)
        {
            if (physEntity != this.physEntity)
                return;

            CameraSystem.CreatePlayerCamera(this);

            Init_Input();
            Init_State();
        }

        public override void Update()
        {
            base.Update();
             
            Update_Input();
            Update_State();
        }
        public override void LateUpdate()
        {
            base.LateUpdate();

            LateUpdate_Input();
        }

        public override void Destroy()
        {
            base.Destroy();

            Destroy_Input();
            Destroy_State();

            EntitySystem.DestroyPhysEntity(physEntity);
        }

#if UNITY_EDITOR
        private GUIStyle GizmosGUIStyle = null;
        StringBuilder builder;

        public void OnDrawGizmos()
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
