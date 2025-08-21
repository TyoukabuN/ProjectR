using Sirenix.OdinInspector;

namespace PJR.Dev.Game.DataContext
{
    public enum DataType : uint
    {
        [LabelText("基础生命值")]
        BasicHP = 1000,
        [LabelText("基础攻击力")]
        BasicAtk = 2000,
        [LabelText("基础防御力")]
        BasicDef = 3000,
    }
}