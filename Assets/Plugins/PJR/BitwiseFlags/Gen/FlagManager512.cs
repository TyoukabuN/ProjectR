using System.Collections.Generic;
public class FlagManager512
{
	public static Dictionary<int, FlagManager512> category2Mgr = new Dictionary<int, FlagManager512>();
	public static FlagManager512 Get(int category) 
	{
	    category2Mgr = category2Mgr ?? new Dictionary<int, FlagManager512>();
	    if (!category2Mgr.TryGetValue(category, out var mgr))
	    {
	        mgr = new FlagManager512(category);
	        category2Mgr.Add(category, mgr);
	    }
	    return mgr;
	}
	public static Flag512 GetFlag(int category, string idStr)
	{
	    if (category <= 0 || string.IsNullOrEmpty(idStr))
	        return Flag512.Empty;
	    var mgr = Get(category);
	    if (mgr == null)
	        return Flag512.Empty;
	    return mgr.StringToFlag(idStr);
	}
	public static Flag512 GetFlag(FlagHandle512 flagHandle)
	{
	    if (flagHandle == null)
	        return Flag512.Empty;
	    return GetFlag(flagHandle.CategoryID, flagHandle.FlagIDStr);
	}
	public static Flag512 GetFlag(FlagDefine flagDefine)
	{
	    if (flagDefine == null)
	        return Flag512.Empty;
	    return GetFlag(flagDefine.CategoryID, flagDefine.IDStr);
	}
	public static Flag512 GetFlag(int id)
	{
	    FlagIDInfo info = new FlagIDInfo(id);
	    if (!info.IsValid())
	        return Flag512.Empty;
	    return GetFlag(info.categoryID, info.flagIDStr);
	}
	public static Flag512 GetFlag(params int[] ids)
	{
	    var res = Flag512.Empty;
	    foreach (var id in ids)
	        res.FlagOr(GetFlag(id));
	    return res;
	}
	
	public Dictionary<string, Flag512> ActionEnum2Flag => _string2Flag;
	private Dictionary<string, Flag512> _string2Flag;
	private Dictionary<int, Flag512> _flagBitIndex2Flag;
	private int _bitCount = -1;
	private const int TotalBitCount = 512;
	private const int BITUnit = 32;
	
	public int category = -1;
	public FlagManager512()
	{
	    _string2Flag = new Dictionary<string, Flag512>();
	    _flagBitIndex2Flag = new Dictionary<int, Flag512>();
	}
	public FlagManager512(int category) : this()
	{
	    this.category = category;
	}
	public Flag512 StringToFlag(List<string> keys) => StringToFlag(keys.ToArray());
	public Flag512 StringToFlag(params string[] keys)
	{
	    var temp = Flag512.Empty;
	    for (int i = 0; i < keys.Length; i++)
	        temp |= StringToFlag(keys[i]);
	    return temp;
	}
	public Flag512 StringToFlag(string key)
	{
	    var flag = Flag512.Empty;
	    int targetBitIndex = _bitCount + 1;
	    if (targetBitIndex > TotalBitCount)
	    {
	        targetBitIndex = -1;
	        return flag;
	    }
	
	    _string2Flag ??= new Dictionary<string, Flag512>(TotalBitCount);
	
	    if (string.IsNullOrEmpty(key))
	    {
	        targetBitIndex = -1;
	        return Flag512.Empty;
	    }
	
	    if (!_string2Flag.TryGetValue(key, out flag))
	    {
	        flag = GetFlagByBitIndex(targetBitIndex);
	        _string2Flag[key] = flag;
	        _bitCount++;
	    }
	    return flag;
	}
	private Flag512 GetFlagByBitIndex(int flagBitIndex)
	{
	    _flagBitIndex2Flag ??= new Dictionary<int, Flag512>(TotalBitCount);
	    if (_flagBitIndex2Flag.TryGetValue(flagBitIndex, out Flag512 res))
	        return res;
	    int pos = flagBitIndex / BITUnit;
	    var temp = Flag512.Empty;
		if (pos == 0) temp.Value0 = (uint)(1 << flagBitIndex);
		if (pos == 1) temp.Value1 = (uint)(1 << flagBitIndex);
		if (pos == 2) temp.Value2 = (uint)(1 << flagBitIndex);
		if (pos == 3) temp.Value3 = (uint)(1 << flagBitIndex);
		if (pos == 4) temp.Value4 = (uint)(1 << flagBitIndex);
		if (pos == 5) temp.Value5 = (uint)(1 << flagBitIndex);
		if (pos == 6) temp.Value6 = (uint)(1 << flagBitIndex);
		if (pos == 7) temp.Value7 = (uint)(1 << flagBitIndex);
		if (pos == 8) temp.Value8 = (uint)(1 << flagBitIndex);
		if (pos == 9) temp.Value9 = (uint)(1 << flagBitIndex);
		if (pos == 10) temp.Value10 = (uint)(1 << flagBitIndex);
		if (pos == 11) temp.Value11 = (uint)(1 << flagBitIndex);
		if (pos == 12) temp.Value12 = (uint)(1 << flagBitIndex);
		if (pos == 13) temp.Value13 = (uint)(1 << flagBitIndex);
		if (pos == 14) temp.Value14 = (uint)(1 << flagBitIndex);
		if (pos == 15) temp.Value15 = (uint)(1 << flagBitIndex);
	    _flagBitIndex2Flag[flagBitIndex] = temp;
	    return temp;
	}
}
