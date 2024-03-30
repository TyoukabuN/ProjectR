using System;

namespace Sirenix.OdinInspector
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class HunterDirectionPanelAttribute : Attribute
    {
        public float Width = 200f;
    }
}
