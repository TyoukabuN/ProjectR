using Sirenix.OdinInspector;
using UnityEngine.Pool;

namespace PJR.Dev.Game.DataContext
{
    public abstract partial class PropertyBlock
    {
        [LabelText("Runtime get/new用")]
        public class Temp : PropertyBlock
        {
            public override bool IsTemp => true;

            public static Temp Get()
            {
                var temp = GenericPool<Temp>.Get();
                temp.Reset();
                return temp;
            }

            public static Temp Get(PropertyBlock other)
            {
                var temp = GenericPool<Temp>.Get();
                if (other == null)
                {
                    temp.Reset();
                    return temp;
                }
                temp.FloatBlock = other.FloatBlock;
                temp.IntBlock = other.IntBlock;
                temp.BoolBlock = other.BoolBlock;
                temp.StringBlock = other.StringBlock;
                return temp;
            }

            public override void Release() => GenericPool<Temp>.Release(this);
        }
    }
}