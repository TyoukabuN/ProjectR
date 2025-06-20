using System;
using System.Collections.Generic;
using UnityEngine;

namespace PJR.Core
{
    /// <summary>
    /// 提供诸如类型转VariableBuffer<T>,间接释放VariableBuffer的等服务, 
    /// </summary>
    public static class GenericBufferCenter
    {
        private static Dictionary<Type, IVariableBuffer> type2VariableBuffer;
        public static Dictionary<Type, IVariableBuffer> Type2VariableBuffer => type2VariableBuffer??= new Dictionary<Type, IVariableBuffer>(64);
        public static bool Register(IVariableBuffer variableBuffer)
        {
            if (variableBuffer == null)
                return false;
            if (Type2VariableBuffer.TryGetValue(variableBuffer.VariableType, out var existOne))
            {
                if (variableBuffer == existOne)
                {
                    Debug.LogWarning($"重复注册IVariableBuffer:[VariableBuffer<{variableBuffer.VariableType.Name}>]");
                    return false;
                }
                else
                {
                    Debug.LogWarning($"已存在注册的相同类型的IVariableBuffer:[VariableBuffer<{variableBuffer.VariableType.Name}>]");
                    Type2VariableBuffer[variableBuffer.VariableType] = variableBuffer;
                    return true;
                }
            }

            Type2VariableBuffer[variableBuffer.VariableType] = variableBuffer;
            return true;
        }

        public static bool TryGetBuffer<T>(Type bufferVariableTypef, out GenericBuffer<T> genericBuffer)
        {
            genericBuffer = null;
            if (!Type2VariableBuffer.TryGetValue(bufferVariableTypef, out var temp))
                return false;
            genericBuffer = temp as GenericBuffer<T>;
            return true;
        }
        
        public static bool TryClearBuffer(Type bufferVariableTypef, int index, uint guid)
        {
            if (!Type2VariableBuffer.TryGetValue(bufferVariableTypef, out var variableBuffer))
                return false;
            return variableBuffer.ClearBuffer(index, guid);
        }
    }
}