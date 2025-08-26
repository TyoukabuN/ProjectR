using System;

namespace PJR.Dev.Game.DataContext
{
    public interface IEffectProperty : IDisposable, ITemporary
    {
        public void Release();
    }
}