using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public static class EntityAnimationDefine
    {
        public struct DirectionNameSet { public string F, FL, FR, B, BL, BR; }

        public static class AnimationName
        {
            public const string Idle = "Idle";

            public const string Jump = "Jump";
            public const string Jump_Air_Start = "Jump_Air_Start";
            public const string Jump_Start = "Jump_Start";
            public const string Jump_Fall_Start = "Jump_Fall_Start";
            public const string Jump_Fall_Loop = "Jump_Fall_Loop";
            public const string Jump_Land = "Jump_Land";

            public const string Jump_Land_Wait = "Jump_Land_Wait";
            public const string Jump_Land_Move = "Jump_Land_Move";

            public const string Stumble = "Stumble";


            public const string WALK = "Walk";
            public const string WALK_F = "Walk_Front";
            public const string WALK_FL = "Walk_Front_L";
            public const string WALK_FR = "Walk_Front_R";
            public const string WALK_B = "Walk_Back";
            public const string WALK_BL = "Walk_Back_L";
            public const string WALK_BR = "Walk_Back_R";

            public const string RUN = "Run";
            public const string DASH_F = "Dash_Front";
            public const string DASH_FL = "Dash_Front_L";
            public const string DASH_FR = "Dash_Front_R";
            public const string DASH_B = "Dash_Back";
            public const string DASH_BL = "Dash_Back_L";
            public const string DASH_BR = "Dash_Back_R";

            public const string Action_1 = "Action_1";


            public static DirectionNameSet Walk = new DirectionNameSet()
            {
                F = WALK_F,
                FL = WALK_FL,
                FR = WALK_FR,
                B = WALK_B,
                BL = WALK_BL,
                BR = WALK_BR,
            };
            public static DirectionNameSet Dash = new DirectionNameSet()
            {
                F = DASH_F,
                FL = DASH_FL,
                FR = DASH_FR,
                B = DASH_B,
                BL = DASH_BL,
                BR = DASH_BR,
            };
        }
    }
}