using System;
using System.Collections.Generic;
using UnityEditor;
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
            public static InputKey Move = Register(KeyCategory.PlayerInput, "Move");
            public static InputKey Run = Register(KeyCategory.PlayerInput, "Run");
            public static InputKey Jump = Register(KeyCategory.PlayerInput, "Jump");
            public static void Init() { }
        }
    }
}