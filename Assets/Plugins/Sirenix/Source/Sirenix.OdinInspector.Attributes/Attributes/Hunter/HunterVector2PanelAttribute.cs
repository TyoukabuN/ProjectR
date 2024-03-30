using System;
using UnityEngine;

namespace Sirenix.OdinInspector
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class HunterVector2PanelAttribute : Attribute
    {
        public readonly Vector2 Center;
        public readonly int Radius;
        public readonly bool ClampOffsetMagnitude;
        public readonly bool SnapMagnitude;
        public readonly float Width;
        
        public HunterVector2PanelAttribute() : this(Vector2.zero, 1, true)
        {
        }

        public HunterVector2PanelAttribute(int radius, bool clampMagnitude = true, bool snapMagnitude = false) : this(Vector2.zero, radius, clampMagnitude)
        {
        }

        public HunterVector2PanelAttribute(Vector2 center, int radius, bool clampOffsetMagnitude = true, bool snapMagnitude = false, float width = 200f)
        {
            Center = center;
            this.Radius = radius <= 0 ? 1 : radius;
            this.ClampOffsetMagnitude = clampOffsetMagnitude;
            this.SnapMagnitude = snapMagnitude;
            this.Width = width;
        }
    }
}
