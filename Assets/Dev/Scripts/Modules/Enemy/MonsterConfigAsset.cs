using System.Collections.Generic;

namespace PJR
{
    public class MonsterConfigAsset : EntityConfigAsset
    {
        public float Hp = 10f;

        public override void GenrateEntity()
        {
            
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/PJR/创建配置/实体配置/实体配置")]
        public static void CreateAsset()
        {
            CSConfigHelper.CreateScriptableObject<MonsterConfigAsset>();
        }
#endif
    }
}
