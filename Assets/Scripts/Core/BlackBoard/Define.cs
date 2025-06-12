using System;
using System.Diagnostics;
using PJR.BlackBoard.Inspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace PJR.BlackBoard
{
    public enum EGetValueApproach
    {
        LocalValue = 0,
        FromBoard,
        ByEvaluation
    }
}

namespace PJR.BlackBoard.CachedValueBoard
{
    public class BoardValueReference
    {
        public CacheableValueBoard Board;
        public string Key;   
    }
}

namespace PJR.BlackBoard.Inspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public class BoardValueReferenceAttribute : Attribute
    {
    }
}

