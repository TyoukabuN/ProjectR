using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;

namespace PJR
{
    public class TrapEntity : LogicEntity
    {
        public TrapConfigAsset configAsset;

        public override void OnCreate(EntityContext context)
        {
            var physEntity = EntitySystem.CreatePhysEntity();
            //
            var physRequire = PhysEntityComponentRequire.Default;
            physRequire.kinematicCharacterMotor = false;
            
            physEntity.CreateAvatar(this);
            physEntity.logicEntity = this;
            physEntity.physRequire = physRequire;
            physEntity.onDrawGizmos += OnDrawGizmos;

            //
            this.physEntity = physEntity;
            this.gameObject = physEntity.gameObject;
            this.transform = physEntity.transform;

            physEntity.onTriggerEnter += OnTriggerEnter;
        }

        public void OnTriggerEnter(Collider collider)
        {
            var phys = collider.GetComponent<PhysEntity>();
            if (phys != null && phys.logicEntity != null)
            {
                foreach (var phareEvt in configAsset.entityPhaseEvents)
                {
                    if (phareEvt.entityPhase != TEntityPhase.OnTriggerEnter)
                        continue;
                    foreach (var evt in phareEvt.events)
                    {
                        if (evt.actionType == TActionType.AddForce)
                        {
                            var param = evt.trapActionEventParam as TrapActionParamAsset_AddForce;
                            if (param == null)
                                continue;
                            var controller = phys.logicEntity.AddExtraVelocity(
                                entityContext.LogicEntityIDStr, 
                                param.force, 
                                param.duration,
                                param.damp);
                            controller.easeType = param.easing;
                        }
                    }
                }
            }
        }

        public void OnDrawGizmos()
        {
            if (physEntity.boxCollider)
            {
                Gizmos.DrawCube(physEntity.transform.position, physEntity.boxCollider.size);
            }
        }
    }
}
