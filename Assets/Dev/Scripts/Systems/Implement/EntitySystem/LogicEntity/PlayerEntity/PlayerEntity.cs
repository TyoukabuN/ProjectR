using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public partial class PlayerEntity : LogicEntity
    {
        public override void OnCreate(EntityContext context)
        { 
            var physEntity = EntitySystem.CreatePhysEntity();

            var assetNames = new AvatarAssetNames()
            {
                modelName = "Assets/Art/Character/GlazyRunner/Prefabs/Avater_DefaultPlayer.prefab",
                animationClipSet = "Assets/Art/Character/GlazyRunner/Animations/AnimatiomClipTransitionSet.asset"
            };
            physEntity.CreateAvatar(assetNames);
            physEntity.onAvatarLoadDone += OnAvatarLoadDone;
            //
            this.physEntity = physEntity;
        }
        void OnAvatarLoadDone(PhysEntity physEntity)
        {
            if (physEntity != this.physEntity)
                return;

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
    }
}
