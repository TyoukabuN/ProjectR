using System.Collections.Generic;
using UnityEngine;
using System;

public static class Flag256Util
{
    public const bool USING_FLAG = true;

    private static Dictionary<string, Flag256> _actionEnum2Flag = new Dictionary<string, Flag256>();
    private static int bitCount = -1;
    private static int totalBitCount = 256;
    
    public static Flag256 RequireFlag(string key)
    {
        int _bitCount = bitCount + 1;
        if (_bitCount > totalBitCount)
        {
            Debug.LogError($"_bitCount > totalBitCount({totalBitCount}) ");
            throw new Exception("Required FlagCount OutOf TotalBitCount!");
        }

        if (!_actionEnum2Flag.TryGetValue(key, out var flag))
        {
            flag = Flag256Util.Flag(_bitCount);
            _actionEnum2Flag[key] = flag;
            bitCount = _bitCount; 
        }

        return flag;
    }
    
    private const int BitUnit = 32;
    private static Dictionary<int, Flag256> FlagBitIndex2Flag256;
    public static Flag256 Flag(int flagBitIndex)
    {
        FlagBitIndex2Flag256 ??= new Dictionary<int, Flag256>(256);
       
        if (FlagBitIndex2Flag256.TryGetValue(flagBitIndex,out Flag256 res))
            return res;

        int pos = flagBitIndex / BitUnit;
        var temp = Flag256.Empty;
        if (pos == 0) temp.Value0 = (uint)(1 << flagBitIndex);
        if (pos == 1) temp.Value1 = (uint)(1 << flagBitIndex);
        if (pos == 2) temp.Value2 = (uint)(1 << flagBitIndex);
        if (pos == 3) temp.Value3 = (uint)(1 << flagBitIndex);
        if (pos == 4) temp.Value4 = (uint)(1 << flagBitIndex);
        if (pos == 5) temp.Value5 = (uint)(1 << flagBitIndex);
        if (pos == 6) temp.Value6 = (uint)(1 << flagBitIndex);
        if (pos == 7) temp.Value7 = (uint)(1 << flagBitIndex);
        FlagBitIndex2Flag256[flagBitIndex] = temp; 
        return temp;
    }
}