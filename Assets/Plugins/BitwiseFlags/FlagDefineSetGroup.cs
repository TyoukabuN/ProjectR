using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlagDefineSetGroup : SerializedScriptableObject, IFlagDefine
{
    public int id = -1;
    public string nameStr = null;

    [ListDrawerSettings( DefaultExpandedState = true )]
    public List<FlagDefineSet> FlagDefineSets;

    #region IFlagDefine
    public int ID => id;
    public string Name => nameStr;
    public bool IsValid()
    {
        if (FlagDefineSets == null)
            return true;
        if (FlagDefineSets.Count <= 0)
            return true;
        if(FlagDefineSets.All(set=>!set.IsValid()))
            return true;
        return false;
    }
    public string GetKey()
    {
        if (!IsValid())
            return string.Empty;
        return FlagDefineSets[0].GetKey();
    }

    [NonSerialized]
    private string[] keys = null;
    public string[] GetKeys()
    {
        if (keys != null)
            return keys;
        //
        if (IsValid())
        {
            if (keys == null)
                keys = new string[0];
            return keys;
        }
        //
        if (keys == null)
            keys = new string[FlagDefineSets.Count];
        for (int i = 0; i < FlagDefineSets.Count; i++)
        {
            if (!FlagDefineSets[i].IsValid())
                continue;
            keys[i] = FlagDefineSets[i].GetKey();
        }
        return keys;
    }
    #endregion
}
