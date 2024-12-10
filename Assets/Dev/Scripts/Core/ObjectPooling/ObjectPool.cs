using System;
using System.Collections.Generic;

namespace PJR.Core.Pooling
{
    public interface IPoolItem
    {
        public void OnGet();
        public void OnRelease();
    }
    public class ObjectPool<ObjectType> where ObjectType : IPoolItem, new()
    {
        private static Dictionary<Type, Queue<ObjectType>> typeToTransition;

        private static Queue<ObjectType> GetQueue()
        {
            if (typeToTransition == null)
                typeToTransition = new Dictionary<Type, Queue<ObjectType>>();

            Type type = typeof(ObjectType);
            if (!typeToTransition.TryGetValue(typeof(ObjectType), out var queue))
            {
                queue = new Queue<ObjectType>();
                typeToTransition[type] = queue;
            }
            return queue;
        }
        public static ObjectType Get()
        {
            var list = GetQueue();

            ObjectType transition = default;
            if (list.Count > 0)
            {
                transition = list.Dequeue();
            }
            else
            {
                transition = (ObjectType)Activator.CreateInstance(typeof(ObjectType), true);
            }

            transition.OnGet();
            return transition;
        }

        public static bool Release(ObjectType transition)
        {
            if (transition == null)
                return false;
            var list = GetQueue();
            list.Enqueue(transition);

            transition.OnRelease();
            return true;
        }
    }
}
