using System;

namespace Sirenix.OdinInspector
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [DontApplyToListElements]
    public class DrawListElementIndividualAttribute : Attribute
    {
    }
}