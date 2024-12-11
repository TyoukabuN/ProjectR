#if UNITY_EDITOR
using Sirenix.Utilities;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using PJR.Systems;

namespace PJR
{
    public partial class ResourceDefine
    {
        [MenuItem("PJR/资源/写入UI绑定预制体")]
        public static void FindScript()
        {
            List<string> prefabs_names = new List<string>();
            string fullPath = "Assets/Art/UI/Prefabs";
            if (Directory.Exists(fullPath))
            {
                UIAssetDict ud = new UIAssetDict();
                string filters = " t:ScriptableObject t:Prefab";
                string[] searchInFolders = new string[]{fullPath,};
                var guids = AssetDatabase.FindAssets(filters, searchInFolders);
                guids.ForEach(guid =>
                {
                    prefabs_names.Add(AssetDatabase.GUIDToAssetPath(guid));
                });
                //DirectoryInfo direction = new DirectoryInfo(fullPath);
                //FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
                //for (int i = 0; i < files.Length; i++)
                //{
                //    if (files[i].Name.EndsWith(".prefab"))
                //    {
                //        string ab_name = "Assets/Art/UI/";
                //        if (files[i].Directory.Name != "Assets/Art/UI/")
                //        {
                //            ab_name += files[i].Directory.Name + "/";
                //        }
                //        prefabs_names.Add(ab_name + files[i].Name);
                //    }
                //}
                for (int i = 0; i < prefabs_names.Count; i++)
                {
                    GameObject go = AssetDatabase.LoadAssetAtPath(prefabs_names[i], typeof(System.Object)) as GameObject;
                    if (go != null)
                    {
                        if (go.transform.TryGetComponent<UINode>() != null)
                        {
                            UINode node = go.transform.TryGetComponent<UINode>();
                            
                            LogSystem.Log("<color=darkblue>Find it: " + go.name + "/" + go.transform.name + "</color>");
                            if (node!=null)
                            {
                                UIAsset ua = new UIAsset(node.UIName,node.prefab);
                                if (ud.assets.ContainsKey(node.UIName))
                                {
                                    LogSystem.LogError($"存在相同的key:{node.UIName},{ud.assets[node.UIName].prefab}与{node.prefab}");
                                    return;
                                }
                                ud.assets[node.UIName] = ua;
                            }
                        }
                    }
                }
                string jsStr = Newtonsoft.Json.JsonConvert.SerializeObject(ud, Newtonsoft.Json.Formatting.Indented);
                string writePath = Application.dataPath + "/Dev/Scripts/Modules/UI/UIBindJs.json";
                //string writePath = Application.dataPath + "/UIBindJs.json";
                //if (!File.Exists(writePath))
                //{
                //    File.Create(writePath);
                //}
                LogSystem.Log(writePath);
                if (Directory.Exists(Application.dataPath + "/Dev/Scripts/Modules/UI"))
                {
                    LogSystem.Log("存在路径");
                    File.WriteAllText(writePath, jsStr);
                }
                LogSystem.Log(jsStr);
            }
        }
    }
}
#endif
