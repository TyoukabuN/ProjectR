using System.Collections.Generic;

namespace PJR
{
    public class TrapManualPool : MonoSingleton<TrapManualPool>
    {
        private Dictionary<int,LogicEntity> entities=new Dictionary<int,LogicEntity>();
        public Dictionary<int,LogicEntity> Entities{ get { return entities; } }

        private Dictionary<int, Dictionary<int, LogicEntity>> entitiesGroup = new Dictionary<int, Dictionary<int, LogicEntity>>();
        public Dictionary<int, Dictionary<int, LogicEntity>> EntitiesGroup{ get { return entitiesGroup; } }
        public void Add(int key ,LogicEntity value)
        {
            if (entities.ContainsKey(key))
            {
                return;
            }
            entities[key]=value;
        }
        public LogicEntity GetEntity(int key){ return entities[key]; }

        public override void Clear()
        {
            entities.Clear();
        }
    }
}

