using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace PJR
{
    public class ItemEntity : LogicEntity
    {
        public ItemConfig config;
        public ItemBase itembase;
        public override void OnCreate(EntityContext context)
        {
            var physEntity = EntitySystem.CreatePhysEntity();
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
        public void OnDrawGizmos()
        {
        }
        public void OnTriggerEnter(Collider collider)
        {
            var phys = collider.GetComponent<PhysEntity>();
            if (phys != null && phys.logicEntity != null)
                config.ExecutePhaseEvent(TEntityPhase.OnTriggerEnter, this, phys.logicEntity);

        }
        public override void Destroy()
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}

