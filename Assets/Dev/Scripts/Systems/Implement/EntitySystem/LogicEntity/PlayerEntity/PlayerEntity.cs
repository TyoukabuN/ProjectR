using PJR.ScriptStates.Player;
using System.Text;
using UnityEngine;
using PJR.Systems;

#if UNITY_EDITOR
using UnityEditor;
using Sirenix.Utilities.Editor;
#endif

namespace PJR
{
    public partial class PlayerEntity : LogicEntity
    {
        public override string entityName => "PlayerEntity";

        public override bool OnCreate(EntityContext context)
        { 
            var physEntity = EntitySystem.GetPhysEntityInstance();
            if (physEntity == null)
            {
                LogSystem.LogError("Failure to create a PhysEntity", true);
                return false;
            }

            context.avatarAssetNames = new AvatarAssetNames()
            {
                modelName = "Avater_KCCTester.prefab",
            };

            if (ResourceSystem.TryGetAsset("EntityPhysicsConfig.asset", out var loader))
                physicsConfig = loader.GetRawAsset<EntityPhysicsConfigAsset>();
            else
                LogSystem.LogError("PlayerEntity.OnCreate 加载 EntityPhysicsConfig 失败");

            //
            physEntity.gameObject.tag = EntityDefine.EntityTag.Player;
            physEntity.CreateAvatar(this, PhysEntityComponentRequire.NonAnimancerOnly, OnAvatarLoadDone, OnDrawGizmos);
            //
            this.physEntity = physEntity;

#if UNITY_EDITOR
            GUIHelper.PingObject(physEntity);
#endif

            return true;
        }
        protected override void OnAvatarLoadDone(PhysEntity physEntity)
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

            LogicUnits_OnUpdate(Time.deltaTime);
            //Update_Input();
            //Update_State();
        }
        public override void LateUpdate()
        {
            base.LateUpdate();

            LogicUnits_OnLatedUpdate();
        }

        public override void Destroy()
        {
            base.Destroy();

            LogicUnits_OnDestroy();
            //Destroy_Input();
            //Destroy_State();

            EntitySystem.ReleasePhysEntity(physEntity);
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
