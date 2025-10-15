#if UNITY_EDITOR
using PJR.Editor;
using UnityEditor;
#endif

namespace PJR
{
    public class FloatConstConfig : ListConfigAsset<ConstConfigItem<float>>
    {

#if UNITY_EDITOR
        [MenuItem("Assets/PJR/创建配置/Float常数配置")]
        public static void CreateConstConfigAsset()
        {
            CSConfigHelper.CreateListConfigAsset<FloatConstConfig, ConstConfigItem<float>>();
        }

        //[OnInspectorGUI]
        //static void OnInspectorGUI()
        //{ 
        //    GUILayout.FlexibleSpace();
        //    if (GUILayout.Button("生成C#配置wrap"))
        //    {
        //        if (Selection.activeObject == null)
        //            return;
        //        if (!(Selection.activeObject is FloatConstConfig))
        //            return;
        //        FloatConstConfig config = (FloatConstConfig) Selection.activeObject;

        //        string assetName = Selection.activeObject.name;
        //        string assetPath = $"Assets/Scripts/Config/ConfigWrap/Gen/{Selection.activeObject.name}.Gen.cs";

        //        var builder = new CSharpScriptBuilder(assetPath);
        //        using (builder.BeginNameSpace("PJR"))
        //        {
        //            using (builder.BeginClass(assetName))
        //            {
        //                builder.AppendLine($"public {nameof(FloatConstConfig)}  asset;");
        //                builder.AppendEmptyLine();
        //                for (int i = 0; i < config.items.Count; i++)
        //                {
        //                    var item = config.items[i];
        //                    builder.AppendLine("/// <summary>");
        //                    builder.AppendLine($"/// {item.desc}");
        //                    builder.AppendLine("/// <summary>");
        //                    builder.AppendLine($"public float {item.key} => GetValue({i});");
        //                }

        //                builder.AppendLine("float GetValue(int index)");
        //                using (builder.Tab(true))
        //                {
        //                    builder.AppendLine("if(asset == null || asset.items == null || asset.items.Count <= index)");
        //                    builder.AppendLine("\t return 0f;");
        //                    builder.AppendLine("return asset.items[index].value;");
        //                }
        //            }
        //        }
        //        builder.Gen();

        //        EditorApplication.delayCall += () =>
        //        {
        //            var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
        //            if (asset != null)
        //                GUIHelper.PingObject(asset);
        //            AssetDatabase.OpenAsset(asset);
        //        };
        //    }
        //}
#endif
    }
}
