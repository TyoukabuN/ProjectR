using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public abstract class FlagBase<FlagType> where FlagType : IBitwiseFlag<FlagType>, new()
{
    public abstract FlagType Flag { get; set; }

    protected FlagType flag = new FlagType();

#if FLAG_DEBUG
    [SerializeField]
    public string flagStr = String.Empty;
#endif

    private FlagType GetEmpty()
    {
        return flag.GetEmpty();
    }
    /// <summary>
    /// 对于现有的String类型的Tag方便的转成用Flag，需要实现一套StringToFlag的Bit位置管理
    /// 参考<see cref="MotionFlag.StringToFlag(string)"/> 
    /// 和<see cref="MotionFlag.GetFlagByBitIndex(int)"/> 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    protected abstract FlagType Internal_StringToFlag(string key);

    public virtual void Set(string flagStr)
    {
        flag = GetEmpty();
        flag = flag.FlagOr(Internal_StringToFlag(flagStr));
#if  FLAG_DEBUG
        this.flagStr = ToString();
#endif
    }
    public virtual void Set(List<string> flags)
    {
        flag = GetEmpty();
        for (int i = 0; i < flags.Count; i++)
        {
            flag = flag.FlagOr(Internal_StringToFlag(flags[i]));
        }
#if  FLAG_DEBUG
        flagStr = ToString();
#endif
    }
    public virtual void Set(string[] flags)
    {
        flag = GetEmpty();
        for (int i = 0; i < flags.Length; i++)
        {
            flag = flag.FlagOr(Internal_StringToFlag(flags[i]));
        }
#if  FLAG_DEBUG
        flagStr = ToString();
#endif
    }
    /// <summary>
    /// 包含传入的flag的所有位点
    /// </summary>
    /// <param name="flag256"></param>
    /// <returns></returns>
    public virtual bool Contains(FlagType flag256) { return Flag.HasAll(flag256); }
    public virtual bool Contains(FlagBase<FlagType> flagBase) { return Contains(flagBase.Flag); }
    public virtual bool Contains(string flagStr) { return Contains(Internal_StringToFlag(flagStr)); }
    /// <summary>
    /// 包含传入的flag的某些位点
    /// </summary>
    /// <param name="flag256"></param>
    /// <returns></returns>
    public virtual bool Overlaps(FlagType flag256) { return Flag.HasAny(flag256); }
    public virtual bool Overlaps(FlagBase<FlagType> flagBase) { return Overlaps(flagBase.Flag); }
    public virtual bool Overlaps(string flagStr) { return Overlaps(Internal_StringToFlag(flagStr)); }

    public new virtual string ToString() { return string.Empty;}

    #region edit

    /// <summary>
    /// Or传入flag
    /// </summary>
    /// <param name="flag256"></param>
    public virtual void AddFlag(FlagType flag256) { flag = Flag.FlagOr(flag256); }
    public virtual void AddFlag(string key) { AddFlag(Internal_StringToFlag(key)); }
    public virtual void AddFlags(List<string> keys)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            AddFlag(Internal_StringToFlag(keys[i]));
        }
    }

    /// <summary>
    /// Exclusive传入flag
    /// </summary>
    /// <param name="flag256"></param>
    public void RemoveFlag(FlagType flag256) { flag = Flag.FlagAnd(flag256.FlagComplement()); }
    public void RemoveFlag(string key) { RemoveFlag(Internal_StringToFlag(key)); }
    public void RemoveFlags(List<string> keys)
    {
        for (int i = 0; i < keys.Count; i++)
            RemoveFlag(Internal_StringToFlag(keys[i]));
    }
    #endregion
}

public interface IBitwiseFlag<FlagType>
{
    public FlagType GetEmpty();
    public bool HasAny(FlagType f);
    public bool HasAll(FlagType f);
    public FlagType FlagOr(FlagType f2);
    public FlagType FlagOrExclusive(FlagType f2);
    public FlagType FlagAnd(FlagType f2);
    public FlagType FlagComplement();
    public bool Equals(FlagType f2);
    public bool IsEmpty();
}
public class ProfileScope : System.IDisposable
{
    public string name = String.Empty;
    private DateTime beginStamp;

    private ProfileScope()
    {
        beginStamp = System.DateTime.Now;
    }
    public ProfileScope(string name) : this()
    {
        this.name = name;
        UnityEngine.Profiling.Profiler.BeginSample(name);
    }

    public void Dispose()
    {
        UnityEngine.Profiling.Profiler.EndSample();
    }
}

public class MemoryCostScope : IDisposable
{
    public string name = String.Empty;
    public long beginStamp;
    public MemoryCostScope(string name)
    {
        this.name = name;
        beginStamp = GC.GetTotalMemory(true);
    }
    public void Dispose()
    {
        long res = (GC.GetTotalMemory(false) - beginStamp) / 1000;
        Debug.Log($"[Cost][{name}] : [{res}] kb");
    }
}