using System;
using System.Collections.Generic;

namespace PJR.Dev.Game.DataContext
{
    public interface IDataPackage :IDisposable
    {
        public bool Valid { get; }
        public bool IsTemp { get; } 
        public Dictionary<DataType, DataValue> DataMap { get; }
        public DataValue GetData(DataType dataType);
        public bool TryGetData(DataType dataType, out DataValue value);
        public TempDataPackage GetTemplate();
    }
}