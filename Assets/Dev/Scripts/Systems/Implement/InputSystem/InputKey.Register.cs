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
            public static InputKey W = Register(KeyCategory.PlayerInput, "w");
            public static InputKey A = Register(KeyCategory.PlayerInput, "a");
            public static InputKey S = Register(KeyCategory.PlayerInput, "s");
            public static InputKey D = Register(KeyCategory.PlayerInput, "d");

            public static InputKey LeftControl = Register(KeyCategory.PlayerInput, "LeftControl");
            public static InputKey LeftShift = Register(KeyCategory.PlayerInput, "LeftShift");

            public static InputKey Space = Register(KeyCategory.PlayerInput, "Space");
        }
    }
}