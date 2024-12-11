using Sirenix.OdinInspector;
using PJR.Systems;

namespace PJR
{
    public class MonsterConfigHost : EntityConfigHost
    {
        public override EntityConfigAsset EntiyConfigAsset => configAsset;

        [InlineEditor]
        public MonsterConfigAsset configAsset;

        public override void GenrateEntity()
        {
            if (configAsset == null)
            {
                LogSystem.LogError("[EntityConfigHost.GenrateEntity] EntiyConfigAsset == null");
                return;
            }
            EntitySystem.CreateMonster(this);
        }
    }
}