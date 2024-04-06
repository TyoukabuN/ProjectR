using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public partial class EntitySystem : MonoSingletonSystem<EntitySystem>
    {
        public override void Init()
        {

        }

        public static bool Valid()
        {
            if (!Application.isPlaying)
            { 
                LogSystem.LogError("[EntitySystem] Invoke system function not in Playmode");
                return false;
            }
            return true;
        }

        public override void Update()
        {
            base.Update();

            foreach (var pair in id2LogicEntity) 
            {
                var entity = pair.Value;
                if (entity == null)
                    continue;
                entity.Update();
            }
        }

        public override void LateUpdate()
        {
            base.LateUpdate();

            foreach (var pair in id2LogicEntity)
            {
                var entity = pair.Value;
                if (entity == null)
                    continue;
                entity.LateUpdate();
            }
        }
    }
}
