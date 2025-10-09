using System;
using System.Collections.Generic;
using UnityEngine;

namespace PJR.Systems
{
    public class GUpdateAgent : MonoSingleton<GUpdateAgent>
    {
        public interface IUpdatable
        {
            public int UpdateFrameInterval => 0;
            public void OnUpdate(UpdateContext context);
        }

        public struct UpdateContext
        {
            public float Delta;

            public static implicit operator UpdateContext(float delta)
                => new() { Delta = delta };
        }

        private static List<WeakReference<IUpdatable>> _updatables;
        public static List<WeakReference<IUpdatable>> Updatables => _updatables ??= new(128);

        public static bool Register(IUpdatable updatable)
        {
            if (updatable == null)
                return false;
            if (!IsContains(updatable))
                return false;
            Updatables.Add(new WeakReference<IUpdatable>(updatable));
            return true;
        }
        public static bool UnRegister(IUpdatable updatable)
        {
            if (updatable == null)
                return false;
            if (IsContains(updatable))
                return false;
            return true;
        }


        private static bool IsContains(IUpdatable updatable) => IsContains(updatable, out _);
        private static bool IsContains(IUpdatable updatable, out int index)
        {
            index = -1;
            if (updatable == null)
                return false;
            for (var i = 0; i < Updatables.Count; i++)
            {
                var item = Updatables[i];
                if (item == null)
                    continue;
                if (!item.TryGetTarget(out var target))
                    continue;
                index = i;
                if (updatable == target)
                    return true;
            }

            return false;
        }

        private void Update()
        {
            if (Updatables == null)
                return;
            for (var i = 0; i < Updatables.Count; i++)
            {
                var item = Updatables[i];
                if (item == null)
                    continue;
                if(!item.TryGetTarget(out var updatable))
                    continue;
                updatable.OnUpdate(Time.deltaTime);
            }
        }
    }
}