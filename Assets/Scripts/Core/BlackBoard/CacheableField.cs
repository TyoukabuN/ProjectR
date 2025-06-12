using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using PJR.BlackBoard.CachedValueBoard;
using PJR.BlackBoard.Inspector;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.Drawers;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.Serialization;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace PJR.BlackBoard.CachedValueBoard
{
    [Serializable]
    public class CacheableField<T> : CacheableValue<T>
    {
        public bool UsingLocalValue =>  (EGetValueApproach)_valueGainApproach == EGetValueApproach.LocalValue;
        public bool UsingValueFromBoard =>  (EGetValueApproach)_valueGainApproach == EGetValueApproach.FromBoard;
        //public bool UsingValueByEvaluation =>  (EGetValueApproach)_valueGainApproach == EGetValueApproach.ByEvaluation;
        public bool AnyBoardValueReference => _boardValueReference != null;
        

        private CacheableValueBoard _boardReference = null;
        public CacheableValueBoard BoardReference => _boardReference;

        [HorizontalGroup("Value",width:20)]
        [GetValueApproach]
        [SerializeField]
        private int _valueGainApproach = 0;
        
        [HorizontalGroup("Value/LocalValue"),] 
        [SerializeField] 
        [HideLabel]
        [ShowIf("UsingLocalValue")]
        private T _localValue;

        [HorizontalGroup("Value/BoardValue"),] 
        [SerializeField] 
        [HideLabel]
        [ShowIf("ShowBoardValueReference")]
        [BoardValueReference]
        private BoardValueReference _boardValueReference;

        public void WriteFromBoard(CacheableValueBoard board)
        {
        }

        public static IEnumerable<Type> GetFiTypeFilter()
        {
            yield return typeof(CacheableField<>).MakeGenericType(typeof(T));
        }
        
#if UNITY_EDITOR
        public bool ShowBoardValueReference => UsingValueFromBoard && AnyBoardValueReference;
        public bool ShowBoardValueGainButton => UsingValueFromBoard && !AnyBoardValueReference;
        
        [HorizontalGroup("Value/BoardValue")] 
        [Button("获取黑板值")]
        [ShowIf("ShowBoardValueGainButton")]
        public static void Editor_FindBoard()
        {
            
        }
#endif
    }
}

