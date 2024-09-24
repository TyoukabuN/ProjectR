using System.Collections.Generic;
public class FlagManager256
{
	public static Dictionary<int, FlagManager256> category2Mgr = new Dictionary<int, FlagManager256>();
	public static FlagManager256 Get(int category) 
	{
	    category2Mgr = category2Mgr ?? new Dictionary<int, FlagManager256>();
	    if (!category2Mgr.TryGetValue(category, out var mgr))
	    {
	        mgr = new FlagManager256(category);
	        category2Mgr.Add(category, mgr);
	    }
	    return mgr;
	}
	public static Flag256 GetFlag(int category, string idStr)
	{
	    if (category <= 0 || string.IsNullOrEmpty(idStr))
	        return Flag256.Empty;
	    var mgr = Get(category);
	    if (mgr == null)
	        return Flag256.Empty;
	    return mgr.StringToFlag(idStr);
	}
	public static Flag256 GetFlag(FlagDefine flagDefine)
	{
	    if (flagDefine == null)
	        return Flag256.Empty;
	    return GetFlag(flagDefine.CategoryID, flagDefine.IDStr);
	}
	public static Flag256 GetFlag(int id)
	{
	    FlagIDInfo info = new FlagIDInfo(id);
	    if (!info.IsValid())
	        return Flag256.Empty;
	    return GetFlag(info.categoryID, info.flagIDStr);
	}
	public static Flag256 GetFlag(params int[] ids)
	{
	    var res = Flag256.Empty;
	    foreach (var id in ids)
	        res.FlagOr(GetFlag(id));
	    return res;
	}
	
	public Dictionary<string, Flag256> ActionEnum2Flag => _string2Flag;
	private Dictionary<string, Flag256> _string2Flag;
	private Dictionary<int, Flag256> _flagBitIndex2Flag;
	private int _bitCount = -1;
	private const int TotalBitCount = 256;
	private const int BITUnit = 32;
	
	public int category = -1;
	public FlagManager256()
	{
	    _string2Flag = new Dictionary<string, Flag256>();
	    _flagBitIndex2Flag = new Dictionary<int, Flag256>();
	}
	public FlagManager256(int category) : this()
	{
	    this.category = category;
	}
	public Flag256 StringToFlag(List<string> keys) => StringToFlag(keys.ToArray());
	public Flag256 StringToFlag(params string[] keys)
	{
	    var temp = Flag256.Empty;
	    for (int i = 0; i < keys.Length; i++)
	        temp |= StringToFlag(keys[i]);
	    return temp;
	}
	public Flag256 StringToFlag(string key)
	{
	    var flag = Flag256.Empty;
	    int targetBitIndex = _bitCount + 1;
	    if (targetBitIndex > TotalBitCount)
	    {
	        targetBitIndex = -1;
	        return flag;
	    }
	
	    _string2Flag ??= new Dictionary<string, Flag256>(TotalBitCount);
	
	    if (string.IsNullOrEmpty(key))
	    {
	        targetBitIndex = -1;
	        return Flag256.Empty;
	    }
	
	    if (!_string2Flag.TryGetValue(key, out flag))
	    {
	        flag = GetFlagByBitIndex(targetBitIndex);
	        _string2Flag[key] = flag;
	        _bitCount++;
	    }
	    return flag;
	}
	private Flag256 GetFlagByBitIndex(int flagBitIndex)
	{
	    _flagBitIndex2Flag ??= new Dictionary<int, Flag256>(TotalBitCount);
	    if (_flagBitIndex2Flag.TryGetValue(flagBitIndex, out Flag256 res))
	        return res;
	    int pos = flagBitIndex / BITUnit;
	    var temp = Flag256.Empty;
		if (pos == 0) temp.Value0 = (uint)(1 << flagBitIndex);
		if (pos == 1) temp.Value1 = (uint)(1 << flagBitIndex);
		if (pos == 2) temp.Value2 = (uint)(1 << flagBitIndex);
		if (pos == 3) temp.Value3 = (uint)(1 << flagBitIndex);
		if (pos == 4) temp.Value4 = (uint)(1 << flagBitIndex);
		if (pos == 5) temp.Value5 = (uint)(1 << flagBitIndex);
		if (pos == 6) temp.Value6 = (uint)(1 << flagBitIndex);
		if (pos == 7) temp.Value7 = (uint)(1 << flagBitIndex);
	    _flagBitIndex2Flag[flagBitIndex] = temp;
	    return temp;
	}
}
