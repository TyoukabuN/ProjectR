using System;
using System.Text;
public struct Flag512 : IBitwiseFlag<Flag512>
{
	public static readonly Flag512 Empty =
		new(){
			Value0 = 0,
			Value1 = 0,
			Value2 = 0,
			Value3 = 0,
			Value4 = 0,
			Value5 = 0,
			Value6 = 0,
			Value7 = 0,
			Value8 = 0,
			Value9 = 0,
			Value10 = 0,
			Value11 = 0,
			Value12 = 0,
			Value13 = 0,
			Value14 = 0,
			Value15 = 0,
		};
	
	public uint Value0;
	public uint Value1;
	public uint Value2;
	public uint Value3;
	public uint Value4;
	public uint Value5;
	public uint Value6;
	public uint Value7;
	public uint Value8;
	public uint Value9;
	public uint Value10;
	public uint Value11;
	public uint Value12;
	public uint Value13;
	public uint Value14;
	public uint Value15;
	
	public Flag512 GetEmpty()
	{
		return Empty;
	}
	
	
	public bool HasAny(Flag512 f)
	{
		return (this & f);
	}
	
	public bool HasAll(Flag512 f)
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
		if (pos == 8) return (Value8 & value) != 0;
		if (pos == 9) return (Value9 & value) != 0;
		if (pos == 10) return (Value10 & value) != 0;
		if (pos == 11) return (Value11 & value) != 0;
		if (pos == 12) return (Value12 & value) != 0;
		if (pos == 13) return (Value13 & value) != 0;
		if (pos == 14) return (Value14 & value) != 0;
		if (pos == 15) return (Value15 & value) != 0;
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
		if (pos == 8) return Value8;
		if (pos == 9) return Value9;
		if (pos == 10) return Value10;
		if (pos == 11) return Value11;
		if (pos == 12) return Value12;
		if (pos == 13) return Value13;
		if (pos == 14) return Value14;
		if (pos == 15) return Value15;
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
		if (pos == 8) Value8 = value;
		if (pos == 9) Value9 = value;
		if (pos == 10) Value10 = value;
		if (pos == 11) Value11 = value;
		if (pos == 12) Value12 = value;
		if (pos == 13) Value13 = value;
		if (pos == 14) Value14 = value;
		if (pos == 15) Value15 = value;
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
		if (pos == 8) Value8 |= value;
		if (pos == 9) Value9 |= value;
		if (pos == 10) Value10 |= value;
		if (pos == 11) Value11 |= value;
		if (pos == 12) Value12 |= value;
		if (pos == 13) Value13 |= value;
		if (pos == 14) Value14 |= value;
		if (pos == 15) Value15 |= value;
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
		if (pos == 8) Value8 &= value;
		if (pos == 9) Value9 &= value;
		if (pos == 10) Value10 &= value;
		if (pos == 11) Value11 &= value;
		if (pos == 12) Value12 &= value;
		if (pos == 13) Value13 &= value;
		if (pos == 14) Value14 &= value;
		if (pos == 15) Value15 &= value;
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
		if (pos == 8) Value8 = ~Value8;
		if (pos == 9) Value9 = ~Value9;
		if (pos == 10) Value10 = ~Value10;
		if (pos == 11) Value11 = ~Value11;
		if (pos == 12) Value12 = ~Value12;
		if (pos == 13) Value13 = ~Value13;
		if (pos == 14) Value14 = ~Value14;
		if (pos == 15) Value15 = ~Value15;
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
		if (pos == 8) Value8 = Value8 ^ value;
		if (pos == 9) Value9 = Value9 ^ value;
		if (pos == 10) Value10 = Value10 ^ value;
		if (pos == 11) Value11 = Value11 ^ value;
		if (pos == 12) Value12 = Value12 ^ value;
		if (pos == 13) Value13 = Value13 ^ value;
		if (pos == 14) Value14 = Value14 ^ value;
		if (pos == 15) Value15 = Value15 ^ value;
	}
	
	public Flag512 FlagOr(Flag512 f2)
	{
		FlagOr(0, f2.Value0);
		FlagOr(1, f2.Value1);
		FlagOr(2, f2.Value2);
		FlagOr(3, f2.Value3);
		FlagOr(4, f2.Value4);
		FlagOr(5, f2.Value5);
		FlagOr(6, f2.Value6);
		FlagOr(7, f2.Value7);
		FlagOr(8, f2.Value8);
		FlagOr(9, f2.Value9);
		FlagOr(10, f2.Value10);
		FlagOr(11, f2.Value11);
		FlagOr(12, f2.Value12);
		FlagOr(13, f2.Value13);
		FlagOr(14, f2.Value14);
		FlagOr(15, f2.Value15);
		return this;
	}
	
	public Flag512 FlagOrExclusive(Flag512 f2)
	{
		FlagOrExclusive(0, f2.Value0);
		FlagOrExclusive(1, f2.Value1);
		FlagOrExclusive(2, f2.Value2);
		FlagOrExclusive(3, f2.Value3);
		FlagOrExclusive(4, f2.Value4);
		FlagOrExclusive(5, f2.Value5);
		FlagOrExclusive(6, f2.Value6);
		FlagOrExclusive(7, f2.Value7);
		FlagOrExclusive(8, f2.Value8);
		FlagOrExclusive(9, f2.Value9);
		FlagOrExclusive(10, f2.Value10);
		FlagOrExclusive(11, f2.Value11);
		FlagOrExclusive(12, f2.Value12);
		FlagOrExclusive(13, f2.Value13);
		FlagOrExclusive(14, f2.Value14);
		FlagOrExclusive(15, f2.Value15);
		return this;
	}
	
	public Flag512 FlagAnd(Flag512 f2)
	{
		FlagAnd(0, f2.Value0);
		FlagAnd(1, f2.Value1);
		FlagAnd(2, f2.Value2);
		FlagAnd(3, f2.Value3);
		FlagAnd(4, f2.Value4);
		FlagAnd(5, f2.Value5);
		FlagAnd(6, f2.Value6);
		FlagAnd(7, f2.Value7);
		FlagAnd(8, f2.Value8);
		FlagAnd(9, f2.Value9);
		FlagAnd(10, f2.Value10);
		FlagAnd(11, f2.Value11);
		FlagAnd(12, f2.Value12);
		FlagAnd(13, f2.Value13);
		FlagAnd(14, f2.Value14);
		FlagAnd(15, f2.Value15);
		return this;
	}
	
	public Flag512 FlagComplement()
	{
		FlagComplement(0);
		FlagComplement(1);
		FlagComplement(2);
		FlagComplement(3);
		FlagComplement(4);
		FlagComplement(5);
		FlagComplement(6);
		FlagComplement(7);
		FlagComplement(8);
		FlagComplement(9);
		FlagComplement(10);
		FlagComplement(11);
		FlagComplement(12);
		FlagComplement(13);
		FlagComplement(14);
		FlagComplement(15);
		return this;
	}
	
	public bool Equals(Flag512 f2)
	{
		if (Value0 != f2.Value0) return false;
		if (Value1 != f2.Value1) return false;
		if (Value2 != f2.Value2) return false;
		if (Value3 != f2.Value3) return false;
		if (Value4 != f2.Value4) return false;
		if (Value5 != f2.Value5) return false;
		if (Value6 != f2.Value6) return false;
		if (Value7 != f2.Value7) return false;
		if (Value8 != f2.Value8) return false;
		if (Value9 != f2.Value9) return false;
		if (Value10 != f2.Value10) return false;
		if (Value11 != f2.Value11) return false;
		if (Value12 != f2.Value12) return false;
		if (Value13 != f2.Value13) return false;
		if (Value14 != f2.Value14) return false;
		if (Value15 != f2.Value15) return false;
		return true;
	}
	
	public static Flag512 operator | (Flag512 f1, Flag512 f2)
	{
		f1.FlagOr(0, f2.Value0);
		f1.FlagOr(1, f2.Value1);
		f1.FlagOr(2, f2.Value2);
		f1.FlagOr(3, f2.Value3);
		f1.FlagOr(4, f2.Value4);
		f1.FlagOr(5, f2.Value5);
		f1.FlagOr(6, f2.Value6);
		f1.FlagOr(7, f2.Value7);
		f1.FlagOr(8, f2.Value8);
		f1.FlagOr(9, f2.Value9);
		f1.FlagOr(10, f2.Value10);
		f1.FlagOr(11, f2.Value11);
		f1.FlagOr(12, f2.Value12);
		f1.FlagOr(13, f2.Value13);
		f1.FlagOr(14, f2.Value14);
		f1.FlagOr(15, f2.Value15);
		return f1;
	}
	
	public static Flag512 operator ^ (Flag512 f1, Flag512 f2)
	{
		f1.FlagOrExclusive(0, f2.Value0);
		f1.FlagOrExclusive(1, f2.Value1);
		f1.FlagOrExclusive(2, f2.Value2);
		f1.FlagOrExclusive(3, f2.Value3);
		f1.FlagOrExclusive(4, f2.Value4);
		f1.FlagOrExclusive(5, f2.Value5);
		f1.FlagOrExclusive(6, f2.Value6);
		f1.FlagOrExclusive(7, f2.Value7);
		f1.FlagOrExclusive(8, f2.Value8);
		f1.FlagOrExclusive(9, f2.Value9);
		f1.FlagOrExclusive(10, f2.Value10);
		f1.FlagOrExclusive(11, f2.Value11);
		f1.FlagOrExclusive(12, f2.Value12);
		f1.FlagOrExclusive(13, f2.Value13);
		f1.FlagOrExclusive(14, f2.Value14);
		f1.FlagOrExclusive(15, f2.Value15);
		return f1;
	}
	
	public static Flag512 operator & (Flag512 f1, Flag512 f2)
	{
		f1.FlagAnd(0, f2.Value0);
		f1.FlagAnd(1, f2.Value1);
		f1.FlagAnd(2, f2.Value2);
		f1.FlagAnd(3, f2.Value3);
		f1.FlagAnd(4, f2.Value4);
		f1.FlagAnd(5, f2.Value5);
		f1.FlagAnd(6, f2.Value6);
		f1.FlagAnd(7, f2.Value7);
		f1.FlagAnd(8, f2.Value8);
		f1.FlagAnd(9, f2.Value9);
		f1.FlagAnd(10, f2.Value10);
		f1.FlagAnd(11, f2.Value11);
		f1.FlagAnd(12, f2.Value12);
		f1.FlagAnd(13, f2.Value13);
		f1.FlagAnd(14, f2.Value14);
		f1.FlagAnd(15, f2.Value15);
		return f1;
	}
	
	public static Flag512 operator ~ (Flag512 f1)
	{
		f1.FlagComplement(0);
		f1.FlagComplement(1);
		f1.FlagComplement(2);
		f1.FlagComplement(3);
		f1.FlagComplement(4);
		f1.FlagComplement(5);
		f1.FlagComplement(6);
		f1.FlagComplement(7);
		f1.FlagComplement(8);
		f1.FlagComplement(9);
		f1.FlagComplement(10);
		f1.FlagComplement(11);
		f1.FlagComplement(12);
		f1.FlagComplement(13);
		f1.FlagComplement(14);
		f1.FlagComplement(15);
		return f1;
	}
	
	public static bool operator == (Flag512 f1, Flag512 f2)
	{
		if (f1.Value0 != f2.Value0) return false;
		if (f1.Value1 != f2.Value1) return false;
		if (f1.Value2 != f2.Value2) return false;
		if (f1.Value3 != f2.Value3) return false;
		if (f1.Value4 != f2.Value4) return false;
		if (f1.Value5 != f2.Value5) return false;
		if (f1.Value6 != f2.Value6) return false;
		if (f1.Value7 != f2.Value7) return false;
		if (f1.Value8 != f2.Value8) return false;
		if (f1.Value9 != f2.Value9) return false;
		if (f1.Value10 != f2.Value10) return false;
		if (f1.Value11 != f2.Value11) return false;
		if (f1.Value12 != f2.Value12) return false;
		if (f1.Value13 != f2.Value13) return false;
		if (f1.Value14 != f2.Value14) return false;
		if (f1.Value15 != f2.Value15) return false;
		return true;
	}
	
	public static bool operator != (Flag512 f1, Flag512 f2)
	{
		if (f1.Value0 != f2.Value0) return true;
		if (f1.Value1 != f2.Value1) return true;
		if (f1.Value2 != f2.Value2) return true;
		if (f1.Value3 != f2.Value3) return true;
		if (f1.Value4 != f2.Value4) return true;
		if (f1.Value5 != f2.Value5) return true;
		if (f1.Value6 != f2.Value6) return true;
		if (f1.Value7 != f2.Value7) return true;
		if (f1.Value8 != f2.Value8) return true;
		if (f1.Value9 != f2.Value9) return true;
		if (f1.Value10 != f2.Value10) return true;
		if (f1.Value11 != f2.Value11) return true;
		if (f1.Value12 != f2.Value12) return true;
		if (f1.Value13 != f2.Value13) return true;
		if (f1.Value14 != f2.Value14) return true;
		if (f1.Value15 != f2.Value15) return true;
		return false;
	}
	
	public static implicit operator bool(Flag512 f)
	{
		if (f.Value0 > 0) return true;
		if (f.Value1 > 0) return true;
		if (f.Value2 > 0) return true;
		if (f.Value3 > 0) return true;
		if (f.Value4 > 0) return true;
		if (f.Value5 > 0) return true;
		if (f.Value6 > 0) return true;
		if (f.Value7 > 0) return true;
		if (f.Value8 > 0) return true;
		if (f.Value9 > 0) return true;
		if (f.Value10 > 0) return true;
		if (f.Value11 > 0) return true;
		if (f.Value12 > 0) return true;
		if (f.Value13 > 0) return true;
		if (f.Value14 > 0) return true;
		if (f.Value15 > 0) return true;
		return false;
	}
	
	public override string ToString()
	{
		var sb = new StringBuilder();
		sb.AppendLine($"[ToString] {nameof(Flag512)}");
		sb.AppendLine(Convert.ToString(Value0, 2));
		sb.AppendLine(Convert.ToString(Value1, 2));
		sb.AppendLine(Convert.ToString(Value2, 2));
		sb.AppendLine(Convert.ToString(Value3, 2));
		sb.AppendLine(Convert.ToString(Value4, 2));
		sb.AppendLine(Convert.ToString(Value5, 2));
		sb.AppendLine(Convert.ToString(Value6, 2));
		sb.AppendLine(Convert.ToString(Value7, 2));
		sb.AppendLine(Convert.ToString(Value8, 2));
		sb.AppendLine(Convert.ToString(Value9, 2));
		sb.AppendLine(Convert.ToString(Value10, 2));
		sb.AppendLine(Convert.ToString(Value11, 2));
		sb.AppendLine(Convert.ToString(Value12, 2));
		sb.AppendLine(Convert.ToString(Value13, 2));
		sb.AppendLine(Convert.ToString(Value14, 2));
		sb.AppendLine(Convert.ToString(Value15, 2));
		return sb.ToString();
	}
	
	public bool IsEmpty()
	{
		return this == Empty;
	}
	
}
