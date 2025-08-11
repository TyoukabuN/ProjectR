using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PJR.Systems
{
    public class ConfigSystem : MonoSingletonSystem<ConfigSystem>
    {
        protected static bool _initialized = false;

        protected static Dictionary<string, TextAsset> name2JsonAsset = new Dictionary<string, TextAsset>();

        public static void Reset()
        {
            name2JsonAsset = new Dictionary<string, TextAsset>();
        }
        //public async UniTask InitializeAll()
        //{
        //    _initialized = false;
        //}

        public static string GetJsonPath(string jsonFileName)
        {
            return Path.Combine(PathUtil.JsonConfigRoot,jsonFileName);
        }
//        public static string LoadJsonConfig(string jsonFileName)
//        {
//            name2JsonAsset ??= new Dictionary<string, TextAsset>();

//            using (new ProfileScope("ConfigSystem.LoadJsonConfig"))
//            {
//                TextAsset asset = null;
//                if (name2JsonAsset.TryGetValue(jsonFileName, out asset))
//                    return asset.text;

//                string assetPath = GetJsonPath(jsonFileName);

//                if (Application.isEditor && !ResourceSystem.InABMode)
//                {
//#if UNITY_EDITOR
//                    asset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
//#endif
//                }
//                else
//                {
//                    var handle = AssetSystem.LoadAsset(jsonFileName);
//                }
//                if (asset == null)
//                    return string.Empty;

//                name2JsonAsset[jsonFileName] = asset;
//                return asset.text;
//            }
//        }
    }
}

