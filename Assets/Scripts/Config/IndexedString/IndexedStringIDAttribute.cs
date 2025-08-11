using System;
using System.Diagnostics;

namespace PJR.Config
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [Conditional("UNITY_EDITOR")]
    public class IndexedStringIDAttribute : Attribute
    {
    }
}
