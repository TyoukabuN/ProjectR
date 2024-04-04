using System.Collections.Generic;
namespace PJR
{
	public class FlagHandle256
	{
		public Dictionary<string, Flag256> ActionEnum2Flag => _string2Flag;
		private Dictionary<string, Flag256> _string2Flag;
		private Dictionary<int, Flag256> _flagBitIndex2Flag;
		private int _bitCount = -1;
		private const int TotalBitCount = 256;
		private const int BITUnit = 32;
		
		public int category;
		public FlagHandle256(int category)
		{
		    this.category = category;
		    _string2Flag = new Dictionary<string, Flag256>();
		    _flagBitIndex2Flag = new Dictionary<int, Flag256>();
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
}
