using PJR.Editor;
using UnityEditor;

namespace PJR
{
    public class MonsterConfigAsset : EntityConfigAsset
    {
        public float Hp = 10f;

        public override void GenrateEntity()
        {
            
        }

#if UNITY_EDITOR
        [MenuItem("Assets/PJR/创建配置/实体配置/实体配置")]
        public static void CreateAsset()
        {
            CSConfigHelper.CreateScriptableObject<MonsterConfigAsset>();
        }
#endif
    }
}
