using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace ModuleStatLoc
{
    /// <summary>
    /// 中文语言下覆盖模组三项属性名。替换文本来自 Localization CSV，括号后缀按配置拼接。
    /// </summary>
    [HarmonyPatch]
    internal static class LocOverridePatch
    {
        private const string MechanicTable = "Mechanic";
        private const string SuffixFormatKey = "suffix_format";
        private const string DefaultSuffixFormat = "{0}（{1}）";

        private static readonly HashSet<string> StatKeys = new HashSet<string>
        {
            "mech_module_stat_performance",
            "mech_module_stat_efficiency",
            "mech_module_stat_quality"
        };

        private static readonly Dictionary<string, string> TextCache = new Dictionary<string, string>();
        private static string cachedLocale;
        private static bool isChinese;

        #region MARK: 补丁目标
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(LocHelper), "Get", new[]
            {
                typeof(string),
                typeof(string),
                typeof(object[])
            });
        }
        #endregion

        #region MARK: Postfix
        private static void Postfix(string table, string key, ref string __result)
        {
            if (table != MechanicTable || string.IsNullOrEmpty(key) || !StatKeys.Contains(key))
            {
                return;
            }

            if (!IsChineseLocale() || string.IsNullOrEmpty(__result))
            {
                return;
            }

            string name = Loc(key);
            if (name == key)
            {
                return;
            }

            switch (StatLocConfig.Suffix)
            {
                case SuffixMode.Original:
                    __result = FormatSuffix(name, __result);
                    break;
                case SuffixMode.English:
                    __result = FormatSuffix(name, Loc(key + "_en"));
                    break;
                default:
                    __result = name;
                    break;
            }
        }
        #endregion

        #region MARK: 语言与缓存
        private static bool IsChineseLocale()
        {
            string code = LocHelper.GetCurrentLocaleCode();
            if (code != cachedLocale)
            {
                cachedLocale = code;
                isChinese = !string.IsNullOrEmpty(code) && code.StartsWith("zh", StringComparison.Ordinal);
                TextCache.Clear();
            }

            return isChinese;
        }

        private static string Loc(string csvKey)
        {
            if (TextCache.TryGetValue(csvKey, out string cached))
            {
                return cached;
            }

            string value = ModHelper.GetLocalized(ModEntry.MOD_ID, csvKey);
            if (value != csvKey)
            {
                TextCache[csvKey] = value;
            }

            return value;
        }

        private static string FormatSuffix(string name, string suffix)
        {
            string format = Loc(SuffixFormatKey);
            if (format == SuffixFormatKey)
            {
                format = DefaultSuffixFormat;
            }

            try
            {
                return string.Format(format, name, suffix);
            }
            catch (FormatException)
            {
                return name + "（" + suffix + "）";
            }
        }
        #endregion
    }
}
