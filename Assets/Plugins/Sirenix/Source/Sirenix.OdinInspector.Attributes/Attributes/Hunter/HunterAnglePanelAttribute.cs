using System;

namespace Sirenix.OdinInspector
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class HunterAnglePanelAttribute : Attribute
    {
        public readonly float Width;
        public readonly bool ClockWise;
        public HunterAnglePanelAttribute(float width = 200f, bool clockWise = true)
        {
            Width = width;
            ClockWise = clockWise;
        }
    }
}
