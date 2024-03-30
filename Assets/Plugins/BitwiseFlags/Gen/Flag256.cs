using System;
using System.Text;
public struct Flag256 : IBitwiseFlag<Flag256>
{
	public static readonly Flag256 Empty =
		new(){
			Value0 = 0,
			Value1 = 0,
			Value2 = 0,
			Value3 = 0,
			Value4 = 0,
			Value5 = 0,
			Value6 = 0,
			Value7 = 0,
		};
	
	public uint Value0;
	public uint Value1;
	public uint Value2;
	public uint Value3;
	public uint Value4;
	public uint Value5;
	public uint Value6;
	public uint Value7;
	
	public Flag256 GetEmpty()
	{
		return Empty;
	}
	
	
	public bool HasAny(Flag256 f)
	{
		return (this & f);
	}
	
	public bool HasAll(Flag256 f)
	{
		return (this & f) == f;
	}
	
	private bool HasFlag(int pos, uint value)
	{
		if (pos == 0) return (Value0 & value) != 0;
		if (pos == 1) return (Value1 & value) != 0;
		if (pos == 2) return (Value2 & value) != 0;
		if (pos == 3) return (Value3 & value) != 0;
		if (pos == 4) return (Value4 & value) != 0;
		if (pos == 5) return (Value5 & value) != 0;
		if (pos == 6) return (Value6 & value) != 0;
		if (pos == 7) return (Value7 & value) != 0;
		return false;
	}
	
	private uint GetFlag(int pos)
	{
		if (pos == 0) return Value0;
		if (pos == 1) return Value1;
		if (pos == 2) return Value2;
		if (pos == 3) return Value3;
		if (pos == 4) return Value4;
		if (pos == 5) return Value5;
		if (pos == 6) return Value6;
		if (pos == 7) return Value7;
		return 0;
	}
	
	private void SetFlag(int pos, uint value)
	{
		if (pos == 0) Value0 = value;
		if (pos == 1) Value1 = value;
		if (pos == 2) Value2 = value;
		if (pos == 3) Value3 = value;
		if (pos == 4) Value4 = value;
		if (pos == 5) Value5 = value;
		if (pos == 6) Value6 = value;
		if (pos == 7) Value7 = value;
	}
	
	private void FlagOr(int pos, uint value)
	{
		if (pos == 0) Value0 |= value;
		if (pos == 1) Value1 |= value;
		if (pos == 2) Value2 |= value;
		if (pos == 3) Value3 |= value;
		if (pos == 4) Value4 |= value;
		if (pos == 5) Value5 |= value;
		if (pos == 6) Value6 |= value;
		if (pos == 7) Value7 |= value;
	}
	
	private void FlagAnd(int pos, uint value)
	{
		if (pos == 0) Value0 &= value;
		if (pos == 1) Value1 &= value;
		if (pos == 2) Value2 &= value;
		if (pos == 3) Value3 &= value;
		if (pos == 4) Value4 &= value;
		if (pos == 5) Value5 &= value;
		if (pos == 6) Value6 &= value;
		if (pos == 7) Value7 &= value;
	}
	
	private void FlagComplement(int pos)
	{
		if (pos == 0) Value0 = ~Value0;
		if (pos == 1) Value1 = ~Value1;
		if (pos == 2) Value2 = ~Value2;
		if (pos == 3) Value3 = ~Value3;
		if (pos == 4) Value4 = ~Value4;
		if (pos == 5) Value5 = ~Value5;
		if (pos == 6) Value6 = ~Value6;
		if (pos == 7) Value7 = ~Value7;
	}
	
	private void FlagOrExclusive(int pos, uint value)
	{
		if (pos == 0) Value0 = Value0 ^ value;
		if (pos == 1) Value1 = Value1 ^ value;
		if (pos == 2) Value2 = Value2 ^ value;
		if (pos == 3) Value3 = Value3 ^ value;
		if (pos == 4) Value4 = Value4 ^ value;
		if (pos == 5) Value5 = Value5 ^ value;
		if (pos == 6) Value6 = Value6 ^ value;
		if (pos == 7) Value7 = Value7 ^ value;
	}
	
	public Flag256 FlagOr(Flag256 f2)
	{
		FlagOr(0, f2.Value0);
		FlagOr(1, f2.Value1);
		FlagOr(2, f2.Value2);
		FlagOr(3, f2.Value3);
		FlagOr(4, f2.Value4);
		FlagOr(5, f2.Value5);
		FlagOr(6, f2.Value6);
		FlagOr(7, f2.Value7);
		return this;
	}
	
	public Flag256 FlagOrExclusive(Flag256 f2)
	{
		FlagOrExclusive(0, f2.Value0);
		FlagOrExclusive(1, f2.Value1);
		FlagOrExclusive(2, f2.Value2);
		FlagOrExclusive(3, f2.Value3);
		FlagOrExclusive(4, f2.Value4);
		FlagOrExclusive(5, f2.Value5);
		FlagOrExclusive(6, f2.Value6);
		FlagOrExclusive(7, f2.Value7);
		return this;
	}
	
	public Flag256 FlagAnd(Flag256 f2)
	{
		FlagAnd(0, f2.Value0);
		FlagAnd(1, f2.Value1);
		FlagAnd(2, f2.Value2);
		FlagAnd(3, f2.Value3);
		FlagAnd(4, f2.Value4);
		FlagAnd(5, f2.Value5);
		FlagAnd(6, f2.Value6);
		FlagAnd(7, f2.Value7);
		return this;
	}
	
	public Flag256 FlagComplement()
	{
		FlagComplement(0);
		FlagComplement(1);
		FlagComplement(2);
		FlagComplement(3);
		FlagComplement(4);
		FlagComplement(5);
		FlagComplement(6);
		FlagComplement(7);
		return this;
	}
	
	public bool Equals(Flag256 f2)
	{
		if (Value0 != f2.Value0) return false;
		if (Value1 != f2.Value1) return false;
		if (Value2 != f2.Value2) return false;
		if (Value3 != f2.Value3) return false;
		if (Value4 != f2.Value4) return false;
		if (Value5 != f2.Value5) return false;
		if (Value6 != f2.Value6) return false;
		if (Value7 != f2.Value7) return false;
		return true;
	}
	
	public static Flag256 operator | (Flag256 f1, Flag256 f2)
	{
		f1.FlagOr(0, f2.Value0);
		f1.FlagOr(1, f2.Value1);
		f1.FlagOr(2, f2.Value2);
		f1.FlagOr(3, f2.Value3);
		f1.FlagOr(4, f2.Value4);
		f1.FlagOr(5, f2.Value5);
		f1.FlagOr(6, f2.Value6);
		f1.FlagOr(7, f2.Value7);
		return f1;
	}
	
	public static Flag256 operator ^ (Flag256 f1, Flag256 f2)
	{
		f1.FlagOrExclusive(0, f2.Value0);
		f1.FlagOrExclusive(1, f2.Value1);
		f1.FlagOrExclusive(2, f2.Value2);
		f1.FlagOrExclusive(3, f2.Value3);
		f1.FlagOrExclusive(4, f2.Value4);
		f1.FlagOrExclusive(5, f2.Value5);
		f1.FlagOrExclusive(6, f2.Value6);
		f1.FlagOrExclusive(7, f2.Value7);
		return f1;
	}
	
	public static Flag256 operator & (Flag256 f1, Flag256 f2)
	{
		f1.FlagAnd(0, f2.Value0);
		f1.FlagAnd(1, f2.Value1);
		f1.FlagAnd(2, f2.Value2);
		f1.FlagAnd(3, f2.Value3);
		f1.FlagAnd(4, f2.Value4);
		f1.FlagAnd(5, f2.Value5);
		f1.FlagAnd(6, f2.Value6);
		f1.FlagAnd(7, f2.Value7);
		return f1;
	}
	
	public static Flag256 operator ~ (Flag256 f1)
	{
		f1.FlagComplement(0);
		f1.FlagComplement(1);
		f1.FlagComplement(2);
		f1.FlagComplement(3);
		f1.FlagComplement(4);
		f1.FlagComplement(5);
		f1.FlagComplement(6);
		f1.FlagComplement(7);
		return f1;
	}
	
	public static bool operator == (Flag256 f1, Flag256 f2)
	{
		if (f1.Value0 != f2.Value0) return false;
		if (f1.Value1 != f2.Value1) return false;
		if (f1.Value2 != f2.Value2) return false;
		if (f1.Value3 != f2.Value3) return false;
		if (f1.Value4 != f2.Value4) return false;
		if (f1.Value5 != f2.Value5) return false;
		if (f1.Value6 != f2.Value6) return false;
		if (f1.Value7 != f2.Value7) return false;
		return true;
	}
	
	public static bool operator != (Flag256 f1, Flag256 f2)
	{
		if (f1.Value0 != f2.Value0) return true;
		if (f1.Value1 != f2.Value1) return true;
		if (f1.Value2 != f2.Value2) return true;
		if (f1.Value3 != f2.Value3) return true;
		if (f1.Value4 != f2.Value4) return true;
		if (f1.Value5 != f2.Value5) return true;
		if (f1.Value6 != f2.Value6) return true;
		if (f1.Value7 != f2.Value7) return true;
		return false;
	}
	
	public static implicit operator bool(Flag256 f)
	{
		if (f.Value0 > 0) return true;
		if (f.Value1 > 0) return true;
		if (f.Value2 > 0) return true;
		if (f.Value3 > 0) return true;
		if (f.Value4 > 0) return true;
		if (f.Value5 > 0) return true;
		if (f.Value6 > 0) return true;
		if (f.Value7 > 0) return true;
		return false;
	}
	
	public override string ToString()
	{
		var sb = new StringBuilder();
		sb.AppendLine($"[ToString] {nameof(Flag256)}");
		sb.AppendLine(Convert.ToString(Value0, 2));
		sb.AppendLine(Convert.ToString(Value1, 2));
		sb.AppendLine(Convert.ToString(Value2, 2));
		sb.AppendLine(Convert.ToString(Value3, 2));
		sb.AppendLine(Convert.ToString(Value4, 2));
		sb.AppendLine(Convert.ToString(Value5, 2));
		sb.AppendLine(Convert.ToString(Value6, 2));
		sb.AppendLine(Convert.ToString(Value7, 2));
		return sb.ToString();
	}
	
	public bool IsEmpty()
	{
		return this == Empty;
	}
	
}
