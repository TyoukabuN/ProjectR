using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJR.Config
{
    public abstract class OrdinalConfigItemAsset<TItem> : OrdinalConfigItemAsset
        where TItem : OrdinalConfigItem
    {
        public TItem item;

        public override int ID { get => item?.ID ?? -1; set => item.ID = value; }
        public override string Name { get => item?.Name; set => item.Name = value; }
        public override bool Valid => item?.Valid ?? false;
        public override string Editor_LabelName => item?.Name;
        public override bool Editable => item?.Editable ?? false;
    }

    public abstract class OrdinalConfigItemAssetTemplate : OrdinalConfigItemAsset
    {
        [SerializeField, DisableIf("@true")]
        protected int _id;
        [SerializeField, HideIf("@IsHideDefaultNameField")]
        protected string _name; 
        public override int ID { get => _id; set => _id = value; }
        public override string Name { get => _name; set => _name = value; }
    }


    public abstract class OrdinalConfigItemAsset : SerializedScriptableObject, IOrdinalConfigItem
    {
        public abstract int ID { get; set; }
        public abstract string Name { get; set; }
        public virtual bool Valid => ID > 0;
        public int CompareTo(IOrdinalConfigItem other)
        {
            return ID.CompareTo(other.ID);
        }
        public virtual string Editor_LabelName => Name;
        public virtual bool Editable => false;
        public virtual bool IsHideDefaultNameField => false;
    }

    public interface IOrdinalConfigItem: IComparable<IOrdinalConfigItem>
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool Valid { get; }
        public string Editor_LabelName { get; }
        /// <summary>
        /// 可以用于强制配置id自增
        /// </summary>
        public bool Editable { get; }
        public bool IsHideDefaultNameField { get; }
    }
}
