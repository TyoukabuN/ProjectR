using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public partial class PlayerEntity : LogicEntity
    {
        public override void Init()
        {
            base.Init();

            Init_Input();
        }
    }
}