using System;
using System.Collections.Generic;
using System.Reflection;

namespace LS.Game
{
    public class EffectConfig : JsonConfig
    {
        public class ConfigItem : System.Object
        {
            public int ID = -1;
            public string AssetName = string.Empty;
            public bool Poolable = true;
            public int PoolSize = 6;

            public override string ToString()
            {
                var sb = new System.Text.StringBuilder();

                sb.AppendLine($"[ToString] {this.GetType().Name} = ");
                sb.AppendLine($"{{");

                foreach (FieldInfo field in this.GetType().GetFields())
                {
                    string fieldName = field.Name;
                    object fieldValue = field.GetValue(this);
                    sb.AppendLine(string.Format("{0} = {1}", fieldName, fieldValue));
                }
                sb.AppendLine($"}}");
                return sb.ToString();
            }
        }
        public override string FileName => "ls_effect_config.json";

        public static EffectConfig instance;

        private Dictionary<int,ConfigItem> config = null;
        public static Dictionary<int, ConfigItem> Config {
            get { 
                if(instance.CheckValid())
                    return instance.config;
                return null;
            }
        }

        static EffectConfig()
        {
            instance = Create<EffectConfig>();
        }
        private void Initialize()
        {
            //config = JsonToDic<int, ConfigItem>(FileName);
        }
        protected override void Reset()
        {
            config = null;
        }
        public bool CheckValid()
        {
            if (config == null)
                Initialize();
            return config != null;
        }

        /// <summary>
        /// 根据id获取对应特效的名字
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetEffectNameById(int id)
        {
            var config = GetEffectConfig(id);
            if (config == null)
                return string.Empty;
            return config.AssetName;
        }

        public static ConfigItem GetEffectConfig(int id)
        {
            if (!instance.CheckValid())
                return null;
            ConfigItem item = null;
            if (!instance.config.TryGetValue(id, out item))
                return null;
            return item;
        }
        public static ConfigItem GetEffectConfig(string assetName)
        {
            if (!instance.CheckValid())
                return null;
            foreach (var configItem in instance.config)
            {
                if (configItem.Value.AssetName == assetName)
                    return configItem.Value;
            }
            return null;
        }
    }
}
