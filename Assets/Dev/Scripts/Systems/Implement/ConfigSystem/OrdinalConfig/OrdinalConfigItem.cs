using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace LS.Game
{
    [HideReferenceObjectPicker]
    [Serializable]
    public abstract class OrdinalConfigItem : IOrdinalConfigItem
    {
        [SerializeField, DisableIf("@!Editable")]
        protected int id = -1;
        [SerializeField]
        protected string name = null;
        public virtual int ID { get => id; set => id = value; }
        public virtual string Name { get => name; set => name = value; }
        public virtual bool Valid
        {
            get
            {
                if (ID < 0)
                    return false;
                return true;
            }
        }
        public virtual int CompareTo(IOrdinalConfigItem other)
        {
            return ID.CompareTo(other.ID);
        }

#if UNITY_EDITOR
        public virtual string Editor_LabelName => Name;
        [NonSerialized, HideIf("@true")]
        public bool Editing = false;
        public virtual bool Editable => Editing;
#endif
    }
}