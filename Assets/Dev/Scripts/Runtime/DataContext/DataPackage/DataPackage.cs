using System;
using System.Collections.Generic;
using PJR.ClassExtension;
using PJR.Core.Pooling;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace PJR.Dev.Game.DataContext
{
    /// <summary>
    /// DataPackage基类
    /// </summary>
    public abstract class DataPackage : IDataPackage
    {
        public static readonly string InvalidString = "无效DataPackage";
        public static readonly string NonDataString = "没Data";
        public static readonly string AttemptModifyNonTemplateString = "尝试修改非临时DataPackage";

        /// <summary>
        /// 为了区分Pre-Runtime和Runtime创建的DataPackage实例
        /// 不让Pre-Runtime创建的实例进行Release,Add之类的修改_dataMap的操作
        /// Pre-Runtime创建的实例:配置类的持久化对象
        /// </summary>
        public abstract bool IsTemp { get; }
        public Dictionary<DataType, DataValue> DataMap => _dataMap;
        public bool Valid => _dataMap != null;
        [SerializeField,ShowInInspector]
        protected Dictionary<DataType, DataValue> _dataMap;
        
        public DataValue GetData(DataType dataType)
        {
            if (!TryGetData(dataType, out var value))
                return 0f;
            return value;
        }
        public bool TryGetData(DataType dataType, out DataValue dataValue)
        {
            dataValue = 0f;
            if (!Valid)
                return false;
            if (!_dataMap.TryGetValue(dataType, out var temp))
                return false;
            dataValue = temp;
            return true;
        }
        public void Add(DataType dataType, DataValue rhs)
        {
            if (!Valid)
                return;
            if (!IsTemp)
            {
                Debug.LogError(AttemptModifyNonTemplateString);
                return;
            }
            if (_dataMap.TryGetValue(dataType, out var lhs))
                _dataMap[dataType] =  lhs.Combine(dataType, rhs);
            else
                _dataMap[dataType] = rhs;
        }
        public void Add(IDataPackage other)
        {
            if (!IsTemp)
            {
                Debug.LogError(AttemptModifyNonTemplateString);
                return;
            }
            if (!this.Valid || !other.Valid)
                return;
            if (other.DataMap == null)
                return;
            foreach (var pair in other.DataMap)
                Add(pair.Key, pair.Value);
        }
        public void Add(DataTypeValuePair pair) => Add(pair.DataType, pair.DataValue);

        public override string ToString()
        {
            if (!Valid)
                return InvalidString;
            if(_dataMap.Count <= 0)
                return NonDataString;
            using var sb = PooledStringBuilder.Get();
            sb.StringBuilder.AppendLine($"[{this.GetType().Name}.ToString] dataCount:{_dataMap.Count}");
            foreach (var pair in _dataMap)
                sb.StringBuilder.AppendLine($"[{pair.Key.GetEnumNiceName()}]: {pair.Value}");
            return sb.StringBuilder.ToString();
        }
        public abstract TempDataPackage GetTemplate();
        void IDisposable.Dispose() => Release();
        public abstract void Release();
        
#if UNITY_EDITOR
        public void Editor_Add(DataType dataType, DataValue rhs)
        {
            if (!Valid)
                return;
            if (_dataMap.TryGetValue(dataType, out var lhs))
                _dataMap[dataType] =  lhs.Combine(dataType, rhs);
            else
                _dataMap[dataType] = rhs;
        }
        public void Editor_Add(IDataPackage other)
        {
            if (!this.Valid || !other.Valid)
                return;
            if (other.DataMap == null)
                return;
            foreach (var pair in other.DataMap)
                Editor_Add(pair.Key, pair.Value);
        }
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    public class DataPackageGroup : IDataPackage
    {
        public bool Valid => _dataPackages != null;
        public bool IsTemp => true;
        
        private List<IDataPackage> _dataPackages;

        public static DataPackageGroup Get()
        {
            var temp = GenericPool<DataPackageGroup>.Get();
            temp._dataPackages ??= ListPool<IDataPackage>.Get();
            return temp;
        }

        public void Dispose()
        {
            if (_dataPackages != null)
            {
                foreach (var dataPackage in _dataPackages)
                    dataPackage?.Dispose();
                ListPool<IDataPackage>.Release(_dataPackages);
                _dataPackages = null;
            }
        }


        public Dictionary<DataType, DataValue> DataMap => null;
        public DataValue GetData(DataType dataType)
        {
            throw new NotImplementedException();
        }

        public bool TryGetData(DataType dataType, out DataValue value)
        {
            throw new NotImplementedException();
        }

        public TempDataPackage GetTemplate()
        {
            var temp = TempDataPackage.Get();
            if (!Valid)
                return temp;
            foreach (var dataPackage in _dataPackages)
            {
                if(dataPackage != null)
                    temp.Add(dataPackage); 
            }
            return temp;
        }
    }
}