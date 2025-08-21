using System;

namespace LS.Game.DataContext
{
    public interface ILogicProperty : IDisposable, ITemporary
    {
        public void Release();
    }
}