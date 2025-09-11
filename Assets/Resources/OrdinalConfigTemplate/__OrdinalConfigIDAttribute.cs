#if UNITY_EDITOR
using System;
using System.Diagnostics;

namespace PJR.Config
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [Conditional("UNITY_EDITOR")]
    public class __OrdinalConfigIDAttribute : OrdinalConfigIDAttribute
    {
        public __OrdinalConfigIDAttribute(EDisplayOption displayOption = EDisplayOption.All): base(displayOption)
        {
        }
    }
}
#endif