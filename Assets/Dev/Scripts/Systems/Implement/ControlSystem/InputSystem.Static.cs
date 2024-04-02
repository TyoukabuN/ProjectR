using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public partial class InputSystem : MonoSingletonSystem<InputSystem>
    {
        public static void Tick()
        {
            instance.OnUpdate();
        }
    }
}
