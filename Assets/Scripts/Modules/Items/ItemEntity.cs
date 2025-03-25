using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using PJR.Systems;
using PJR.ClassExtension;

namespace PJR
{
    public class ItemEntity : LogicEntity
    {
        public override string entityName => "ItemEntity";
        public ItemConfigAsset config;
        public ItemRepopHandler itembase;
        public override bool OnCreate(EntityContext context)
        {
            physEntity = EntitySystem.GetPhysEntityInstance();
            if (physEntity == null)
            {
                LogSystem.LogError("Failure to create a PhysEntity", true);
                return false;
            }
            physEntity.CreateAvatar(this, PhysEntityComponentRequire.NonKCCOnly, OnDrawGizmos);
            //
            physEntity.onTriggerEnter += OnTriggerEnter;
            physEntity.onTriggerStay += OnTriggerEnter;

            itembase = physEntity.AddComponent<Item_Buff>();
            itembase.CanRegenerateTimes = config.CanRegenerateTimes;
            itembase.interval = config.interval;
            itembase.itemconfig = config;
            return true;
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

