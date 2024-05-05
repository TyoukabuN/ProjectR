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
        public override string entityName => "TrapEntity";
        public TrapConfigAsset configAsset;
        public TrapConfigHost configHost;
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
                configAsset.TryExecutePhaseEvent(PhyEntityPhase.OnTriggerEnter,this, phys.logicEntity);
        }

        public override void OnDrawGizmos()
        {
            if (physEntity.boxCollider)
            {
                Gizmos.DrawCube(physEntity.transform.position, physEntity.boxCollider.size);
            }
        }
    }
}
