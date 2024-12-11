using UnityEngine;
using PJR.Systems;

namespace PJR
{
    public class TrapEntity : LogicEntity
    {
        public override string entityName => "TrapEntity";
        public TrapConfigAsset configAsset;
        public TrapConfigHost configHost;
        public override bool OnCreate(EntityContext context)
        {
            physEntity = EntitySystem.CreatePhysEntity();
            if (physEntity == null)
            {
                LogSystem.LogError("Failure to create a PhysEntity", true);
                return false;
            }
            physEntity.CreateAvatar(this, PhysEntityComponentRequire.NonKCCOnly, OnDrawGizmos);
            //

            physEntity.onTriggerEnter += OnTriggerEnter;
            return true;
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
