using System;
using System.Diagnostics;

namespace PJR.Config
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [Conditional("UNITY_EDITOR")]
    public abstract class OrdinalConfigIDAttribute : Attribute
    {
        [Flags]
        public enum EDisplayOption
        {
            None = 0,
            CreateButton = 1 << 0,
            CopyButton = 1 << 1,
            SelectButton = 1 << 2,
            Detail = 1 << 3,
            
            All = CreateButton | 
                  CopyButton | 
                  SelectButton | 
                  Detail
        }
        public EDisplayOption DisplayOption = EDisplayOption.All;
        public OrdinalConfigIDAttribute(EDisplayOption displayOption)
        {
            DisplayOption = displayOption;
        }
    }
}
