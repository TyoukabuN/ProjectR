using System.Collections.Generic;

public static class EntityAttributeDefine
{
    //[10000,20000)  Physics
    public const int MaxGroundedMoveSpeed =         10001;
    public const int GroundedMoveACCSpeed =         10002;
    public const int ACCMaxGroundedMoveSpeed =      10003;
    public const int ACCGroundedMoveACCSpeed =      10004;
    public const int MaxAirMoveSpeed =              10005;
    public const int AirAccelerationSpeed =         10006;
    public const int JumpUpSpeed =                  10007;
    public const int JumpableTime =                 10008;
    public const int Gravity =                      10009;
    public const int SpeedDamping =                 10010;
    public const int OrientationSharpness =         10011;

    //[20000,30000)  Property
    public const int Hp =                           20001;
    public const int Attack =                       20002;


    public static Dictionary<int, string> EntityAttrId2CN = new Dictionary<int, string>()
    {
    //[10000,20000)  Physics
        {MaxGroundedMoveSpeed ,"地面最大移动速度"},
        {GroundedMoveACCSpeed ,"地面移动加速度"},
        {ACCMaxGroundedMoveSpeed ,"加速时地面最大移动速度"},
        {ACCGroundedMoveACCSpeed ,"加速时地面移动加速度"},
        {MaxAirMoveSpeed ,"空中最大移动速度"},
        {AirAccelerationSpeed ,"空中移动加速度"},
        {JumpUpSpeed ,"跳跃上升速度"},
        {JumpableTime ,"可跳跃次数"},
        {Gravity ,"重力加速度"},
        {SpeedDamping ,"速度衰减系数"},
        {OrientationSharpness ,"转向系数" },
    //[20000,30000)  Property
        {Hp ,"生命值"},
        {Attack ,"攻击力"},
    };
}


