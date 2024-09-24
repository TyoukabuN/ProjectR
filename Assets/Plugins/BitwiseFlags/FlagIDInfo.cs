public class FlagIDInfo
{
    public int flagID = -1;
    public string flagIDStr = string.Empty;
    /// <summary>
    /// 大类
    /// </summary>
    public int category = -1;
    /// <summary>
    /// 小类
    /// </summary>
    public int kind = -1;
    /// <summary>
    /// 类型
    /// </summary>
    public int type = -1;

    /// <summary>
    /// 大类的ID
    /// </summary>
    public int categoryID = -1;

    public FlagIDInfo() { }
    public FlagIDInfo(int flagID)
    { 
        SetID(flagID);
    }

    public bool IsValid()
    {
        if (category <= 0 && kind < 0 && type < 0)
            return false;
        return true;
    }
    //6003001
    //6/003/001
    public static FlagIDInfo GetInfo(int flagID)
    {
        return new FlagIDInfo(flagID);

    }
    private void SetID(int flagID)
    {
        this.flagID = flagID;
        if (flagID < 0)
        {
            AsInvalid();
            return;
        }
        string str = flagID.ToString();
        if (str.Length < 7)
        {
            AsInvalid();
            return;
        }
        flagIDStr = str;
        type = int.Parse(str.Substring(str.Length - 3, 3));
        kind = int.Parse(str.Substring(str.Length - 6, 3));
        category = int.Parse(str.Substring(0, str.Length - 6));

        categoryID = category * 1000000;
    }
    private void AsInvalid()
    {
        flagIDStr = string.Empty;
        type = -1;
        kind = -1;
        category = -1;
        categoryID = -1;
    }
    public static FlagIDInfo GetInfo(FlagDefine flagDefine)
    {
        return GetInfo(flagDefine.ID);
    }
    public override string ToString()
    {
        return $"[category]; {category}\n[kind]; {kind}\n[type]; {type}";
    }
}
