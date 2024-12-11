using System;
using UnityEngine;

namespace PJR.Systems
{
    public partial class EffectSystem : MonoSingletonSystem<ResourceSystem>
    {
        public static bool TryGetEffectConfig(int effectID)
        {
            Debug.LogError($"[{nameof(EffectSystem.TryGetEffectConfig)}] failed to load effect!  effectID = {effectID} config == null");
            return false;
        }

        public static EffectHandle LoadEffectByID(int effectID, bool invokeLoadManual = false)
        {
            return LoadEffectByID(effectID, null, null, invokeLoadManual);
        }
        public static EffectHandle LoadEffectByID(int effectID, EffectParams param, bool invokeLoadManual = false)
        {
            return LoadEffectByID(effectID, param, null, invokeLoadManual);
        }
        public static EffectHandle LoadEffectByID(int effectID, Action<EffectHandle> callback, bool invokeLoadManual = false)
        {
            return LoadEffectByID(effectID, null, callback, invokeLoadManual);
        }
        public static EffectHandle LoadEffectByID(int effectID, EffectParams param, Action<EffectHandle> callback, bool invokeLoadManual = false)
        {
            if (param == null)
            {
                param = new EffectParams();
                param.transformSpace = param != null ? EffectParams.TransformSpace.Local : EffectParams.TransformSpace.World;
                param.effectID = effectID;
                param.callback = callback;
            }
            EffectHandle handle = new EffectHandle(param);
            if (!invokeLoadManual)
                handle.Load();
            return handle;
        }

        public static EffectHandle LoadEffectByName(string effectName, bool invokeLoadManual = false)
        {
            return LoadEffectByName(effectName, null, null, invokeLoadManual);
        }
        public static EffectHandle LoadEffectByName(string effectName, EffectParams param, bool invokeLoadManual = false)
        {
            return LoadEffectByName(effectName, param, null, invokeLoadManual);
        }
        public static EffectHandle LoadEffectByName(string effectName, Action<EffectHandle> callback, bool invokeLoadManual = false)
        {
            return LoadEffectByName(effectName, null, callback, invokeLoadManual);
        }
        public static EffectHandle LoadEffectByName(string effectName, EffectParams param, Action<EffectHandle> callback, bool invokeLoadManual = false)
        {
            if (callback == null)
                LogSystem.LogWarning($"[{nameof(EffectSystem.LoadEffectByName)}] callback == null");
            if (param == null)
            {
                param = new EffectParams();
                param.transformSpace = param != null ? EffectParams.TransformSpace.Local : EffectParams.TransformSpace.World;
                param.effectName = effectName;
                param.callback = callback;
            }
            EffectHandle handle = new EffectHandle(param);
            if (!invokeLoadManual)
                handle.Load();
            return handle;
        }

        public static EffectHandle LoadEffectByConfig(EffectConfig.ConfigItem config, Action<EffectHandle> callback, bool invokeLoadManual = false)
        {
            return LoadEffectByConfig(config, null, callback, invokeLoadManual);
        }

        public static EffectHandle LoadEffectByConfig(EffectConfig.ConfigItem config, EffectParams param, Action<EffectHandle> callback, bool invokeLoadManual = false)
        {
            if (param == null)
            {
                param = new EffectParams();
                param.transformSpace = param != null ? EffectParams.TransformSpace.Local : EffectParams.TransformSpace.World;
                param.config = config;
                param.callback = callback;
            }
            EffectHandle handle = new EffectHandle(param);
            if (!invokeLoadManual)
                handle.Load();
            return handle;
        }
    }

}