using System;
using System.Diagnostics;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)][Conditional("UNITY_EDITOR")]
public class AvatarConfigIDAttribute : Attribute
{

}

