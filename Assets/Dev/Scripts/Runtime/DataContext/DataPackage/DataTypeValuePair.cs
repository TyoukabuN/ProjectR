using Sirenix.OdinInspector;
using UnityEngine;

namespace PJR.Dev.Game.DataContext
{
    public struct DataTypeValuePair
    {
        public DataType DataType => _dataType;
        public DataValue DataValue => _dataValue;
        
        [HorizontalGroup("Layout",width:160)]
        [SerializeField,HideLabel]
        private DataType _dataType;
        [HorizontalGroup("Layout")]
        [SerializeField,HideLabel]
        private DataValue _dataValue;
    }
}