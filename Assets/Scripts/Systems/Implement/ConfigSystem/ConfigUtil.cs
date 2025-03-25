using System.IO;

namespace PJR.Config
{
    public static class ConfigUtil
    {
        public static string GetConfigPath(string relativePath)
        {
            return $"{ConfigConstant.ConfigRoot}/{relativePath}";
        }
        public static string ConfigNameToAssetPath(string configName)
        {
            return GetConfigPath($"{configName}/{configName}Asset.asset");
        }
        public static string ConfigNameToConfigAssetName(string configName)
        {
            return $"{configName}Asset.asset";
        }
    }
}