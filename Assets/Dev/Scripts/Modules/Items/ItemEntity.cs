using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace PJR
{
    public class ItemEntity : LogicEntity
    {
        public override string entityName => "ItemEntity";
        public ItemConfigAsset config;
        public ItemRepopHandler itembase;
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
            physEntity.onTriggerStay += OnTriggerEnter;

            itembase = physEntity.AddComponent<Item_Buff>();
            itembase.CanRegenerateTimes = config.CanRegenerateTimes;
            itembase.interval = config.interval;
            itembase.itemconfig = config;
        }
        public void OnTriggerEnter(Collider collider)
        {
            var phys = collider.GetComponent<PhysEntity>();
            if (phys != null && phys.avatar.activeSelf && phys.logicEntity != null)
                config.TryExecutePhaseEvent(PhyEntityPhase.OnTriggerEnter, this, phys.logicEntity);
        }
        public override void Destroy()
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}

