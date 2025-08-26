using System;
using Sirenix.OdinInspector;

namespace PJR.Dev.Game.DataContext
{
    public abstract partial class PropertyBlock
    {
        [Serializable]
        [LabelText("持久化配置用")]
        public class Persistent : PropertyBlock
        {
            public override bool IsTemp => false;
        }
    }
}