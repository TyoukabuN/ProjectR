using System;

namespace PJR
{
    public abstract class EntityStateMachine
    {
        public abstract Type StateType { get; }
        public abstract int CurrentEState { get; }
        public abstract void Init();
        public abstract void Update(float deltaTime);
        public abstract bool State_Change(int ePlayerState);
        public abstract void UpdateContext();
    }
}
