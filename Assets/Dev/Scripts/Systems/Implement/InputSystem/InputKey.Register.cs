using System;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public partial class InputKey
    {
        public enum KeyCategory
        {
            PlayerInput = 0,
        }
        public class RegisterKeys
        {
            public static InputKey KeyW = Register(KeyCategory.PlayerInput, "KeyW");
            public static InputKey KeyA = Register(KeyCategory.PlayerInput, "KeyA");
            public static InputKey KeyS = Register(KeyCategory.PlayerInput, "KeyS");
            public static InputKey KeyD = Register(KeyCategory.PlayerInput, "KeyD");
        }
    }
}