using System;

namespace LS.Game.DataContext
{
    public interface IPropertyBlock : ITemporary, IDisposable
    {
        public float GetFloat(int index);
        public int GetInt(int index);
        public bool GetBool(int index);
        public bool SetFloat(int index, float value, bool force = false);
        public bool SetInt(int index, int value, bool force = false);
        public bool SetBool(int index, bool value, bool force = false);

    }
}