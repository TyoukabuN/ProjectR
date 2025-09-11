using System;
using System.Diagnostics;
using PJR.Config;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)][Conditional("UNITY_EDITOR")]
public class AvatarConfigIDAttribute : OrdinalConfigIDAttribute
{
    public AvatarConfigIDAttribute(EDisplayOption displayOption = EDisplayOption.All) : base(displayOption)
    {
    }
}

