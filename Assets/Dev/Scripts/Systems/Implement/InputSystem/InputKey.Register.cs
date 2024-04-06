using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PJR.Input
{
    public enum KeyCategory
    {
        PlayerInput = 0,
    }
    public class RegisterKeys
    {
        public static InputKey Move = InputKey.Register(KeyCategory.PlayerInput, "Move");
        public static InputKey Run = InputKey.Register(KeyCategory.PlayerInput, "Run");
        public static InputKey Jump = InputKey.Register(KeyCategory.PlayerInput, "Jump");
        /// <summary>
        /// ������Ϊ�˳�ʼ��static field
        /// </summary>
        public static void Init() { }
    }
}