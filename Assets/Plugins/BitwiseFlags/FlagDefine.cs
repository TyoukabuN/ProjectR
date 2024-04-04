using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PJR
{
    public enum FlagCategory : int
    {
        Input = 0,
    }
    public static class FlagDefine
    {
        public static FlagHandle256 InputFlag = new FlagHandle256((int)FlagCategory.Input);
    }
}
