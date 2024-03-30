using UnityEngine;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;

[Serializable]
public class MotionFlag : FlagBase<Flag256>
{
    [NonSerialized]
    private bool _init = false;
    public override Flag256 Flag
    {
        get
        {
            if (!_init)
            {
                if (InitByCustomConfig())
                {
                }
                else
                    OnFindNotConfig();
                 

                _init = true;
            }

            return flag;
        }
        set
        {
            flag = value;
        }
    }

    public MotionFlag()
    {
        flag = Flag256.Empty;
    }
    public MotionFlag(string flagStr) : this() { Set(flagStr); }
    public MotionFlag(List<string> flags) : this() { Set(flags); }
    public MotionFlag(string[] flags) : this() { Set(flags); }
    protected virtual void OnFindNotConfig() {  /*flag = Flag256.Empty;*/ }
    protected virtual bool InitByCustomConfig() { return false; }
    public virtual bool AnyCustomConfig => true;

    /// <summary>
    /// 传入的flag是否包含this的flag的某些位点
    /// </summary>
    /// <param name="motionFlag"></param>
    /// <returns></returns>
    public bool ContainsAny(MotionFlag motionFlag)
    {
        if (Flag.IsEmpty())
            return false;
        return motionFlag.Flag.HasAny(Flag);
    }

    public bool ContainsAll(MotionFlag motionFlag)
    {
        if (Flag.IsEmpty())
            return false;
        if (motionFlag.Flag.IsEmpty())
            Debug.LogError("motionFlag.Flag.IsEmpty");
        return Flag.HasAll(motionFlag.Flag);
    }
    public bool Contains(MotionFlag motionFlag) { return Contains(motionFlag.Flag); }
    public bool Overlaps(MotionFlag motionFlag) { return Overlaps(motionFlag.Flag); }
    public void AddFlag(MotionFlag motionFlag) { AddFlag(motionFlag.Flag); }
    public void RemoveFlag(MotionFlag motionFlag) { RemoveFlag(motionFlag.Flag); }

    public override string ToString()
    {
        if (_actionEnum2Flag == null)
            return Flag.ToString();
        var sb = new System.Text.StringBuilder();
        foreach (var pair in _actionEnum2Flag)
        {
            if (flag.HasAny(pair.Value))
                sb.Append($"{pair.Key} \\ ");
        }
        return sb.ToString();
    }

    #region Static functions
    public static implicit operator Flag256(MotionFlag motionFlag)
    {
        if (motionFlag == null)
            return Flag256.Empty;
        return motionFlag.Flag;
    }

    protected override Flag256 Internal_StringToFlag(string key)
    {
        return StringToFlag(key);
    }
    public static Flag256 StringToFlag(params string[] keys)
    {
        var temp = Flag256.Empty;
        for (int i = 0; i < keys.Length; i++)
            temp |= StringToFlag(keys[i]);
        return temp;
    }
    public static Flag256 StringToFlag(List<string> keys)
    {
        var temp = Flag256.Empty;
        for (int i = 0; i < keys.Count; i++)
            temp |= StringToFlag(keys[i]);
        return temp;
    }
    #endregion

    #region Bit位置管理
    public static Dictionary<string, Flag256> ActionEnum2Flag => _actionEnum2Flag;
    private static Dictionary<string, Flag256> _actionEnum2Flag;
    private static Dictionary<int, Flag256> _flagBitIndex2Flag;
    private static int _bitCount = -1;
    private const int TotalBitCount = 256;
    private const int BITUnit = 32;
    /// <summary>
    /// 申请分配好唯一bit位置的flag
    /// </summary>
    /// <param name="key"></param>
    /// <param name="targetBitIndex"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static Flag256 StringToFlag(string key)
    {
        var flag = Flag256.Empty;
        int targetBitIndex = _bitCount + 1;
        if (targetBitIndex > TotalBitCount)
        {
            Debug.LogError($"_bitCount > totalBitCount({TotalBitCount}) ");
            targetBitIndex = -1;
            return flag;
        }

        _actionEnum2Flag ??= new Dictionary<string, Flag256>(TotalBitCount);

        if (string.IsNullOrEmpty(key))
        {
            targetBitIndex = -1;
            return Flag256.Empty;
        }

        if (!_actionEnum2Flag.TryGetValue(key, out flag))
        {
            flag = GetFlagByBitIndex(targetBitIndex);
            _actionEnum2Flag[key] = flag;
            _bitCount++;
        }
        return flag;
    }

    /// <summary>
    /// 返回对应bit位置为1的flag
    /// </summary>
    /// <param name="flagBitIndex"></param>
    /// <returns></returns>
    private static Flag256 GetFlagByBitIndex(int flagBitIndex)
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
    #endregion
}