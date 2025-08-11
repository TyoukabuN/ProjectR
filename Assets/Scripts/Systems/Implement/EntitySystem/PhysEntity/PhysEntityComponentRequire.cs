namespace PJR
{
    /// <summary>
    /// 给物理实体的组件请求列表
    /// </summary>
    public struct PhysEntityComponentRequire
    {
        public bool kinematicCharacterMotor;
        public bool animancer;
        public bool collider;
        public static PhysEntityComponentRequire Default => new PhysEntityComponentRequire{
            kinematicCharacterMotor = true,
            animancer = true,
            collider = true,
        };
        public static PhysEntityComponentRequire All => new PhysEntityComponentRequire
        {
            kinematicCharacterMotor = true,
            animancer = true,
            collider = true,
        };
        public static PhysEntityComponentRequire None => new PhysEntityComponentRequire
        {
            kinematicCharacterMotor = false,
            animancer = false,
            collider = false,
        };
        public static PhysEntityComponentRequire NonKCCOnly => new PhysEntityComponentRequire
        {
            kinematicCharacterMotor = false,
            animancer = true,
            collider = true,
        };
        public static PhysEntityComponentRequire NonAnimancerOnly => new PhysEntityComponentRequire
        {
            kinematicCharacterMotor = true,
            animancer = false,
            collider = true,
        };
    }
}