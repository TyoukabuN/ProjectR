using System;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public partial class InputKey
    {
        public class RegisterKeys
        {
            public static InputKey KeyADown = InputKey.Register(1, "KeyADown");
        }
    }
}