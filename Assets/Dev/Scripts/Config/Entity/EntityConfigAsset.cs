using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace PJR
{
    [Serializable]
    public abstract class EntityConfigAsset : ScriptableObject
    {
        [NonSerialized]
        public EntityConfigHost host = null;

        [LabelText("是否运动")]
        public bool isPhysics = false;
        public virtual void GenrateEntity()
        {
        }
    }
}