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
            physEntity.CreateAvater(context.assetFullName);
            physEntity.onAvaterLoadDone += OnAvaterLoadDone;
            //
            this.physEntity = physEntity;
        }
        void OnAvaterLoadDone(PhysEntity physEntity)
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
    }
}
