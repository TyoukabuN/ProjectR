using PJR.Systems.Input;
using PJR.Systems;
using UnityEngine;
using PJR.LogicUnits;

namespace PJR
{
    public partial class PlayerEntity
    {
        public EntityInputLogicUnit inputLogicUnit;
        public EntityInputLogicUnit InputLogicUnit => inputLogicUnit;
        protected virtual void Init_Input()
        {
            inputLogicUnit = AddLogicUnit<EntityInputLogicUnit>();
        }
    }
}
