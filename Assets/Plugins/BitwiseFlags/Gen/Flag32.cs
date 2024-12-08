#pragma warning disable CS0660,CS0661
using System;
using System.Text;
public struct Flag32
{
	public static readonly Flag32 Empty =
		new(){
			Value0 = 0,
		};
	
	public uint Value0;
	
	public Flag32 GetEmpty()
	{
		return Empty;
	}
	
	
	public bool HasAny(Flag32 f)
	{
		return (this & f);
	}
	
	public bool HasAll(Flag32 f)
	{
		return (this & f) == f;
	}
	
	private bool HasFlag(int pos, uint value)
	{
		if (pos == 0) return (Value0 & value) != 0;
		return false;
	}
	
	private uint GetFlag(int pos)
	{
		if (pos == 0) return Value0;
		return 0;
	}
	
	private void SetFlag(int pos, uint value)
	{
		if (pos == 0) Value0 = value;
	}
	
	private void FlagOr(int pos, uint value)
	{
		if (pos == 0) Value0 |= value;
	}
	
	private void FlagAnd(int pos, uint value)
	{
		if (pos == 0) Value0 &= value;
	}
	
	private void FlagComplement(int pos)
	{
		if (pos == 0) Value0 = ~Value0;
	}
	
	private void FlagOrExclusive(int pos, uint value)
	{
		if (pos == 0) Value0 = Value0 ^ value;
	}
	
	public Flag32 FlagOr(Flag32 f2)
	{
		FlagOr(0, f2.Value0);
		return this;
	}
	
	public Flag32 FlagOrExclusive(Flag32 f2)
	{
		FlagOrExclusive(0, f2.Value0);
		return this;
	}
	
	public Flag32 FlagAnd(Flag32 f2)
	{
		FlagAnd(0, f2.Value0);
		return this;
	}
	
	public Flag32 FlagComplement()
	{
		FlagComplement(0);
		return this;
	}
	
	public bool Equals(Flag32 f2)
	{
		if (Value0 != f2.Value0) return false;
		return true;
	}
	
	public static Flag32 operator | (Flag32 f1, Flag32 f2)
	{
		f1.FlagOr(0, f2.Value0);
		return f1;
	}
	
	public static Flag32 operator ^ (Flag32 f1, Flag32 f2)
	{
		f1.FlagOrExclusive(0, f2.Value0);
		return f1;
	}
	
	public static Flag32 operator & (Flag32 f1, Flag32 f2)
	{
		f1.FlagAnd(0, f2.Value0);
		return f1;
	}
	
	public static Flag32 operator ~ (Flag32 f1)
	{
		f1.FlagComplement(0);
		return f1;
	}
	
	public static bool operator == (Flag32 f1, Flag32 f2)
	{
		if (f1.Value0 != f2.Value0) return false;
		return true;
	}
	
	public static bool operator != (Flag32 f1, Flag32 f2)
	{
		if (f1.Value0 == f2.Value0) return false;
		return true;
	}
	
	public static implicit operator bool(Flag32 f)
	{
		if (f.Value0 > 0) return true;
		return false;
	}
	
	public override string ToString()
	{
		var sb = new StringBuilder();
		sb.AppendLine($"[ToString] {nameof(Flag32)}");
		sb.AppendLine(Convert.ToString(Value0, 2));
		return sb.ToString();
	}
	
	public bool IsEmpty()
	{
		return this == Empty;
	}
}
#pragma warning restore CS0660,CS0661
