using System.Collections.Generic;

namespace PJR.Dev.Game.DataContext
{
    /// <summary>
    /// 定义了每个DataType是怎样合并的
    /// 因为IDataPackage很有合并的可能性
    /// </summary>
    public static class DataTypeCombineStrategy
    {
        /// <summary>
        /// 默认的DataType的合并策略,加法
        /// </summary>
        public static ICombineStrategy DefaultDataTypeCombineStrategy => new DefaultCombineStrategy(ECombineStrategyType.Add);

        /// <summary>
        /// 自定定义DataType合并策略Map
        /// </summary>
        public static Dictionary<DataType, ICombineStrategy> CustomDataTypeCombineStrategy = new()
        {
        };
        
        /// <summary>
        /// 合并策略接口,方便自定义
        /// </summary>
        public interface ICombineStrategy
        {
            public DataValue Combine(DataValue lhs, DataValue rhs);
        }

        public struct DefaultCombineStrategy : ICombineStrategy
        {
            private ECombineStrategyType _combineStrategyType;
            public DefaultCombineStrategy(ECombineStrategyType combineStrategyType)
            {
                _combineStrategyType = combineStrategyType;
            }
            public DataValue Combine(DataValue lhs, DataValue rhs)
                => DefaultDataValueCombineFunc(_combineStrategyType, lhs, rhs);
        }

        public enum ECombineStrategyType
        {
            Add = 0, //Default
            Subtract,
            Multiply,
            Divide,
            //把后面搬一个shader里alphablend OP那套过来,OneMinus之类的
            //
            Fixed,
            Override,
            //
            Custom,
        }

        public static DataValue DataValueCombineFunc(DataType dataType, DataValue lhs, DataValue rhs)
        {
            if (CustomDataTypeCombineStrategy.TryGetValue(dataType, out ICombineStrategy strategy))
                return strategy.Combine(lhs, rhs);
            return DefaultDataTypeCombineStrategy.Combine(lhs, rhs);
        }
        
        public static DataValue DefaultDataValueCombineFunc(ECombineStrategyType combineStrategyType,DataValue lhs, DataValue rhs)
        {
            if (!lhs.IsValueTypeMatch(rhs))
                return lhs;
            if (combineStrategyType == ECombineStrategyType.Fixed)
                return lhs;
            if (combineStrategyType == ECombineStrategyType.Override)
                return rhs;
            if (combineStrategyType == ECombineStrategyType.Add)
                return lhs + rhs;
            if (combineStrategyType == ECombineStrategyType.Subtract)
                return lhs - rhs;
            if (combineStrategyType == ECombineStrategyType.Multiply)
                return lhs * rhs;
            if (combineStrategyType == ECombineStrategyType.Divide)
                return lhs / rhs;
            return lhs;
        }
        public static DataValue Combine(this DataValue lhs, DataType dataType, DataValue rhs)
            => DataValueCombineFunc(dataType, lhs, rhs);
    }
}