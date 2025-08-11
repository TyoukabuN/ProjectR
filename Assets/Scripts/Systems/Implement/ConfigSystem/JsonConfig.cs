using System;
using System.Collections.Generic;

namespace PJR
{
    public abstract class JsonConfig : ConfigBase
    {
        protected abstract void Reset();

        public const int DefaultConfigCount = 30;
        public static Dictionary<string,JsonConfig> configs = new Dictionary<string,JsonConfig>(DefaultConfigCount);

        /// <summary>
        /// 创建配置实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Create<T>() where T: JsonConfig , new()
        {
            configs = configs ?? new Dictionary<string, JsonConfig>(DefaultConfigCount);
            string clsName = typeof(T).Name;
            if (!configs.TryGetValue(clsName, out var config))
            { 
                config = new T();
                configs.Add(clsName,config);
            }
            return (T)config;
        }

        /// <summary>
        /// 重置全部配置
        /// </summary>
        public static void ResetAllConfig()
        {
            foreach (var pair in configs)
                pair.Value?.Reset();
        }

        /// <summary>
        /// 将Json转换成对应类型Dic
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="jsonFileName"></param>
        /// <returns></returns>
        //public static Dictionary<TKey, TValue> JsonToDic<TKey,TValue>(string jsonFileName) where TValue : new()
        //{
        //    Dictionary<TKey, TValue> res = new Dictionary<TKey, TValue>();

        //    string text = ConfigSystem.LoadJsonConfig(jsonFileName);
        //    var dic = Json.Deserialize(text) as Dictionary<string, object>;
        //    for (int i = 0; i < dic.Count; i++)
        //    {
        //        var item = dic.ElementAt(i);
        //        var itemValueMap = dic.ElementAt(i).Value as Dictionary<string, object>;

        //        TValue configItem = new TValue();
        //        foreach (var field in itemValueMap)
        //        {
        //            var _field = typeof(TValue).GetField(field.Key);
        //            var fieldName = field.Key;
        //            //xlsx工具会将value都转成string
        //            string strValue = (string)field.Value;
        //            if (string.IsNullOrEmpty(strValue))
        //                continue;

        //            typeof(TValue).GetField(field.Key).SetValue(configItem, GetParseValue(_field.FieldType, field.Value));
        //        }

        //        res[(TKey)GetParseValue(typeof(TKey), item.Key)] = configItem;
        //    }
        //    return res;
        //}

        /// <summary>
        /// 将Json转换成对应类型List
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="jsonFileName"></param>
        /// <returns></returns>
        //public static List<TValue> JsonToList<TValue>(string jsonFileName) where TValue : new()
        //{
        //    string text = ConfigSystem.LoadJsonConfig(jsonFileName);
        //    var list = Json.Deserialize(text) as List<object>;

        //    List<TValue> res = new List<TValue>(list.Count);
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        var itemValueMap = list[i] as Dictionary<string, object>;

        //        TValue configItem = new TValue();
        //        foreach (var field in itemValueMap)
        //        {
        //            var _field = typeof(TValue).GetField(field.Key);
        //            var fieldName = field.Key;

        //            typeof(TValue).GetField(field.Key).SetValue(configItem, GetParseValue(_field.FieldType, field.Value));
        //        }
        //        res.Add(configItem);
        //    }
        //    return res;
        //}

        //public Dictionary<TKey, TValue> JsonToDic<TKey, TValue>() where TValue : new()
        //{
        //    return JsonConfig.JsonToDic<TKey, TValue>(FileName);
        //}
        //public List<TValue> JsonToList<TValue>() where TValue : new()
        //{
        //    return JsonConfig.JsonToList<TValue>(FileName);
        //}


        /// <summary>
        /// 因为配置值同意配的string，所以需要转一下
        /// 没有的类型加一下
        /// 但可能只需要支持string和int
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object GetParseValue(Type fieldType,object value)
        {
            if (fieldType == typeof(int))
                return Convert.ToInt32((string)value);
            if (fieldType == typeof(float))
                return Convert.ToSingle((string)value);
            if (fieldType == typeof(double))
                return Convert.ToDouble((string)value);
            if (fieldType == typeof(bool))
                return Convert.ToBoolean(((string)value).ToLower());
            return value;
        }
        public static object GetParseValue(Type fieldType, string value)
        {
            return GetParseValue(fieldType, (object)value);
        }
    }
}