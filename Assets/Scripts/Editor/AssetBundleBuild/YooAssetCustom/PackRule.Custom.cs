using System;
using System.IO;
using YooAsset.Editor;

namespace PJR
{
    [DisplayName("资源包名: 收集器下次级文件夹路径")]
    public class PackSecondaryDirectory : IPackRule
    {
        /// <summary>
        /// 以收集器路径下顶级文件夹为资源包名
        /// 注意：文件夹下所有文件打进一个资源包
        /// 例如：收集器路径为 "Assets/__LS/Art/Characters"
        /// 例如："Assets/__LS/Art/Characters/Beast/Beast0101_Pig/Image/Beast0101_Pig_Body_C.png" --> "assets___ls_art_characters_beast_beast0101_pig.bundle"
        /// 例如："Assets/__LS/Art/Characters/Beast/Beast0101_Pig/Mesh/Beast0101_Pig.fbx" --> "assets___ls_art_characters_beast_beast0101_pig.bundle"
        /// </summary>
        public PackRuleResult GetPackRuleResult(PackRuleData data)
        {
            string assetPath = data.AssetPath.Replace(data.CollectPath, string.Empty);
            assetPath = assetPath.TrimStart('/');
            string[] splits = assetPath.Split('/');
            if (splits.Length > 0)
            {
                if (Path.HasExtension(splits[0]))
                    throw new Exception($"Not found root directory : {assetPath}");
                if (Path.HasExtension(splits[1]))
                    throw new Exception($"Not found root directory : {assetPath}");
                string bundleName = $"{data.CollectPath}/{splits[0]}/{splits[1]}";
                PackRuleResult result = new PackRuleResult(bundleName, DefaultPackRule.AssetBundleFileExtension);
                return result;
            }
            else
            {
                throw new Exception($"Not found root directory : {assetPath}");
            }
        }
    }
}
