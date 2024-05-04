using UnityEngine;
using System.Security.Policy;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR
{
    [RequireComponent(typeof(BoxCollider))]
    public class DeadZone : MonoBehaviour
    {
        public BoxCollider boxCollider;
        public Renderer renderer;
        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
            renderer = GetComponent<Renderer>();
            if(renderer != null) renderer.enabled = false;
        }
        protected virtual void OnTriggerEnter(Collider other)
        {
            var entity = EntitySystem.GetPhysEntityByGobjInstanceId(other.gameObject.GetInstanceID());
            if (entity == null)
                return;
            if (!SceneSystem.instance.GetClosestResetPoint(entity.transform.position, out var closestPoint))
                return;
            entity.logicEntity.RemoveExtendValue(EntityDefine.ExtraValueKey.Dash);
            entity.SetPosition(closestPoint);
        }
        protected virtual void OnTriggerStay(Collider other)
        {
        }
        protected virtual void OnTriggerExit(Collider other)
        {
        }

        protected void OnDrawGizmos()
        {
            if (boxCollider == null)
                return;
            Gizmos.DrawWireCube(transform.position + boxCollider.center, boxCollider.size);
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(DeadZone))]
    public class DeadZoneEditor : Editor
    {
        private BoxCollider boxCollider;
        public void OnSceneGUI()
        {
            if(target == null)
                return;
            DeadZone inst = (DeadZone)target;
            boxCollider = inst.GetComponent<BoxCollider>();
            Handles.DrawWireCube(inst.transform.position + boxCollider.center, boxCollider.size);
        }
    }
#endif
}
