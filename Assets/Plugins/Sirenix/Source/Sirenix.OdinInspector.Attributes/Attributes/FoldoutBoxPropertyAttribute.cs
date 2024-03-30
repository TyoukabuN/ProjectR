using System;

namespace Sirenix.OdinInspector
{
    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public class FoldoutBoxPropertyAttribute : Attribute
    {
        public bool Expanded;
    }
}
