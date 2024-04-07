namespace PJR.ScriptStates
{
    public abstract class ScriptState
    {
        public enum Phase
        {
            Running,
            End,
        }
        public Phase phase;
        public ScriptState() { OnInit(); }
        public virtual bool CanChange(int from) { return true; }
        public virtual float normalizeTime { get { return 1f; } }
        #region Phase
        protected void ToPhaseEnd() { phase = Phase.End; }
        public virtual void OnInit() { }
        public virtual void OnEnter(LogicEntity entity) { phase = Phase.Running;}
        public virtual void OnUpdate() { }
        public virtual void OnChange(int from) { }
        public virtual void OnFinish() { }
        #endregion
    }
}
