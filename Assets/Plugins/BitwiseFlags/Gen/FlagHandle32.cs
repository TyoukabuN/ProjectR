using System.Collections.Generic;
namespace PJR
{
	public class FlagHandle32
	{
		public Dictionary<string, Flag32> ActionEnum2Flag => _string2Flag;
		private Dictionary<string, Flag32> _string2Flag;
		private Dictionary<int, Flag32> _flagBitIndex2Flag;
		private int _bitCount = -1;
		private const int TotalBitCount = 32;
		private const int BITUnit = 32;
		
		public int category;
		public FlagHandle32(int category)
		{
		    this.category = category;
		    _string2Flag = new Dictionary<string, Flag32>();
		    _flagBitIndex2Flag = new Dictionary<int, Flag32>();
		}
		public Flag32 StringToFlag(List<string> keys) => StringToFlag(keys.ToArray());
		public Flag32 StringToFlag(params string[] keys)
		{
		    var temp = Flag32.Empty;
		    for (int i = 0; i < keys.Length; i++)
		        temp |= StringToFlag(keys[i]);
		    return temp;
		}
		public Flag32 StringToFlag(string key)
		{
		    var flag = Flag32.Empty;
		    int targetBitIndex = _bitCount + 1;
		    if (targetBitIndex > TotalBitCount)
		    {
		        targetBitIndex = -1;
		        return flag;
		    }
		
		    _string2Flag ??= new Dictionary<string, Flag32>(TotalBitCount);
		
		    if (string.IsNullOrEmpty(key))
		    {
		        targetBitIndex = -1;
		        return Flag32.Empty;
		    }
		
		    if (!_string2Flag.TryGetValue(key, out flag))
		    {
		        flag = GetFlagByBitIndex(targetBitIndex);
		        _string2Flag[key] = flag;
		        _bitCount++;
		    }
		    return flag;
		}
		private Flag32 GetFlagByBitIndex(int flagBitIndex)
		{
		    _flagBitIndex2Flag ??= new Dictionary<int, Flag32>(TotalBitCount);
		    if (_flagBitIndex2Flag.TryGetValue(flagBitIndex, out Flag32 res))
		        return res;
		    int pos = flagBitIndex / BITUnit;
		    var temp = Flag32.Empty;
			if (pos == 0) temp.Value0 = (uint)(1 << flagBitIndex);
		    _flagBitIndex2Flag[flagBitIndex] = temp;
		    return temp;
		}
	}
}
