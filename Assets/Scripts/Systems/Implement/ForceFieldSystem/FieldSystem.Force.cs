using Sirenix.OdinInspector;
using UnityEngine;
using static PJR.Systems.ForceField;

namespace PJR.Systems
{
    public partial class FieldSystem : MonoSingletonSystem<FieldSystem>
    {
        /// <summary>
        /// 场型状
        /// </summary>
        public enum FieldShape
        {
            [LabelText("圆型")]
            Sphere,
            [LabelText("矩形(还没弄)")]
            Cube,
        }

        /// <summary>
        /// 添加圆形力场
        /// </summary>
        public ForceField AddSphereForceField(AttenuateMode attenuateMode, float radius, Vector3 translationOffset, float duration)
        {
            //TODO:[T]后面加个ObjectPool
            if (instance == null)
                return null;
            int id = ++s_id;
            string name = $"[ForceField]_{id}";
            GameObject gobj = new GameObject(name);
            //
            var fieldObj = gobj.AddComponent<ForceField>();
            fieldObj.id = id;
            fieldObj.attenuateMode = attenuateMode;
            fieldObj.Radius = radius;
            fieldObj.translationOffset = translationOffset;
            fieldObj.sqrRadius = radius * radius;
            fieldObj.duration = duration;
            fieldObj.OnCreate();
            //
            gobj.transform.SetParent(GetFieldRoot(), false);
            //
            fields.Add(fieldObj);
            id2handles[id] = fieldObj;
            return fieldObj;
        }
        public ForceField AddSphereForceField(AttenuateMode attenuateMode, float radius, float duration)
        {
            return AddSphereForceField(attenuateMode, radius, Vector3.zero, duration);
        }

        public int AddSphereForceField(AttenuateMode attenuateMode, float radius, float duration,out ForceField fieldObj)
        {
            fieldObj = AddSphereForceField(attenuateMode, radius, duration);
            return fieldObj.id;
        }
        public int AddSphereForceField(AttenuateMode attenuateMode, float radius, Vector3 translationOffset, float duration, out ForceField fieldObj)
        {
            fieldObj = AddSphereForceField(attenuateMode, radius, translationOffset, duration);
            return fieldObj.id;
        }
    }





    public interface ForceAppliable
    {
        public void ApplyForce(Vector3 force);
    }
}
