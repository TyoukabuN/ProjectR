using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public abstract class EntityStateMachine
    {
        public abstract int CurrentState { get; }

        public abstract void Init();
        public abstract void Update();
        public abstract bool State_Change(int ePlayerState);

        public abstract void UpdateContext();

    }
}
