using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)] 
public class FlagConfigMarkAttribute : Attribute { 
    public FlagConfigMarkAttribute() 
    { 
    } 
}
