using System;

namespace PJR.Core
{
    public interface IVariableBuffer
    {
        public Type VariableType { get; }
        public bool ClearBuffer(int index, uint guid);
    }
}