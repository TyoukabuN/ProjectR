using Microsoft.CodeAnalysis.CSharp.Syntax;

public abstract class IFlag<FlagType> where FlagType : struct
{
	public static IFlag<FlagType> Empty { get; }
	public abstract bool HasAny(IFlag<FlagType> f);
	public abstract bool HasAll(IFlag<FlagType> f);
	protected abstract bool HasFlag(int pos, uint value);
	protected abstract uint GetFlag(int pos);
	protected abstract void SetFlag(int pos, uint value);
	protected abstract void FlagOr(int pos, uint value);
	protected abstract void FlagAnd(int pos, uint value);
	protected abstract void FlagComplement(int pos);
	protected abstract void FlagOrExclusive(int pos, uint value);
	public static IFlag<FlagType> operator |(IFlag<FlagType> f1, IFlag<FlagType> f2) { return Empty; }
	public static IFlag<FlagType> operator ^(IFlag<FlagType> f1, IFlag<FlagType> f2) { return Empty; }
	public static IFlag<FlagType> operator &(IFlag<FlagType> f1, IFlag<FlagType> f2) { return Empty; }
	public static IFlag<FlagType> operator ~(IFlag<FlagType> f1) { return Empty; }
	public static bool operator ==(IFlag<FlagType> f1, IFlag<FlagType> f2) { return false; }
	public static bool operator !=(IFlag<FlagType> f1, IFlag<FlagType> f2) { return false; }
	public static implicit operator bool(IFlag<FlagType> f) { return false; }
	public abstract string ToString();
}