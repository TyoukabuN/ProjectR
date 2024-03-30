using System;
using UnityEngine;

namespace Sirenix.OdinInspector
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public class HideObjectExpandedAttribute : Attribute
    {
    }
}