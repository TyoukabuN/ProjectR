using System;

namespace LS.Game.DataContext
{
    public interface IEffectProperty : IDisposable, ITemporary
    {
        public void Release();
    }
}